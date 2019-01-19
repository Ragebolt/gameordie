using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Enum2ButtonsAttribute))]
public class Enum2ButtonsAttributeDrawer : PropertyDrawer {
    private const bool ByThree = true;

    [Flags]
    private enum TriBool {
        Unset = 0,
        False = 1,
        True  = 2,
        Both  = 3
    }

    private struct Entry {
        public string  label;
        public int     mask;
        public TriBool currentValue;
    }

    private List<SerializedProperty> _properties;
    private List<Entry>              _entries;
    private int                      _rowCount;
    private int                      _columnCount;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        if (_properties == null)
            Initialize(property);
        return _rowCount * EditorGUIUtility.singleLineHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        label = EditorGUI.BeginProperty(position, label, property);

        var labelRect = new Rect(
                                 position.x,
                                 position.y,
                                 EditorGUIUtility.labelWidth,
                                 position.height);
        EditorGUI.LabelField(labelRect, label);

        var buttonStride = (position.width - labelRect.width) / _columnCount;
        var buttonWidth  = buttonStride * 1.0f;
        var buttonRect = new Rect(
                                  labelRect.max.x,
                                  labelRect.y,
                                  buttonWidth,
                                  EditorGUIUtility.singleLineHeight);

        var column = 0;

        var mixedButton = new GUIStyle(GUI.skin.button);
        mixedButton.normal.textColor = Color.grey;

        for (var i = 0; i < _entries.Count; i++) {
            //Debug.Log(_entries[i].label + " : " + _entries[i].currentValue);
            var entry = _entries[i];

            EditorGUI.BeginChangeCheck();
            var pressed = GUI.Toggle(
                                     buttonRect,
                                     entry.currentValue == TriBool.True,
                                     entry.label,
                                     entry.currentValue == TriBool.Both ? mixedButton : GUI.skin.button);

            if (EditorGUI.EndChangeCheck()) {
                if (pressed) {
                    foreach (var prop in _properties)
                        prop.intValue |= entry.mask;
                    entry.currentValue = TriBool.True;
                }
                else {
                    foreach (var prop in _properties)
                        prop.intValue &= ~entry.mask;
                    entry.currentValue = TriBool.False;
                }
                _entries[i] = entry;
            }

            buttonRect.x += buttonStride;
            if (++column >= _columnCount) {
                column       =  0;
                buttonRect.x =  labelRect.max.x;
                buttonRect.y += buttonRect.height;
            }
        }

        foreach (var prop in _properties)
            prop.serializedObject.ApplyModifiedProperties();

        EditorGUI.EndProperty();

        EditorGUI.showMixedValue = false;
    }

    private void Initialize(SerializedProperty property) {
        var allTargetObjects = property.serializedObject.targetObjects;
        _properties = new List<SerializedProperty>(allTargetObjects.Length);
        foreach (var targetObject in allTargetObjects) {
            var iteratedObject   = new SerializedObject(targetObject);
            var iteratedProperty = iteratedObject.FindProperty(property.propertyPath);
            if (iteratedProperty != null)
                _properties.Add(iteratedProperty);
        }

//        var parentType = property.serializedObject.targetObject.GetType();
        var enumType  = fieldInfo.FieldType;
        var trueNames = Enum.GetNames(enumType);

        var typedValues = GetTypedValues(property, enumType);
        var display     = property.enumDisplayNames;
        var names       = property.enumNames;

        _entries = new List<Entry>();

        for (var i = 0; i < names.Length; i++) {
            var sortedIndex = Array.IndexOf(trueNames, names[i]);
            var value       = typedValues[sortedIndex];
            var bitCount    = 0;

            for (var temp = value; temp != 0 && bitCount <= 1; temp >>= 1)
                bitCount += temp & 1;

            //Debug.Log(names[i] + ": " + value + " ~ " + bitCount);

            if (bitCount != 1)
                continue;

            var consensus = TriBool.Unset;
            foreach (var prop in _properties)
                if ((prop.intValue & value) == 0)
                    consensus |= TriBool.False;
                else
                    consensus |= TriBool.True;

            _entries.Add(new Entry { label = display[i], mask = value, currentValue = consensus });
        }

        _rowCount    = Mathf.CeilToInt(_entries.Count / 3f);
        _columnCount = Mathf.Min(_entries.Count, Mathf.CeilToInt(_entries.Count / 2f));
    }

    private int[] GetTypedValues(SerializedProperty property, Type enumType) {
        var values     = Enum.GetValues(enumType);
        var underlying = Enum.GetUnderlyingType(enumType);

        if (underlying == typeof(int))
            return ConvertFrom<int>(values);
        else if (underlying == typeof(uint))
            return ConvertFrom<uint>(values);
        else if (underlying == typeof(short))
            return ConvertFrom<short>(values);
        else if (underlying == typeof(ushort))
            return ConvertFrom<ushort>(values);
        else if (underlying == typeof(sbyte))
            return ConvertFrom<sbyte>(values);
        else if (underlying == typeof(byte))
            return ConvertFrom<byte>(values);
        else
            throw new InvalidCastException("Cannot use enum backing types other than byte, sbyte, ushort, short, uint, or int.");
    }

    private int[] ConvertFrom<T>(Array untyped) where T : IConvertible {
        var typedValues = new int[untyped.Length];

        for (var i = 0; i < typedValues.Length; i++)
            typedValues[i] = Convert.ToInt32((T) untyped.GetValue(i));

        return typedValues;
    }
}