using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    private int health;
    private int numOfHearts;

    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    void Update()
    {
        health = GameObject.FindWithTag("Player").GetComponent<Player>().health;
        numOfHearts = GameObject.FindWithTag("Player").GetComponent<Player>().maxHealth;
        if (health > numOfHearts)
        {
            health = numOfHearts;
        }

        for (int i = 0; i < hearts.Length; i++)
        {

            if (i < health)
            {
                hearts[i].sprite = fullHeart;
            }
            else
            {
                hearts[i].sprite = emptyHeart;
            }

            if (i < numOfHearts)
            {
                hearts[i].enabled = true;
            }
            else
            {
                hearts[i].enabled = false;
            }
        }

    }

    // Updates the current amount of health
    public void updateHealth(int n) {
        health += n;
    }

    // Sets the current amount of health
    public void setHealth(int n) {
        health = n;
    }

    // Sets the max health
    public void setMaxHealth(int n) {
        numOfHearts = n;
    }

    // Gets the max health
    public int getMaxHealth() {
        return numOfHearts;
    }

    public bool isDead() {
        return (health <= 0);
    }

}
