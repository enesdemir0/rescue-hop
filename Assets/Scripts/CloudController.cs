using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudController : MonoBehaviour
{
    
    public float moveSpeed = 3f;
    public Transform[] patrolPoints;
    public float waitAtEnd = 1f;

    private int currentPointIndex;
    private Transform currentTarget;
    private float waitCounter;
    private bool isWaiting;

    void Start()
    {
        // Detach the points so they don't move with the cloud parent
        foreach (Transform point in patrolPoints)
        {
            point.parent = null;
        }

        // Initialize target
        currentPointIndex = 0;
        currentTarget = patrolPoints[0];
        waitCounter = waitAtEnd;
    }

    void Update()
    {
        // 1. Handling the Wait Time
        if (isWaiting)
        {
            waitCounter -= Time.deltaTime;
            if (waitCounter <= 0)
            {
                isWaiting = false;
                SwitchTarget();
            }
            return;
        }

        // 2. Moving the Cloud
        // Since we are NOT parenting the player, this will slide 
        // out from under the player's feet. The player must walk to keep up!
        transform.position = Vector3.MoveTowards(transform.position, currentTarget.position, moveSpeed * Time.deltaTime);

        // 3. Checking Distance
        if (Vector3.Distance(transform.position, currentTarget.position) < 0.1f)
        {
            isWaiting = true;
            waitCounter = waitAtEnd;
        }
    }

    void SwitchTarget()
    {
        currentPointIndex++;
        if (currentPointIndex >= patrolPoints.Length)
        {
            currentPointIndex = 0;
        }
        currentTarget = patrolPoints[currentPointIndex];
    }

}
