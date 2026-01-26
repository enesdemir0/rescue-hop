using UnityEngine;

public class PaddleRotator : MonoBehaviour
{
    public float rowSpeed = 300f;
    public float maxRotationAngle = 45f;

    // Assign the Boat's Rigidbody here in Inspector
    public Rigidbody boatRB;

    private float currentAngle;
    private float direction = 1;

    void Update()
    {
        // Only row if the boat is actually moving
        if (boatRB.velocity.magnitude > 0.5f)
        {
            // Calculate a "Ping Pong" motion (Back and Forth)
            float rotateAmount = rowSpeed * Time.deltaTime * direction;
            transform.Rotate(Vector3.right * rotateAmount); // Rotate around X axis

            currentAngle += rotateAmount;

            // Change direction if we rowed too far back or forward
            if (currentAngle > maxRotationAngle || currentAngle < -maxRotationAngle)
            {
                direction *= -1; // Reverse direction
            }
        }
    }
}