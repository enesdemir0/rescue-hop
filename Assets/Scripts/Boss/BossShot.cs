using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossShot : MonoBehaviour
{

    public float moveSpeed;
    public Rigidbody theRB;
    public GameObject impactEffect;



    // Start is called before the first frame update
    void Start()
    {
        transform.LookAt(PlayerController.instance.transform.position + Vector3.up);
        AudioManager.instance.PlaySFXPitched(0);
        
    }

    // Update is called once per frame
    void Update()
    {
        theRB.velocity = transform.forward * moveSpeed;

        
    }


    private void OnTriggerEnter(Collider other)
    {
        // 1. Hit Player
        if (other.tag == "Player")
        {
            PlayerHealthController.instance.DamagePlayer();
            Destroy(gameObject);
        }
        if (impactEffect != null)
        {
            Instantiate(impactEffect, transform.position, transform.rotation);
        }
        

        // 2. HIT BUBBLE (New Feature!)
        // Make sure your Bubble has the Tag "Bubble"
        if (other.tag == "Bubble")
        {
            // Find the script and pop it
            BubblePlatform bubble = other.GetComponent<BubblePlatform>();
            if (bubble != null)
            {
                bubble.Pop(); // The player falls!

                // Optional: Play a "Pop" sound here
                // AudioManager.instance.PlaySFX(..);
            }
            Destroy(gameObject); // Destroy fireball
        }
    }
}
