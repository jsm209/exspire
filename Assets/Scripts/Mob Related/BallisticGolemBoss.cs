using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// General behavior:
// Alternates between two phases, slowly getting "faster" over time.
// "Faster" in the sense that it will shoot faster projectiles, more projectiles, and more often, and move faster.
// Phase A: Actively chases the player. Shoots unaimed projectiles and ground pounds.
// Phase B: Retreats to center of map, shoots a bunch of aimed projectiles while standing still.

// It will swap phases whenever hp % 20 == 0, so every 20 HP until it is killed.
// Every loop will increase speed and strength.

// Attacks:
// Ground pound: Pounds the ground, producing a dangerous circular area. Occurs whenever player is very close.
// Ring: Unaimed area attack, shoots a single ring of projectiles out.
// Shotgun: Aimed area attack, shoots a chunk of projectiles in direction of player.
// *NOTE: ATTACKRADIUS VARIABLE IS USED FOR ATTACK RADIUS FOR WHEN TO LEAP FOR GROUND POUND ATTACK.

public class BallisticGolemBoss : EnemyBoss
{
    public float actionDelay; // Time waited right after doing an attack (should decrease over time) 
    private float currentActionDelay; // The current time we're at

    public float actionStrength; // Multiplier for the amount of projectiles used in attacks (should increase over time)
    public float jumpForce; // The force at which to jump at the player when using a ground pound attack.

    public GameObject projectile; // The projectile this boss uses.

    public Transform centerOfArena; // The spot the boss will attempt to walk to during Phase B.

    public GameObject mobSpawner; // A mob spawner the boss can trigger during battle.


    private bool inPhaseA;

    // Start is called before the first frame update
    protected override void Start()
    {
        inPhaseA = true;
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (isDead) {
            this.GetComponent<Rigidbody2D>().centerOfMass = Vector2.zero;
            return;
        }

        facePlayer();
        updateHealthBar();

        // breaks up HP into chunks of 20, and will
        // toggle the phase bool back and forth every 20 hp.
        int curChunk = (this.hp - 1) / 20;

        // checks if it's odd, or if we're in the last chunk
        // curChunk == 0 forces us to end on PhaseA 
        if (curChunk % 2 != 0 || curChunk == 0)
        {
            inPhaseA = true;
        }
        else
        {
            inPhaseA = false;
        }

        // If we're in phase A, do jumping attacks periodically, and chase the player.
        if (inPhaseA && isPlayerInRadius(attackRadius, player.transform.position) && !currentlyAttacking)
        {
            StartCoroutine(poundAttack(this.hitboxes[0].GetComponent<Hitbox>(), 0.85f, 0.2f, this.anim, "attack2", this.actionDelay));

            // Attempts to spawn mobs
            mobSpawner.GetComponent<MobSpawner>().attemptSpawn();

        }
        else if (!inPhaseA && Vector2.Distance(this.transform.position, centerOfArena.position) < 5f && !currentlyAttacking)
        {
            if (Random.Range(0, 2) == 0)
            {
                StartCoroutine(shotgunAttack(Mathf.RoundToInt(this.actionStrength), this.actionStrength, 0.5f, this.anim, "attack1", this.actionDelay));
            }
            else
            {
                StartCoroutine(ringAttack(Mathf.RoundToInt(this.actionStrength), 0.5f, this.anim, "attack1", this.actionDelay));
            }

            

        }

        //Debug.Log("PHASE A: " + inPhaseA + " | CURRENTLY ATTACKING: " + currentlyAttacking);
    }

    protected override void FixedUpdate()
    {
        // Chases player during Phase A.
        if (inPhaseA && !currentlyAttacking)
        {
            this.anim.SetBool("move", true);
            base.Move();
        }
        else if (!inPhaseA && !currentlyAttacking)
        { // Walks towards center of map during Phase B.
            this.transform.position = Vector2.MoveTowards(transform.position, centerOfArena.position, moveSpeed * Time.deltaTime);
            this.anim.SetBool("move", true);

        }
        else
        { // Stops moving otherwise during attacking.
            this.anim.SetBool("move", false);
        }
    }

