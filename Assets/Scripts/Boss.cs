using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boss : MonoBehaviour
{

    #region public variables
    [Header("Boss Stats")]
    public string bossName = "";
    public GameObject[] phases;

    [Header("Other")]
    // UI Related
    private Text bossNameText;
    #endregion

    #region private variables
    private int currentPhaseIndex;
    private BossPhase currentPhase;
    private bool calledPhaseStart;
    private int totalHealth;
    private int currentPhaseHealth;
    private GameObject healthbar;

    #endregion

    void Start()
    {

        bossNameText = GameObject.FindWithTag("TitleText").GetComponent<Text>();

        // Setup health bar
        this.healthbar = GameObject.FindWithTag("Healthbar");
        this.healthbar.SetActive(true);
        totalHealth = this.getTotalHealth();
        this.healthbar.GetComponent<Slider>().maxValue = totalHealth;
        this.updateHealthBar();

        currentPhaseIndex = 0;
        calledPhaseStart = false;
        bossNameText.text = bossName;

        if (phases.Length == 0)
        {
            death();
        }

    }

    void Update()
    {
        // if calledPhaseStart is false
        // Find the current phase and start it.
        // set calledPhaseStart to true.
        if (!calledPhaseStart)
        {
            currentPhase = phases[currentPhaseIndex].GetComponent<BossPhase>();
            currentPhaseHealth = currentPhase.getHealth();
            currentPhase.startPhase();
            calledPhaseStart = true;
        }
    }


    // Moves the boss to the next phase if it exists.
    // If it doesn't, the boss kills itself.
    public void nextPhase()
    {
        Destroy(phases[currentPhaseIndex]);
        currentPhaseIndex++;
        if (currentPhaseIndex > phases.Length - 1)
        {
            death();
        }
        else
        {
            calledPhaseStart = false; // Will allow the next phase to restart.
        }
    }

    // Kills the boss.
    void death()
    {
        bossNameText.text = "";
        this.healthbar.SetActive(false);
        Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "PlayerProjectile")
        {
            int damage = other.gameObject.GetComponent<Projectile>().damage;

            currentPhaseHealth -= damage;

            if (currentPhaseHealth <= 0) {
                int actualDamageDealt = currentPhaseHealth + damage; // handles remainders
                currentPhase.Stop();
                this.nextPhase();
                this.totalHealth -= actualDamageDealt;
            } else {
                this.totalHealth -= damage;
            }

            
            // Update boss health
            this.updateHealthBar();
        }
    }

    int getTotalHealth() {
        int total = 0;
        for (int i = 0; i < phases.Length; i++) {
            total += phases[i].GetComponent<BossPhase>().getHealth();
        }
        return total;
    }

    void updateHealthBar() {
        this.healthbar.GetComponent<Slider>().value = this.totalHealth;
    }
}
