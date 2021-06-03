using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDoor : MonoBehaviour
{

    public Sprite[] doorStates; // Sprites the door will progress through.
    public GameObject portal; // The portal that leads to the next room.

    private int doorStep; // Current step the door is for activating.


    public void incrementDoor()
    {
        int maxStates = doorStates.Length;
        if (doorStep < maxStates)
        {
            doorStep++;
        }

        if (doorStep == maxStates)
        {
            activate();
        }
        gameObject.GetComponent<SpriteRenderer>().sprite = doorStates[doorStep - 1];
    }

    void activate()
    {
        portal.SetActive(true);
    }
}
