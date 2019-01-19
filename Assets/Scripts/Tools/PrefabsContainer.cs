using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class PrefabsContainer {
    private static readonly List<GameObject> prefabs = new List<GameObject>();
    private static readonly List<int>        IDs     = new List<int>();

    public static void AddPrefab(GameObject prefab, int id) {
        if (!IDs.Contains(id)) {
            prefabs.Add(prefab);
            IDs.Add(id);
        }
    }

    public static void AddRange(GameObject[] _prefabs, int[] _ids) {
        prefabs.AddRange(_prefabs);
        IDs.AddRange(_ids);
    }

    public static void RemovePrefab(int id) {
        if (IDs.Contains(id)) {
            var index = IDs.IndexOf(id);
            prefabs.RemoveAt(index);
            IDs.RemoveAt(index);
        }
    }

    public static void ClearPrefabs() {
        prefabs.Clear();
        IDs.Clear();
    }

    public static GameObject GetPrefab(int id) {
        return !IDs.Contains(id) ? null : prefabs[IDs.IndexOf(id)];
    }

    public static int GetPrefabID(GameObject prefab) {
        if (!prefabs.Contains(prefab)) return -1;
        return IDs[prefabs.IndexOf(prefab)];
    }

    public static int[] GetIDs() {
        return IDs.ToArray();
    }

    public static GameObject[] GetPrefabs() {
        return prefabs.ToArray();
    }

    static PrefabsContainer() {
        LoadPrefabs();
    }

    public static void LoadPrefabs(string data = "Prefabs") {
        var database = (PrefabsList) Resources.Load(data);
        if (database != null)
            database.UpdateContainer();
    }
}