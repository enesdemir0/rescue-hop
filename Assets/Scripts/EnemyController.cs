using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{

    public float moveSpeed, turnSpeed;
    public Transform[] patrolPoints;
    private int currentPatrolPoints;
    public Rigidbody theRB;
    private Vector3 moveDirection, lookTarget;
    private float yStore;

    private PlayerController thePlayer;

    public enum EnemyState { idle, patrolling, chasing, returning};
    public EnemyState currentState;

    public float waitTime, waitChance;
    private float waitCounter;
    public float chaseDistance, chaseSpeed, looseDistance;
    public float waitBeforeReturning;
    private float returnCounter;

    public float hopForce, waitToChase;
    private float chaseWaitCounter;

    public float waitBeforeDieng = .5f;
    public float squashSpeed;
    private float diengCounter;
    public GameObject deathEffect;
    public Animator anim;

    // Start is called before the first frame update
    void Start()
    {

        thePlayer = FindObjectOfType<PlayerController>();

        foreach(Transform pp in patrolPoints)
        {
            pp.parent = null;
        }

        currentState = EnemyState.idle;
        waitCounter = waitTime;
    }

    // Update is called once per frame
    void Update()
    {
        if(diengCounter > 0)
        {
            diengCounter -= Time.deltaTime;
            theRB.velocity = new Vector3(0f, theRB.velocity.y, 0f);
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(1.2f, .5f, 1.2f), squashSpeed * Time.deltaTime);
            if(diengCounter <= 0)
            {
                if(deathEffect != null)
                {
                    Instantiate(deathEffect, transform.position, Quaternion.identity);
                }
                AudioManager.instance.PlaySFX(6);
                Destroy(gameObject);

            }
        }
        else
        {
            switch (currentState)
            {
                case EnemyState.idle:

                    yStore = theRB.velocity.y;
                    theRB.velocity = new Vector3(0f, yStore, 0f);

                    waitCounter -= Time.deltaTime;
                    if (waitCounter <= 0)
                    {
                        currentState = EnemyState.patrolling;
                        NextPatrolPoint();
                    }


                    break;

                case EnemyState.patrolling:

                    yStore = theRB.velocity.y;

                    moveDirection = patrolPoints[currentPatrolPoints].position - transform.position;
                    moveDirection.y = 0f;
                    moveDirection.Normalize();
                    theRB.velocity = moveDirection * moveSpeed;
                    theRB.velocity = new Vector3(theRB.velocity.x, yStore, theRB.velocity.z);

                    if (Vector3.Distance(transform.position, patrolPoints[currentPatrolPoints].position) <= 1f)
                    {

                        NextPatrolPoint();

                    }
                    else
                    {
                        lookTarget = patrolPoints[currentPatrolPoints].position;
                    }
                    break;

                case EnemyState.chasing:
                    lookTarget = thePlayer.transform.position;
                    if (chaseWaitCounter > 0)
                    {
                        chaseWaitCounter -= Time.deltaTime;
                    }
                    else
                    {

                        yStore = theRB.velocity.y;

                        moveDirection = thePlayer.transform.position - transform.position;

                        moveDirection.y = 0f;
                        moveDirection.Normalize();
                        theRB.velocity = moveDirection * chaseSpeed;
                        theRB.velocity = new Vector3(theRB.velocity.x, yStore, theRB.velocity.z);

                    }

                    if (Vector3.Distance(thePlayer.transform.position, transform.position) > looseDistance)
                    {
                        currentState = EnemyState.returning;
                        returnCounter = waitBeforeReturning;
                    }


                    break;

                case EnemyState.returning:

                    returnCounter -= Time.deltaTime;
                    if (returnCounter <= 0)
                    {
                        currentState = EnemyState.patrolling;
                    }
                    break;


            }


            if (currentState != EnemyState.chasing)
            {
                if (Vector3.Distance(thePlayer.transform.position, transform.position) < chaseDistance)
                {
                    currentState = EnemyState.chasing;
                    theRB.velocity = Vector3.up * hopForce;
                    chaseWaitCounter = waitToChase;
                }
            }





            //lookTarget = thePlayer.transform.position;
            lookTarget.y = transform.position.y;
            //transform.LookAt(lookTarget);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookTarget - transform.position), turnSpeed * Time.deltaTime);


            anim.SetBool("isIdle", currentState == EnemyState.idle);
            // 2. Handle MOVING (Patrolling OR Returning)
            // We group Returning with Patrolling because they usually look the same (Walking)
            
            anim.SetBool("isPatrolling", currentState == EnemyState.patrolling);
            anim.SetBool("isChasing",  currentState == EnemyState.chasing);
            anim.SetBool("isReturning", currentState == EnemyState.returning);

        }

   
    }

    public void NextPatrolPoint()
    {
        if(Random.Range(0f, 100f)  < waitChance)
        {
            waitCounter = waitTime;
            currentState = EnemyState.idle;
        }
        else
        {
            currentPatrolPoints++;
            if (currentPatrolPoints >= patrolPoints.Length)
            {
                currentPatrolPoints = 0;
            }

        }

         
    }


    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player" && diengCounter == 0)
        {
            PlayerHealthController.instance.DamagePlayer();
            chaseWaitCounter = waitToChase;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Player" && diengCounter == 0)
        {
            PlayerHealthController.instance.DamagePlayer();
            chaseWaitCounter = waitToChase;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            //Destroy(gameObject);
            diengCounter = waitBeforeDieng;
            AudioManager.instance.PlaySFX(7);
            thePlayer.Bounce();

        }
    }



}
