using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RunEndPopup : MonoBehaviour
{

    public GameObject RunStatsBox;

    public TextMeshProUGUI killCount;
    public TextMeshProUGUI secondsSurvivedCount;

    [Header("Buttons")]
    public Button mainMenuButton;

    private float totalSecondsSurvived;

    private float secondsPlayerIsDead;
    private bool deletedPlayerSave;

    // Start is called before the first frame update
    void Start()
    {
        secondsPlayerIsDead = 0.0f;
        deletedPlayerSave = false;

        RunStatsBox.SetActive(false);
        mainMenuButton.onClick.AddListener(OnClickMainMenu);
    }

    // Update is called once per frame
    void Update()
    {
        totalSecondsSurvived = GameObject.FindWithTag("Player").GetComponent<Player>().timeSurvived;
        killCount.text = GameObject.FindWithTag("Player").GetComponent<Player>().kills + "";
        secondsSurvivedCount.text = GameObject.FindWithTag("Player").GetComponent<Player>().formatTime(totalSecondsSurvived);
    
        // The game end screen will only appear after the player has been dead for 3 seconds.
        float playerHealth = GameObject.FindWithTag("Player").GetComponent<Player>().health;
        if (playerHealth <= 0) {
            secondsPlayerIsDead += Time.deltaTime * 1;
            if (secondsPlayerIsDead >= 3.0f) {
                RunStatsBox.SetActive(true);
                if (!deletedPlayerSave) {
                    SaveSystem.DeletePlayer();
                    deletedPlayerSave = true;
                }
            }

        } else {
            secondsPlayerIsDead = 0.0f;
        }


    }

    void OnClickMainMenu() {
        SceneManager.LoadScene("TitleScreen");
    }
}
