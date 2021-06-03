using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{

    public float initialDelay;
    public float waitTime;
    public float activeTime;
    public float shootDelay;
    public float projectileForce;
    public GameObject laserBullet;

    public bool shootLeftInstead;
    private Vector3 shootDirection = Vector3.right;

    // Start is called before the first frame update
    void Start()
    {
        if (activeTime > waitTime)
        {
            StartCoroutine("constantLaser");
        }
        else
        {
            InvokeRepeating("DoAttack", initialDelay, waitTime + activeTime);
        }

        if (shootLeftInstead) {
            this.GetComponent<SpriteRenderer>().flipX = true;
            shootDirection = Vector3.left;
        }

    }

    void DoAttack()
    {
        StartCoroutine("laser");
    }

    IEnumerator constantLaser()
    {
        this.GetComponent<Animator>().SetBool("isShooting", true);
        while (true)
        {
            shoot(laserBullet);
            yield return new WaitForSeconds(shootDelay);
        }
    }

    IEnumerator laser()
    {
        this.GetComponent<Animator>().SetBool("isShooting", true);
        yield return new WaitForSeconds(1.0f);
        var startTime = Time.time;
        var endTime = Time.time + activeTime;
       
        while (Time.time < endTime)
        {
            shoot(laserBullet);
            yield return new WaitForSeconds(shootDelay);
        }
        this.GetComponent<Animator>().SetBool("isShooting", false);
        StopCoroutine("laser");
    }

    void shoot(GameObject projectile)
    {
        GameObject bullet = Instantiate(projectile, this.transform.position, this.transform.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.AddForce(shootDirection * projectileForce, ForceMode2D.Impulse);
    }
}