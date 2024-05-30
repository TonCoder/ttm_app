using System;
using CreativeVeinStudio.Simple_Dialogue_System.Attributes;
using UnityEditor;
using UnityEngine;

namespace CreativeVeinStudio.Simple_Dialogue_System.Editor
{
    [CustomPropertyDrawer(typeof(FieldTitleAttribute), true)]
    public class
        TitlePropertyDrawer : DecoratorDrawer //INFO -> https://docs.unity3d.com/ScriptReference/DecoratorDrawer.html
    {
        private GUIContent contentTitle;
        private float propertyHeight = 0;

        public override void OnGUI(Rect position)
        {
            position.y += 15;
            contentTitle = new GUIContent((this.attribute as FieldTitleAttribute)?.fieldTitle);
            var style = new GUIStyle(EditorStyles.toolbar)
            {
                fixedHeight = 30,
                alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 15
            };

            EditorGUI.LabelField(position, contentTitle, style);
            propertyHeight = style.CalcHeight(contentTitle, 1f);
        }

        public override float GetHeight()
        {
            return propertyHeight + EditorGUIUtility.singleLineHeight;
        }

        // public override VisualElement CreatePropertyGUI()
        // {
        //     Label propertyGui = new Label((this.attribute as FieldTitleAttribute).fieldTitle);
        //     propertyGui.AddToClassList("unity-header-drawer__label");
        //
        //     
        //     return (VisualElement)propertyGui;
        // }
    }
}