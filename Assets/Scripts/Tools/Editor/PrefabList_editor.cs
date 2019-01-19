using System.Collections.Generic;
using System.ComponentModel.Design;
using EasyEditorGUI;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PrefabsList))]
public class PrefabList_editor : Editor {
    private PrefabsList prefabs;
    private GameObject  sel;

    private void OnEnable() {
        prefabs = (PrefabsList) target;
    }

    public override void OnInspectorGUI() {
        if (prefabs.prefabs == null) {
            prefabs.prefabs = new List<GameObject>();
            prefabs.ids     = new List<int>();
        }
        var so = new SerializedObject(prefabs);
        var sPrefabs = so.FindProperty("prefabs");
        var sIDs = so.FindProperty("ids");
        if (GUILayout.Button("CLEAR ALL")) {
            prefabs.prefabs.Clear();
            prefabs.ids.Clear();
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
                    GUI.enabled = true;
                    w           = GUILayout.Width(eGUI.indentWidth * 0.72f);
                    EditorGUILayout.ObjectField(prefabs.prefabs[i], typeof(GameObject), false, w);
                    w = GUILayout.Width(eGUI.indentWidth * 0.1f);
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
        GUILayout.BeginHorizontal();
        {
            sel = (GameObject) EditorGUILayout.ObjectField(sel, typeof(GameObject), false);
            if (sel == null) GUI.enabled = false;
            if (GUILayout.Button("Add")) {
                prefabs.AddPrefab(sel);
                EditorUtility.SetDirty(prefabs);
            }
        }
        GUILayout.EndHorizontal();
        GUI.enabled = true;
    }
}