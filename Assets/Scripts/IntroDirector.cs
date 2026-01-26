using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroDirector : MonoBehaviour
{
    [Header("Actors")]
    public GameObject rabbit;
    public GameObject player;
    public Animator rabbitAnim;
    public Animator playerAnim;
    public Transform mainCamera;

    [Header("Dragon Settings")]
    public GameObject dragon;
    public Animator dragonAnim;
    public Transform dragonStartPos;
    public Transform dragonGrabPos;
    public Transform dragonExitPos;

    [Header("Speeds")]
    public float dragonFlySpeed = 20f;
    public float playerRunSpeed = 6f;
    public float circleSpeed = 40f;

    [Header("Waypoints")]
    public Transform runTarget;
    public string nextLevelName = "LevelSelect"; // Changed to LevelSelect as you requested

    // State Variables
    private bool isCircling = true;
    private bool isWalkingToSpot = false;
    private bool isDragonIncoming = false;
    private bool isPlayerSprinting = false;
    private bool isDragonLeaving = false;

    private float angle = 0f;
    private float circleRadius = 2f;
    private Transform cameraTarget;

    void Start()
    {
        // --- UI FADE IN (Start of Scene) ---
        // Make sure the screen starts black and fades to clear
        if (UIController.instance != null)
        {
            // Set alpha to 1 immediately so we don't see a flash of game before fade
            //UIController.instance.fadeScreen.color = new Color(0, 0, 0, 1);
            UIController.instance.fadeScreen.alpha = 1f;
            UIController.instance.FadeFromBlack();
        }
        // -----------------------------------

        cameraTarget = rabbit.transform;

        if (rabbitAnim != null) rabbitAnim.SetBool("isRunning", true);

        if (playerAnim != null)
        {
            playerAnim.SetFloat("speed", 0f);
            playerAnim.SetBool("isGrounded", true);
        }

        if (dragon != null) dragon.transform.position = dragonStartPos.position;
        if (dragonAnim != null) dragonAnim.SetBool("isFlying", true);
        
    }

    void Update()
    {
        // ... (Keep all your existing Update logic exactly the same) ...
        // I am hiding it here to save space, but DO NOT DELETE your Update function!

        // --- PHASE 1: HAPPY CIRCLE ---
        if (isCircling)
        {
            angle += circleSpeed * Time.deltaTime;
            float x = Mathf.Cos(angle * Mathf.Deg2Rad) * circleRadius;
            float z = Mathf.Sin(angle * Mathf.Deg2Rad) * circleRadius;
            rabbit.transform.position = new Vector3(x, 0, z);

            float nextX = Mathf.Cos((angle + 5f) * Mathf.Deg2Rad) * circleRadius;
            float nextZ = Mathf.Sin((angle + 5f) * Mathf.Deg2Rad) * circleRadius;
            rabbit.transform.LookAt(new Vector3(nextX, 0, nextZ));

            if (angle >= 720f)
            {
                isCircling = false;
                StartCoroutine(WalkToSpotSequence());
            }
        }

        // --- PHASE 2: WALK TO SPOT ---
        if (isWalkingToSpot)
        {
            
            rabbit.transform.position = Vector3.MoveTowards(rabbit.transform.position, runTarget.position, 3f * Time.deltaTime);
            rabbit.transform.LookAt(runTarget);

            Vector3 playerTarget = runTarget.position - (Vector3.forward * 3f);
            player.transform.position = Vector3.MoveTowards(player.transform.position, playerTarget, 3f * Time.deltaTime);
            player.transform.LookAt(runTarget);

            if (playerAnim != null) playerAnim.SetFloat("speed", 0.5f);

            if (Vector3.Distance(rabbit.transform.position, runTarget.position) < 0.1f)
            {
                isWalkingToSpot = false;
                StartCoroutine(CinematicSequence());
            }
            
        }

        if (isDragonIncoming)
        {
            dragon.transform.position = Vector3.MoveTowards(dragon.transform.position, dragonGrabPos.position, dragonFlySpeed * Time.deltaTime);
            dragon.transform.LookAt(dragonGrabPos);
        }

        if (isPlayerSprinting)
        {
            player.transform.position = Vector3.MoveTowards(player.transform.position, dragonGrabPos.position, playerRunSpeed * Time.deltaTime);
            player.transform.LookAt(dragonGrabPos);
            if (playerAnim != null) playerAnim.SetFloat("speed", 1f);
        }

        if (isDragonLeaving)
        {
            dragon.transform.position = Vector3.MoveTowards(dragon.transform.position, dragonExitPos.position, dragonFlySpeed * Time.deltaTime);
            dragon.transform.LookAt(dragonExitPos);
        }

        if (mainCamera != null && cameraTarget != null)
        {
            mainCamera.LookAt(cameraTarget);
            mainCamera.Translate(Vector3.forward * 1f * Time.deltaTime);
        }
    }

    IEnumerator WalkToSpotSequence()
    {
        yield return new WaitForSeconds(0.2f);
        isWalkingToSpot = true;
    }

    IEnumerator CinematicSequence()
    {
        // 1. ARRIVAL
        rabbitAnim.SetBool("isRunning", false);
        playerAnim.SetFloat("speed", 0f);

        yield return new WaitForSeconds(1f);

        // SHOT 1: THE REVEAL
        cameraTarget = dragon.transform;
        AudioManager.instance.PlaySFX(13);
        yield return new WaitForSeconds(2.0f);

        // SHOT 2: THE DIVE
        
        isDragonIncoming = true;
        yield return new WaitForSeconds(2.0f);
       

        // SHOT 3: PLAYER PANIC
        cameraTarget = player.transform;
        isPlayerSprinting = true;

        float dist = Vector3.Distance(dragon.transform.position, dragonGrabPos.position);
        while (dist > 0.5f)
        {
            dist = Vector3.Distance(dragon.transform.position, dragonGrabPos.position);
            yield return null;
        }

        // SHOT 4: THE CAPTURE
        isDragonIncoming = false;
        isPlayerSprinting = false;
        playerAnim.SetFloat("speed", 0f);
        cameraTarget = dragon.transform;

        if (dragonAnim != null) dragonAnim.SetTrigger("attack");
        AudioManager.instance.PlaySFX(14);
        yield return new WaitForSeconds(0.5f);

        rabbit.transform.SetParent(dragon.transform);
        rabbit.transform.localPosition = new Vector3(0, -1.5f, 1f);

        yield return new WaitForSeconds(1.5f);
       

        // SHOT 5: THE ESCAPE
        isDragonLeaving = true;
        if (dragonAnim != null) dragonAnim.SetBool("isFlying", true);

        yield return new WaitForSeconds(3f);

        // SHOT 6: SAD PLAYER
        cameraTarget = player.transform;
        AudioManager.instance.PlaySFX(15);
        yield return new WaitForSeconds(2f);
        // --- UI FADE OUT (End of Scene) ---
        if (UIController.instance != null)
        {
            UIController.instance.FadeToBlack();
        }

        // Wait 2 seconds for the screen to turn fully black
        yield return new WaitForSeconds(2f);
        // ----------------------------------

        SceneManager.LoadScene(nextLevelName);
    }
}