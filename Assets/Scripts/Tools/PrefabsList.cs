using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Prefabs", menuName = "Prefabs list")]
public class PrefabsList : ScriptableObject {
    public List<GameObject> prefabs;
    public List<int>        ids;
    public List<string>     tags;

    private void OnEnable() {
        if (prefabs == null) {
            prefabs = new List<GameObject>();
            ids     = new List<int>();
            tags    = new List<string>();
        }
        UpdateContainer();
    }

    public void UpdateContainer() {
        PrefabsContainer.ClearPrefabs();
        while (tags.Count < prefabs.Count)
            tags.Add("");
        PrefabsContainer.AddRange(prefabs.ToArray(), ids.ToArray(), tags.ToArray());
    }

    public void AddPrefab(GameObject prefab, string tag) {
        if (prefabs.Contains(prefab)) return;
        prefabs.Add(prefab);
        var id = prefabs.Count - 1;
        while (ids.Contains(id)) id++;
        ids.Add(id);
        tags.Add(tag);
        UpdateContainer();
    }

    public void RemovePrefab(int id) {
        var index = ids.IndexOf(id);
        prefabs.RemoveAt(index);
        ids.RemoveAt(index);
        UpdateContainer();
    }

    public void RemovePrefabAt(int index) {
        prefabs.RemoveAt(index);
        ids.RemoveAt(index);
    }

    public void Clear() {
        prefabs.Clear();
        ids.Clear();
        tags.Clear();
    }
}