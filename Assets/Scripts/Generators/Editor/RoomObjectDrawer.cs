using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using EasyEditorGUI;
using Generation;

[CustomPropertyDrawer(typeof(RoomObject), false)]
public class RoomObjectDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var rect = position;

        var spawnRect = property.FindPropertyRelative("spawnRect");
        EditorGUI.PropertyField(rect, spawnRect);

        var textureRect = new Rect(eGUI.indentWidth - rect.height - 5f, rect.y, rect.height, rect.height);
        EditorGUI.DrawTextureTransparent(textureRect, SpawnPatternEditor.DrawRectangle(spawnRect.rectValue));
        rect.y += rect.height;
        rect.width += rect.height;

        rect.height = EditorGUIUtility.singleLineHeight;
        var spMin = property.FindPropertyRelative("minRotation");
        var spMax = property.FindPropertyRelative("maxRotation");

        GUILayout.BeginHorizontal();
        {
            rect.width *= 0.5f;
            EditorGUI.PropertyField(rect, spMin);
            rect.x += rect.width;
            EditorGUI.PropertyField(rect, spMax);
            rect.x -= rect.width;
            rect.width *= 2f;
        }
        GUILayout.EndHorizontal();
        rect.y += rect.height;

        float min = spMin.floatValue;
        float max = spMax.floatValue;
        EditorGUI.MinMaxSlider(rect, "Rotation", ref min, ref max, -360f, 360f);
        var f = max - min;
        if (f > 360f)
        {
            f -= 360f;
            max -= f;
        }
        spMin.floatValue = min;
        spMax.floatValue = max;
        rect.y += rect.height;


        var spawnChance = property.FindPropertyRelative("spawnChance");
        EditorGUI.Slider(rect, spawnChance, 0f, 1f);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight * 2f;
    }
}
