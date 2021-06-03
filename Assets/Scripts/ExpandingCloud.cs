using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpandingCloud : MonoBehaviour
{

    public float lifetime;

    private Animator animator;

    void Start() {
        animator = this.GetComponent<Animator>();

        if (lifetime != 0f) {
            Invoke("death", lifetime);
        }
    }


    void death() {
        animator.SetTrigger("death");
        Invoke("DestroyMe", 0.35f);
    }


    public void DestroyMe() 
    { 
        Destroy(gameObject);
    } 
}
