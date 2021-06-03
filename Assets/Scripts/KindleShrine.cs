using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KindleShrine : MonoBehaviour
{

    public GameObject bossDoor;
    private bool isActivated;

    void Start() {
        isActivated = false;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "PlayerProjectile" && !isActivated)
        {
            isActivated = true;
            gameObject.GetComponent<Animator>().SetBool("isActivated", isActivated);
            bossDoor.GetComponent<BossDoor>().incrementDoor();
            GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().addMaxKindles(1);
        }
    }
}
