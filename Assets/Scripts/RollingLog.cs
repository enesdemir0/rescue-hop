using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingLog : MonoBehaviour
{
    [Header("Damage Settings")]
    public bool causeKnockback = true;

    // This runs when the heavy log hits the player
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            // 1. Deal Damage
            if (PlayerHealthController.instance != null)
            {
                PlayerHealthController.instance.DamagePlayer();
            }

            // 2. Knockback
            if (causeKnockback && PlayerController.instance != null)
            {
                // We use your existing Bounce function to squash/push the player
                PlayerController.instance.Bounce();
            }
        }
    }
}