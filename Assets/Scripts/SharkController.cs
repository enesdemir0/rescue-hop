using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharkController : MonoBehaviour
{
    [Header("Shark Stats")]
    public float swimSpeed = 8f;
    public float turnSpeed = 2f;
    public int damageToBoat = 1;
    public float attackCooldown = 2f;

    [Header("Patrol Settings")]
    public float patrolRange = 10f; // How far it swims from home
    public float patrolSpeed = 4f;  // Swims slower when not chasing

    [Header("Animation")]
    public Animator anim;

    private BoatController targetBoat;
    private bool canAttack = true;

    private Vector3 homePosition;
    private Vector3 currentPatrolPoint;

    void Start()
    {
        targetBoat = FindObjectOfType<BoatController>();

        // Set home to where you placed the shark in the scene
        homePosition = transform.position;

        // Pick the first random spot to swim to
        PickNewPatrolPoint();
    }

    void Update()
    {
        if (targetBoat == null) return;

        // --- LOGIC SPLIT ---

        if (targetBoat.isDriving)
        {
            // CASE A: Player is driving -> CHASE MODE
            // Swim fast towards boat
            MoveToPoint(targetBoat.transform.position, swimSpeed);
        }
        else
        {
            // CASE B: Boat is empty -> PATROL MODE
            // Swim slower towards random points
            Patrol();
        }
    }

    void Patrol()
    {
        // 1. Move towards the current random point
        MoveToPoint(currentPatrolPoint, patrolSpeed);

        // 2. Check if we reached that point
        float distanceToPoint = Vector3.Distance(transform.position, currentPatrolPoint);

        if (distanceToPoint < 1f)
        {
            // We arrived! Pick a new spot.
            PickNewPatrolPoint();
        }
    }

    void PickNewPatrolPoint()
    {
        // Get a random point inside a sphere
        Vector3 randomPoint = Random.insideUnitSphere * patrolRange;

        // Add it to home position
        currentPatrolPoint = homePosition + randomPoint;

        // CRITICAL: Keep the Y (Height) the same as Home.
        // Sharks shouldn't fly into the sky or dig into the ground.
        currentPatrolPoint.y = homePosition.y;
    }

    // I combined Chase and Patrol movement into one function to keep it clean
    void MoveToPoint(Vector3 targetPos, float currentSpeed)
    {
        // 1. Calculate direction
        Vector3 direction = targetPos - transform.position;
        direction.y = 0; // Keep shark level

        // 2. Rotate smoothly
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }

        // 3. Swim Forward (Only if not bouncing back)
        if (canAttack)
        {
            transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        BoatController boat = other.gameObject.GetComponent<BoatController>();

        if (boat != null)
        {
            // ONLY Attack if the player is actually driving!
            if (canAttack && boat.isDriving)
            {
                AttackBoat();
            }
            else
            {
                // If we bumped into the boat but player isn't driving, 
                // just pick a new patrol point so we don't get stuck pushing it.
                PickNewPatrolPoint();
            }
        }
    }

    void AttackBoat()
    {
        canAttack = false;

        if (targetBoat != null)
        {
            targetBoat.TakeDamage(damageToBoat);
        }

        StartCoroutine(BounceBackCo());
    }

    IEnumerator BounceBackCo()
    {
        // Move backward (Hit and Run effect)
        float bounceTimer = 0.5f;
        while (bounceTimer > 0)
        {
            bounceTimer -= Time.deltaTime;
            transform.Translate(Vector3.back * 5f * Time.deltaTime);
            yield return null;
        }

        yield return new WaitForSeconds(attackCooldown);

        canAttack = true;
    }
}