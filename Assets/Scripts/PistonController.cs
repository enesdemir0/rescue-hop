using UnityEngine;

public class PistonController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public float moveDistance = 3f; // How far it moves out

    [Header("Direction")]
    // Set X=1 for Right, X=-1 for Left, Y=1 for Up, etc.
    public Vector3 direction = new Vector3(1f, 0f, 0f);

    public float startDelay = 0f; // Optional: offset the timing

    private Vector3 startPos;

    void Start()
    {
        // Remember exactly where it is relative to the parent
        startPos = transform.localPosition;
    }

    void Update()
    {
        // Calculate the movement (0 -> Distance -> 0)
        float cycle = Mathf.PingPong((Time.time + startDelay) * moveSpeed, moveDistance);

        // Move the block smoothly
        transform.localPosition = startPos + (direction.normalized * cycle);
    }
}