using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MessageBoxPopup : MonoBehaviour
{
    public string[] messages; // What messages should the conversation show?
    public string[] directions; // What directions should each conversation box show?
    public Sprite[] characters; // What character pictures should each conversation box show?
    public float typingSpeed; // How fast should the text be typed out?
    private GameObject hintObject;
    public string hint; // What interaction hint should be given to the player?

    private TextMeshProUGUI messageText;
    private TextMeshProUGUI directionText;
    private Image messageBox;
    private Image messageImage;
    private int index;
    private bool canInteract;
    private bool firstInteract;
    private bool interacting;

    private Camera cam;
    private float initialCamSize;
    private float zoomDuration;
    private float timeElapsed;
//
    // Start is called before the first frame update
    void Start()
    {
        messageText = GameObject.FindWithTag("MessageBoxText").GetComponent<TextMeshProUGUI>();
        directionText = GameObject.FindWithTag("MessageBoxDirection").GetComponent<TextMeshProUGUI>();
        messageBox = GameObject.FindWithTag("MessageBox").GetComponent<Image>();
        messageImage = GameObject.FindWithTag("MessageBoxImage").GetComponent<Image>();
        
        cam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        if (cam) {
            initialCamSize = cam.orthographicSize;
        }
        zoomDuration = 2.0f;

        canInteract = false;
        interacting = false;
        hintObject = transform.Find("Hint").gameObject;
        hintObject.GetComponent<TextMeshPro>().text = "";
    }

    void Update()
    {
        if (canInteract && Input.GetKeyDown(KeyCode.E) && messages.Length > 0)
        {

            interacting = true;
            
            if (firstInteract) // Displays the textbox if this is our first interaction
            {
                messageText.enabled = true;
                directionText.enabled = true;
                messageBox.enabled = true;
                messageImage.enabled = true;
                StartCoroutine(TypeSentence());
                firstInteract = false;

                timeElapsed = 0.0f; // triggers zoom in only on the first interact
            }
            else if (messageText.text == messages[index]) // If it's not our first interaction, then try to get the next message.
            {
                NextMessage();
            }

        }

        // Camera will zoom in if we're interacting and zoom out if we're not
        if (interacting) {
            ZoomCameraIn();
        } else {
            ZoomCameraOut();
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.gameObject.tag == "Player")
        {
            hintObject.GetComponent<TextMeshPro>().text = hint;
            canInteract = true;
            firstInteract = true;

            // Resets the textbox
            index = 0;
            messageText.text = "";
            directionText.text = "";
        }


    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            hintObject.GetComponent<TextMeshPro>().text = "";
            canInteract = false;
            messageText.enabled = false;
            directionText.enabled = false;
            messageBox.enabled = false;
            messageImage.enabled = false;
            StopCoroutine("TypeSentence");

            interacting = false;
            timeElapsed = 0.0f; // so zoom duration can reset
        }
    }

    IEnumerator TypeSentence()
    {
        messageImage.sprite = characters[index];
        foreach (char letter in messages[index].ToCharArray())
        {
            messageText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        directionText.text = directions[index];
    }

    public void NextMessage()
    {
        directionText.text = "";

        if (index < messages.Length - 1) // if we still have more messages, get the next message.
        {
            index++;
            messageText.text = "";
            StartCoroutine(TypeSentence());
        }
        else // if not, we're done
        {
            messageText.text = "";
            canInteract = true;
            messageText.enabled = false;
            directionText.enabled = false;
            messageBox.enabled = false;
            messageImage.enabled = false;

            interacting = false;
            timeElapsed = 0.0f; // so zoom duration can reset
        }
    }

    public void ZoomCameraIn() {
        if (timeElapsed < zoomDuration) {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, 20, timeElapsed / zoomDuration);
            timeElapsed += Time.deltaTime;
        }
        
    } 

    public void ZoomCameraOut() {
        if (timeElapsed < zoomDuration) {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, initialCamSize, timeElapsed / zoomDuration);
            timeElapsed += Time.deltaTime;

            if (cam.orthographicSize > 39.9) {
                cam.orthographicSize = initialCamSize;
            }
        }


    }
}
