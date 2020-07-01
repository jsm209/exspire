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

public class BossPhase : MonoBehaviour
{

    #region public variables
    [Header("Boss Phase Parameters")]
    public string bossPhaseName = "";
    public int health;
    public float speed; // movement speed
    public int power; // Scale for how many bullets to fire during attacks
    public float actionInterval;
    public float projectileForce;
    public GameObject[] projectiles;
    public Sprite sprite;
    public string[] actionPool;
    #endregion

    #region private variables
    private string currentAction;
    private GameObject boss;
    private Transform tf; // boss's transform
    private Transform playerPos;
    #endregion

    // Start is called before the first frame update
    public void StartPhase(GameObject parentBossObject)
    {
        boss = parentBossObject;
        tf = boss.GetComponent<Transform>();
        playerPos = GameObject.FindGameObjectWithTag("Player").transform;
        InvokeRepeating("pickAction", actionInterval, actionInterval);
    }

    // Stops the phase
    void Stop()
    {
        CancelInvoke();
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Transform>().position = tf.position; // setting this bossPhase gameobject to follow the parent boss object.

        switch (currentAction)
        {
            case "wait":
                break;
            case "ring":
                attackRing(projectiles[Random.Range(0, (projectiles.Length))], Random.Range(power, power * 3));
                currentAction = "wait";
                break;
            case "circle":
                StartCoroutine(attackCircle(projectiles[Random.Range(0, (projectiles.Length))], Random.Range(power, power * 2), 0.05f));
                currentAction = "wait";
                break;
            case "dualCircle":
                StartCoroutine(attackDualCircle(projectiles[Random.Range(0, (projectiles.Length))], Random.Range(power, power * 2), 0.05f));
                currentAction = "wait";
                break;
            case "chase":
                tf.position = Vector2.MoveTowards(tf.position, playerPos.position, speed * Time.fixedDeltaTime);
                break;
            case "flee":
                tf.position = Vector2.MoveTowards(tf.position, playerPos.position, -speed * Time.fixedDeltaTime);
                break;
        }
    }

    void pickAction()
    {
        currentAction = actionPool[Random.Range(0, (actionPool.Length))];
        Debug.Log("Current action : " + currentAction);
    }

    #region behavioral helper functions
    // Fires a single projectile at an angle.
    void fireProjectileAtAngle(GameObject projectile, Vector3 degreeVector)
    {
        GameObject bullet = Instantiate(projectile, tf.position, tf.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.AddForce(degreeVector * projectileForce, ForceMode2D.Impulse);
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
    #endregion
}
