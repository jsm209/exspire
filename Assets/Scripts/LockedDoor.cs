using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LockedDoor : MonoBehaviour
{
    public GameObject roadblock; // What should be destroyed upon unlock?

    public int probability; // 1 in probability chance of appearing

    public GameObject hint; // The gameobject with all the prompts and hints

    public TextMeshPro costText; // The text object for the cost

    private bool playerInRange; // Is the player currently in range?

    private bool unlocked; // has this door been unlocked yet?

    private int cost;

    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        playerInRange = false;
        unlocked = false;
        hint.SetActive(false);

        player = GameObject.FindWithTag("Player");

        // If probability is 0, always spawn the locked door. Else, roll for it.
        if (Random.Range(0, probability) == probability - 1) {
            Kill();
        }

        // 1 in 6 chance of costing 2 minutes
        // 2 in 6 chance of costing 1 minute
        // 3 in 6 chance of costing 30 seconds
        int rand = Random.Range(0, 6);
        if (rand == 5) {
            cost = 120;
            costText.text = "2:00";
        } else if (rand >= 3) {
            cost = 60;
            costText.text = "1:00";
        } else {
            cost = 30;
            costText.text = "0:30";
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Unlock();
            player.GetComponent<Player>().addTime(-cost);
        }

        if (unlocked) {
            SpriteRenderer sprite = this.GetComponent<SpriteRenderer>();
            sprite.color = new Color(1f, 1f, 1f, Mathf.Lerp(sprite.color.a, 0.0f, Time.deltaTime));
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            hint.SetActive(true);
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            hint.SetActive(false);
            playerInRange = false;
        }
    }

    void Unlock()
    {
        unlocked = true;
        Destroy(roadblock);
        this.GetComponent<Animator>().SetBool("isUnlocked", true);
        hint.SetActive(false);
        Invoke("Kill", 2.0f);
    }

    public void Kill()
    {
        Destroy(gameObject);
    }
}
