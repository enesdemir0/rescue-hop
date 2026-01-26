using UnityEngine;
using UnityEngine.UI; // Needed for UI

public class UIButtonSound : MonoBehaviour
{
    public int clickSoundIndex = 24; // Change this to your Click SFX index number

    void Start()
    {
        // 1. Get the Button component on this object
        Button btn = GetComponent<Button>();

        // 2. Tell the button: "When clicked, run this code"
        if (btn != null)
        {
            btn.onClick.AddListener(PlaySound);
        }
    }

    void PlaySound()
    {
        // Call your existing Audio Manager
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX(clickSoundIndex);
        }
    }
}