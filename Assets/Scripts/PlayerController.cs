using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Generation;
<<<<<<< HEAD
=======
using UI;
>>>>>>> Stalin

public class PlayerController : MonoBehaviour {
    [SerializeField] private     float          speed;
    [SerializeField] private     Rigidbody2D    rb;
    [SerializeField] private new SpriteRenderer renderer;
<<<<<<< HEAD
    [SerializeField] private     Transform      origin;

    [Space]
    public ShieldBase shield;
    [SerializeField] private GameObject     shieldRoot;
    [SerializeField] private Transform      visualShieldTransform;
    [SerializeField] private Transform      colliderShieldTransform;
    [SerializeField] private SpriteRenderer shieldRenderer;
    public                   GameObject     secondShield;
    public                   Transform      shootingPoint;

    public Vector3 Direction2D { get; private set; }

    public Transform Origin {
        get { return origin; }
    }
=======
    [SerializeField] private Transform origin;
    [SerializeField] private Damagable damagable;

    public Transform Origin { get { return origin; } }
    public SpriteRenderer Renderer { get { return renderer; } }
    public float TimeScale { get; set; } = 1f;

    public Generator.RoomInfo Room { get; private set; }
>>>>>>> Stalin

    private Generator.RoomInfo curRoom;

    public static PlayerController Instance { get; private set; }

<<<<<<< HEAD
    void Awake() {
        Instance = this;
    }

    void Update() {
=======

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
>>>>>>> Stalin
        Move();
        CheckRoom();
<<<<<<< HEAD

        if (Input.GetKeyDown(KeyCode.Mouse0)) shield.StartAbility();
=======
    }
>>>>>>> Stalin

        shield.AbilityUpdate();
    }

    private void Move() {
        float xInput = Input.GetAxisRaw("Horizontal");
        float yInput = Input.GetAxisRaw("Vertical");

        Vector2 input = new Vector2(xInput, yInput);

        input.Normalize();

<<<<<<< HEAD
        rb.velocity = input * speed;
    }

    private void Rotate() {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        Vector3 pos = origin.position;
        pos.z = 0f;

        Direction2D = (mousePos - pos).normalized;

        Quaternion rot = Quaternion.LookRotation(Vector3.forward, Direction2D);
        visualShieldTransform.localRotation = Quaternion.Euler(0f, -rot.eulerAngles.z + 270f, 0f);

        colliderShieldTransform.rotation = rot * Quaternion.Euler(0f, 0f, 90f);

        float angle = Mathf.Repeat(rot.eulerAngles.z, 360f);
        if (angle > 270f || angle < 90f) shieldRenderer.sortingOrder = renderer.sortingOrder - 1;
        else shieldRenderer.sortingOrder                             = renderer.sortingOrder + 1;
=======
        rb.velocity = input * speed * TimeScale;
>>>>>>> Stalin
    }

    private void CheckRoom() {
        Generator.RoomInfo room = Generator.Instance.GetRoomOnCoords(origin.position);

        // При переходе в новую комнату...
<<<<<<< HEAD
        if (curRoom != room) {
            CameraController.Instance.target = room.GameObject.transform;

            if (curRoom != null) {
                foreach (var enemy in curRoom.enemies) {
=======
        if (Room != room)
        {
            CameraController.Instance.target = room.GameObject.transform;

            if (Room != null)
            { 
                foreach (var enemy in Room.enemies)
                {
>>>>>>> Stalin
                    enemy.Disable();
                }
                Room.Carcass.OpenDoors();
            }

            foreach (var enemy in room.enemies) {
                enemy.Activate();
            }

            if (!room.IsPassed) room.Carcass.CloseDoors();
        }

        Room = room;
    }
<<<<<<< HEAD

    public void TakeShield(GameObject prefab) {
        Destroy(shield.gameObject);
        shield            = Instantiate(prefab, shieldRoot.transform).GetComponent<ShieldBase>();
        shield.shieldRoot = shieldRoot;
    }
=======
>>>>>>> Stalin
}