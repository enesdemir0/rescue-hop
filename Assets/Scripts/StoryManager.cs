using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro; // <--- IMPORTANT: You must add this line!

public class StoryManager : MonoBehaviour
{
    [Header("Configuration")]
    public string level1Name;

    [Header("UI Elements")]
    // CHANGED THIS: From 'Text' to 'TMP_Text'
    public TMP_Text storyText;

    public Image storyImage;
    public GameObject pressSpaceText;

    [Header("Story Content")]
    [TextArea(3, 10)]
    public string[] sentences;
    public Sprite[] images;

    private int currentIndex = 0;
    private bool canSkip = false;

    void Start()
    {
        UpdateUI();
        StartCoroutine(EnableSkipping());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            if (canSkip)
            {
                NextSlide();
            }
        }
    }

    void NextSlide()
    {
        currentIndex++;

        if (currentIndex >= sentences.Length)
        {
            StartGame();
        }
        else
        {
            UpdateUI();
        }
    }

    void UpdateUI()
    {
        // This works exactly the same way
        storyText.text = sentences[currentIndex];

        if (images.Length > currentIndex && images[currentIndex] != null)
        {
            storyImage.sprite = images[currentIndex];
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene(level1Name);
    }

    IEnumerator EnableSkipping()
    {
        canSkip = false;
        yield return new WaitForSeconds(0.5f);
        canSkip = true;
        if (pressSpaceText != null) pressSpaceText.SetActive(true);
    }
}