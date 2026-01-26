using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelSelectController : MonoBehaviour
{
    public static LevelSelectController instance;

    private void Awake()
    {
        instance = this;
    }

    public GameObject LevelInfoBox;

    // Updated line: Added descriptionText here
    public TMP_Text levelText, ActionText, descriptionText;

    private void Start()
    {
        // Keeping your music logic
        AudioManager.instance.PlayMusic(1);
    }
}