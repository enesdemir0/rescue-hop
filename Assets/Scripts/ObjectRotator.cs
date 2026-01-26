using UnityEngine;

public class ObjectRotator : MonoBehaviour
{
    [Header("Rotation & Bobbing (Idle)")]
    public Vector3 rotationAngle = new Vector3(0, 50, 0);
    public float bobSpeed = 2f;
    public float bobHeight = 0.05f;

    [Header("Running Sway (Movement)")]
    public float swaySpeed = 10f;  // How fast it swings (match your run animation speed)
    public float swayAmount = 0.1f; // How far forward/back it moves

    private Vector3 startPos;

    void Start()
    {
        // Remember where it is relative to the hand/back
        startPos = transform.localPosition;
    }

    void Update()
    {
        // 1. Rotate around itself (Always happens)
        transform.Rotate(rotationAngle * Time.deltaTime, Space.Self);

        // 2. Calculate Vertical Bobbing (Always happens)
        float newY = startPos.y + (Mathf.Sin(Time.time * bobSpeed) * bobHeight);

        // 3. Calculate Forward/Back Sway (Only when running)
        float zOffset = 0f;

        // We check the Player Controller to see if we are moving
        if (PlayerController.instance != null)
        {
            // Get the speed ignoring Y (jumping/falling)
            Vector3 horizontalVelocity = PlayerController.instance.characterCon.velocity;
            horizontalVelocity.y = 0;

            // If player is moving faster than 0.1, add the sway
            if (horizontalVelocity.magnitude > 0.1f)
            {
                // We use Cos or Sin to create a wave based on time
                zOffset = Mathf.Sin(Time.time * swaySpeed) * swayAmount;
            }
        }

        // 4. Apply all changes to position
        // startPos.z + zOffset makes it move forward/back
        transform.localPosition = new Vector3(startPos.x, newY, startPos.z + zOffset);
    }
}