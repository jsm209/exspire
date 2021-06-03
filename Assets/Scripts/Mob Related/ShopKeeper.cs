using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopKeeper : MonoBehaviour
{
    public GameObject[] shopInventoryRegular;
    public GameObject[] shopInventoryRare;

    public GameObject[] displaySpawns;

    // Start is called before the first frame update
    void Start()
    {
        spawnInventory();
    }

    // We want to spawn 1 rare item and have the rest be regular items.
    void spawnInventory() {
        for (int i = 0; i < displaySpawns.Length; i++) {
            GameObject spawn = null;
            if ( i == 0 && shopInventoryRare.Length > 0) {
                spawn = shopInventoryRare[Random.Range(0, shopInventoryRare.Length)];   
            } else {
                spawn = shopInventoryRegular[Random.Range(0, shopInventoryRegular.Length)];   
            }
            displaySpawns[i].GetComponent<ShopDisplay>().SetAndSpawnItem(spawn);
        }
    }
}
