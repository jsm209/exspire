using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyable : MonoBehaviour { 
    public void DestroyMe() 
    { 
        Destroy(gameObject); 
    } 
}
