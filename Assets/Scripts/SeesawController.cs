using UnityEngine;

public class SeesawController : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float rotateSpeed = 15f; // How fast it tilts

    [Header("Target Angles (X Axis)")]
    // If Player is on Platform A, the object will rotate to this X angle.
    // Try 25 or -25. If it goes the wrong way, just change the number here!
    public float angleForPlatformA = 25f;

    // If Player is on Platform B, the object will rotate to this X angle.
    public float angleForPlatformB = -25f;

    // These variables are controlled by the Sensor scripts automatically
    [HideInInspector] public bool playerOnA;
    [HideInInspector] public bool playerOnB;

    void Update()
    {
        // 1. Determine the Target Angle
        float targetX = 0f; // Default = 0 (Balanced / Flat)

        if (playerOnA)
        {
            targetX = angleForPlatformA;
        }
        else if (playerOnB)
        {
            targetX = angleForPlatformB;
        }

        // 2. Calculate the Smooth Rotation
        // We take current rotation...
        Quaternion currentRot = transform.localRotation;

        // ...and we want to go to the new X angle (keeping Y and Z at 0)
        Quaternion targetRot = Quaternion.Euler(targetX, 0f, 0f);

        // 3. Apply Rotation
        transform.localRotation = Quaternion.RotateTowards(
            currentRot,
            targetRot,
            rotateSpeed * Time.deltaTime
        );
    }
}