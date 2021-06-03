using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

////////////////////////////
// Potential Boss Actions //
////////////////////////////

// wait - Do nothing, skips its action.

// Movements
// walk - Walks to a preset place in the map.
// flee - Actively runs away from player.
// chase - Chases down player.
// wander - wanders around.
// teleport - teleports to a random place in the map/ preset places in the map?

// Attacks
// ring - Shoots out a single ring of projectiles.
// shotgun - Shoots out a wave of projectiles in the general direction of the player.
// circle - Sprays a ring of projectiles.
// dualcircle - Sprays a ring of projectiles from both directions.
// pulse - Pulses rings of projectiles until the next action.
// random - Spews out bullets in random directions
// spray- Sprays out a stream of projectiles in a cone like shape in the generla direction of the player.
// sniper - Shoots out a single, really fast projectile aimed right at the player.
// slug - Shoots out a single, really slow projectile aimed right at the player.
public class BossPhase : MonoBehaviour
{

    #region public variables
    [Header("Boss Phase Parameters")]
    public GameObject parentBossObject;
    public string bossPhaseName;
    public int health;
    public float speed; // movement speed
    public int power; // Scale for how many bullets to fire during attacks
    public float actionInterval;
    public float projectileForce;
    public Sprite sprite;


    [Header("Containers")]
    public GameObject[] projectiles;
    public string[] actionPool;
    public bool inOrder;
    public GameObject[] walkingPoints;
    public GameObject[] teleportPoints;

    #endregion

    #region private variables
    private string currentAction;
    private Transform tf; // boss's transform
    private Transform playerPos;
    private Transform walkPoint; // When the boss has to walk, will walk to this point.
    private int currentActionPoolIndex; // If in order is enabled, then will progress in order through action pool.

    // UI Related
    private Text bossPhaseNameText;
    private Animator animator;

    #endregion


    public void Start()
    {
        currentActionPoolIndex = 0;

        bossPhaseNameText = GameObject.FindWithTag("SubtitleText").GetComponent<Text>();
        tf = parentBossObject.GetComponent<Transform>();
        playerPos = GameObject.FindGameObjectWithTag("Player").transform;
        animator = parentBossObject.GetComponent<Animator>();
    }

    // Actually starts the boss's phase.
    public void startPhase()
    {
        animator.SetBool("isMoving", false);
        InvokeRepeating("pickAction", actionInterval, actionInterval);
        bossPhaseNameText.text = bossPhaseName;
    }

