using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
public class Mob : MonoBehaviour
{

    #region public variables
    [Header("Mob Parameters")]
    public string mobName;
    public int health;
    public float speed; // movement speed
    public int power; // Scale for how many bullets to fire during attacks
    public float actionInterval;
    public float projectileForce;

    public GameObject keyDrop;

    [Header("Containers")]
    public GameObject[] projectiles;
    public string[] actionPool;
    public bool inOrder;
    public GameObject[] walkingPoints;
    public GameObject[] teleportPoints;

    #endregion

    #region private variables
    private string currentAction;
    private Transform playerPos;
    private Transform tf; // this mob's transform
    private Transform walkPoint; // When the mob has to walk, will walk to this point.
    private int currentActionPoolIndex; // If in order is enabled, then will progress in order through action pool.
    private bool isInvulnerable; // is this mob currently invulnerable? (after being hit)

    #endregion


    public void Start()
    {
        playerPos = GameObject.FindGameObjectWithTag("Player").transform;
        tf = this.GetComponent<Transform>();
        currentAction = "wait";
        isInvulnerable = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        string tag = other.gameObject.tag;
        if (tag == "Player")
        {
            startPhase();
        }

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        string tag = other.gameObject.tag;
        if (tag == "Player")
        {
            stop();
        }

    }

    // Start the mob
    public void startPhase()
    {
        InvokeRepeating("pickAction", actionInterval, actionInterval);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "PlayerProjectile" && !isInvulnerable)
        {
            this.takeDamage(other.gameObject.GetComponent<Projectile>().damage);
            //StartCoroutine("Invulnerable");

            StartCoroutine(GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraShake>().Shake(.15f, .4f));
            Vector3 knockbackDirection = this.transform.position - other.gameObject.transform.position;
            this.GetComponent<Rigidbody2D>().AddForce(Vector3.Normalize(knockbackDirection) * other.gameObject.GetComponent<Projectile>().knockbackForce, ForceMode2D.Impulse);
        }
    }

    // Takes damage. If it drops the HP below 0, stop the phase and start the next phase.
    public void takeDamage(int damage)
    {
        health -= damage;
        currentAction = "wait";
        if (health <= 0)
        {
            death();
        }
    }

    void death() {
        GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().addKillCount(1);

        this.GetComponent<SpawnOnDestroy>().Spawn();
        attemptKeyDrop();
        Destroy(gameObject);

    }

    // Stops the Mmob
    void stop()
    {
        currentAction = "wait";
        CancelInvoke();
    }

    // Update is called once per frame
    void Update()
    {

        // Assumes the sprite faces right
        facePlayer();

        if (currentAction == "wait")
        {
            this.GetComponent<Animator>().SetBool("idle", true);
        }
        else
        {
            this.GetComponent<Animator>().SetBool("idle", false);
        }

        switch (currentAction)
        {
            // Hybrid
            case "hunt": // chase and shoot periodically
                StartCoroutine(turret(projectiles[Random.Range(0, (projectiles.Length - 1))], 2f));
                currentAction = "chase";
                break;

            case "kite": // runs away and shoots
                StartCoroutine(turret(projectiles[Random.Range(0, (projectiles.Length - 1))], 2f));
                currentAction = "flee";
                break;

            // Movement
            case "wait":
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
                tf.position = teleportPoints[Random.Range(0, (teleportPoints.Length - 1))].transform.position;
                currentAction = "wait";
                break;

            // Attacks
            case "ring":
                attackRing(projectiles[Random.Range(0, (projectiles.Length - 1))], Random.Range(power, power * 3));
                currentAction = "wait";
                break;
            case "circle":
                StartCoroutine(attackCircle(projectiles[Random.Range(0, (projectiles.Length - 1))], Random.Range(power, power * 2), 0.05f));
                currentAction = "wait";
                break;
            case "dualCircle":
                StartCoroutine(attackDualCircle(projectiles[Random.Range(0, (projectiles.Length - 1))], Random.Range(power, power * 2), 0.05f));
                currentAction = "wait";
                break;
            case "sniper":
                shootPlayer(projectiles[Random.Range(0, (projectiles.Length - 1))], 3f);
                currentAction = "wait";
                break;
            case "slug":
                shootPlayer(projectiles[Random.Range(0, (projectiles.Length - 1))], 0.25f);
                currentAction = "wait";
                break;
            case "shotgun":
                attackShotgun(projectiles[Random.Range(0, (projectiles.Length - 1))], Random.Range(1, power), 25f);
                currentAction = "wait";
                break;
            case "spray":
                StartCoroutine(attackSpray(projectiles[Random.Range(0, (projectiles.Length - 1))], Random.Range(power, power * 2), 15f, 0.05f));
                currentAction = "wait";
                break;
            case "pulse":
                StartCoroutine(attackPulse(projectiles[Random.Range(0, (projectiles.Length - 1))], Random.Range(power, power * 3), 0.25f));
                currentAction = "wait";
                break;
            case "turret":
                StartCoroutine(turret(projectiles[Random.Range(0, (projectiles.Length - 1))], 0.5f));
                currentAction = "wait";
                break;
        }
    }

    void pickAction()
    {
        // If in order, pick the next action,
        // else pick a random action.
        int index;
        if (inOrder)
        {
            index = currentActionPoolIndex;
            currentActionPoolIndex++;
            if (currentActionPoolIndex > actionPool.Length - 1)
            {
                currentActionPoolIndex = 0;
            }
        }
        else
        {
            index = Random.Range(0, (actionPool.Length));
        }
        currentAction = actionPool[index];
        if (currentAction == "walk")
        {
            walkPoint = walkingPoints[Random.Range(0, (walkingPoints.Length - 1))].transform;
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

    // Assumes the sprite is facing right.
    void facePlayer()
    {
        if (playerPos.position.x < tf.position.x)
        {
            this.GetComponent<SpriteRenderer>().flipX = true;
        }
        else
        {
            this.GetComponent<SpriteRenderer>().flipX = false;
        }
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

    // Will attempt to drop the key to exit the floor.
    // Likelihood of dropping the key increases as more enemies are killed.
    void attemptKeyDrop() {
        if (GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().getFloorKey()) {
            return;
        }

        GameObject[] remainingEnemies = GameObject.FindGameObjectsWithTag("Enemy");

        if (Random.Range(0, remainingEnemies.Length) == 0) {
            Instantiate(keyDrop, this.transform.position, this.transform.rotation);
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


    // Makes the mob unable to collide with "Player" or "PlayerProjectile layer, as well as turn 
    // partially transparent for a given amount of time.
    IEnumerator Invulnerable()
    {
        isInvulnerable = true;
        this.GetComponent<BoxCollider2D>().enabled = false;
        gameObject.GetComponent<SpriteRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        yield return new WaitForSeconds(0.5f);
        gameObject.GetComponent<SpriteRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        isInvulnerable = false;
        this.GetComponent<BoxCollider2D>().enabled = true;
        yield return null;
    }

    #endregion
}
