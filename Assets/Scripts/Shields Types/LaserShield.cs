using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Laser Shield", menuName = "Shields/Laser Shield")]
public class LaserShield : ShieldBase
{
    [SerializeField] private float chargePerBullet;
    [SerializeField] private GameObject laserPrefab;
    [SerializeField] private float laserDamage;

    private float charge = 0f;


    public override void OnBullet(Bullet bullet, Collision2D collision)
    {
        Destroy(bullet.gameObject);

        charge += chargePerBullet;

        charge = Mathf.Clamp01(charge);
    }

    public override void StartAbility()
    {
        if (charge != 1f) return;

        charge = 0f;

        var go = Instantiate(laserPrefab);

        Laser laser = go.GetComponent<Laser>();

        laser.StartPoint = PlayerController.Instance.shootingPoint.position;
        laser.Creator = PlayerController.Instance.gameObject;
        laser.SetToDirection(PlayerController.Instance.Direction2D);
        laser.CauseDamage(laserDamage);

        Destroy(go, 0.2f);
    }

    public override void AbilityUpdate()
    {
        
    }
}
