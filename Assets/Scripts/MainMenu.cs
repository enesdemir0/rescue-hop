using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Scene Names")]
    public string introScene;   // NEW: The name of your movie scene (e.g. "IntroCutscene")
    public string firstLevel;   // The name of Level 1 (e.g. "Level1")
    public string levelSelect;  // The name of the Map (e.g. "LevelSelect")

    public GameObject continueButton;

    void Start()
    {

        Cursor.lockState = CursorLockMode.None; // Free the mouse
        Cursor.visible = true; // Show the mouse

        if (PlayerPrefs.HasKey("GameStarted"))
        {
            if (PlayerPrefs.GetInt("GameStarted") == 1)
            {
                continueButton.SetActive(true);
            }
        }
        AudioManager.instance.PlayMusic(3);
    }

    public void NewGame()
    {
        // 1. DELETE OLD DATA
        PlayerPrefs.DeleteAll();

        // 2. MARK GAME AS STARTED
        PlayerPrefs.SetInt("GameStarted", 1);

        // 3. SET INITIAL MAP POSITION
        // Even though we go to the Intro now, we save "Level1" as the current level.
        // Why? So when the Intro ends and loads the LevelSelect, 
        // the map knows to put the player/camera at the Level 1 spot.
        PlayerPrefs.SetString("CurrentLevel", firstLevel);

        // 4. LOAD THE INTRO MOVIE
        SceneManager.LoadScene(introScene);
    }

    public void Continue()
    {
        SceneManager.LoadScene(levelSelect);
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}