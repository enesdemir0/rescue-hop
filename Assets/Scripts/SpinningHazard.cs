using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningHazard : MonoBehaviour
{
    [Header("Alive Animation Settings")]
    public Vector3 rotationSpeed = new Vector3(45f, 45f, 0f);

    [Header("Knockback Setting")]
    public bool causeKnockback = true;

    void Update()
    {
        // 1. Spin the object
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // 2. Check if the object we hit is the Player
        if (other.tag == "Player")
        {
            // A. Deal Damage (Using Instance)
            if (PlayerHealthController.instance != null)
            {
                PlayerHealthController.instance.DamagePlayer();
            }

            // B. Knockback (Using YOUR PlayerController Instance)
            // No need for GetComponent! We just call the instance directly.
            if (causeKnockback)
            {
                PlayerController.instance.Bounce();
            }
        }
    }
}