using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Prefabs", menuName = "Prefabs list")]
public class PrefabsList : ScriptableObject {
    public List<GameObject> prefabs;
    public List<int>        ids;

    private void OnEnable() {
        if (prefabs == null) {
            prefabs = new List<GameObject>();
            ids     = new List<int>();
        }
        UpdateContainer();
    }

    public void UpdateContainer() {
        PrefabsContainer.ClearPrefabs();
        PrefabsContainer.AddRange(prefabs.ToArray(), ids.ToArray());
    }

    public void AddPrefab(GameObject prefab) {
        if (prefabs.Contains(prefab)) return;
        prefabs.Add(prefab);
        var id = prefabs.Count - 1;
        while (ids.Contains(id)) id++;
        ids.Add(id);
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
}