using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Generation;

public abstract class Enemy : MonoBehaviour
{
    private Generator.RoomInfo room;
    public Generator.RoomInfo Room { set { room = value; } }

    public abstract void Activate();

    public abstract void Disable();


    protected virtual void OnDestroy()
    {
        if (room != null) room.enemies.Remove(this);
    }
}
