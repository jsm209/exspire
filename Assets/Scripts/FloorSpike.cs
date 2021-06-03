using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorSpike : MonoBehaviour
{
    public float initialDelay;
    public float waitTime;
    public float activeTime;

    public GameObject hitboxObject;
    private Hitbox hitbox;

    private Animator anim;


    // Start is called before the first frame update
    void Start()
    {
        hitbox = hitboxObject.GetComponent<Hitbox>();
        anim = this.GetComponent<Animator>();
        Invoke("startAttack", initialDelay);
    }

    void startAttack()
    {
        StartCoroutine("attack");
    }

    IEnumerator attack()
    {
        while (true)
        {
            anim.SetBool("isActive", true);

            yield return new WaitForSeconds(activeTime);

            anim.SetBool("isActive", false);

            yield return new WaitForSeconds(waitTime);
        }
    }

    void enableHitbox()
    {
        hitbox.active = true;
    }

    void disableHitbox()
    {
        hitbox.active = false;
    }
}
