using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingDirector : MonoBehaviour
{
    [Header("Actors")]
    public GameObject player;
    public GameObject rabbit;
    public Animator playerAnim;
    public Animator rabbitAnim;
    public Transform mainCamera;

    [Header("Settings")]
    public Transform meetingPoint;
    public GameObject theEndText;
    public float runSpeed = 5f;
    public string mainMenuSceneName = "MainMenu";

    // How far apart they should stop (1.0 means they stand 2 meters apart)
    public float stopDistance = 1.0f;

    private bool isReuniting = false;

    void Start()
    {
        AudioManager.instance.PlayMusic(3);
        // 1. UI FADE IN
        if (UIController.instance != null)
        {
            //UIController.instance.fadeScreen.color = new Color(0, 0, 0, 1);
            // USE THIS (Set Alpha to 1 means "Fully Visible")
            UIController.instance.fadeScreen.alpha = 1f;
            UIController.instance.FadeFromBlack();
        }

        if (theEndText != null) theEndText.SetActive(false);

        // 2. FORCE IDLE & GROUNDED
        if (playerAnim != null)
        {
            playerAnim.SetBool("isGrounded", true); // Fixes the "Stuck in Jump" bug
            playerAnim.SetFloat("speed", 0f);
            playerAnim.SetFloat("yVel", 0f);
        }

        StartCoroutine(EndingSequence());
    }

    void Update()
    {
        if (isReuniting)
        {
            // Move Player towards Center
            player.transform.position = Vector3.MoveTowards(player.transform.position, meetingPoint.position, runSpeed * Time.deltaTime);
            player.transform.LookAt(meetingPoint);

            // Move Rabbit towards Center
            rabbit.transform.position = Vector3.MoveTowards(rabbit.transform.position, meetingPoint.position, runSpeed * Time.deltaTime);
            rabbit.transform.LookAt(meetingPoint);

            if (mainCamera != null) mainCamera.LookAt(meetingPoint);

            // --- DISTANCE FIX ---
            // Stop them BEFORE they hit the center point so they don't look weird/clip
            if (Vector3.Distance(player.transform.position, meetingPoint.position) < stopDistance)
            {
                isReuniting = false;
            }
        }
    }

    IEnumerator EndingSequence()
    {
        yield return new WaitForSeconds(1f);

        // ============================================
        // SHOT 1: START RUNNING
        // ============================================
        isReuniting = true;

        if (playerAnim != null)
        {
            playerAnim.SetBool("isGrounded", true);
            playerAnim.SetFloat("speed", 1f);
        }

        if (rabbitAnim != null) rabbitAnim.SetBool("isRunning", true);

        // Wait until they arrive (Logic is in Update)
        while (isReuniting)
        {
            yield return null;
        }

        // ============================================
        // SHOT 2: ARRIVAL & STOP
        // ============================================
        if (playerAnim != null) playerAnim.SetFloat("speed", 0f);
        if (rabbitAnim != null) rabbitAnim.SetBool("isRunning", false);

        // Face Each Other
        player.transform.LookAt(rabbit.transform);
        rabbit.transform.LookAt(player.transform);

        // ============================================
        // SHOT 3: THE END TEXT (Faster now)
        // ============================================
        // I removed the jumping loop here. Now they just stand together.

        yield return new WaitForSeconds(0.5f); // Short pause

        if (theEndText != null) theEndText.SetActive(true);

        yield return new WaitForSeconds(2.0f); // Show text for only 2 seconds (Faster)

        // ============================================
        // SHOT 4: FADE OUT & MENU
        // ============================================
        if (UIController.instance != null)
        {
            UIController.instance.FadeToBlack();
        }

        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene(mainMenuSceneName);
    }
}