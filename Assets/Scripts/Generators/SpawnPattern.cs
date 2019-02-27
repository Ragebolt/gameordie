using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Generation
{
    [CreateAssetMenu(fileName = "New Pattern", menuName = "Spawn Pattern")]
    public class SpawnPattern : ScriptableObject
    {
        public Vector2Int[] sizes = new Vector2Int[0];
        public Vector2Int editorRoomSize = new Vector2Int();
        public RoomObject[] objects = new RoomObject[0];

        public List<GameObject> Generate(Vector2 roomSize, Vector2 roomCenter, Quaternion roomRotation, Transform roomTransform)
        {
            var gameObjects = new List<GameObject>();

            foreach (var obj in objects)
            {
                if (Random.Range(0f, 1f) > obj.spawnChance) continue;

                var rot = Quaternion.Euler(0f, 0f, Random.Range(obj.minRotation, obj.maxRotation)) * roomRotation;
                Vector2 roomLocalCenter = roomSize / 2f;
                Vector2 rectPos = RotateVectorAround(obj.spawnRect.position, new Vector2(.5f, .5f), roomRotation);
                Vector2 rectSize = RotateVectorAround(obj.spawnRect.size, Vector2.zero, roomRotation);
                Rect rect = new Rect(rectPos, rectSize);
                var spawnPoint = new Vector2(Random.Range(rect.xMin, rect.xMax), Random.Range(rect.yMin, rect.yMax));
                spawnPoint -= new Vector2(0.5f, 0.5f);
                spawnPoint *= roomSize;
                if (obj.snapToGrid) spawnPoint = Vector2Int.RoundToInt(spawnPoint);
                var go = Instantiate(obj.prefabData.Prefab, roomCenter + spawnPoint, rot, roomTransform);
                gameObjects.Add(go);

                //if (obj.useCustomConfig) go.GetComponent<IConfig>()?.SetConfig(obj.config);
            }
            return gameObjects;
        }

        private Vector2 RotateVectorAround(Vector2 vector, Vector2 center, Quaternion rotation)
             => -(Vector2)(rotation * (center - vector)) + center;
    }
}