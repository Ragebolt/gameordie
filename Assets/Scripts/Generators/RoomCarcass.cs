using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Generation
{
    public class RoomCarcass : MonoBehaviour
    {
        public GameObject[] doorwayBlockers;
        public GameObject doorPrefab;

        private Animator[] doors = new Animator[4];


        public void CreateDoorway(int direction)
        {
            doorwayBlockers[direction].SetActive(false);
            var dbTransform = doorwayBlockers[direction].transform;

            var door = Instantiate(doorPrefab, dbTransform.parent);
            var dTransform = door.transform;
            dTransform.localPosition = dbTransform.localPosition;
            dTransform.up = (Vector2)Generator.DirectionToVector(direction);
            doors[direction] = door.GetComponent<Animator>();
        }

        public void OpenDoors()
        {
            foreach (var door in doors) if (door != null) door.SetBool("Closed", false);
        }

        public void CloseDoors()
        {
            foreach (var door in doors) if (door != null) door.SetBool("Closed", true);
        }
    }
}