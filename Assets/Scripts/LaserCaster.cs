using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserCaster : Enemy
{
    [SerializeField] private GameObject laserPrefab;

    public float angle;
    public float laserDamagePerSecond = 1f;
    public float laserDamageRate = 10f;

    private Laser laser;

    private bool isDisabled = false;


    void Start()
    {
        laser = Instantiate(laserPrefab, Vector3.zero, Quaternion.identity).GetComponent<Laser>();

        laser.StartPoint = transform.position;

		laser.SetToDirection(Quaternion.Euler(0f, 0f, transform.eulerAngles.z + angle) * Vector3.up);

        laser.Damage = laserDamagePerSecond / laserDamageRate;
        laser.LoopDamageRate = laserDamageRate;
        laser.Creator = gameObject;

        laser.StartLoopDamage();

        if (isDisabled) laser.Disable();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (laser != null) Destroy(laser.gameObject);
    }


    public override void Disable()
    {
        if (laser != null) laser.Disable();

        isDisabled = true;
    }

    public override void Activate()
    {
        if (laser != null) laser.Activate();

        isDisabled = false;
    }
}
