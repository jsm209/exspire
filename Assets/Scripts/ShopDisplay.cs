using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShopDisplay : MonoBehaviour
{
    public Transform spawnpoint;
    public GameObject priceTag;

    private TextMeshPro priceTagText;

    public void SetAndSpawnItem(GameObject spawn) {
        Instantiate(spawn, spawnpoint.transform.position, spawnpoint.transform.rotation);
        priceTagText = priceTag.GetComponent<TextMeshPro>();
        priceTagText.text = spawn.GetComponent<Item>().itemCost + "";
    }

}
