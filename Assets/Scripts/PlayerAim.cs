using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    public Rigidbody2D rbAim;
    public Camera cam;
    Vector2 mousePos;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
    }

    void FixedUpdate() {
        Vector2 lookDir = mousePos - rbAim.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        rbAim.rotation = angle;
    }
}
