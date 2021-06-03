using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeUIText : MonoBehaviour
{

    public string prompt;

    private TextMesh promptText;



    // Start is called before the first frame update
    void Start()
    {
        promptText = GameObject.FindWithTag("PromptText").GetComponent<TextMesh>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            promptText.text = prompt;
        }


    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            promptText.text = "";
        }
    }
}
