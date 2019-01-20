using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class PrefabsContainer {
    private class PrefabItem {
        public GameObject prefab;
        public int        id;
        public string     tag;
    }

    private static readonly List<PrefabItem> prefabs = new List<PrefabItem>();

    public static void AddPrefab(GameObject _prefab, int _id, string _tag) {
        if (!prefabs.Any(t => t.id == _id)) {
            prefabs.Add(new PrefabItem() {
                                             prefab = _prefab,
                                             id     = _id,
                                             tag    = _tag
                                         });
        }
    }

    public static void AddRange(GameObject[] _prefabs, int[] _ids, string[] _tags) {
        for (int i = 0; i < _prefabs.Length; i++)
            prefabs.Add(new PrefabItem() {
                                             prefab = _prefabs[i],
                                             id     = _ids[i],
                                             tag    = _tags[i]
                                         });
    }

    public static void RemovePrefab(int _id) {
        PrefabItem pi = prefabs.FirstOrDefault(t => t.id == _id);
        if (pi != null)
            prefabs.Remove(pi);
    }

    public static void ClearPrefabs() {
        prefabs.Clear();
    }

    public static GameObject GetPrefab(int _id) {
        return prefabs.First(t => t.id == _id).prefab;
    }

    public static int GetPrefabID(GameObject prefab) {
        var p = prefabs.FirstOrDefault(t => t.prefab == prefab);
        if (p == null) return -1;
        return p.id;
    }

    public static string GetTag(int _id) {
        if (_id < 0) return "<none>";
        return prefabs.First(t => t.id == _id).tag;
    }

    public static int[] GetIDs() {
        return prefabs.Select(t => t.id).ToArray();
    }

    public static GameObject[] GetPrefabs() {
        return prefabs.Select(t => t.prefab).ToArray();
    }

    public static int[] GetIDsByTag(string tag) {
        if (tag == "")
            return prefabs.Select(t => t.id).ToArray();
        return prefabs.Where(t => t.tag == tag)
                      .Select(y => y.id).ToArray();
    }

    public static string[] GetTags() {
        return prefabs.Select(t => t.tag).ToArray();
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