using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubblePlatform : MonoBehaviour
{
    [Header("Movement & Physics")]
    public float riseSpeed = 2f; // Constant Upward speed
    public Rigidbody rb;

    [Header("Visuals (Jiggle)")]
    public float jiggleSpeed = 4f;    // Fast wobble
    public float jiggleAmount = 0.05f; // Subtle amount

    [Header("Bounce Physics")]
    public float bounceForce = 20f; // Make sure this exists!

    [Header("Pop Settings")]
    public float lifeTimeAfterLand = 5f;
    public GameObject popEffect;
    public AudioSource popSound;

    [Header("Impact Animation")]
    public AnimationCurve squashCurve = new AnimationCurve(
        new Keyframe(0, 1),
        new Keyframe(0.1f, 0.6f), // Squash
        new Keyframe(0.3f, 1.2f), // Stretch
        new Keyframe(0.5f, 0.9f),
        new Keyframe(0.7f, 1.0f)
    );
    public float animationSpeed = 1.0f;

    private bool hasTriggered = false;
    private Vector3 initialScale;
    private Vector3 currentSquashScale = Vector3.one;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        initialScale = transform.localScale;
        Destroy(gameObject, 20f);
    }

    void Update()
    {
        // --- 1. JIGGLE LOGIC (Visuals) ---
        // Calculate the "Breathing" effect
        float wave = Mathf.Sin(Time.time * jiggleSpeed) * jiggleAmount;
        Vector3 jiggleScale = new Vector3(1 + wave, 1 - wave, 1 + wave);

        // Combine Jiggle + Impact Squash
        Vector3 finalScale = Vector3.Scale(initialScale, jiggleScale);
        finalScale = Vector3.Scale(finalScale, currentSquashScale);

        transform.localScale = finalScale;
    }

    void FixedUpdate()
    {
        // --- 2. MOVEMENT LOGIC (Physics) ---

        Vector3 velocity = rb.velocity;

        // OLD CODE: Overrode everything
        // velocity.y = riseSpeed; 

        // NEW CODE: 
        // We gently ADD the rise speed, but we respect the existing X/Z movement.
        // We set Y directly so it floats, but we keep X and Z velocity (from the Gun shot or Player push).

        velocity.y = riseSpeed;

        // Apply back
        rb.velocity = velocity;
    }

    // Called by Player when landing
    public void TriggerBubble()
    {
        if (hasTriggered) return;

        hasTriggered = true;
        StartCoroutine(CurveAnimation());
        StartCoroutine(PopSequence());
    }

    IEnumerator CurveAnimation()
    {
        float timer = 0f;
        float duration = squashCurve[squashCurve.length - 1].time;

        while (timer < duration)
        {
            timer += Time.deltaTime * animationSpeed;
            float curveValue = squashCurve.Evaluate(timer);

            float height = curveValue;
            float width = 1 + (1 - curveValue);

            currentSquashScale = new Vector3(width, height, width);
            yield return null;
        }
        currentSquashScale = Vector3.one;
    }

    IEnumerator PopSequence()
    {
        yield return new WaitForSeconds(lifeTimeAfterLand - 0.5f);

        // Warning Shake
        float shakeTimer = 0.5f;
        while (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
            float shake = Random.Range(0.9f, 1.1f);
            currentSquashScale = new Vector3(shake, shake, shake);
            yield return null;
        }
        Pop();
    }

    public void Pop()
    {
        if (popEffect) Instantiate(popEffect, transform.position, Quaternion.identity);
        if (popSound) AudioSource.PlayClipAtPoint(popSound.clip, transform.position);
        Destroy(gameObject);
    }
}