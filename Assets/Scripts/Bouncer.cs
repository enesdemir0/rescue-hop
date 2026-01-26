using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bouncer : MonoBehaviour
{
    // Add the force variable here so you can change it in Inspector
    public Animator anim;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            // Get the script directly from the object that hit us
            
            PlayerController player = PlayerController.instance;
            if (player != null)
            {
                // Send the force amount to the player
                AudioManager.instance.PlaySFX(16);
                player.Bouncer();
                

                // USE TRIGGER instead of Bool
                // Triggers fire once and reset automatically
                anim.SetTrigger("startBounce");
            }
        }
    }
}