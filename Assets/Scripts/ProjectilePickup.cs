using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePickup : MonoBehaviour
{

    public GameObject projectilePickup;

    private bool interactable = true;

    public GameObject getProjectile() {
        return this.projectilePickup;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // on collision, figure out what projectile the player currently has selected.
    // modify the player's projectile array to replace it with this projectile.
    // take the old projectile, and replace this pickup's sprite and projectilePickup, and 
    // take the old projectile, and spawn a new pickup a set distance away in a random direction with its sprite and projectile pickup
    // destroy this object

    void OnCollisionEnter2D(Collision2D col)
    {
        // COLLISION LOGIC MOVED TO PLAYER SCRIPT
        /*
        if (interactable && col.gameObject.tag == "Player")
        {
            TemporaryInactivity();
            PlayerAbilities pa = col.gameObject.GetComponentInChildren<PlayerAbilities>();
            GameObject oldProjectile = pa.getCurrentProjectile();
            pa.replaceCurrentProjectile(this.projectilePickup);
            pa.updateProjectileDisplay(this.projectilePickup);
            projectilePickup = oldProjectile;
            Destroy(this.gameObject);
            //this.GetComponent<SpriteRenderer>().sprite = oldProjectile.GetComponent<SpriteRenderer>().sprite;
        }
        */
    }


    IEnumerator TemporaryInactivity()
    {
        interactable = false;
        Physics2D.IgnoreLayerCollision(9, 16, true);
        this.GetComponent<SpriteRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        yield return new WaitForSeconds(1.0f);
        this.GetComponent<SpriteRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        Physics2D.IgnoreLayerCollision(9, 16, false);
        interactable = true;
        yield return null;
    }
}
