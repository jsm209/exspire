using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAbilities : MonoBehaviour
{
    #region public variables

    [Header("Player Related")]

    public GameObject player; // Player gameobject
    public Camera cam; // Camera to use in order to reference click positions
    private float baseProjectileForce; // Standard speed to fire projectiles
    private GameObject[] projectiles; // All the projectiles the player can switch between right now

    [Header("Ability Related")]

    public GameObject shield; // What to use for the shield ability

    #endregion

    #region private variables


    private Rigidbody2D body; // Will refer to "this" object's rigidbody
    private Transform firePoint; // Where to spawn the projectile
    private Vector2 mousePos; // Current mouse position.
    private int currentProjectileIndex;

    #endregion


    void Start()
    {
        // Update player attributes
        baseProjectileForce = GameObject.FindWithTag("Player").GetComponent<Player>().baseProjectileForce;
        projectiles = GameObject.FindWithTag("Player").GetComponent<Player>().projectiles;

        // body is literally the thing this script is attached to
        // firePoint is the transform for this object.
        body = GetComponent<Rigidbody2D>();
        firePoint = GetComponent<Transform>();
        currentProjectileIndex = 0;
        this.updateProjectileDisplay(projectiles[currentProjectileIndex]);
    }

    void Update()
    {

        Player player = GameObject.FindWithTag("Player").GetComponent<Player>();

        // If the player is currently dead, do not allow anything to happen
        if (player.health <= 0) {
            return;
        }

        // Update player attributes
        baseProjectileForce = player.baseProjectileForce;
        projectiles = player.projectiles;


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
            Projectile bullet = projectiles[currentProjectileIndex].GetComponent<Projectile>();
            if (tryToSpendKindles(bullet.kindleCost))
            {
                Shoot(projectiles[currentProjectileIndex], baseProjectileForce * bullet.speedMultiplier, firePoint.up);
                StartCoroutine(GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraShake>().Shake(.15f, .4f));
            }

        }

        // Shield ability
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (tryToSpendKindles(2))
            {
                Shield();
            }
        }

        if (Input.GetAxis("Mouse ScrollWheel") != 0f)
        {
            int curScroll = 0;
            float inputScroll = Input.GetAxis("Mouse ScrollWheel");
            if (inputScroll < 0.0f)
            {
                curScroll = -1;
            }
            else
            {
                curScroll = 1;
            }
            currentProjectileIndex += curScroll;
            if (currentProjectileIndex > projectiles.Length - 1)
            {
                currentProjectileIndex = 0;
            }
            else if (currentProjectileIndex < 0)
            {
                currentProjectileIndex = projectiles.Length - 1;
            }
            //Debug.Log(currentProjectileIndex);
            //Debug.Log(currentProjectileIndex);
            this.updateProjectileDisplay(projectiles[currentProjectileIndex]);
        }

        // Teleport ability
        /*
        if (Input.GetKeyDown(KeyCode.Mouse2))
        {
            if (tryToSpendKindles(2))
            {
                player.GetComponent<Transform>().position = mousePos;
            }
        }
        */
        #endregion
    }

    public void updateProjectileDisplay(GameObject projectile)
    {
        GameObject projectileIcon = GameObject.FindGameObjectWithTag("UI_CurrentPlayerProjectileImage");
        GameObject projectileName = GameObject.FindGameObjectWithTag("UI_CurrentPlayerProjectileName");
        GameObject projectileCost = GameObject.FindGameObjectWithTag("UI_CurrentPlayerProjectileCost");

        projectileIcon.GetComponent<Image>().sprite = projectile.GetComponent<SpriteRenderer>().sprite;
        projectileName.GetComponent<Text>().text = projectile.GetComponent<Projectile>().projectileName;
        projectileCost.GetComponent<Text>().text = projectile.GetComponent<Projectile>().kindleCost + "";
    }

    // Given a projectile gameobject and the force to shoot it at
    // will spawn in the bullet in the mouse direction.
    void Shoot(GameObject projectile, float force, Vector3 direction)
    {
        GameObject bullet = Instantiate(projectile, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.AddForce(direction * force, ForceMode2D.Impulse);
    }

    public void ShootRandomDirection()
    {
        Projectile bullet = projectiles[currentProjectileIndex].GetComponent<Projectile>();
        Vector3 direction = Random.onUnitSphere;
        Shoot(projectiles[currentProjectileIndex], baseProjectileForce * bullet.speedMultiplier, Vector3.Normalize(direction));
    }

    // Will spawn a personal shield and make the player wait inside the shield.
    public void Shield()
    {
        Instantiate(shield, firePoint.position, firePoint.rotation);
        //player.GetComponent<PlayerMovement>().wait(1);
    }

    // Will check if the player has enough kindles to perform the ability
    // and if so, will subtract it and return true. Else false.
    // Special case: If player is holding "Glass Cannon" item (itemID = 14) then
    // it will consume a second instead.
    bool tryToSpendKindles(int n)
    {
        int[] inventory = player.GetComponent<Player>().inventory;

        if (inventory[14] > 0)
        {
            player.GetComponent<Player>().addTime(-n);
            return true;
        }
        else if (player.GetComponent<Player>().kindles >= n)
        {
            player.GetComponent<Player>().addKindles(-n);
            return true;
        }
        return false;
    }

    // Returns the projectile the player is currently using
    public GameObject getCurrentProjectile()
    {
        return this.projectiles[currentProjectileIndex];
    }

    public string getCurrentProjectileName()
    {
        return this.projectiles[currentProjectileIndex].GetComponent<Projectile>().name;
    }

    public void replaceCurrentProjectile(GameObject newProjectile)
    {
        this.projectiles[currentProjectileIndex] = newProjectile;
    }
}
