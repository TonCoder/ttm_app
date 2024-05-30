using System.Linq;
using Creative_Vein_Studio.PropertyAttributes;
using UnityEditor;
using UnityEngine;

namespace Creative_Vein_Studio.Editor
{
    [CustomPropertyDrawer(typeof(ScenePathInfoAttribute))]
    public class ScenePathInfoPropertyDrawer : PropertyDrawer
    {
        private SceneAsset[] _allScenes;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var oldScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(property.stringValue);
            if (oldScene == null && !string.IsNullOrEmpty(property.stringValue))
            {
                // well, maybe by name then?
                _allScenes = _allScenes ?? AssetDatabase.FindAssets("t:scene")
                    .Select(x => AssetDatabase.GUIDToAssetPath(x))
                    .Select(x => AssetDatabase.LoadAssetAtPath<SceneAsset>(x))
                    .ToArray();

                var matchedByName = _allScenes.Where(x => x.name == property.stringValue).ToList();
                ;

                if (matchedByName.Count == 0)
                {
                    Debug.Log($"Scene not found: {property.stringValue}");
                }
                else
                {
                    oldScene = matchedByName[0];
                    if (matchedByName.Count > 1)
                    {
                        Debug.Log("There are multiple scenes with this name");
                    }
                }
            }

            using (new EditorGUILayout.VerticalScope())
            {
                EditorGUI.BeginChangeCheck();
                var newScene =
                    EditorGUI.ObjectField(position, label, oldScene, typeof(SceneAsset), false) as SceneAsset;
                if (EditorGUI.EndChangeCheck())
                {
                    var assetPath = AssetDatabase.GetAssetPath(newScene);
                    property.stringValue = assetPath;
                    property.serializedObject.ApplyModifiedProperties();
                }
            }
        }
    }
}