using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LSLevelEntry : MonoBehaviour
{
    public string levelName, displayName;

    // --- NEW: Description Variable ---
    // [TextArea] makes a big box in the Inspector so you can type easier
    [TextArea]
    public string levelDescription;
    // ---------------------------------

    public GameObject mapPointActive, mapPointInactive;
    private bool canLoadLevel, levelLoading;

    public int crystalsRequired;
    private bool levelUnlucked;


    // Start is called before the first frame update
    void Start()
    {
        if (crystalsRequired > 0 && LevelManager.instance.currentCrystals < crystalsRequired)
        {
            levelUnlucked = false;
            mapPointActive.SetActive(false);
            mapPointInactive.SetActive(true);
        }
        else
        {
            levelUnlucked = true;
            mapPointActive.SetActive(true);
            mapPointInactive.SetActive(false);
        }

        if (PlayerPrefs.GetString("CurrentLevel") == levelName)
        {
            PlayerController player = FindObjectOfType<PlayerController>();

            // Disable CharacterController momentarily to allow instant teleport
            player.characterCon.enabled = false;
            player.transform.position = transform.position;
            player.characterCon.enabled = true;

            FindObjectOfType<CameraController>().SnapToTarget();
            LevelManager.instance.respawnPoint = transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (canLoadLevel && levelUnlucked && Input.GetButtonDown("Jump") && !levelLoading)
        {
            levelLoading = true;
            StartCoroutine(LoadLevelCo());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            canLoadLevel = true;

            // 1. Show the Box
            LevelSelectController.instance.LevelInfoBox.SetActive(true);

            // 2. Set the Name
            LevelSelectController.instance.levelText.text = displayName;

            // 3. --- NEW: Set the Description ---
            // We check if the text slot exists to prevent errors
            if (LevelSelectController.instance.descriptionText != null)
            {
                LevelSelectController.instance.descriptionText.text = levelDescription;
            }
            // -----------------------------------

            if (levelUnlucked)
            {
                LevelSelectController.instance.ActionText.text = "Press Jump To Enter";
            }
            else
            {
                LevelSelectController.instance.ActionText.text = crystalsRequired.ToString() + " Crystals required to unluck";
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            canLoadLevel = false;
            LevelSelectController.instance.LevelInfoBox.SetActive(false);
        }
    }

    public IEnumerator LoadLevelCo()
    {
        FindObjectOfType<PlayerController>().stopMoving = true;
        UIController.instance.FadeToBlack();

        AudioManager.instance.PlaySFX(9);

        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(levelName);
        PlayerPrefs.SetString("CurrentLevel", levelName);
    }
}