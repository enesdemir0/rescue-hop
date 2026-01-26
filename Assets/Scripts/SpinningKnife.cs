using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningKnife : MonoBehaviour
{
    [Header("Settings")]
    public float spinSpeed = 180f; // Degrees per second (360 = 1 full spin per second)
    public bool causeKnockback = true;

    // 1. ROTATION (Animation)
    void Update()
    {
        // Rotate only on the Y axis (0, Speed, 0)
        transform.Rotate(0f, spinSpeed * Time.deltaTime, 0f);
    }

    // 2. DAMAGE (Interaction)
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            // Deal Damage
            if (PlayerHealthController.instance != null)
            {
                PlayerHealthController.instance.DamagePlayer();
            }

            // Knock the player back (so they don't get hit 10 times instantly)
            if (causeKnockback && PlayerController.instance != null)
            {
                PlayerController.instance.Bounce();
            }
        }
    }
}