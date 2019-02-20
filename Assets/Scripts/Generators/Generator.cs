using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.Tilemaps;
using Entities;
using GeneralAlgorithms;
using Random = UnityEngine.Random;
using MapGeneration.Core.MapDescriptions;
using MapGeneration.Core.LayoutGenerators;
using MapGeneration.Core.Doors;
using MapGeneration.Core.Doors.DoorModes;
using GeneralAlgorithms.DataStructures.Polygons;
using GeneralAlgorithms.DataStructures.Common;
using MapGeneration.Utils;
using MapGeneration.Interfaces.Core.MapLayouts;

namespace Generation
{
    public class Generator : MonoBehaviour
    {
        public Transform roomsHolder;
        public GameObject wallPrefab;
        public GameObject doorPrefab;
        public int doorLength = 1;
        public Tilemap floorTilemap;
        public TileBase floorTile;

        [Space]
        [SerializeField] private BranchSettings mainBranchSettings;
        [SerializeField] private bool loopMainBranch = false;
        [SerializeField] private BranchSettings subBranchSettings;
        [SerializeField] private int subBranchesMinCount = 2;
        [SerializeField] private int subBranchesMaxCount = 3;
        [System.Serializable]
        private class BranchSettings
        {
            public int minLength = 1;
            public int maxLength = 2;
        }


        [SerializeField] private List<SpawnPattern> spawnPatterns = new List<SpawnPattern>();

        private readonly List<RoomInfo> roomsList = new List<RoomInfo>();
        private Dictionary<Vector2Int, List<SpawnPattern>> sizeToPatterns = new Dictionary<Vector2Int, List<SpawnPattern>>();
        private Dictionary<Vector2Int, Doorway> doorways = new Dictionary<Vector2Int, Doorway>();

        public class RoomInfo
        {
            public GameObject GameObject { get; }
            public IRoom<int> Room { get; }
            public Bounds Bounds
            { 
                get
                {
                    var bounds = new Bounds(GameObject.transform.position + new Vector3(1, 1), Vector3.zero);
                    bounds.Encapsulate((Vector2)bounds.min + (Size - Vector2Int.one));
                    return bounds;
                }
            }
            public Vector2Int Size
            {
                get
                {
                    var rect = Room.Shape.BoundingRectangle;
                    return new Vector2Int(rect.Width, rect.Height);
                }
            }
            public bool IsPassed { get; set; }
            public ObservableCollection<Enemy> enemies;
            public List<Doorway> doorways;

            public RoomInfo(IRoom<int> room, GameObject go)
            {
                Room = room;
                GameObject = go;

                enemies = new ObservableCollection<Enemy>();
                enemies.CollectionChanged += (sender, e) => 
                {
                    if (enemies.Count == 0)
                    {
                        IsPassed = true;
                        OpenDoors();
                    }
                };

                doorways = new List<Doorway>();
            }

            public void OpenDoors()
            {
                foreach (var door in doorways)
                {
                    door.Open();
                }
            }

            public void CloseDoors()
            {
                foreach (var door in doorways)
                {
                    door.Close();
                }
            }
        }
        public class Doorway
        {
            public List<GameObject> GameObjects { get; }

            public Doorway()
            {
                GameObjects = new List<GameObject>();
            }

            public void Open()
            {
                foreach (var go in GameObjects)
                {
                    go?.SetActive(false);
                }
            }

            public void Close()
            {
                foreach (var go in GameObjects)
                {
                    go?.SetActive(true);
                }
            }
        }

        public static Generator Instance { get; private set; }


        private void Awake()
        {
            Instance = this;

            GenerateUsingProceduralGeneration();
        }


        public RoomInfo GetRoomAt(Vector3 position)
            => roomsList.FirstOrDefault((info) => info.Bounds.Contains(position));

        public void ForAllEnemies(Action<Enemy> action)
        {
            foreach (var room in roomsList)
                foreach (var enemy in room.enemies)
                    action(enemy);
        }


