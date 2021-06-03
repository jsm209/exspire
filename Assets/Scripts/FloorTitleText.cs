using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FloorTitleText : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<TextMeshProUGUI>().text = "FLOOR " + GameObject.FindWithTag("Player").GetComponent<Player>().currentFloor;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
