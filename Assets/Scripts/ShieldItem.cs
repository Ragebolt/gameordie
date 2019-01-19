using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldItem : MonoBehaviour
{
    public GameObject shieldPrefab;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "Player") return;

        PlayerController.Instance.TakeShield(shieldPrefab);
        Destroy(gameObject);
    }
}
