using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaddlePickup : MonoBehaviour
{
    public GameObject pickupEffect;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            // 1. Set the variable to TRUE
            PlayerController.instance.hasPaddle = true;

            // 2. Make the paddle on the player's back visible
            if (PlayerController.instance.paddleModel != null)
            {
                PlayerController.instance.paddleModel.SetActive(true);
            }

            Debug.Log("Paddle Collected!");

            // 3. Play particle effect
            if (pickupEffect != null)
            {
                
                Instantiate(pickupEffect, transform.position, transform.rotation);
            }

            // 4. Destroy the paddle on the ground
            AudioManager.instance.PlaySFX(18);
            Destroy(gameObject);
        }
    }
}