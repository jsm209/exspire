using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyedByWalls : MonoBehaviour
{

    void Start() {
        if (CheckOverlap()) {
            Destroy(gameObject);
        }
    }

    private bool CheckOverlap()
    {
        var myCollider = gameObject.GetComponent<BoxCollider2D>();
 
        int collider = myCollider.OverlapCollider(new ContactFilter2D(), new Collider2D[1]);
 
        if (collider == 0)
            return false;
 
        return true;
    }

    void OnCollisionEnter2D(Collision2D other)
    {

        if (other.gameObject.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }

    }
}
