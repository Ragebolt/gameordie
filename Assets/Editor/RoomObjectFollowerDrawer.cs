using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Generation;

[CustomPropertyDrawer(typeof(RoomObjectFollower), false)]
public class RoomObjectFollowerDrawer : RoomObjectDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Debug.Log(1);

        base.OnGUI(position, property, label);

        EditorGUI.PropertyField(position, property.FindPropertyRelative("config"));
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight * 2f;
    }
}