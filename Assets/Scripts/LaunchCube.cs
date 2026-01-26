using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchCube : MonoBehaviour
{
    [Header("Physics Settings")]
    public float launchForce = 20f;

    [Header("Animation Settings")]
    public float pushDistance = 0.5f;
    public float squashAmount = 0.5f; // 1 = Normal, 0.5 = Half Height (Squashed)
    public float animationSpeed = 15f;

    [Header("Effects")]
    public GameObject effect;
    public AudioSource sound;

    private Vector3 originalPosition;
    private Vector3 originalScale; // Store the size
    private bool isAnimating = false;

    private void Start()
    {
        originalPosition = transform.position;
        originalScale = transform.localScale;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            PlayerController player = other.GetComponent<PlayerController>();

            if (player != null)
            {
                // 1. Launch Player
                Vector3 launchVector = transform.up * launchForce;
                player.LaunchPlayer(launchVector);
                AudioManager.instance.PlaySFXPitched(17);

                // 2. Play Effects



                // 3. Start Animation
                if (!isAnimating)
                {
                    
                    StartCoroutine(AnimateSpring());
                }
            }
        }
    }

    IEnumerator AnimateSpring()
    {
        isAnimating = true;

        Vector3 targetPos = originalPosition + (transform.up * pushDistance);
        Vector3 squashedScale = new Vector3(originalScale.x, originalScale.y * squashAmount, originalScale.z);

        // PHASE 1: SQUASH (Compress down)
        // We shrink the Y axis quickly
        float timer = 0;
        while (timer < 1f)
        {
            timer += Time.deltaTime * animationSpeed;
            // Go to Squashed size
            transform.localScale = Vector3.Lerp(originalScale, squashedScale, timer);
            yield return null;
        }

        // PHASE 2: LAUNCH (Expand + Push Out)
        // We move position OUT and return scale to NORMAL at the same time
        while (Vector3.Distance(transform.position, targetPos) > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPos, animationSpeed * Time.deltaTime);
            transform.localScale = Vector3.Lerp(transform.localScale, originalScale, animationSpeed * Time.deltaTime);
            yield return null;
        }

        // PHASE 3: RESET (Return to start)
        while (Vector3.Distance(transform.position, originalPosition) > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, originalPosition, (animationSpeed / 2) * Time.deltaTime);
            yield return null;
        }

        transform.position = originalPosition;
        transform.localScale = originalScale;
        isAnimating = false;
    }
}