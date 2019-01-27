using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Generation
{
    [CreateAssetMenu(fileName = "New Pattern", menuName = "Spawn Pattern")]
    public class SpawnPattern : ScriptableObject
    {
        public RoomObject[] objects = new RoomObject[0];

        public List<GameObject> Generate(Vector2 roomSize, Vector2Int roomPos, Generator generator)
        {
            var gameObjects = new List<GameObject>();

            foreach (var obj in objects)
            {
                if (Random.Range(0f, 1f) > obj.spawnChance) continue;
                var rot = Quaternion.Euler(0f, 0f, Random.Range(obj.minRotation, obj.maxRotation));
                var spawnPoint = new Vector2(Random.Range(obj.spawnRect.xMin, obj.spawnRect.xMax + 1), Random.Range(obj.spawnRect.yMin, obj.spawnRect.yMax + 1));
                spawnPoint -= generator.roomSize * new Vector2(0.5f, 0.5f);
                var go = Instantiate(obj.prefabData.prefab, generator.LocalToGlobal(roomPos) + (Vector3)spawnPoint, rot);
                gameObjects.Add(go);
                if (obj.useCustomConfig) go.GetComponent<IConfig>()?.SetConfig(obj.config);
            }
            return gameObjects;
        }
    }
}