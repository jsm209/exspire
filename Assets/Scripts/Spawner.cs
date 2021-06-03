using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    // The object to spawn.
    public GameObject spawn;

    // Max speed to shoot off spawns.
    public float maxSpeed;

    // How long between each spawn.
    public float interval;

	// Use this for initialization
	void Start () {
        InvokeRepeating("Spawn", 2.0f, interval);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void Spawn() {
        GameObject spawned = Instantiate(spawn, transform.position, transform.rotation);
        Rigidbody2D rb = spawned.GetComponent<Rigidbody2D>();
        rb.velocity = Random.onUnitSphere * Random.Range(0.1f, 1f) * maxSpeed;
    }
}
