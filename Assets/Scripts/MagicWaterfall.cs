using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicWaterfall : MonoBehaviour
{
    private bool hasBeenUsed;

    public GameObject[] lights;

    // Start is called before the first frame update
    void Start()
    {
        hasBeenUsed = false;
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        string tag = other.gameObject.tag;
        if (tag == "Player" && !hasBeenUsed)
        {
            other.gameObject.GetComponent<Player>().health = other.gameObject.GetComponent<Player>().maxHealth;

            for (int i = 0; i < lights.Length; i++) {
                Destroy(lights[i]);
            }

            hasBeenUsed = true;
        }

    }
}
