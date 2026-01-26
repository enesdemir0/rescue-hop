using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : MonoBehaviour
{
    public float moveSpeed = 15f;
    public float lifeTime = 5f;
    public int damageAmount = 1;

    public GameObject impactEffect; // Optional explosion effect
    public Rigidbody theRB;

    void Start()
    {
        // 1. Force the ball to fly forward instantly
        theRB.velocity = transform.forward * moveSpeed;

        // 2. Destroy it after X seconds so the game doesn't lag
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // If it hits the Player
        if (other.tag == "Player")
        {
            // Assuming you have a Health Controller
            // PlayerHealthController.instance.DamagePlayer(damageAmount);
            // OR simple approach:
            FindObjectOfType<PlayerHealthController>().DamagePlayer();

            DestroyBall();
        }

        // If it hits the ground or walls (optional, check Layer or Tag)
        // You might want to ignore the "Cannon" itself so it doesn't explode instantly
        if (other.tag != "Cannon" && other.tag != "Player" && other.tag != "Checkpoint")
        {
            DestroyBall();
        }
    }

    void DestroyBall()
    {
        if (impactEffect != null)
        {
            Instantiate(impactEffect, transform.position, transform.rotation);
        }
        Destroy(gameObject);
    }
}