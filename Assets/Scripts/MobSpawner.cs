using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobSpawner : MonoBehaviour
{

    public GameObject spawn; // What to spawn
    public int numberOfSpawns; // How many to spawn at a time
    public float spawnRadius; // How far away from origin the objects can spawn
    public float spawnCooldown; // Cooldown between spawns

    private bool canTrigger; // Is this spawner off cooldown yet?
    private float currentCooldown; // The current cooldown time. 

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (currentCooldown <= 0.0f)
        {
            canTrigger = true;
        }
        else
        {
            currentCooldown -= Time.deltaTime;
            canTrigger = false;
        }
    }

    public void attemptSpawn()
    {
        if (canTrigger)
        {
            StartCoroutine("spawnGroup");
        }
    }

    // Spawns the ground of spawns randomly in an area.
    // Spawns each spawn with a random delay in between.
    IEnumerator spawnGroup()
    {
        canTrigger = false;
        currentCooldown = spawnCooldown;

        for (int i = 0; i < numberOfSpawns; i++)
        {
            Vector3 direction = new Vector3(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
            Vector3 position = this.transform.position + (direction * spawnRadius);
            Instantiate(spawn, position, this.transform.rotation);
            yield return new WaitForSeconds(Random.Range(0.0f, 0.5f));
        }
    }
}
