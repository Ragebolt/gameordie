using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Absorbent Shield", menuName = "Shields/Absorbent Shield")]
public class AbsorbentShield : ShieldBase
{
    [SerializeField] private Bullet.Settings bulletsSettings;
    [SerializeField] private GameObject bulletPrefab;

    private int charge = 0;

    public override void OnBullet(Bullet bullet, Collision2D collision)
    {
        Destroy(bullet.gameObject);

        charge++;
    }

    public override void StartAbility()
    {
        if (charge == 0) return;

        charge--;

        var bullet = Instantiate(bulletPrefab, transform.position, Quaternion.LookRotation(Vector3.forward, PlayerController.Instance.Direction2D));

        Bullet controller = bullet.GetComponent<Bullet>();

        controller.Creator = PlayerController.Instance.gameObject;
        controller.settings = bulletsSettings;
    }
}