using System;
using UnityEngine;

[Serializable]
public class PrefabData {
    public int id = -1;

    public GameObject prefab {
        get { return PrefabsContainer.GetPrefab(id); }
    }
}