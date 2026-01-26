using UnityEngine;

public class PushSpinner : MonoBehaviour
{
    [Header("Motion Settings")]
    public float swingSpeed = 3f;
    public float maxAngle = 90f;

    [Header("Push Settings")]
    public float pushForce = 40f;

    private Quaternion startRotation;

    void Start()
    {
        startRotation = transform.rotation;
    }

    void Update()
    {
        float angle = maxAngle * Mathf.Sin(Time.time * swingSpeed);
        transform.rotation = startRotation * Quaternion.Euler(0f, 0f, angle);
    }

    // CHANGED FROM COLLISION TO TRIGGER
    private void OnTriggerEnter(Collider other)
    {
        // 1. Check tag
        if (other.tag == "Player")
        {
            if (PlayerController.instance != null)
            {
                // 2. Calculate Direction (Trap -> Player)
                Vector3 pushDirection = other.transform.position - transform.position;

                // 3. Send Signal
                Debug.Log("Trap Triggered! Sending Knockback...");
                PlayerController.instance.Knockback(pushDirection, pushForce);
                PlayerHealthController.instance.DamagePlayer();
            }

            
        }
    }
}