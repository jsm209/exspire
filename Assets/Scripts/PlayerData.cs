using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int kindles;
    public float timeLeft;
    public float[] position;
    public bool defeatedDenialBoss;

    public PlayerData(Player player) {
        kindles = player.kindles;
        timeLeft = player.timeLeft;

        position = new float[3];
        position[0] = player.transform.position.x;
        position[1] = player.transform.position.y;
        position[2] = player.transform.position.z;

        defeatedDenialBoss = player.defeatedDenialBoss;
    }

}

