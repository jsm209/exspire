using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    #region public variables
    [Header("Player Stats")]
    public int kindles = 20; // Currency used for casting abilities.
    public float timeLeft = 600; // Time left in game.
    public bool defeatedDenialBoss = false; // Checks if denial boss is defeated.


    [Header("Other")]
    // UI Related
    public Text timerText;
    #endregion

    // Saves the player's data
    public void SavePlayer()
    {
        SaveSystem.SavePlayer(this);
    }

    // Loads the stored player data
    public void LoadPlayer()
    {
        PlayerData data = SaveSystem.LoadPlayer();
        kindles = data.kindles;
        timeLeft = data.timeLeft;

        Vector3 position;
        position.x = data.position[0];
        position.y = data.position[1];
        position.z = data.position[2];
        transform.position = position;
    }

    // Adds to the kindle count.
    public void addKindles(int n)
    {
        kindles += n;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        timeLeft -= 1 * Time.deltaTime;
        updateTimerDisplay();
    }

    // Updates a text object to display the appropriate amount of
    // minutes and seconds from total seconds.
    void updateTimerDisplay()
    {
        int minutes = (int)timeLeft / 60;
        int seconds = (int)timeLeft % 60;
        timerText.text = minutes.ToString() + " : " + seconds.ToString();
    }


}
