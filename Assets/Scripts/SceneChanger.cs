using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{

    public string sceneName;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter2D(Collision2D col)
    {
        // If it's the player, save their progress, load the next scene
        if (col.gameObject.tag == "Player" && sceneName != "") {
            //Debug.Log("Saved and loaded new scene");
            col.gameObject.GetComponent<Player>().SavePlayer();
            SceneManager.LoadScene(sceneName);
        }
    }
}
