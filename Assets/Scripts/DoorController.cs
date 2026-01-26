using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [Header("Door Settings")]
    public GameObject doorModel; // The part of the door that moves
    public float openAngle = 90f; // How many degrees to open? (try 90 or -90)
    public float openSpeed = 2f;  // How fast it opens

    [Header("Effects")]
    public GameObject openEffect;

    private bool isOpened = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (isOpened) return;

            // Check if player has the key
            if (PlayerController.instance.hasKey == true)
            {
                AudioManager.instance.PlaySFX(23);
                // Start the opening process
                StartCoroutine(RotateDoor());
            }
            else
            {
                Debug.Log("Locked! Find the key.");
            }
        }
    }

    IEnumerator RotateDoor()
    {
        isOpened = true;

        // 1. Take the key away
        PlayerController.instance.hasKey = false;
        if (PlayerController.instance.keyHandModel != null)
        {
            PlayerController.instance.keyHandModel.SetActive(false);
        }

        // 2. Play Effect
        if (openEffect != null)
        {
            Instantiate(openEffect, transform.position, transform.rotation);
        }

        // 3. SMOOTH ROTATION LOGIC
        // We want to rotate FROM where we are TO the current angle + 90 degrees
        Quaternion startRotation = doorModel.transform.rotation;
        Quaternion endRotation = Quaternion.Euler(0, startRotation.eulerAngles.y + openAngle, 0);

        float time = 0;

        while (time < 1)
        {
            // Slerp moves smoothly between two rotations
            doorModel.transform.rotation = Quaternion.Slerp(startRotation, endRotation, time);

            time += Time.deltaTime * openSpeed;
            yield return null; // Wait for next frame
        }

        // Ensure it ends exactly at the final angle
        doorModel.transform.rotation = endRotation;
    }
}