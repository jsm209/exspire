using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleScreenManager : MonoBehaviour
{
   [Header("Menus")]
    public GameObject titleMenu;
    public GameObject saveMenu;

   [Header("Buttons")]
    public Button playButton;
    public Button quitButton;
    public Button backButton;

    void Start() {
        playButton.onClick.AddListener(OnClickPlay);
        quitButton.onClick.AddListener(OnClickQuit);
        backButton.onClick.AddListener(OnClickBack);
    }

    void OnClickPlay() {
        titleMenu.SetActive(false);
        saveMenu.SetActive(true);
    }

    void OnClickQuit() {
        Application.Quit();
    }

    void OnClickBack() {
        titleMenu.SetActive(true);
        saveMenu.SetActive(false);
    }
    
}
