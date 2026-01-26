using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindArea : MonoBehaviour
{
    [Header("Wind Settings")]
    public float windStrength = 15f;
    public Vector3 direction = new Vector3(0, 1, 0); // Default = UP

    private void OnTriggerStay(Collider other)
    {
        // Use CompareTag because it is faster than "=="
        if (other.CompareTag("Player"))
        {
            // Talk directly to the Player Singleton
            // We use .normalized so the speed is constant even if you type (0, 100, 0)
            Vector3 pushVector = direction.normalized * windStrength * Time.deltaTime;

            PlayerController.instance.characterCon.Move(pushVector);
        }
    }
}