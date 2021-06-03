using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class FloorPortal : MonoBehaviour
{
    public bool isAlreadyActive;
    public GameObject firstHint;

    private TextMeshPro promptText;
    private bool canInteract;
    private Animator animator;
    private bool canSceneChange;

    // Start is called before the first frame update
    void Start()
    {
        GameObject hintObject = transform.Find("Hint").gameObject;
        promptText = hintObject.GetComponent<TextMeshPro>();
        promptText.enabled = false;
        canInteract = false;
        canSceneChange = false;
        animator = this.GetComponent<Animator>();
        if (isAlreadyActive)
        {
            canSceneChange = true;
            animator.SetBool("isOpen", true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // If the player interacts with it and they have the key to open the portal
        // and they haven't previously tried to open the portal.
        if (Input.GetKeyDown(KeyCode.E) && canInteract)
        {
            animator.SetBool("isOpen", true);
            promptText.enabled = false;
            GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().useFloorKey();
            Debug.Log("CAN SCENE CHANGE? " + canSceneChange);
            canSceneChange = true;
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.gameObject.tag == "Player")
        {
            if (other.gameObject.GetComponent<Player>().getFloorKey() && !canSceneChange)
            {
                promptText.enabled = true;
                canInteract = true;
                Destroy(firstHint);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            promptText.enabled = false;
            canInteract = false;
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        if (col.gameObject.tag == "Player" && canSceneChange)
        {
            player.currentFloor += 1;
            player.SavePlayer();

            // Every 10th floor, initiate a boss fight
            // Every 3rd floor, visit the shop
            // On floor 30, or multiples of 3 and 10, it's a boss fight
            // Any other time is a regular floor
            if (player.currentFloor % 10 == 0) {
                StartCoroutine(player.changeScene("BossBallisticGolem"));
            } else if (player.currentFloor % 3 == 0) {
                StartCoroutine(player.changeScene("Shop"));
            } else {
                StartCoroutine(player.changeScene("MainDungeon"));
            }
        }
    }
}
