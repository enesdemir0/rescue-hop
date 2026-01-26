using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleGun : MonoBehaviour
{
    public GameObject bubblePrefab; // Drag from PROJECT FOLDER
    public Transform firePoint;
    public KeyCode shootKey = KeyCode.E;
    public float shootForce = 5f;

    private PlayerController player;
    private float cooldownTimer;

    void Start()
    {
        player = GetComponent<PlayerController>();
    }

    void Update()
    {
        if (cooldownTimer > 0) cooldownTimer -= Time.deltaTime;

        // CHECK: Do we have the gun?
        if (player.hasBubbleGun && Input.GetKeyDown(shootKey) && cooldownTimer <= 0)
        {
            Shoot();
        }
    }

    [Header("Gun Adjustments")]
    public float upwardAngle = 0.5f; // Controls how steep the stairs are (0.5 is good)

    void Shoot()
    {
        cooldownTimer = 0.2f;

        // Visuals

        if (bubblePrefab != null)
        {
            // 1. Spawn at the wand position
            GameObject newBubble = Instantiate(bubblePrefab, firePoint.position, firePoint.rotation);

            Rigidbody rb = newBubble.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // 2. CALCULATE DIRECTION (Forward + UP)
                // This creates the "Staircase" effect automatically
                Vector3 shootDir = firePoint.forward + (Vector3.up * upwardAngle);

                // Normalize so speed is consistent
                shootDir.Normalize();

                // 3. Apply Force
                rb.velocity = shootDir * shootForce;
            }
        }
    }
}