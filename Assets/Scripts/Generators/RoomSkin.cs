using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Generation
{
    public class RoomSkin : MonoBehaviour
    {
        public GameObject[] doorwayBlockers;


        public void EnableDoorway(int direction)
        {
            doorwayBlockers[direction].SetActive(false);
        }
    }
}