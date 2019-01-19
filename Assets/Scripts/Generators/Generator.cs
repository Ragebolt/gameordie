using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Generation
{
    public class Generator : MonoBehaviour
    {
        public GameObject roomCarcassPrefab;
        public Vector2Int roomSize = Vector2Int.one;
        public int roomsCount;
        [System.Obsolete, HideInInspector] public float margin;
        [System.Obsolete, HideInInspector] public GameObject wallPrefab;
        [System.Obsolete, HideInInspector] public float wallWidth;
        [System.Obsolete, HideInInspector] public float doorWidth;

        [Space]
        public SpawnPattern[] enemiesSpawnPatterns;

        [Space]
        public GameObject[] roomsSkins;

        private readonly List<RoomInfo> roomsList = new List<RoomInfo>();

        public class RoomInfo
        {
            public Vector2Int position;
            public int direction;
            public GameObject GameObject { get { return Carcass.gameObject; } }
            public RoomCarcass Carcass { get; set; }
            public bool IsPassed { get; set; }
            public ObservableCollection<Enemy> enemies;

            public RoomInfo(Vector2Int position, int direction)
            {
                this.position = position;
                this.direction = direction;

                enemies = new ObservableCollection<Enemy>();
                enemies.CollectionChanged += (sender, e) => 
                {
                    if (enemies.Count == 0)
                    {
                        IsPassed = true;
                        Carcass.OpenDoors();
                    }
                };
            }
        }

        public static Generator Instance { get; private set; }


        private void Awake()
        {
            Instance = this;
            Generate();
            Render();
        }

        private void Start() { }

        public void Generate()
        {
            var curPos = new Vector2Int(0, 0);
            var dir = -1;
            roomsList.Add(new RoomInfo(curPos, dir));
            for (var i = 0; i < roomsCount - 1; i++)
            {
                Vector2Int newPos;
                var index = 0;
                do
                {
                    newPos = curPos;
                    dir = Random.Range(0, 4);
                    newPos += DirectionToVector(dir);
                    index++;
                    if (index == 20) goto end;
                } while (!CanSpawnRoom(newPos));
                curPos = newPos;
                roomsList.Add(new RoomInfo(curPos, dir));
            }
            end:
            Debug.Log(roomsList.Count + " Rooms Generated");
        }

        public void Render()
        {
            for (var i = 0; i < roomsList.Count; i++)
            {
                var pDir = -1;
                if (i > 0)
                    pDir = InvertDirection(roomsList[i].direction);
                var nDir = -1;
                if (i < roomsList.Count - 1)
                    nDir = roomsList[i + 1].direction;

                roomsList[i].Carcass = SpawnRoomCarcass(roomsList[i].position, pDir, nDir);

                var skin = SpawnRoomSkin(roomsList[i].GameObject.transform, pDir, nDir);

                if (i == 0)
                {
                    roomsList[i].IsPassed = true;
                    continue;
                }

                var enemiesSpawnPattern = enemiesSpawnPatterns[Random.Range(0, enemiesSpawnPatterns.Length)];
                var enemies = enemiesSpawnPattern.Generate(roomSize, roomsList[i].position, this);
                foreach (var enemy in enemies)
                {
                    var enemiesComponents = enemy.GetComponents<Enemy>();
                    foreach (var e in enemiesComponents)
                    {
                        e.Room = roomsList[i];
                        roomsList[i].enemies.Add(e);
                        e.Disable();
                    }
                }
            }
        }

        public RoomInfo GetRoomOnCoords(Vector3 coords)
        {
            return roomsList[GetRoomIdOnCoords(GlobalToLocal(coords))];
        }

        public void ForAllEnemies(Action<Enemy> action)
        {
            foreach (var room in roomsList)
                foreach (var enemy in room.enemies)
                    action(enemy);
        }

        private bool CanSpawnRoom(Vector2Int pos)
        {
            return GetRoomIdOnCoords(pos) == -1;
        }

        private int GetRoomIdOnCoords(Vector2Int coords)
        {
            for (var i = 0; i < roomsList.Count; i++)
                if (roomsList[i].position == coords)
                    return i;
            return -1;
        }

        private GameObject SpawnRoomSkin(Transform roomCarcass, int pDir, int nDir)
        {
            var skin = Instantiate(roomsSkins[Random.Range(0, roomsSkins.Length)], roomCarcass);
            skin.transform.localPosition = Vector3.zero;

            var controller = skin.GetComponent<RoomSkin>();
            if (pDir > -1) controller.EnableDoorway(pDir);
            if (nDir > -1) controller.EnableDoorway(nDir);

            return skin;
        }

        private RoomCarcass SpawnRoomCarcass(Vector2Int pos, int pDir, int nDir)
        {
            var go = Instantiate(roomCarcassPrefab, transform);
            go.transform.localPosition = LocalToGlobal(pos);

            var carcass = go.GetComponent<RoomCarcass>();
            if (pDir > -1) carcass.CreateDoorway(pDir);
            if (nDir > -1) carcass.CreateDoorway(nDir);

            return carcass;
        }

        /// <summary>
        /// Use SpawnRoom method
        /// </summary>
        [System.Obsolete]
        private GameObject GenerateRoom(Vector2Int pos, int pDir, int nDir)
        {
            var room = Instantiate(roomCarcassPrefab, transform);
            room.transform.localPosition = LocalToGlobal(pos);
            room.transform.localScale = (Vector2)roomSize;
            var spriteRenderer = room.transform.GetChild(0).GetComponent<SpriteRenderer>();
            spriteRenderer.transform.localScale = new Vector2(1f / roomSize.x, 1f / roomSize.y);
            spriteRenderer.size = (Vector2)roomSize;
            for (var i = 0; i < 4; i++)
                if (i == pDir)
                    GenerateDoor(room.transform, i);
                else if (i == nDir) GenerateDoor(room.transform, i);
                else GenerateWall(room.transform, i);
            return room;
        }

        [System.Obsolete]
        private void GenerateWall(Transform room, int dir)
        {
            var wall = Instantiate(wallPrefab, room);

            float roomLocalScale;
            if (dir == 0 || dir == 2) roomLocalScale = room.localScale.y;
            else roomLocalScale = room.localScale.x;

            wall.transform.localScale = new Vector3(1f / roomLocalScale * wallWidth, 1f);
            wall.transform.localPosition = (Vector2)DirectionToVector(dir) * 0.5f - 1f / roomLocalScale * wallWidth / 2f * (Vector2)DirectionToVector(dir);
            wall.transform.localRotation = Quaternion.Euler(0f, 0f, (dir - 1) * 90f);
        }

        [System.Obsolete]
        private void GenerateDoor(Transform room, int dir)
        {
            for (var i = -1; i < 2; i += 2)
            {
                var wall = Instantiate(wallPrefab, room);

                float roomLocalScaleVer;
                if (dir == 0 || dir == 2) roomLocalScaleVer = room.localScale.y;
                else roomLocalScaleVer = room.localScale.x;

                float roomLocalScaleHor;
                if (dir == 0 || dir == 2) roomLocalScaleHor = room.localScale.x;
                else roomLocalScaleHor = room.localScale.y;

                wall.transform.localScale = new Vector3(1f / roomLocalScaleVer * wallWidth, (1f - 1f / roomLocalScaleHor * doorWidth) / 2);

                Vector2 doorOffset;
                if (dir == 0 || dir == 2) doorOffset = new Vector2(1, 0);
                else doorOffset = new Vector2(0, 1);

                wall.transform.localPosition = (Vector2)DirectionToVector(dir) * 0.5f - 1f / roomLocalScaleVer * wallWidth / 2f * (Vector2)DirectionToVector(dir) +
                                               doorOffset * (0.5f - (1f - 1f / roomLocalScaleHor * doorWidth) / 4) * i;
                wall.transform.localRotation = Quaternion.Euler(0f, 0f, (dir - 1) * 90f);
            }
        }


        public Vector3 LocalToGlobal(Vector2Int vector)
        {
            return vector * (roomSize + new Vector2(margin, margin));
        }

        public Vector3 LocalToGlobal(Vector2 vector)
        {
            return vector * (roomSize + new Vector2(margin, margin));
        }

        public Vector2Int GlobalToLocal(Vector3 vector)
        {
            return Round(vector / (Vector2)roomSize);
        }

        public static Vector2Int DirectionToVector(int direction)
        {
            switch (direction)
            {
                case 0: return new Vector2Int(0, 1);
                case 1: return new Vector2Int(1, 0);
                case 2: return new Vector2Int(0, -1);
                case 3: return new Vector2Int(-1, 0);

                default: return Vector2Int.zero;
            }
        }

        private int InvertDirection(int direction)
        {
            switch (direction)
            {
                case 0: return 2;
                case 1: return 3;
                case 2: return 0;
                case 3: return 1;

                default: return -1;
            }
        }

        private Vector2Int Round(Vector2 vector)
        {
            var x = Mathf.RoundToInt(vector.x);
            var y = Mathf.RoundToInt(vector.y);

            return new Vector2Int(x, y);
        }
    }
}