using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entities;

/// <summary>
/// Контроллер управления
/// </summary>
public class InputController : MonoBehaviour
{
    public static event System.Action OnActiveDefenceButton = () => { };
    public static event System.Action OnSuperAbilityButton = () => { };

    public static Vector3 Direction { get; private set; }
    public static event System.Action OnDirectionChaged = () => { };

    public static InputController Instance { get; private set; }

    [SerializeField] private KeyCode activeDefenceKey = KeyCode.Mouse0;
    [SerializeField] private KeyCode superAbilityKey = KeyCode.Mouse1;


    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(activeDefenceKey)) OnActiveDefenceButton();
        if (Input.GetKeyDown(superAbilityKey)) OnSuperAbilityButton();

        CalculateDirection();
    }

    private void CalculateDirection()
    {
        if (PlayerController.Instance == null) return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        Vector3 pos = PlayerController.Instance.Origin.position; pos.z = 0f;

        var oldDir = Direction;
        Direction = (mousePos - pos).normalized;

        if (oldDir != Direction) OnDirectionChaged();
    }
}