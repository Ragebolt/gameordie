using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entities;

namespace Generation
{
    [System.Serializable]
    public class RoomObjectFollower : RoomObject
    {
        public Follower.Configuration config;

        public override string GetConfig()
        {
            return JsonUtility.ToJson(config);
        }

        public override void TakeDataFrom(RoomObject roomObject)
        {
            base.TakeDataFrom(roomObject);

            if (roomObject is RoomObjectFollower)
            {
                RoomObjectFollower roomObjectFollower = roomObject as RoomObjectFollower;

                config = roomObjectFollower.config;
            }
        }
    }
}