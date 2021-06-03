using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{

    public GameObject ItemUIImage;

    private int[] playerInventory;
    private Sprite[] itemSprites;

    // Start is called before the first frame update
    void Start()
    {
        displayInventory();
    }

    public void displayInventory()
    {
        foreach (Transform child in this.transform)
            Destroy(child.gameObject);

        playerInventory = GameObject.FindWithTag("Player").GetComponent<Player>().inventory;

        itemSprites = GameObject.FindWithTag("SpriteDatabase").GetComponent<SpriteDatabase>().items;

        int spacer = 0;

        for (int i = 0; i < playerInventory.Length; i++)
        {
            if (playerInventory[i] > 0)
            {
                Vector3 pos = this.transform.position;
                pos = pos + new Vector3(spacer * 50, 0, 0);
                GameObject uiImage = Instantiate(ItemUIImage, pos, this.transform.rotation);

                ItemUIImage uiImageScript = uiImage.GetComponent<ItemUIImage>();
                uiImageScript.item = itemSprites[i];
                uiImageScript.quantity = playerInventory[i];

                uiImage.transform.parent = this.transform;

                spacer++;
            }


        }
    }

}
