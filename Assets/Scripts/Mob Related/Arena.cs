using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Arena : MonoBehaviour
{
    // public string encounterTitle;
    // public string encounterSubtitle;

    public GameObject barrier;
    public float radius;
    public int barrierCount;

    public GameObject[] spawnpoints;
    public GameObject[] enemies;

    private List<GameObject> barriers = new List<GameObject>();
    private bool triggered;

    private Text titleText;
    private Text subtitleText;
    private float curTime;

    void Start()
    {
        //titleText = GameObject.FindWithTag("TitleText").GetComponent<Text>();
        //subtitleText = GameObject.FindWithTag("SubtitleText").GetComponent<Text>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (!triggered && other.gameObject.tag == "Player")
        {
            spawnEnemies();
            StartCoroutine(spawnArena(barrier, barrierCount, 0.015f, radius));
            triggered = true;
        }



    }

    void Update()
    {
        if (triggered)
        {
            curTime += Time.deltaTime;
        }

        if (curTime > 5.0f && areAllEnemiesDead())
        {
            deactivate();
        }
    }

    // Coroutine function for firing a spray of bullets in a circular motion.
    IEnumerator spawnArena(GameObject pillar, int amount, float delay, float distance)
    {
        //titleText.text = encounterTitle;
        //subtitleText.text = encounterSubtitle;

        
        int current = 1;
        while (current <= amount)
        {
            float degree = (360 / (float)amount) * current;
            float radians = degree * (Mathf.PI / 180);
            Vector3 degreeVector = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0);
            barriers.Add(Instantiate(pillar, distance * degreeVector + gameObject.transform.position, gameObject.transform.rotation));
            current = current + 1;
            yield return new WaitForSeconds(delay);
        }
        //spawnEnemies();
        StopCoroutine("spawnArena");
    }

    void spawnEnemies()
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i] = Instantiate(enemies[i], spawnpoints[Random.Range(0, (spawnpoints.Length))].transform);
        }
    }


    // Destroys the arena and barriers.
    void deactivate()
    {
        foreach (GameObject barrier in barriers)
        {
            Destroy(barrier);
        }

        // titleText.text = "";
        // subtitleText.text = "";

        Destroy(gameObject);

    }


    bool areAllEnemiesDead()
    {

        GameObject[] currentEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        return (currentEnemies.Length == 0);
    }
}
