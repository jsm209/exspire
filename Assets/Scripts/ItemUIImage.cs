using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemUIImage : MonoBehaviour
{
    public Sprite item;
    public int quantity;

    public TextMeshProUGUI quantityText;

    // Start is called before the first frame update
    void Start()
    {
        if (quantity == 0) {
            Destroy(gameObject);
        }

        this.GetComponent<Image>().sprite = item;

        if (quantity >= 2) {
            quantityText.text = "x" + quantity.ToString();
        } else {
            quantityText.text = "";
        }
    }
}
