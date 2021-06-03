using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fox : MonoBehaviour
{

    public float speed;

    private bool closeToPlayer;

    private Rigidbody2D rb;
    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
        anim = this.GetComponent<Animator>();
        closeToPlayer = true;

        GameObject.FindWithTag("PromptText").GetComponent<MeshRenderer>().sortingLayerName="text";
    }

    // Update is called once per frame
    void Update()
    {
        if (!closeToPlayer) {
            moveTowardsPlayer();
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            closeToPlayer = true;
            anim.SetBool("isRunning", false);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            closeToPlayer = false;
            anim.SetBool("isRunning", true);
        }
    }

    void moveTowardsPlayer()
    {
        Vector3 playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;
        Vector3 moveDir = playerPos - this.transform.position;
        rb.velocity = moveDir * speed;

        if (GameObject.FindGameObjectWithTag("Player").transform.position.x < this.transform.position.x) {
            this.GetComponent<SpriteRenderer>().flipX = true;
        } else {
            this.GetComponent<SpriteRenderer>().flipX = false;
        }
    }
}
