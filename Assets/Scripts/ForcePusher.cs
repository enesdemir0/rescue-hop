using UnityEngine;

public class ForcePusher : MonoBehaviour
{
    [Header("Settings")]
    public float pushForce = 35f; // How hard to throw the player

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            // 1. Calculate direction AWAY from the arm
            Vector3 pushDir = other.transform.position - transform.position;
            pushDir.y = 0; // Keep it horizontal
            pushDir.Normalize();

            // 2. Call the Knockback function on the Player
            if (PlayerController.instance != null)
            {
                // We reuse the Knockback function you already have!
                PlayerController.instance.Knockback(pushDir, pushForce);
            }
        }
    }
}