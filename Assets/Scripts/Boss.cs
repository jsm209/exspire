using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{

    #region public variables
    [Header("Boss Stats")]
    public string bossName = "";
    public GameObject[] phases;
    #endregion

    #region private variables
    private int currentPhaseIndex;
    private BossPhase currentPhase;
    private bool calledPhaseStart;

    #endregion

    void Start() {
        
        currentPhaseIndex = 0;
        calledPhaseStart = false;

        if (phases.Length == 0) {
            // death();
        }
        
    }

    void Update() {
        // if calledPhaseStart is false
        // Find the current phase and start it.
        // set calledPhaseStart to true.
        if (!calledPhaseStart) {
            currentPhase = phases[currentPhaseIndex].GetComponent<BossPhase>();
            currentPhase.StartPhase(this.gameObject);
            calledPhaseStart = true;
        }
    }


    // Moves the boss to the next phase if it exists.
    // If it doesn't, the boss kills itself.
    void nextPhase() {
        if (currentPhaseIndex > phases.Length - 1) {
            death();
        } else {
            currentPhaseIndex++;
            calledPhaseStart = false; // Will allow the next phase to restart.
        }
    }

    // Kills the boss.
    void death() {
        Destroy(gameObject);
    }
}