    protected override void takeDamage(int damage)
    {
        base.takeDamage(damage);

        // Gradually become faster and stronger as it takes damage
        this.actionStrength += damage * 0.5f;
        this.actionDelay -= damage * 0.02f;
    }

    // Lands on the ground and deals damage to player.
    // If HP is 40 or below, will begin to also spawn a ring of projectiles upon landing.
    IEnumerator poundAttack(Hitbox hitbox, float beginningDelay, float endDelay, Animator anim, string animationTriggerName, float cooldownDelay)
    {
        Debug.Log("Doing a pound attack");
        Invoke("leap", 0.4f);
        currentlyAttacking = true;
        anim.ResetTrigger(animationTriggerName);
        anim.SetTrigger(animationTriggerName);
        yield return new WaitForSeconds(beginningDelay);

        if (hitbox != null)
        {
            hitbox.active = true;
        }

        if (this.hp <= 40)
        {
            // Actually shoot
            for (float degree = 1; degree <= 360; degree += (360 / (float)actionStrength))
            {
                float radians = degree * (Mathf.PI / 180);
                Vector3 degreeVector = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0);
                fireProjectileAtAngle(this.projectile, degreeVector, actionStrength);
            }
        }

        yield return new WaitForSeconds(endDelay);

        if (hitbox != null)
        {
            hitbox.active = false;
        }

        anim.SetBool("move", false);
        yield return new WaitForSeconds(cooldownDelay);
        currentlyAttacking = false;
        StopCoroutine("poundAttack");

    }

    void leap()
    {
        this.GetComponent<Rigidbody2D>().AddForce(base.getDirectionTowardsPlayer() * jumpForce, ForceMode2D.Impulse);
    }

    // Shoots out a chunk of projectiles towards player.
    IEnumerator shotgunAttack(int amount, float spread, float beginningDelay, Animator anim, string animationTriggerName, float cooldownDelay)
    {
        currentlyAttacking = true;
        anim.ResetTrigger(animationTriggerName);
        anim.SetTrigger(animationTriggerName);
        yield return new WaitForSeconds(beginningDelay);

        // Actually shoot
        for (int i = 1; i <= amount; i++)
        {
            // Picks a target position to shoot at
            // based on the player's position offset by a random value.
            float offset = Random.Range(-1.0f, 1.0f) * spread;
            Vector3 randomlyShifted = new Vector3(base.player.transform.position.x + offset, base.player.transform.position.y + offset, 0);

            // Multiplies final vector by a multiplier to vary its force
            // This helps spread out the bullets.
            float multiplier = Random.Range(0.8f, 1.0f);
            Vector3 degreeVector = Vector3.Normalize(randomlyShifted - this.transform.position) * multiplier;
            fireProjectileAtAngle(this.projectile, degreeVector, actionStrength);
        }

        anim.SetBool("move", false);
        yield return new WaitForSeconds(cooldownDelay);
        currentlyAttacking = false;
        StopCoroutine("shotgunAttack");
    }

    // Shoots a single ring of projectiles out in all directions.
    IEnumerator ringAttack(int amount, float beginningDelay, Animator anim, string animationTriggerName, float cooldownDelay)
    {
        currentlyAttacking = true;
        anim.ResetTrigger(animationTriggerName);
        anim.SetTrigger(animationTriggerName);
        yield return new WaitForSeconds(beginningDelay);

        // Actually shoot
        for (float degree = 1; degree <= 360; degree += (360 / (float)amount))
        {
            float radians = degree * (Mathf.PI / 180);
            Vector3 degreeVector = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0);
            fireProjectileAtAngle(this.projectile, degreeVector, actionStrength);
        }

        anim.SetBool("move", false);
        yield return new WaitForSeconds(cooldownDelay);
        currentlyAttacking = false;
        StopCoroutine("ringAttack");
    }

    // Fires a single projectile at an angle.
    void fireProjectileAtAngle(GameObject projectile, Vector3 degreeVector, float projectileForce)
    {
        GameObject bullet = Instantiate(projectile, this.transform.position, this.transform.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.AddForce(degreeVector * projectileForce, ForceMode2D.Impulse);
    }
}
