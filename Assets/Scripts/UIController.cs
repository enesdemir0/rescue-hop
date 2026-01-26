using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    public static UIController instance;

    private void Awake()
    {
        instance = this;
    }

    [Header("Dreamy Fade Settings")]
    public CanvasGroup fadeScreen; // The Group controlling Opacity
    public RectTransform fadeImageRect; // The Image controlling Size
    public float fadeSpeed = 1.5f;

    private bool isFadingToBlack, isFadingFromBlack;

    public Slider healthSlider;
    public TMP_Text healthText, timeText;
    public TMP_Text coinText, crystalText;

    public GameObject pauseScreen;
    public string mainMenu, levelSelect;

    void Start()
    {
        // Optional: ensure defaults
    }

    void Update()
    {
        // --- FADE OUT (Game Disappearing) ---
        // The screen turns Pink/Purple and "Closes in"
        if (isFadingToBlack)
        {
            // 1. Fade Alpha to 1
            fadeScreen.alpha = Mathf.MoveTowards(fadeScreen.alpha, 1f, fadeSpeed * Time.deltaTime);

            // 2. Scale DOWN from Giant (3x) to Normal (1x)
            // This creates a "Settling" effect
            if (fadeImageRect != null)
            {
                float newScale = Mathf.MoveTowards(fadeImageRect.localScale.x, 1f, fadeSpeed * Time.deltaTime);
                fadeImageRect.localScale = new Vector3(newScale, newScale, 1f);
            }

            if (fadeScreen.alpha >= 1f) fadeScreen.blocksRaycasts = true;
        }

        // --- FADE IN (Game Appearing) ---
        // The screen clears and "Zooms Out" like flying through clouds
        if (isFadingFromBlack)
        {
            // 1. Fade Alpha to 0
            fadeScreen.alpha = Mathf.MoveTowards(fadeScreen.alpha, 0f, fadeSpeed * Time.deltaTime);

            // 2. Scale UP from Normal (1x) to Giant (2.5x)
            // This looks like the cloud is flying past the camera
            if (fadeImageRect != null)
            {
                float newScale = Mathf.MoveTowards(fadeImageRect.localScale.x, 2.5f, fadeSpeed * Time.deltaTime);
                fadeImageRect.localScale = new Vector3(newScale, newScale, 1f);
            }

            if (fadeScreen.alpha <= 0f) fadeScreen.blocksRaycasts = false;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseUnpause();
        }
    }

    public void FadeToBlack()
    {
        isFadingToBlack = true;
        isFadingFromBlack = false;

        // When we start fading OUT, set scale to slightly larger so it shrinks down
        if (fadeImageRect != null) fadeImageRect.localScale = new Vector3(1.5f, 1.5f, 1f);
    }

    public void FadeFromBlack()
    {
        isFadingToBlack = false;
        isFadingFromBlack = true;

        // When we start fading IN, ensure scale is Normal so it grows
        if (fadeImageRect != null) fadeImageRect.localScale = Vector3.one;
    }

    public void UpdateHealthDisplay(int health)
    {
        healthText.text = "Health: " + health + "/" + PlayerHealthController.instance.maxHealt;
        healthSlider.maxValue = PlayerHealthController.instance.maxHealt;
        healthSlider.value = health;
    }

    public void PauseUnpause()
    {
        if (pauseScreen != null)
        {
            pauseScreen.SetActive(!pauseScreen.activeSelf);

            if (pauseScreen.activeSelf)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                Time.timeScale = 0f;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                Time.timeScale = 1f;
            }
        }
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenu);
    }

    public void GoToLevelSelect()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(levelSelect);
    }
}