using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Needed for UI

public class DimensionManager : MonoBehaviour
{
    [Header("Platform Groups")]
    public GameObject[] redObjects;
    public GameObject[] blueObjects;
    public GameObject[] greenObjects;
    public GameObject[] yellowObjects;

    [Header("UI Indicator")]
    public Image indicatorImage; // Drag a UI Image here (Top right of screen)

    // Define the colors for the UI Icon
    public Color colRed = Color.red;
    public Color colBlue = Color.blue;
    public Color colGreen = Color.green;
    public Color colYellow = Color.yellow;

    // 0 = Red, 1 = Blue, 2 = Green, 3 = Yellow
    private int currentMode = 0;

    void Start()
    {
        UpdateWorld();
    }

    void Update()
    {
        // Cycle Forward with Q
        if (Input.GetKeyDown(KeyCode.Q))
        {
            currentMode++;
            if (currentMode > 3) currentMode = 0; // Loop back to start
            UpdateWorld();
        }
    }

    void UpdateWorld()
    {
        // 1. Update Red
        bool isRed = (currentMode == 0);
        foreach (GameObject obj in redObjects) { if (obj) obj.SetActive(isRed); }

        // 2. Update Blue
        bool isBlue = (currentMode == 1);
        foreach (GameObject obj in blueObjects) { if (obj) obj.SetActive(isBlue); }

        // 3. Update Green
        bool isGreen = (currentMode == 2);
        foreach (GameObject obj in greenObjects) { if (obj) obj.SetActive(isGreen); }

        // 4. Update Yellow
        bool isYellow = (currentMode == 3);
        foreach (GameObject obj in yellowObjects) { if (obj) obj.SetActive(isYellow); }

        // 5. Update UI Color (Visual Feedback)
        if (indicatorImage != null)
        {
            switch (currentMode)
            {
                case 0: indicatorImage.color = colRed; break;
                case 1: indicatorImage.color = colBlue; break;
                case 2: indicatorImage.color = colGreen; break;
                case 3: indicatorImage.color = colYellow; break;
            }
        }
    }
}