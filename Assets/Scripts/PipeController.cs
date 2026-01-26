using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeZone : MonoBehaviour
{
    [Header("Settings")]
    // Try these values: (0, 0.8, -0.5) -> Very close behind head
    public Vector3 pipeCameraOffset = new Vector3(0f, 0.8f, -0.5f);

    // Check this box if you want the camera to turn with the player inside the pipe
    public bool lockCameraBehind = true;

    private CameraController theCam;

    void Start()
    {
        theCam = FindObjectOfType<CameraController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (theCam != null)
            {
                // We send 'true' to lock the rotation
                theCam.ChangeOffset(pipeCameraOffset);
                theCam.SetBoatMode(lockCameraBehind); // We re-use BoatMode because it does the exact same rotation logic!
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            if (theCam != null)
            {
                theCam.ResetOffset();
                theCam.SetBoatMode(false); // Turn off rotation lock
            }
        }
    }
}