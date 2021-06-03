using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// This is a class for every regular enemy in the game.
// It contains basic attributes of the enemy and some built in behaviors.
public class Enemy : MonoBehaviour
{
    public string mobName; // Name of enemy
    public int hp; // Health of enemy
    public float moveSpeed; // Speed at which enemey will move at.
    public float detectionRadius; // Radius enemy will be aware of the player.
    public float attackRadius; // Radius enemy will attempt to attack the player.

    public GameObject keyDrop;
    public bool alwaysDropKey;

    public GameObject[] hitboxes;

    [HideInInspector] public GameObject player; // Player reference
    [HideInInspector] public SpriteRenderer sprite; // This enemy's sprite reference
    [HideInInspector] public Animator anim; // This enemy's animator reference
    [HideInInspector] public bool currentlyAttacking; // This is so the enemy won't move while attacking. 

    // Start is called before the first frame update
    protected virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        facePlayer();

        if (isPlayerInRadius(attackRadius, player.transform.position) && !currentlyAttacking)
        {
            doAttack();
        }
    }

    protected virtual void doAttack()
    {
        StartCoroutine(startAttack(hitboxes[0].GetComponent<Hitbox>(), 0.2f, 0.2f, this.anim, "attack1", 1.0f));
    }

    protected virtual void FixedUpdate()
    {

        if (isPlayerInRadius(detectionRadius, player.transform.position) && !currentlyAttacking)
        {
            this.anim.SetBool("move", true);
            Move();
        }
        else if (isPlayerOutsideRadius(detectionRadius, player.transform.position))
        {
            this.anim.SetBool("move", false);
        }

    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "PlayerProjectile")
        {
            this.takeDamage(other.gameObject.GetComponent<Projectile>().damage);

            StartCoroutine(GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraShake>().Shake(.15f, .4f));
            Vector3 knockbackDirection = this.transform.position - other.gameObject.transform.position;
            this.GetComponent<Rigidbody2D>().AddForce(Vector3.Normalize(knockbackDirection) * other.gameObject.GetComponent<Projectile>().knockbackForce, ForceMode2D.Impulse);
        }
    }

    // Main behavior functions

    // Defines the movement pattern for the enemy.
    protected virtual private void Move()
    {
        this.transform.position = Vector2.MoveTowards(transform.position, player.transform.position, moveSpeed * Time.deltaTime);
    }


    // General Helper Functions

    // Checks if the player is in a particular radius.
    protected virtual bool isPlayerInRadius(float radius, Vector3 targetPosition)
    {
        return (Vector2.Distance(this.transform.position, targetPosition) < radius);
    }

    // Checks if the player is outside a particular radius.
    protected virtual bool isPlayerOutsideRadius(float radius, Vector3 targetPosition)
    {
        return (Vector2.Distance(this.transform.position, targetPosition) > radius);
    }

    // Returns a vector3 towards the player.
    protected virtual Vector3 getDirectionTowardsPlayer()
    {
        return player.transform.position - this.transform.position;
    }

    // Makes the enemy take damage.
    protected virtual void takeDamage(int damage)
    {
        anim.ResetTrigger("hit");
        anim.SetTrigger("hit");
        this.hp -= damage;
        if (this.hp <= 0)
        {
            this.anim.SetBool("dead", true);
        }
    }

    // Will attempt to drop the key to exit the floor.
    // Likelihood of dropping the key increases as more enemies are killed.
    void attemptKeyDrop()
    {
        if (alwaysDropKey) {
            Instantiate(keyDrop, this.transform.position, this.transform.rotation);
            return;
        }

        if (GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().getFloorKey())
        {
            return;
        }

        GameObject[] remainingEnemies = GameObject.FindGameObjectsWithTag("Enemy");

        if (Random.Range(0, remainingEnemies.Length) == 0)
        {
            Instantiate(keyDrop, this.transform.position, this.transform.rotation);
        }
    }

    // Makes the enemy die.
    public void death()
    {
        attemptKeyDrop();
        GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().addKillCount(1);
        Destroy(gameObject);
    }

    // Makes the enemy face the player (assumes the enemy by default, faces right)
    protected virtual void facePlayer()
    {
        if (transform.position.x > player.transform.position.x)
        {
            this.transform.localScale = new Vector3(-Mathf.Abs(this.transform.localScale.x), this.transform.localScale.y, this.transform.localScale.z);
        }
        else
        {
            this.transform.localScale = new Vector3(Mathf.Abs(this.transform.localScale.x), this.transform.localScale.y, this.transform.localScale.z);
        }
    }

    // Behavioral Functions

    // Will do an attack
    // hurtbox - The box collider for the dangerous region of the attack
    // beginningDelay - How long to wait before enabling the hurtbox
    // endDelay - How long the duration the hurtbox stays active before begin disabled
    // anim - The animator for this attack
    // animationTriggerName - The trigger for this attack
    // cooldownDelay - How long the enemy sits there after finishing an attack
    protected virtual IEnumerator startAttack(Hitbox hitbox, float beginningDelay, float endDelay, Animator anim, string animationTriggerName, float cooldownDelay)
    {
        currentlyAttacking = true;
        anim.ResetTrigger(animationTriggerName);
        anim.SetTrigger(animationTriggerName);
        yield return new WaitForSeconds(beginningDelay);

        if (hitbox != null)
        {
            hitbox.active = true;
        }

        yield return new WaitForSeconds(endDelay);

        if (hitbox != null)
        {
            hitbox.active = false;
        }

        anim.SetBool("move", false);
        yield return new WaitForSeconds(cooldownDelay);
        currentlyAttacking = false;
        StopCoroutine("startAttack");
    }
}
