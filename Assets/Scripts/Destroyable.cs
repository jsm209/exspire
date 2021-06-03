using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyable : MonoBehaviour { 

    public float lifetime;

    void Start() {
        if (lifetime != 0) {
            Invoke("DestroyMe", lifetime);
        }
    }


    public void DestroyMe() 
    { 
        Destroy(gameObject); 
    } 
}
