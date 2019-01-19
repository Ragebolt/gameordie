using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Generation
{
    [System.Serializable]
    public class RoomObject
    {
        public PrefabData prefabData;
        public RectInt spawnRect = new RectInt() { width = 1, height = 1 };
        public float minRotation;
        public float maxRotation;
        [Range(0f, 1f)]
        public float spawnChance = 1f;

        public bool useCustomConfig;
        public string config;

        public virtual string GetConfig()
        {
            return config;
        }

        public virtual void TakeDataFrom(RoomObject roomObject)
        {
            prefabData = roomObject.prefabData;
            spawnRect = roomObject.spawnRect;
            minRotation = roomObject.minRotation;
            maxRotation = roomObject.maxRotation;
            spawnChance = roomObject.spawnChance;
        }
    }
}