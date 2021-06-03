using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBoss : Enemy
{
    public Text bossNameText; // Text to display name of boss
    public GameObject healthbar;


    public int[] hpPhases;
    public int currentPhase = 0; // zero-indexed, tells boss which hpPhase to use.
    public bool isDead;

    // Start is called before the first frame update
    protected override void Start()
    {
        matchHPToPhases();
        bossNameText.text = this.name;
        initializeHealthBar();
        base.Start();
    }

    // Makes the boss take damage.
    protected override void takeDamage(int damage)
    {
        if (!isDead)
        {
            anim.ResetTrigger("hit");
            anim.SetTrigger("hit");
            this.hpPhases[currentPhase] -= damage;
            if (this.hpPhases[currentPhase] <= 0)
            {
                currentPhase++;
                if (currentPhase > hpPhases.Length - 1)
                {
                    this.anim.SetBool("dead", true);
                    isDead = true;
                }
            }
        }


        matchHPToPhases();
    }

    // Makes sure the HP phases match up to total hp.
    protected virtual void matchHPToPhases()
    {
        int total = 0;
        for (int i = 0; i < hpPhases.Length; i++)
        {
            total += hpPhases[i];
        }
        this.hp = total;
    }

    protected virtual void initializeHealthBar()
    {
        this.healthbar.GetComponent<Slider>().maxValue = this.hp;
    }

    protected virtual void updateHealthBar()
    {
        this.healthbar.GetComponent<Slider>().value = this.hp;
    }
}
