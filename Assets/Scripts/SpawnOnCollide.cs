using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnOnCollide : MonoBehaviour
{
    public int probability;
    public GameObject spawn;
    public string otherTag;

    void OnCollisionEnter2D(Collision2D col)
    {
        // Will spawn if otherTag is not set, or if it is set,
        // check if it's the intended gameobject tag.
        if (otherTag == "" || col.gameObject.tag == otherTag)
        {
            int chance = Random.Range(1, probability);
            Debug.Log(chance);
            if (chance == probability - 1)
            {
                Instantiate(spawn, this.transform.position, this.transform.rotation);
            }
        }

    }
}
