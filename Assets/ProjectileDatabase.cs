using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileDatabase : MonoBehaviour
{

    public GameObject[] projectiles;

    public GameObject getProjectile(int id) {
        return projectiles[id];
    }

    public int getIdOfProjectile(GameObject projectile) {
        string projectileName = projectile.GetComponent<Projectile>().projectileName;
        for (int i = 0; i < projectiles.Length; i++) {
            if (projectiles[i].GetComponent<Projectile>().projectileName == projectileName) {
                return i;
            }
        }
        return 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
