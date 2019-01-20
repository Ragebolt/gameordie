using UnityEngine;

public class PrefabItem {
    public GameObject prefab;
    public int        id;
    public string     tag;

    public PrefabItem(GameObject _prefab, int _id, string _tag) {
        prefab = _prefab;
        id     = _id;
        tag    = _tag;
    }
}