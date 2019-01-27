using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Default Shield", menuName = "Shields/Default Shield")]
public class DefaultShield : ShieldBase
{
    [SerializeField] private float maxDistance;
    [SerializeField] private GameObject thrownShieldPrefab;

    private Vector3 startPoint;
    private ThrownShield thrownShield;

    private ShieldState state = ShieldState.InHands;
    private ShieldState State
    {
        get { return state; }
        set
        {
            state = value;
            if (value == ShieldState.Arrive) thrownShield.Speed *= 1.5f;
        }
    }
    private enum ShieldState
    {
        InHands,
        FlightsAway,
        Arrive
    }


    public override void OnBullet(Bullet bullet, Collision2D collision)
    {
        if (bullet.Creator == PlayerController.Instance.gameObject) return;

        //bullet.transform.up = Vector3.Reflect(bullet.transform.up, PlayerController.Instance.Direction2D);

        bullet.OnContact();
    }

    public override void StartAbility()
    {
        if (state != ShieldState.InHands) return;

        shieldRoot.SetActive(false);

        startPoint = transform.position;

        thrownShield = Instantiate(thrownShieldPrefab, transform.position, Quaternion.LookRotation(Vector3.forward, PlayerController.Instance.Direction2D)).GetComponent<ThrownShield>();

        thrownShield.ReturnShield = () => { state = ShieldState.Arrive; };

        state = ShieldState.FlightsAway;
    }

    public override void AbilityUpdate()
    {
        if (state == ShieldState.FlightsAway && Vector3.Distance(startPoint, thrownShield.transform.position) >= maxDistance)
        {
            state = ShieldState.Arrive;
        }

        else if (state == ShieldState.Arrive)
        {
            Vector3 dir = thrownShield.transform.position - transform.position;

            thrownShield.transform.up = -dir.normalized;

            if (Vector3.Distance(transform.position, thrownShield.transform.position) <= 0.5f)
            {
                state = ShieldState.InHands;

                shieldRoot.SetActive(true);

                Destroy(thrownShield.gameObject);
            }
        }
    }
}