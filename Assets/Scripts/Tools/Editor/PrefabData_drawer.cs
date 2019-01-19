using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(PrefabData))]
public class PrefabData_drawer : PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        var pID   = property.FindPropertyRelative("id");
        var id    = pID.intValue;
        var ids   = PrefabsContainer.GetIDs();
        var names = new List<string>();
        var index = -1;
        foreach (var i in ids) {
            names.Add("(" + i.ToString() + ") " + PrefabsContainer.GetPrefab(i).name);
            if (i == id) index = names.Count - 1;
        }
        var rect = position;
        rect.height =  EditorGUIUtility.singleLineHeight;
        
        var newIndex = EditorGUI.Popup(rect, index, names.ToArray());
        if (newIndex != index)
            pID.intValue = ids[newIndex];
        rect.y      += rect.height;
        
        GUI.enabled =  false;
        var w = rect.width;
        rect.width = 80f;
        EditorGUI.IntField(rect, id);
        rect.x     += rect.width;
        rect.width =  w - rect.width;
        var ww = GUIContent.none;
        EditorGUI.ObjectField(rect, ww, PrefabsContainer.GetPrefab(id), typeof(GameObject), false);
        GUI.enabled = true;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        return EditorGUIUtility.singleLineHeight * 2f;
    }
}