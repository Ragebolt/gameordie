using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Generation;
using UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private new SpriteRenderer renderer;
    [SerializeField] private Transform origin;
    [SerializeField] private Damagable damagable;

    public Transform Origin { get { return origin; } }
    public SpriteRenderer Renderer { get { return renderer; } }
    public float TimeScale { get; set; } = 1f;

    public Generator.RoomInfo Room { get; private set; }


    public static PlayerController Instance { get; private set; }


    private void Awake ()
    {
        Instance = this;
	}

    private void Start()
    {
        damagable.OnHealthChanged += UIController.HPBar.Refresh;
    }

    private void Update ()
    {
        Move();
        CheckRoom();
    }


    private void Move()
    {
        float xInput = Input.GetAxisRaw("Horizontal");
        float yInput = Input.GetAxisRaw("Vertical");

        Vector2 input = new Vector2(xInput, yInput);

        input.Normalize();

        rb.velocity = input * speed * TimeScale;
    }

    private void CheckRoom()
    {
        Generator.RoomInfo room = Generator.Instance.GetRoomOnCoords(origin.position);

        // При переходе в новую комнату...
        if (Room != room)
        {
            CameraController.Instance.target = room.GameObject.transform;

            if (Room != null)
            { 
                foreach (var enemy in Room.enemies)
                {
                    enemy.Disable();
                }
                Room.Carcass.OpenDoors();
            }

            foreach (var enemy in room.enemies)
            {
                enemy.Activate();
            }

            if (!room.IsPassed) room.Carcass.CloseDoors();
        }

        Room = room;
    }
}