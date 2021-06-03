using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnOnDestroy : MonoBehaviour
{
    public int probability;
    public GameObject spawn;
    public int amount;

    public void Spawn() {
        int chance = Random.Range(1, probability + 1);
        if (chance == probability) {
            for (int i = 0; i < amount; i++) {
                GameObject spawned = Instantiate(spawn, this.transform.position, this.transform.rotation);
                Rigidbody2D rb = spawned.GetComponent<Rigidbody2D>();
                rb.velocity = Random.onUnitSphere * Random.Range(0.1f, 1f) * 5.0f;
            }
            
        }
    }
}
