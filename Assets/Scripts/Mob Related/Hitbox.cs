using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{

    public int damage;
    public float knockback;
    public bool active;

    void Start() {
        active = false;
    }

}
