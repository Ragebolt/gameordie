using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(HelpBoxAttribute))]
public class HelpBoxDrawer : PropertyDrawer
{
    private readonly float propertyOffset = 10f;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        HelpBoxAttribute helpBox = attribute as HelpBoxAttribute;

        Rect allRect = position;
        allRect.height += helpBox.height;

        Rect boxRect = position;
        boxRect.height = helpBox.height;
        EditorGUI.HelpBox(boxRect, helpBox.text, MessageType.Info);

        position.y += boxRect.height + propertyOffset;
        position.height -= helpBox.height + propertyOffset;

        EditorGUI.PropertyField(position, property, label, true);

    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        HelpBoxAttribute helpBox = attribute as HelpBoxAttribute;

        return helpBox.height + propertyOffset + EditorGUI.GetPropertyHeight(property, label, true); ;
    }
}