using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Float : MonoBehaviour
{
    public float moveSpeed = 20.0f;
    public float floatSpeed = 4.0f;
    public float floatAmplitude = 0.5f;

    public bool followsPlayer = true;

    private float offset; // to prevent everything floating in unison

    // Start is called before the first frame update
    void Start()
    {
        offset = Random.Range(-10.0f, 10.0f);

        if (floatAmplitude == 0)
        {
            floatAmplitude = 1;
        }

        if (floatSpeed == 0)
        {
            floatSpeed = 1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        Vector3 translate = new Vector3(0.0f, Mathf.Sin((Time.time * floatSpeed) + offset), 0.0f) / 10;
        transform.position = transform.position + (translate * floatAmplitude);
        
        if (followsPlayer)
        {
            transform.position = Vector3.MoveTowards(transform.position, playerPosition, moveSpeed * Time.deltaTime);
        }

    }

}
