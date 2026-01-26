using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinnerDamage : MonoBehaviour
{
    [Header("Trap Settings")]
    public int damageAmount = 1;
    public float knockbackForce = 15f; // How hard to throw the player

    [Header("Cooldown")]
    public float damageCooldown = 0.5f; // Prevent getting hit 100 times in 1 second
    private float damageCounter;

    private void Update()
    {
        // Countdown the timer so player can be hit again
        if (damageCounter > 0)
        {
            damageCounter -= Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            // Only hit if cooldown is ready
            if (damageCounter <= 0)
            {
                // 1. Deal Damage
                // (Assuming you have this script based on your previous questions)
                if (PlayerHealthController.instance != null)
                {
                    PlayerHealthController.instance.DamagePlayer();
                }

                // 2. Calculate Knockback Direction
                // We want to push the player AWAY from the center of the trap
                // Formula: (Player Position) - (Trap Arm Position)
                Vector3 pushDirection = other.transform.position - transform.position;

                // Keep the push flat (don't shoot them into the sky or floor)
                pushDirection.y = 0;
                pushDirection = pushDirection.normalized;

                // 3. Call the function inside YOUR PlayerController
                if (PlayerController.instance != null)
                {
                    PlayerController.instance.Knockback(pushDirection, knockbackForce);
                }

                // 4. Reset Timer
                damageCounter = damageCooldown;
            }
        }
    }
}