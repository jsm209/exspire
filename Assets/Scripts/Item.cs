using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public string itemName; // Name of this item
    public int itemID; // What is the ID number of this item?
    public int itemCost; // How many credits does this item cost?
    public int chanceToFail = 1; // Chance the item will fail (not do anything). If 0, it will always suceed.

    public int maxHealthStat; // current player max health
    public int healthStat; // current player health
    public int maxKindlesStat; // Max amount player can hold. (clip size)
    public float baseProjectileForceStat; // Standard speed to fire projectiles
    public float moveSpeedStat; // Speed at which to move at.
    public float rollSpeedStat; // Speed at which to roll at.
    public float rechargeDelayStat; // Seconds before charges begin to replenish. Expressed as percentage reduction.
    public float rechargeRateStat; // Seconds between replenishing a charge. Expressed as percentage reduction.
    public float invulnerabilitySecondsStat; // How many seconds after being hit that the player has iframes. Expressed as percentage reduction.

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // Checks for all collisions
    void OnCollisionEnter2D(Collision2D col)
    {

        if (col.gameObject.tag == "Player")
        {
            Player player = col.gameObject.GetComponent<Player>();
            if (player.timeLeft >= this.itemCost)
            {
                if (Random.Range(0, chanceToFail) == 0)
                {
                    player.maxHealth += this.maxHealthStat;
                    player.health += this.healthStat;
                    player.maxKindles += this.maxKindlesStat;
                    player.baseProjectileForce += this.baseProjectileForceStat;
                    player.moveSpeed += this.moveSpeedStat;
                    player.rollSpeed += this.rollSpeedStat;
                    player.rechargeDelay -= player.rechargeDelay * this.rechargeDelayStat;
                    player.rechargeRate -= player.rechargeRate * this.rechargeRateStat;
                    player.invulnerabilitySeconds += player.invulnerabilitySeconds * this.invulnerabilitySecondsStat;
                    player.timeLeft -= this.itemCost;

                    player.UpdateAbilityMovementVariables();

                    // Adds the item to the player's inventory and updates the display
                    player.inventory[itemID]++;
                    GameObject.FindWithTag("Inventory").GetComponent<InventoryUI>().displayInventory();

                    if (player.health <= 0 || player.maxHealth <= 0)
                    {
                        player.Death();
                    }

                }


                Destroy(gameObject);
            }

        }
    }
}
