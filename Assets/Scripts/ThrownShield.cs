using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entities;

public class ThrownShield : MonoBehaviour
{
    [SerializeField] private float speed;
    public float Speed
    {
        get { return speed; }
        set { speed = value; }
    }

    [SerializeField] private float damage;
    public float Damage
    {
        get { return damage; }
        set { damage = value; }
    }

    public System.Action ReturnShield { get; set; }


    void Start ()
    {
		
	}
	
	void Update ()
    {
        transform.position += transform.up * Speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player") return;

        if (collision.tag == "Wall") { ReturnShield(); return; }


        Damagable damagable = collision.GetComponent<Damagable>();

        if (damagable != null) damagable.GetDamage(Damage);
    }
}
