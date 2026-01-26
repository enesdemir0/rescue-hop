using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoatController : MonoBehaviour
{
    // ... (Health variables remain the same) ...
    [Header("Boat Health & UI")]
    public int maxHealth = 5;
    private int currentHealth;
    public Slider healthSlider;
    public GameObject damageEffect;
    public GameObject deathEffect;

    [Header("Damage Flash Settings")]
    public Material damageMaterial;
    public Renderer[] boatParts;
    public int numberOfFlashes = 3;
    public float flashDuration = 0.1f;

    private List<Material[]> originalMaterials = new List<Material[]>();
    private Coroutine flashRoutine;

    [Header("Movement Settings")]
    public float moveSpeed = 15f;
    public float turnSpeed = 50f;
    public bool isDriving;

    [Header("References")]
    public Transform seatPoint;
    public Transform exitPoint;
    public Rigidbody theRB;

    [Header("Visuals")]
    public GameObject leftPaddle;
    public GameObject rightPaddle;
    public ParticleSystem[] waterTrails;

    [Header("Paddle Animation")]
    public float paddleRotSpeed = 3f;
    public float paddleSweepAngle = 15f;
    public float paddleDipAngle = 30f;

    // --- AUDIO SETTINGS UPDATED ---
    [Header("Audio Settings")]
    // CHANGE THIS: 1.1f gives a 0.09s gap after your 1.01s sound. 
    // It creates a perfect "Row... Row... Row..." rhythm.
    public float soundRate = 1.1f;
    private float soundCounter;
    // ------------------------------

    private bool playerInRange;
    private PlayerController player;
    private CameraController cam;

    // Inputs
    private float moveInput;
    private float turnInput;

    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        cam = FindObjectOfType<CameraController>();

        currentHealth = maxHealth;
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
            healthSlider.gameObject.SetActive(false);
        }

        foreach (Renderer rend in boatParts)
        {
            originalMaterials.Add(rend.materials);
        }

        if (leftPaddle) leftPaddle.SetActive(false);
        if (rightPaddle) rightPaddle.SetActive(false);
        foreach (ParticleSystem p in waterTrails) if (p) p.Stop();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && playerInRange)
        {
            if (isDriving) ExitBoat();
            else if (player.hasPaddle) EnterBoat();
            else Debug.Log("Need Paddle!");
        }

        if (isDriving)
        {
            moveInput = Input.GetAxis("Vertical");
            turnInput = Input.GetAxis("Horizontal");

            HandleVisuals();
        }
        else
        {
            moveInput = 0;
            turnInput = 0;
        }
    }

    void FixedUpdate()
    {
        if (isDriving)
        {
            if (moveInput != 0)
            {
                Vector3 targetVelocity = transform.forward * moveInput * moveSpeed;
                theRB.velocity = new Vector3(targetVelocity.x, theRB.velocity.y, targetVelocity.z);
            }
            else
            {
                theRB.velocity = new Vector3(0f, theRB.velocity.y, 0f);
            }

            if (turnInput != 0)
            {
                Vector3 rotation = Vector3.up * turnInput * turnSpeed * Time.fixedDeltaTime;
                theRB.MoveRotation(theRB.rotation * Quaternion.Euler(rotation));
            }
        }
    }

    void HandleVisuals()
    {
        // Effects
        if (moveInput != 0) { foreach (ParticleSystem p in waterTrails) if (p && !p.isPlaying) p.Play(); }
        else { foreach (ParticleSystem p in waterTrails) if (p && p.isPlaying) p.Stop(); }

        // Paddles & Sound
        if (moveInput != 0)
        {
            float wave = Mathf.Sin(Time.time * paddleRotSpeed);
            if (leftPaddle) leftPaddle.transform.localRotation = Quaternion.Euler(paddleDipAngle, 90f + (wave * paddleSweepAngle), 0f);
            if (rightPaddle) rightPaddle.transform.localRotation = Quaternion.Euler(paddleDipAngle, -90f - (wave * paddleSweepAngle), 0f);

            // --- AUDIO LOGIC ---
            // If timer runs out, play sound and reset timer to 1.1 seconds
            soundCounter -= Time.deltaTime;
            if (soundCounter <= 0)
            {
                AudioManager.instance.PlaySFXPitched(21);
                soundCounter = soundRate;
            }
        }
        else
        {
            if (leftPaddle) leftPaddle.transform.localRotation = Quaternion.Euler(paddleDipAngle, 90f, 0f);
            if (rightPaddle) rightPaddle.transform.localRotation = Quaternion.Euler(paddleDipAngle, -90f, 0f);

            // Reset counter so sound plays immediately next time you press W
            soundCounter = 0;
        }
    }

    // ... (Keep EnterBoat, ExitBoat, TakeDamage, FlashCo, DestroyBoat exactly as they were before) ...
    // Paste them here if you need, but they haven't changed.

    // Quick copy-paste block for the rest:
    public void TakeDamage(int damage)
    {
        AudioManager.instance.PlaySFX(12);
        if (!isDriving) return;
        if (currentHealth <= 0) return;
        currentHealth -= damage;

        if (healthSlider != null) healthSlider.value = currentHealth;
        if (damageEffect != null) Instantiate(damageEffect, transform.position, transform.rotation);
        if (flashRoutine != null) StopCoroutine(flashRoutine);
        flashRoutine = StartCoroutine(FlashCo());
        if (currentHealth <= 0) DestroyBoat();
    }

    IEnumerator FlashCo()
    {
        for (int i = 0; i < numberOfFlashes; i++)
        {
            foreach (Renderer rend in boatParts)
            {
                Material[] redMats = new Material[rend.materials.Length];
                for (int x = 0; x < redMats.Length; x++) redMats[x] = damageMaterial;
                rend.materials = redMats;
            }
            yield return new WaitForSeconds(flashDuration);
            for (int k = 0; k < boatParts.Length; k++) boatParts[k].materials = originalMaterials[k];
            yield return new WaitForSeconds(flashDuration);
        }
    }

    void DestroyBoat()
    {
        if (isDriving) ExitBoat();
        if (deathEffect != null) Instantiate(deathEffect, transform.position, transform.rotation);
        if (healthSlider != null) healthSlider.gameObject.SetActive(false);
        Destroy(gameObject);
    }

    void EnterBoat()
    {
        isDriving = true;
        if (healthSlider != null) healthSlider.gameObject.SetActive(true);
        player.characterCon.enabled = false;
        player.stopMoving = true;
        player.anim.SetFloat("speed", 0f);
        player.anim.SetBool("isGrounded", true);
        player.anim.SetBool("hasKey", false);
        if (player.paddleModel != null) player.paddleModel.SetActive(false);
        if (leftPaddle) leftPaddle.SetActive(true);
        if (rightPaddle) rightPaddle.SetActive(true);
        player.transform.parent = transform;
        player.transform.position = seatPoint.position;
        player.transform.rotation = transform.rotation;
        if (cam != null) cam.SetBoatMode(true);
    }

    void ExitBoat()
    {
        isDriving = false;
        if (healthSlider != null) healthSlider.gameObject.SetActive(false);
        if (cam != null) cam.SetBoatMode(false);
        foreach (ParticleSystem p in waterTrails) if (p) p.Stop();
        player.transform.parent = null;
        player.transform.position = exitPoint.position;
        player.transform.rotation = Quaternion.identity;
        if (player.paddleModel != null) player.paddleModel.SetActive(false);
        player.hasPaddle = false;
        if (leftPaddle) leftPaddle.SetActive(false);
        if (rightPaddle) leftPaddle.SetActive(false);
        player.characterCon.enabled = true;
        player.stopMoving = false;
    }

    private void OnTriggerEnter(Collider other) { if (other.tag == "Player") playerInRange = true; }
    private void OnTriggerExit(Collider other) { if (other.tag == "Player") playerInRange = false; }
}