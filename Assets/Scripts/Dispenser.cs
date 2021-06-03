using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dispenser : MonoBehaviour
{

    public GameObject dispense;
    public Transform spawnpoint;
    public int cost;
    private bool canInteract = false;

    // Update is called once per frame
    void Update()
    {
        if (canInteract && Input.GetKeyDown(KeyCode.E))
        {
            Player player = GameObject.FindWithTag("Player").GetComponent<Player>();
            if (player.timeLeft >= cost)
            {
                player.timeLeft -= cost;
                dispenseItem();
            }
        }
    }

    void dispenseItem()
    {
        Instantiate(dispense, spawnpoint.position, spawnpoint.rotation);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            canInteract = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            canInteract = false;
        }
    }
}