    // Takes damage. If it drops the HP below 0, stop the phase and start the next phase.
    public void takeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            this.Stop();
            parentBossObject.GetComponent<Boss>().nextPhase();
        }
    }

    // Stops the phase
    public void Stop()
    {
        CancelInvoke();
        animator.SetBool("isMoving", false);
        animator.ResetTrigger("attack");
        bossPhaseNameText.text = "";
    }

    void pickAction()
    {
        animator.SetBool("isMoving", false);
        animator.ResetTrigger("attack");

        currentAction = "wait";

        // If in order, pick the next action,
        // else pick a random action.
        int index;
        if (inOrder) {
            index = currentActionPoolIndex;
            currentActionPoolIndex++;
            if (currentActionPoolIndex > actionPool.Length - 1) {
                currentActionPoolIndex = 0;
            }
        } else {
            index = Random.Range(0, (actionPool.Length));
        }
        currentAction = actionPool[index];

        if (currentAction == "walk")
        {
            walkPoint = walkingPoints[Random.Range(0, (walkingPoints.Length))].transform;
        }

        if (currentAction == "walk" || currentAction == "chase" || currentAction == "flee")
        {
            animator.SetBool("isMoving", true);
        }

        Debug.Log(currentAction);
    }

    public int getHealth()
    {
        return this.health;
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Transform>().position = tf.position; // setting this bossPhase gameobject to follow the parent boss object.

        switch (currentAction)
        {
            // Movement
            case "wait":
                animator.SetBool("isMoving", false);
                break;
            case "chase":
                tf.position = Vector2.MoveTowards(tf.position, playerPos.position, speed * Time.fixedDeltaTime);
                break;
            case "flee":
                tf.position = Vector2.MoveTowards(tf.position, playerPos.position, -speed * Time.fixedDeltaTime);
                break;
            case "walk":
                tf.position = Vector2.MoveTowards(tf.position, walkPoint.position, speed * Time.fixedDeltaTime);
                break;
            case "teleport":
                tf.position = teleportPoints[Random.Range(0, (teleportPoints.Length))].transform.position;
                currentAction = "wait";
                break;

            // Attacks
            case "ring":
                animator.SetTrigger("attack");
                attackRing(projectiles[Random.Range(0, (projectiles.Length))], Random.Range(power, power * 3));
                currentAction = "wait";
                break;
            case "circle":
                animator.SetTrigger("attack");
                StartCoroutine(attackCircle(projectiles[Random.Range(0, (projectiles.Length))], Random.Range(power, power * 2), 0.05f));
                currentAction = "wait";
                break;
            case "dualCircle":
                animator.SetTrigger("attack");
                StartCoroutine(attackDualCircle(projectiles[Random.Range(0, (projectiles.Length - 1))], Random.Range(power, power * 2), 0.05f));
                currentAction = "wait";
                break;
            case "sniper":
                animator.SetTrigger("attack");
                shootPlayer(projectiles[Random.Range(0, (projectiles.Length))], 5f);
                currentAction = "wait";
                break;
            case "slug":
                animator.SetTrigger("attack");
                shootPlayer(projectiles[Random.Range(0, (projectiles.Length ))], 0.25f);
                currentAction = "wait";
                break;
            case "shotgun":
                animator.SetTrigger("attack");
                attackShotgun(projectiles[Random.Range(0, (projectiles.Length))], Random.Range(1, power), 25f);
                currentAction = "wait";
                break;
            case "spray":
                animator.SetTrigger("attack");
                StartCoroutine(attackSpray(projectiles[Random.Range(0, (projectiles.Length))], Random.Range(power, power * 2), 15f, 0.05f));
                currentAction = "wait";
                break;
            case "pulse":
                animator.SetTrigger("attack");
                StartCoroutine(attackPulse(projectiles[Random.Range(0, (projectiles.Length))], Random.Range(power, power * 3), 2f));
                currentAction = "wait";
                break;
            case "turret":
                animator.SetTrigger("attack");
                StartCoroutine(turret(projectiles[Random.Range(0, (projectiles.Length))], 0.5f));
                currentAction = "wait";
                break;
            case "laser":
                animator.SetTrigger("attack");
                StartCoroutine(laser(projectiles[Random.Range(0, (projectiles.Length))], Random.Range(power, power), 4f, 0.05f));
                currentAction = "wait";
                break;
        }
    }

    #region behavioral helper functions

    // Fires a single projectile at an angle.
    void fireProjectileAtAngle(GameObject projectile, Vector3 degreeVector)
    {
        GameObject bullet = Instantiate(projectile, tf.position, tf.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.AddForce(degreeVector * projectileForce, ForceMode2D.Impulse);
    }

    void shootPlayer(GameObject projectile, float multiplier)
    {
        GameObject bullet = Instantiate(projectile, tf.position, tf.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.AddForce(Vector3.Normalize(playerPos.position - tf.position) * projectileForce * multiplier, ForceMode2D.Impulse);
    }

    // Picks a random spot within a certain distance.
    /*
    public static Vector3 RandomNavSphere (Vector3 origin, float distance, int layermask=-1) {
            Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * distance;
           
            randomDirection += origin;
           
            NavMeshHit navHit;
           
            NavMesh.SamplePosition (randomDirection, out navHit, distance, layermask);
           
            return navHit.position;
        }
        */
    #endregion


    #region Shooting Behaviors

    // Fires a ring of projectiles.
    void attackRing(GameObject projectile, int amount)
    {

        for (float degree = 1; degree <= 360; degree += (360 / (float)amount))
        {
            float radians = degree * (Mathf.PI / 180);
            Vector3 degreeVector = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0);
            fireProjectileAtAngle(projectile, degreeVector);
        }

    }

    // Fires a clump of projectiles in the general direction of the player.
    void attackShotgun(GameObject projectile, int amount, float spread)
    {
        for (int i = 1; i <= amount; i++)
        {
            // Picks a target position to shoot at
            // based on the player's position offset by a random value.
            float offset = Random.Range(-1.0f, 1.0f) * spread;
            Vector3 randomlyShifted = new Vector3(playerPos.position.x + offset, playerPos.position.y + offset, 0);

            // Multiplies final vector by a multiplier to vary its force
            // This helps spread out the bullets.
            float multiplier = Random.Range(0.8f, 1.0f);
            Vector3 degreeVector = Vector3.Normalize(randomlyShifted - tf.position) * multiplier;
            fireProjectileAtAngle(projectile, degreeVector);
        }
    }

    // Coroutine function for firing a spray of bullets in a circular motion.
    IEnumerator attackCircle(GameObject projectile, int amount, float delay)
    {
        int current = 1;
        while (current <= amount)
        {
            float degree = (360 / (float)amount) * current;
            float radians = degree * (Mathf.PI / 180);
            Vector3 degreeVector = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0);
            fireProjectileAtAngle(projectile, degreeVector);
            current = current + 1;
            yield return new WaitForSeconds(delay);
        }
        StopCoroutine("attackSpray");
    }

    // Coroutine function for firing a spray of bullets in a circular motion from both sides.
    IEnumerator attackDualCircle(GameObject projectile, int amount, float delay)
    {
        int current = 1;
        while (current <= amount)
        {
            float degree = (360 / (float)amount) * current;
            float radians = degree * (Mathf.PI / 180);
            Vector3 degreeVector1 = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0);
            Vector3 degreeVector2 = new Vector3(Mathf.Cos(radians + Mathf.PI), Mathf.Sin(radians + Mathf.PI), 0);
            fireProjectileAtAngle(projectile, degreeVector1);
            fireProjectileAtAngle(projectile, degreeVector2);
            current = current + 1;
            yield return new WaitForSeconds(delay);
        }
        StopCoroutine("attackDualSpray");
    }

    IEnumerator attackSpray(GameObject projectile, int amount, float spread, float delay)
    {
        int current = 1;
        while (current <= amount)
        {
            // Picks a target position to shoot at
            // based on the player's position offset by a random value.
            float offset = Random.Range(-1.0f, 1.0f) * spread;
            Vector3 randomlyShifted = new Vector3(playerPos.position.x + offset, playerPos.position.y + offset, 0);

            // Multiplies final vector by a multiplier to vary its force
            // This helps spread out the bullets.
            float multiplier = Random.Range(0.8f, 1.0f);
            Vector3 degreeVector = Vector3.Normalize(randomlyShifted - tf.position) * multiplier;
            fireProjectileAtAngle(projectile, degreeVector);

            current = current + 1;
            yield return new WaitForSeconds(delay);
        }
        StopCoroutine("attackSpray");
    }

    IEnumerator attackPulse(GameObject projectile, int amount, float interval)
    {
        float currentTime = 0f;
        while (currentTime <= actionInterval)
        {
            attackRing(projectile, amount);
            currentTime = currentTime + interval;
            yield return new WaitForSeconds(interval);
        }
        StopCoroutine("attackPulse");
    }

    IEnumerator turret(GameObject projectile, float interval)
    {
        float currentTime = 0f;
        while (currentTime <= actionInterval)
        {
            shootPlayer(projectile, 1.0f);
            currentTime = currentTime + interval;
            yield return new WaitForSeconds(interval);
        }
        StopCoroutine("turret");
    }

    IEnumerator laser(GameObject projectile, int amount, float multiplier, float delay)
    {
        int current = 1;
        while (current <= amount)
        {
            shootPlayer(projectile, multiplier);

            current = current + 1;
            yield return new WaitForSeconds(delay);
        }
        StopCoroutine("laser");
    }

    #endregion
}
