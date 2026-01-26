using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogActivator : MonoBehaviour
{
    [Header("Settings")]
    public Rigidbody logRigidbody; // Drag the Cylinder here
    public float pushForce = 5f;   // Optional kick to start it rolling fast

    private bool hasActivated = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !hasActivated)
        {
            hasActivated = true;

            // 1. Unfreeze the Log (Physics takes over)
            logRigidbody.isKinematic = false;

            // 2. (Optional) Give it a push DOWN the slope
            // transform.forward assumes the trigger is rotated correctly. 
            // Often "Vector3.down" or "Vector3.back" is safer depending on your map.
            logRigidbody.AddForce(Vector3.down * pushForce, ForceMode.Impulse);
        }
    }
}