        private MapDescription<int> GenerateMap()
        {
            var map = new MapDescription<int>();

            int nextNode = 0;
            nextNode = GenerateBranch(map, mainBranchSettings, nextNode, loopSelf: loopMainBranch);
            int mainBranchLength = nextNode;
            //nextNode = GenerateBranch(map, mainBranchSettings, nextNode, root: 0, end: mainBranchLength - 1);
            int subBranchesCount = Random.Range(subBranchesMinCount, subBranchesMaxCount);
            for (int i = 0; i < subBranchesCount; i++)
            {
                nextNode = GenerateBranch(map, subBranchSettings, nextNode, Random.Range(1, mainBranchLength - 2));
            }


            var doorMode = new OverlapMode(doorLength - 1, 1);

            foreach (var spawnPattern in spawnPatterns)
            {
                foreach (var size in spawnPattern.sizes)
                {
                    var roomSize = size + new Vector2Int(2, 2);
                    if (!sizeToPatterns.ContainsKey(roomSize)) sizeToPatterns.Add(roomSize, new List<SpawnPattern>());
                    sizeToPatterns[roomSize].Add(spawnPattern);
                }
            }

            foreach (var sizePair in sizeToPatterns)
            {
                var room = new RoomDescription(
                  GridPolygon.GetRectangle(sizePair.Key.x, sizePair.Key.y),
                  doorMode
                );

                map.AddRoomShapes(room);
            }

            map.AddCorridorShapes(new RoomDescription(
                  GridPolygon.GetRectangle(4, 4),
                  doorMode
                ));

            return map;
        }

        private int GenerateBranch(MapDescription<int> map, BranchSettings settings, int nextNode, int root = -1, int end = -1, bool loopSelf = false)
        {
            if (root == -1)
            {
                map.AddRoom(nextNode);
                root = nextNode;
                nextNode++;
            }

            int length = Random.Range(settings.minLength, settings.maxLength);

            int previousNode = root;
            for (int n = 0; n < length; n++)
            {
                map.AddRoom(nextNode + n);
                map.AddPassage(previousNode, nextNode + n);
                previousNode = nextNode + n;
            }

            if (loopSelf) end = root;
            if (end > -1 && length > 1) map.AddPassage(previousNode, end);

            return previousNode + 1;
        }

        public void GenerateUsingProceduralGeneration()
        {
            var mapGenerationTimer = new DebugTimer("Map Generation");
            MapDescription<int> map = GenerateMap();
            mapGenerationTimer.End();

            var getLayoutGeneratorTimer = new DebugTimer("Get Layout Generator");
            var layoutGenerator = LayoutGeneratorFactory.GetDefaultChainBasedGenerator<int>();
            getLayoutGeneratorTimer.End();

            var layoutGeneratorTimer = new DebugTimer("Generate Layout");
            layoutGenerator.EnableBenchmark(true);
            var layouts = layoutGenerator.GetLayouts(map, 1);
            layoutGeneratorTimer.End();


            Vector2 layoutOffset = Vector2.zero;
            foreach (var layout in layouts)
            {
                foreach (var room in layout.Rooms)
                {
                    var rect = room.Shape.BoundingRectangle;
                    Vector2 localRoomPos = new Vector2(room.Position.X, room.Position.Y);
                    Vector2 roomPos = localRoomPos + layoutOffset;
                    var go = new GameObject($"Room ({room.Node})");
                    go.transform.position = roomPos;
                    go.transform.parent = roomsHolder;

                    RoomInfo info = new RoomInfo(room, go);
                    roomsList.Add(info);

                    List<Vector2Int> doorsPoints = new List<Vector2Int>();
                    List<Vector2Int> doorsPointsWorld = new List<Vector2Int>();
                    List<int> doorsNodes = new List<int>();
                    foreach (var door in room.Doors)
                    {
                        if (doorsNodes.Contains(door.Node)) continue;

                        var points = new List<Vector2Int>();
                        foreach (var point in door.DoorLine.GetPoints()) points.Add(ToUnityVector(point));

                        Doorway doorway;
                        bool createDoors = false;
                        if (!IsDictionaryContainsAllKeys(doorways, points))
                        {
                            doorway = new Doorway();
                            createDoors = true;
                        }
                        else doorway = doorways[points[0]];
                        info.doorways.Add(doorway);

                        foreach (var point in points)
                        {
                            doorsPoints.Add(point - Vector2Int.RoundToInt(localRoomPos));
                            if (createDoors)
                            {
                                doorways.Add(point, doorway);
                                doorways[point].GameObjects.Add(CreateDoorAt(go.transform, point));
                            }
                        }
                        doorsPointsWorld.AddRange(points);
                        doorsNodes.Add(door.Node);
                    }

                    CreateWallsForRoom(go.transform, rect, doorsPoints);

                    var descRect = room.RoomDescription.Shape.BoundingRectangle;

                    Vector2Int roomSize = new Vector2Int(descRect.Width, descRect.Height);
                    Vector2Int worldRoomSize = new Vector2Int(rect.Width, rect.Height);

                    CreateFloorRect(new RectInt(Vector2Int.RoundToInt(roomPos) + Vector2Int.one, worldRoomSize - Vector2Int.one * 2));
                    foreach (var doorPos in doorsPointsWorld)
                    {
                        CreateFloorAt(doorPos);
                    }

                    if (room.Node != 0)
                    {
                        List<SpawnPattern> patterns = sizeToPatterns[roomSize];
                        SpawnPattern pattern = patterns[Random.Range(0, patterns.Count)];
                        Quaternion rotation = room.Transformations.Contains(Transformation.Rotate90) ? Quaternion.Euler(0f, 0f, 90f) : Quaternion.Euler(0f, 0f, 0f);
                        var enemies = pattern.Generate(worldRoomSize - new Vector2Int(2, 2), roomPos + (Vector2)worldRoomSize / 2f, rotation, go.transform);
                        foreach (var enemy in enemies)
                        {
                            List<Enemy> enemiesComponents = new List<Enemy>();
                            enemy.GetComponents(enemiesComponents);
                            enemy.transform.GetComponentsInChildren(enemiesComponents);
                            foreach (var e in enemiesComponents)
                            {
                                e.Room = info;
                                info.enemies.Add(e);
                                e.Disable();
                            }
                        }
                    }
                    else
                    {
                        PlayerController.Instance.Origin.position = roomPos + (Vector2)worldRoomSize / 2f + new Vector2(0.5f, 1f);
                        info.IsPassed = true;
                        info.OpenDoors();
                    }    
                }

                layoutOffset.x += 25f;
            }
        }

