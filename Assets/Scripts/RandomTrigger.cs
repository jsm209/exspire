using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomTrigger : MonoBehaviour
{
    public float maxWaitTime;
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        if (maxWaitTime < 1f) {
            maxWaitTime = 1f;
        }
        animator = this.GetComponent<Animator>();
        StartCoroutine(activate());
    }

    IEnumerator activate()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(1f, maxWaitTime));
            animator.ResetTrigger("Activate");
            animator.SetTrigger("Activate");
            
        }
    }
}
