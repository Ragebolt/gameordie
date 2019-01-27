using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Generation;
using Shields.Modules;

public class PlayerController : MonoBehaviour {
    [SerializeField] private     float          speed;
    [SerializeField] private     Rigidbody2D    rb;
    [SerializeField] private new SpriteRenderer renderer;
    [SerializeField] private     Transform      origin;

    [Space]
<<<<<<< HEAD
    public ShieldBase shield;
    [SerializeField] private GameObject     shieldRoot;
    [SerializeField] private Transform      visualShieldTransform;
    [SerializeField] private Transform      colliderShieldTransform;
    [SerializeField] private SpriteRenderer shieldRenderer;
    public                   GameObject     secondShield;
    public                   Transform      shootingPoint;
=======
    [SerializeField] private ShieldController shield;
    public ShieldController Shield { get { return shield; } }

    public Transform shootingPoint;
>>>>>>> Stalin

    public Vector3 Direction2D { get; private set; }

    public Transform Origin {
        get { return origin; }
    }

    private Generator.RoomInfo curRoom;

    public static PlayerController Instance { get; private set; }

    void Awake() {
        Instance = this;
    }

    void Update() {
        Move();
        Rotate();
        CheckRoom();

        if (Input.GetKeyDown(KeyCode.Mouse0)) shield.ActiveDefenceModule?.StartActiveDefence();
        if (Input.GetKeyDown(KeyCode.Mouse1)) shield.SpecialAbilityModule?.StartAbility();
    }

    private void Move() {
        float xInput = Input.GetAxisRaw("Horizontal");
        float yInput = Input.GetAxisRaw("Vertical");

        Vector2 input = new Vector2(xInput, yInput);

        input.Normalize();

        rb.velocity = input * speed;
    }

    private void Rotate() {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        Vector3 pos = origin.position;
        pos.z = 0f;

        Direction2D = (mousePos - pos).normalized;

<<<<<<< HEAD
        Quaternion rot = Quaternion.LookRotation(Vector3.forward, Direction2D);
        visualShieldTransform.localRotation = Quaternion.Euler(0f, -rot.eulerAngles.z + 270f, 0f);

        colliderShieldTransform.rotation = rot * Quaternion.Euler(0f, 0f, 90f);

        float angle = Mathf.Repeat(rot.eulerAngles.z, 360f);
        if (angle > 270f || angle < 90f) shieldRenderer.sortingOrder = renderer.sortingOrder - 1;
        else shieldRenderer.sortingOrder                             = renderer.sortingOrder + 1;
=======
        shield.SetDirection(Direction2D, renderer.sortingOrder);
>>>>>>> Stalin
    }

    private void CheckRoom() {
        Generator.RoomInfo room = Generator.Instance.GetRoomOnCoords(origin.position);

        // При переходе в новую комнату...
        if (curRoom != room) {
            CameraController.Instance.target = room.GameObject.transform;

            if (curRoom != null) {
                foreach (var enemy in curRoom.enemies) {
                    enemy.Disable();
                }
                curRoom.Carcass.OpenDoors();
            }

            foreach (var enemy in room.enemies) {
                enemy.Activate();
            }

            if (!room.IsPassed) room.Carcass.CloseDoors();
        }

        curRoom = room;
    }

<<<<<<< HEAD
    public void TakeShield(GameObject prefab) {
        Destroy(shield.gameObject);
        shield            = Instantiate(prefab, shieldRoot.transform).GetComponent<ShieldBase>();
        shield.shieldRoot = shieldRoot;
    }
=======

    //public void TakeShield(GameObject prefab)
    //{
    //    Destroy(shield.gameObject);
    //    shield = Instantiate(prefab, shieldRoot.transform).GetComponent<ShieldBase>();
    //    shield.shieldRoot = shieldRoot;
    //}
>>>>>>> Stalin
}