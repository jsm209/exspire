using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// SpiritBoxer behavior:
// Chase player
// When in range, attempt to punch
// A fourth of the time, attempt a long range punch

public class SpiritBoxer : Enemy
{
    protected override void doAttack()
    {
            if (Random.Range(0, 4) == 0)
            {
                // Long range punch
                StartCoroutine(base.startAttack(this.hitboxes[1].GetComponent<Hitbox>(), 0.2f, 0.1f, this.anim, "attack2", 1.0f));
            }
            else
            {
                // Normal punch
                StartCoroutine(base.startAttack(this.hitboxes[0].GetComponent<Hitbox>(), 0.5f, .1f, this.anim, "attack1", 1.0f));
            }
    }

    /*
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    protected override void Update()
    {
        base.facePlayer();

        if (isPlayerInRadius(attackRadius, player.transform.position) && !currentlyAttacking)
        {
            if (Random.Range(0, 4) == 0)
            {
                // Long range punch
                StartCoroutine(doAttack(hurtboxes[1], 0.2f, 0.2f, this.anim, "attack2", 1.0f));
            }
            else
            {
                // Normal punch
                StartCoroutine(doAttack(hurtboxes[0], 0.2f, 0.2f, this.anim, "attack1", 1.0f));
            }

        }
    }
    */
}
