using CreativeVeinStudio.Simple_Dialogue_System.Attributes;
using UnityEditor;
using UnityEngine;

namespace Creative_Vein_Studio.Editor
{
    [CustomPropertyDrawer(typeof(ExposeSoAttribute))]
    public class ExposeSoAttributeDrawer : PropertyDrawer
    {
        private UnityEditor.Editor editor = null;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.indentLevel++;
            EditorGUI.PropertyField(position, property, label, true);
            EditorGUI.indentLevel--;

            // Draw foldout arrow
            if (property.objectReferenceValue != null)
            {
                property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, GUIContent.none);
            }

            // draw foldout props
            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;

                if (!editor)
                    UnityEditor.Editor.CreateCachedEditor(property.objectReferenceValue, null, ref editor);

                editor.OnInspectorGUI();

                EditorGUI.indentLevel--;
            }
        }
    }
}