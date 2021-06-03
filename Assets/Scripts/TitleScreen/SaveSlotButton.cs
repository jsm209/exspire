using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;

public class SaveSlotButton : MonoBehaviour
{

    public string slotName;
    private bool isNewGame;

    public Button saveSlot;
    public Text saveButtonText;
    public Button deleteSaveSlot;

    public Button deleteSaveSlotConfirm;
    public Button deleteSaveSlotCancel;

    public GameObject deleteSaveSlotConfirmPopup;

    public Player defaultPlayerStats;

    void Start() {
        saveSlot.onClick.AddListener(onClickSaveSlot);
        deleteSaveSlot.onClick.AddListener(onClickDeleteSaveSlot);
        deleteSaveSlotConfirm.onClick.AddListener(onClickDeleteSaveSlotConfirm);
        deleteSaveSlotCancel.onClick.AddListener(onClickDeleteSaveSlotCancel);
    }

    // Update is called once per frame
    void Update()
    {
        // Check if this is a new game
        string path = Application.persistentDataPath + "/" + slotName + ".kindle";
        if (File.Exists(path)) {
            isNewGame = false;
        } else {
            isNewGame = true;
        }

        if (isNewGame) {
            saveButtonText.text = "NEW GAME";
        } else {
            saveButtonText.text = slotName;
        }
    }

    void onClickDeleteSaveSlot() {
        deleteSaveSlot.gameObject.SetActive(false);
        deleteSaveSlotConfirmPopup.SetActive(true);
    } 

    void onClickSaveSlot() {

        PlayerPrefs.SetString("saveslot", slotName);

        if (isNewGame) {
            SaveSystem.SavePlayer(defaultPlayerStats);
            SceneManager.LoadScene("TutorialDungeon");
        } else {
            SceneManager.LoadScene("MainDungeon");
        }

        

        
    }

    void onClickDeleteSaveSlotConfirm() {
        // find and delete actual save file
        string path = Application.persistentDataPath + "/" + slotName + ".kindle";
        if (File.Exists(path)) {
            File.Delete(path);
        }

        deleteSaveSlot.gameObject.SetActive(true);
        deleteSaveSlotConfirmPopup.SetActive(false);
    }

    void onClickDeleteSaveSlotCancel() {
        deleteSaveSlot.gameObject.SetActive(true);
        deleteSaveSlotConfirmPopup.SetActive(false);
    }
}
