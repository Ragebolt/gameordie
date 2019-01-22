using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using EasyEditorGUI;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PrefabsList))]
public class PrefabList_editor : Editor {
    private PrefabsList prefabs;
    private GameObject  sel;
    private string      tag;

    private void OnEnable() {
        prefabs = (PrefabsList) target;
    }

    public override void OnInspectorGUI() {
        if (prefabs.prefabs == null)
            prefabs.prefabs = new List<GameObject>();
        if (prefabs.ids == null)
            prefabs.ids = new List<int>();
        if (prefabs.tags == null)
            prefabs.tags = new List<string>();
        while (prefabs.prefabs.Count > prefabs.tags.Count)
            prefabs.tags.Add("");

        var so       = new SerializedObject(prefabs);
        var sPrefabs = so.FindProperty("prefabs");
        var sIDs     = so.FindProperty("ids");
        var sTags    = so.FindProperty("tags");
        if (GUILayout.Button("CLEAR ALL")) {
            prefabs.Clear();
            prefabs.UpdateContainer();
            EditorUtility.SetDirty(prefabs);
        }
        if (prefabs.prefabs.Count > 0)
            for (var i = 0; i < prefabs.prefabs.Count; i++) {
                GUILayout.BeginHorizontal();
                {
                    var w = GUILayout.Width(eGUI.indentWidth * 0.1f);
                    GUI.enabled = false;
                    EditorGUILayout.IntField(prefabs.ids[i], w);
                    w               = GUILayout.Width(eGUI.indentWidth * 0.2f);
                    GUI.enabled     = true;
                    var s = EditorGUILayout.TextField(prefabs.tags[i], w);
                    if (s != prefabs.tags[i]) {
                        prefabs.tags[i] = s;
                        prefabs.UpdateContainer();
                        EditorUtility.SetDirty(prefabs);
                    }
                    w               = GUILayout.Width(eGUI.indentWidth * 0.52f);
                    GUI.enabled     = false;
                    EditorGUILayout.ObjectField(prefabs.prefabs[i], typeof(GameObject), false, w);
                    GUI.enabled = true;
                    w           = GUILayout.Width(eGUI.indentWidth * 0.1f);
                    if (GUILayout.Button("[-]", w)) {
                        prefabs.RemovePrefabAt(i);
                        prefabs.UpdateContainer();
                        EditorUtility.SetDirty(prefabs);
                        GUILayout.EndHorizontal();
                        return;
                    }
                }
                GUILayout.EndHorizontal();
            }
        EditorGUILayout.LabelField("--------------------------------------------------------------------------------------------------------------------------");
        sel = (GameObject) EditorGUILayout.ObjectField(sel, typeof(GameObject), false);
        GUILayout.BeginHorizontal();
        {
            var tags  = prefabs.tags.ToArray();
            var index = prefabs.tags.IndexOf(tag);
            index = EditorGUILayout.Popup(index, tags);
            if (index > -1) tag = tags[index];
            tag = EditorGUILayout.TextField(tag);
            if (sel == null) GUI.enabled = false;
            if (GUILayout.Button("Add")) {
                prefabs.AddPrefab(sel, tag);
                EditorUtility.SetDirty(prefabs);
            }
        }
        GUILayout.EndHorizontal();
        GUI.enabled = true;
    }
}