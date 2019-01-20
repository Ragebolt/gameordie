using System.Collections.Generic;
using EasyEditorGUI;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(PrefabData))]
public class PrefabData_drawer : PropertyDrawer {
    private static Dictionary<PrefabData_drawer, int> popup = new Dictionary<PrefabData_drawer, int>();

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        var pID   = property.FindPropertyRelative("id");
        var id    = pID.intValue;
        var ids   = PrefabsContainer.GetIDs();
        var names = new List<string>();

        var index = -1;
        foreach (var i in ids) {
            var s = PrefabsContainer.GetTag(i);
            s = (s == "") ? s = "<none>/" + PrefabsContainer.GetPrefab(i).name : s = s + "/" + PrefabsContainer.GetPrefab(i).name;
            names.Add(s);
            if (i == id) index = names.Count - 1;
        }

        var rect = position;
        rect.height = EditorGUIUtility.singleLineHeight;
        rect.width  = Mathf.Min(100f, position.width * 0.5f);
        EditorGUI.LabelField(rect, label);
        rect.x     += rect.width;
        rect.width =  position.width - rect.width;

        var newIndex = EditorGUI.Popup(rect, index, names.ToArray());
        if (newIndex != index) {
            pID.intValue = ids[newIndex];
            eGUI.MarkAsDirty();
        }
        rect.width =  position.width;
        rect.x     =  position.x;
        rect.y     += rect.height;

        GUI.enabled = false;
        var w = rect.width;
        rect.width = w * 0.2f;
        var ww = GUIContent.none;
        EditorGUI.IntField(rect, ww, id);
        rect.x += rect.width;
        rect.width =  w * 0.3f;
        EditorGUI.TextField(rect, ww, PrefabsContainer.GetTag(id));
        rect.x     += rect.width;
        rect.width =  w * 0.5f;
        EditorGUI.ObjectField(rect, ww, PrefabsContainer.GetPrefab(id), typeof(GameObject), false);
        GUI.enabled = true;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        return EditorGUIUtility.singleLineHeight * 2f;
    }
}