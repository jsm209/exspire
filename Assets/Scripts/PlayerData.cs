using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{

    public int maxHealth; // current player max health
    public int health; // current player health
    public int money;

    public int maxKindles = 3; // Max amount player can hold. (clip size)
    public float baseProjectileForce = 20f; // Standard speed to fire projectiles
    //public GameObject[] projectiles; // All the projectiles the player can switch between right now
    public float moveSpeed = 5f; // Speed at which to move at.
    public float rollSpeed = 250f; // Speed at which to roll at.
    public float rechargeDelay; // Seconds before charges begin to replenish.
    public float rechargeRate; // Seconds between replenishing a charge.
    public float invulnerabilitySeconds; // How many seconds after being hit that the player has iframes.

    public float[] position;
    public bool[] defeatedBosses;
    public float timeLeft;
    public int currentFloor;

    public int[] inventory; // Represents player inventory where index is itemID and value is quantity
    public float timeSurvived; // The total time that the player has survived
    public int kills; // The total amount of kills the player has

    public int[] projectiles;


    public PlayerData(Player player)
    {

        maxHealth = player.maxHealth;
        health = player.health;

        maxKindles = player.maxKindles;
        baseProjectileForce = player.baseProjectileForce;
        //projectiles = player.projectiles;
        moveSpeed = player.moveSpeed;
        rollSpeed = player.rollSpeed;
        rechargeDelay = player.rechargeDelay;
        rechargeRate = player.rechargeRate;
        invulnerabilitySeconds = player.invulnerabilitySeconds;

        position = new float[3];
        position[0] = player.transform.position.x;
        position[1] = player.transform.position.y;
        position[2] = player.transform.position.z;

        defeatedBosses = player.defeatedBosses;
        timeLeft = player.timeLeft;
        currentFloor = player.currentFloor;

        inventory = player.inventory;

        timeSurvived = player.timeSurvived;
        kills = player.kills;

        projectiles = player.getPlayerProjectileIDs();

        
    }

}

