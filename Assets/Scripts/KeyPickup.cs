using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    public GameObject pickupEffect;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            // 1. Set variable
            PlayerController.instance.hasKey = true;
            AudioManager.instance.PlaySFX(22);

            // 2. Make key visible AND fix the trail
            if (PlayerController.instance.keyHandModel != null)
            {
                PlayerController.instance.keyHandModel.SetActive(true);

                // Check for TrailRenderer and clear old lines so it looks clean
                TrailRenderer tr = PlayerController.instance.keyHandModel.GetComponent<TrailRenderer>();
                if (tr != null)
                {
                    tr.Clear();
                }
            }

            Debug.Log("Key Collected!");

            // 3. Play particle effect
            if (pickupEffect != null)
            {
                Instantiate(pickupEffect, transform.position, transform.rotation);
            }

            // 4. Destroy ground key
            Destroy(gameObject);
        }
    }
}