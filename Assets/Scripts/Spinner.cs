using UnityEngine;

public class Spinner : MonoBehaviour
{
    public float rotateSpeed = 90f; // Degrees per second
    public Rigidbody rb;

    void Start()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
    }

    // We use FixedUpdate for moving physical objects to push the player smoothly
    void FixedUpdate()
    {
        // Calculate rotation
        Quaternion turnOffset = Quaternion.Euler(0f, rotateSpeed * Time.fixedDeltaTime, 0f);

        // Apply rotation to the Rigidbody
        rb.MoveRotation(rb.rotation * turnOffset);
    }
}