using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonController : MonoBehaviour
{
    [Header("Setup")]
    public GameObject cannonBallPrefab;
    public Transform firePoint;

    [Header("Settings")]
    public float timeBetweenShots = 3f;
    public float shootRange = 15f; // Only shoot if player is close
    public float rotateSpeed = 5f; // How fast it turns

    [Header("Optional")]
    public GameObject fireEffect;
    public AudioSource shootSound;

    private float shotCounter;
    private PlayerController thePlayer;

    void Start()
    {
        thePlayer = FindObjectOfType<PlayerController>();
        shotCounter = timeBetweenShots;
    }

    void Update()
    {
        if (thePlayer == null) return;

        // 1. Check Distance
        float distanceToPlayer = Vector3.Distance(transform.position, thePlayer.transform.position);

        if (distanceToPlayer <= shootRange)
        {
            // 2. ROTATE towards Player
            // We calculate the direction
            Vector3 targetPosition = thePlayer.transform.position;

            // Aim slightly up (at the chest), not at the feet
            targetPosition.y += 1.0f;

            // Calculate rotation needed
            Vector3 direction = targetPosition - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            // Smoothly rotate
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);

            // 3. SHOOT Logic
            shotCounter -= Time.deltaTime;
            if (shotCounter <= 0)
            {
                Shoot();
                shotCounter = timeBetweenShots;
            }
        }
    }

    void Shoot()
    {
        Instantiate(cannonBallPrefab, firePoint.position, firePoint.rotation);
        AudioManager.instance.PlaySFXPitched(20);

        if (fireEffect != null)
        {
            Instantiate(fireEffect, firePoint.position, firePoint.rotation);


        }

        if (shootSound != null)
        {

            shootSound.Play();
        }
    }
}