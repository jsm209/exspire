using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeshiftCloud : MonoBehaviour
{
    public GameObject shapeshiftInto;
    public bool ignorePlayer; // ignores players and instead detects enemies

    private void OnTriggerEnter2D(Collider2D other)
    {
        string tag = other.gameObject.tag;

        

        if (ignorePlayer)
        {
            if ((tag == "Enemy" || tag == "EnemyProjectile") && !other.isTrigger)
            {
                Instantiate(shapeshiftInto, this.transform.position, this.transform.rotation);
                Destroy(gameObject);
            }

        }
        else
        {
            if (tag == "Player" || tag == "PlayerProjectile")
            {
                Instantiate(shapeshiftInto, this.transform.position, this.transform.rotation);
                Destroy(gameObject);
            }
        }

    }
}
