using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTemplates : MonoBehaviour
{
    private int floorLevel = 1; // What floor are we on? This influences difficulty of floor.

    public GameObject[] bottomRooms;
    public GameObject[] topRooms;
    public GameObject[] leftRooms;
    public GameObject[] rightRooms;

    public GameObject closedRoom;

    public GameObject[] easyEncounters;
    public GameObject[] normalEncounters;
    public GameObject[] hardEncounters;
    public GameObject[] specialEncounters;
    public GameObject portal;

    public List<GameObject> rooms;


    public float waitTime; // How long should this wait to spawn all events.minibosses
    private bool spawnedBoss;
    private bool spawnedPortal; // Have we spawned the portal to leave the floor yet?
    //public GameObject boss;

    void Start()
    {
        Invoke("spawnEncounters", waitTime);
        Invoke("spawnPortal", waitTime);
    }

    void Update()
    {

        /*
        if (waitTime <= 0 && spawnedBoss == false)
        {
            for (int i = 0; i < rooms.Count; i++) {
                if (i == rooms.Count - 1) {
                    Instantiate(boss, rooms[i].transform.position, Quaternion.identity);
                    spawnedBoss = true;
                }
            }

        }
        else
        {
            waitTime -= Time.deltaTime;
        }
        */
    }

    // Will loop through list of rooms
    // and spawn in the correct number of random events
    void spawnEncounters()
    {
        Debug.Log("spawning encounters");
        floorLevel = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().currentFloor;
        Debug.Log("Current floor: " + floorLevel);
        
        // The probability of easy encounter spawns
        int easySpawns = floorLevel + 30;

        // The probability of normal encounter spawns
        int normalSpawns = (2 * floorLevel) + 22;

        // The probability of hard encounter spawns
        int hardSpawns = floorLevel * 3;

        // The amount of special encounter spawns
        //int specialSpawns = floorLevel % 2;

        int totalProbability = easySpawns + normalSpawns + hardSpawns;


        // Loop through the list of rooms
        // loop condition: room list isn't empty, or i hasn't exceeded total spawns yet
        // on every loop, spawn the spawns necessary, starting with special to hard, medium easy.
        // pick a random spawn from each category, and spawn it in the room.

        foreach (GameObject room in rooms)
        {
            if (room.GetComponent<Room>().encounterSpawnPoints.Length < 1)
            {
                continue;
            }

            GameObject encounter = null;

            int chosenProbability = Random.Range(1, totalProbability + 1);



            if (chosenProbability <= easySpawns)
            {
                // Easy encounter spawn
                encounter = easyEncounters[Random.Range(0, easyEncounters.Length)];
            }
            else if (chosenProbability <= (easySpawns + normalSpawns))
            {
                // If it's a normal encounter spawn, there is a 25% chance it could be a special encounter instead.
                if (Random.Range(0, 4) >= 3)
                {
                    encounter = specialEncounters[Random.Range(0, specialEncounters.Length)];
                }
                else
                {
                    encounter = normalEncounters[Random.Range(0, normalEncounters.Length)];
                }


            }
            else if (chosenProbability > (easySpawns + normalSpawns))
            {
                // Hard encounter spawn
                encounter = hardEncounters[Random.Range(0, hardEncounters.Length)];
            }

            if (encounter)
            {
                room.GetComponent<Room>().SpawnEncounter(encounter);
            }
        }

    }

    void spawnPortal()
    {
        for (int i = rooms.Count - 1; i >= 0; i--)
        {
            if (spawnedPortal || rooms[i].GetComponent<Room>().encounterSpawnPoints.Length < 1)
            {
                continue;
            }
            rooms[i].GetComponent<Room>().SpawnEncounter(portal);
            spawnedPortal = true;

        }
    }

}


