using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    [Header("Universal Stats")]
    public int damage;
    public float lifetime;
    public int pierces; // How many times can the bullet pierce?
    public float knockbackForce;

    [Header("Player Related Stats")]
    public string projectileName;
    public float speedMultiplier;
    public int kindleCost;

    private Rigidbody2D rb;
    [HideInInspector] public GameObject self;

    void Start()
    {
        self = this.gameObject;
        rb = this.GetComponent<Rigidbody2D>();
        if (lifetime != 0 || lifetime < 0)
        {
            Invoke("die", lifetime);
        }

    }

    void OnCollisionEnter2D(Collision2D other)
    {

        if (other.gameObject.CompareTag("Reflector"))
        {
            /*
            Vector2 wallNormal = other.contacts[0].normal;

            // Gets the pure direction of the reflection with the normal of the wall.
            Vector2 newDirection = Vector2.Reflect(rb.velocity, wallNormal).normalized;

            rb.velocity = newDirection * rb.velocity;*/
        }
        else if (other.gameObject.CompareTag("PlayerProjectile") || other.gameObject.CompareTag("EnemyProjectile"))
        {
            if (pierces != 0)
            {
                pierces--;
            }
            else
            {
                StartCoroutine(GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraShake>().Shake(.15f, .4f));
                Destroy(gameObject);
            }

        }
        else
        {
            Destroy(gameObject);
        }


    }

    public int getDamage()
    {
        return this.damage;
    }

    void die()
    {
        Destroy(gameObject);
    }

    public float getSpeed() {
        return this.speedMultiplier;
    }

}
