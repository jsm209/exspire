using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    private RoomTemplates templates;
    public GameObject[] encounterSpawnPoints;

    // Start is called before the first frame update
    void Start()
    {
        templates = GameObject.FindGameObjectWithTag("Rooms").GetComponent<RoomTemplates>();
        templates.rooms.Add(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SpawnEncounter(GameObject spawn)
    {
        if (encounterSpawnPoints.Length > 0)
        {
            GameObject spawnPoint = encounterSpawnPoints[Random.Range(0, encounterSpawnPoints.Length)];
            Instantiate(spawn, spawnPoint.transform.position, spawn.transform.rotation);
        }

    }
}
