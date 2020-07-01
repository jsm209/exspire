using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoSpriteSortingOrder : MonoBehaviour
{

    public int yOffset;

    // Update is called once per frame
    void Update()
    {
        this.GetComponent<SpriteRenderer>().sortingOrder = ((int)this.transform.position.y * -1) - yOffset;
    }
}
