using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShot : MonoBehaviour
{
    public float damage;
    public float knock;

    private void Start()
    {
        Destroy(gameObject, 3.75f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Stats._instance.TakeDamage(damage);
            Vector2 knockback = (Vector2)(collision.transform.position - transform.position).normalized * knock + Vector2.up * (knock * 1.5f);
            collision.gameObject.GetComponent<Rigidbody2D>().AddForce(knockback, ForceMode2D.Impulse);
        }
        Destroy(gameObject);
    }
}
