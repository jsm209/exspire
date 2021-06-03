using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobSpawn : MonoBehaviour
{
    public GameObject[] potentialSpawns;
    public GameObject spawnImage;

    private GameObject chosenMob;
    private bool spawnedMob;
    private float currentAlpha;

    void Start()
    {
        chosenMob = potentialSpawns[Random.Range(0, (potentialSpawns.Length))];
        Sprite chosenMobSprite = chosenMob.GetComponent<SpriteRenderer>().sprite;
        spawnImage.GetComponent<SpriteRenderer>().sprite = chosenMobSprite;
        currentAlpha = 0.0f;
    }

    void Update()
    {
        if (!spawnedMob)
        {
            spawnImage.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, currentAlpha);
            currentAlpha += 0.1f;
        }
        else
        {
            spawnImage.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.0f);
        }
    }

    void Spawn()
    {
        GameObject spawned = Instantiate(chosenMob, transform.position, transform.rotation);
        spawnedMob = true;
    }

    void DestroyMe()
    {
        Destroy(gameObject);
    }
}
