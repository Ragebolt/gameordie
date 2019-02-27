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
        public Rect spawnRect = new Rect();
        public bool snapToGrid = true;
        public float minRotation = 0f;
        public float maxRotation = 0f;
        [Range(0f, 1f)]
        public float spawnChance = 1f;

        public bool useCustomConfig;
        public string config;
    }
}