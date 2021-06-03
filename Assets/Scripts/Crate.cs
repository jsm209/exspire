using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate : MonoBehaviour
{
    public GameObject item;
    public int amount;

    private bool alreadyOpened;

    // Start is called before the first frame update
    void Start()
    {
        alreadyOpened = false;
    }

    // Update is called once per frame
    void Update()
    {

    }


    void OnCollisionEnter2D(Collision2D col)
    {

        if (col.gameObject.tag == "PlayerProjectile" && !alreadyOpened)
        {
            this.GetComponent<Animator>().SetTrigger("opened");
            Invoke("open", 1.3f);
            alreadyOpened = true;
        }

    }

    void open()
    {
        for (int i = 0; i < amount; i++) {
            this.spawn(item);
        }
    }

    void spawn(GameObject spawn)
    {
        GameObject spawned = Instantiate(spawn, transform.position, transform.rotation);
        Rigidbody2D rb = spawned.GetComponent<Rigidbody2D>();
        rb.velocity = Random.onUnitSphere * Random.Range(0.1f, 1f) * 5.0f;
    }

}
