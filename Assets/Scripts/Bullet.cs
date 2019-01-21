using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Generation;

public class Bullet : MonoBehaviour
{
    public GameObject Creator { get; set; }
    public Rigidbody2D Rigidbody { get { return rb; } }

    public float StartSpeed
    {
        get { return settings.startSpeed; }
        set { settings.startSpeed = value; }
    }

    private float speed;

    public float Acceleration { get; set; }
    public float RotateSpeed { get; set; }

    public float Damage
    {
        get { return settings.damage; }
        set { settings.damage = value; }
    }

    private int contactsCounter;
    private int ContactsCounter
    {
        get { return contactsCounter; }
        set
        {
            contactsCounter = value;
            if (value >= settings.contactsToDestroy + 1) Destroy(gameObject);
            renderer.color = gradient.Evaluate((float)contactsCounter / (settings.contactsToDestroy));
        }
    }

    public Gradient gradient;

    public Settings settings;
    [System.Serializable]
    public struct Settings
    {
        public float startSpeed;
        public float damage;
        public DestroyCondition destroyCondition;
        public int contactsToDestroy;
        public GameObject destroyEffect;
    }

    public enum DestroyCondition
    {
        AnyHit,
        DamagableHit,
        NoShieldHit,
        HitCount
    }

    private Vector2Int roomCoords;

    [SerializeField] private new SpriteRenderer renderer;
    [SerializeField] private Rigidbody2D rb;



    void Start ()
    {
        speed = StartSpeed;

        ContactsCounter = 0;

        roomCoords = Generator.Instance.GlobalToLocal(transform.position);

        rb.velocity = transform.up * speed;
    }
	
	void Update ()
    {
        //speed += Acceleration * Time.deltaTime;
        //transform.position += transform.up * speed * Time.deltaTime * 10f;

        if (roomCoords != Generator.Instance.GlobalToLocal(transform.position)) Destroy(gameObject);
	}


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject == Creator) return;

        if (collision.collider.tag == "Shield")
        {
            if (PlayerController.Instance.gameObject == Creator) return;

            if (Vector3.Dot(transform.up, PlayerController.Instance.Direction2D) > 0f) return;

            PlayerController.Instance.Shield.ReflectionModule.OnBullet(this, collision);
            return;
        }

        Damagable damagable = collision.collider.GetComponent<Damagable>();
        if (damagable != null)
        {
            damagable.GetDamage(Damage);
            if (settings.destroyCondition == DestroyCondition.DamagableHit) Destroy(gameObject);
        }

        if (settings.destroyCondition == DestroyCondition.NoShieldHit) Destroy(gameObject);

        OnContact();
    }

    public void OnContact()
    {
        switch (settings.destroyCondition)
        {
            case DestroyCondition.AnyHit:
                Destroy(gameObject);
                return;
            case DestroyCondition.HitCount:
                ContactsCounter++;
                break;
        }

        Creator = null;
    }

    private void OnDestroy()
    {
        if (settings.destroyEffect != null) Instantiate(settings.destroyEffect, transform.position, Quaternion.identity);
    }
}