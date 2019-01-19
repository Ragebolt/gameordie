using System;
using UnityEngine;

[Serializable]
public class PrefabData {
    public int id = -1;

    public GameObject Prefab {
        get { return PrefabsContainer.GetPrefab(id); }
    }
}