        private void CreateWallsForRoom(Transform room, GridRectangle rect, List<Vector2Int> ignorePoints)
        {
            int sizeX = rect.Width;
            int sizeY = rect.Height;

            CreateWallsLine(Vector2Int.zero, new Vector2Int(sizeX, 0), room, ignorePoints);
            CreateWallsLine(Vector2Int.zero, new Vector2Int(0, sizeY), room, ignorePoints);
            CreateWallsLine(new Vector2Int(0, sizeY), new Vector2Int(sizeX, sizeY), room, ignorePoints);
            CreateWallsLine(new Vector2Int(sizeX, 0), new Vector2Int(sizeX, sizeY + 1), room, ignorePoints);
        }

        private void CreateWallsLine(Vector2Int from, Vector2Int to, Transform parent = null, List<Vector2Int> ignorePoints = null)
        {
            Vector2 dir = to - from;

            for (int i = 0; i < dir.magnitude; i++)
            {
                Vector2 point = from + dir.normalized * i;
                if (ignorePoints != null && ignorePoints.Contains(Vector2Int.RoundToInt(point))) continue;
                Instantiate(wallPrefab, parent).transform.localPosition = point;
            }
        }

        private GameObject CreateDoorAt(Transform room, Vector2Int position)
        {
            var go = Instantiate(doorPrefab, room);
            go.transform.position = (Vector3Int)position;
            return go;
        }

        private void CreateFloorRect(RectInt rect)
        {
            for (int x = 0; x <= rect.size.x; x++)
            {
                for (int y = 0; y <= rect.size.y; y++)
                {
                    CreateFloorAt(rect.position + new Vector2Int(x, y));
                }
            }
        }

        private void CreateFloorAt(Vector2Int position)
        {
            floorTilemap.SetTile((Vector3Int)position, floorTile);
        }


        public static Vector2Int ToUnityVector(IntVector2 vector)
            => new Vector2Int(vector.X, vector.Y);

        public static bool IsDictionaryContainsAllKeys<TKey, TValue>(Dictionary<TKey, TValue> dictionary, List<TKey> keys)
        {
            if (dictionary.Count == 0) return false;

            foreach (var key in keys)
            {
                if (!dictionary.ContainsKey(key)) return false;
            }
            return true;
        }


        private class DebugTimer
        {
            float startTime;
            string lable;

            public DebugTimer(string lable)
            {
                this.lable = lable;
                Start();
            }

            public void Start()
            {
                startTime = Time.realtimeSinceStartup;
            }

            public void End()
            {
                Debug.Log($"{lable} Took {Time.realtimeSinceStartup - startTime} sec");
            }


            public static void Execute(string lable, System.Action action)
            {
                var timer = new DebugTimer(lable);
                action();
                timer.End();
            }
        }
    }
}