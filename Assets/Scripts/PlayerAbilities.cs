using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilities : MonoBehaviour
{
    #region public variables

    [Header("Player Related")]

    public GameObject player; // Player gameobject
    public Camera cam; // Camera to use in order to reference click positions

    [Header("Short Range Projectile")]

    public GameObject shortRangeProjectile; // What to use for the short range projectile
    public float shortRangeProjectileForce = 20f; // How fast the short range projectile should move

    [Header("Ability Related")]

    public GameObject shield; // What to use for the shield ability

    #endregion

    #region private variables


    private Rigidbody2D body; // Will refer to "this" object's rigidbody
    private Transform firePoint; // Where to spawn the projectile
    private Vector2 mousePos; // Current mouse position.

    #endregion


    void Start()
    {
        // body is literally the thing this script is attached to
        // firePoint is the transform for this object.
        body = GetComponent<Rigidbody2D>();
        firePoint = GetComponent<Transform>();
    }

    void Update()
    {
        // Getting the angle formed by the mouse position and origin (body)
        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 lookDirection = mousePos - body.position;
        float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg - 90f;
        body.rotation = angle;
        firePoint.position = player.transform.position;

        #region Controlling Input 

        // Default fire ability
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot(shortRangeProjectile, shortRangeProjectileForce);
        }

        // Shield ability
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (hasEnoughKindles(2))
            {
                Shield();
            }
        }

        // Teleport ability
        if (Input.GetKeyDown(KeyCode.Mouse2))
        {
            if (hasEnoughKindles(2))
            {
                player.GetComponent<Transform>().position = mousePos;
            }
        }
        #endregion
    }

    // Given a projectile gameobject and the force to shoot it at
    // will spawn in the bullet in the mouse direction.
    void Shoot(GameObject projectile, float force)
    {
        GameObject bullet = Instantiate(projectile, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.AddForce(firePoint.up * force, ForceMode2D.Impulse);
    }

    // Will spawn a personal shield.
    void Shield()
    {
        Instantiate(shield, firePoint.position, firePoint.rotation);
    }

    // Will check if the player has enough kindles to perform the ability
    // and if so, will subtract it and return true. Else false.
    bool hasEnoughKindles(int n)
    {
        if (player.GetComponent<Player>().kindles >= n)
        {
            player.GetComponent<Player>().addKindles(-n);
            return true;
        }
        return false;
    }
}
