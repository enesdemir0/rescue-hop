using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed;
    public float rotateSpeed = 10f;
    public float jumpForce;
    public float gravityScale;
    public float bounceForce;

    [Header("Components")]
    public CharacterController characterCon;
    public Animator anim;
    private CameraController cam;

    [Header("Key & Paddle System")]
    public bool hasKey = false;
    public GameObject keyHandModel;

    [Header("Bubble Gun System")]
    public bool hasBubbleGun = false;
    public GameObject bubbleGunModel; 

    // Paddle Variable
    public bool hasPaddle = false;
    public GameObject paddleModel;

    [Header("Effects")]
    public GameObject jumpParticle;
    public GameObject landParticle;
    public Transform feetPosition;
    private bool wasGrounded;

    // Singleton
    public static PlayerController instance;

    // Movement Variables
    private Vector3 moveAmount;
    private float yStore;
    [HideInInspector]
    public bool stopMoving;

    // Knockback Variable
    private Vector3 knockback;

    // --- FIX FOR BUBBLE BOUNCE ---
    private float bubbleCooldown = 0f; // Timer to prevent double-bouncing
    // -----------------------------

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        cam = FindObjectOfType<CameraController>();
        wasGrounded = true;
    }

    private void FixedUpdate()
    {
        if (!characterCon.isGrounded)
        {
            moveAmount.y = moveAmount.y + (Physics.gravity.y * gravityScale * Time.fixedDeltaTime);
        }
        else
        {
            moveAmount.y = Physics.gravity.y * gravityScale * Time.deltaTime;
        }
    }

    private void Update()
    {
        // 1. BOAT / CUTSCENE STOP LOGIC
        if (stopMoving)
        {
            anim.SetFloat("speed", 0f);
            anim.SetBool("isGrounded", true);
            anim.SetFloat("yVel", 0f);
            return;
        }

        // --- BUBBLE COOLDOWN TIMER ---
        if (bubbleCooldown > 0)
        {
            bubbleCooldown -= Time.deltaTime;
        }
        // -----------------------------

        // 2. NORMAL MOVEMENT LOGIC
        if (Time.timeScale > 0 && !LevelManager.instance.levelComplete)
        {
            yStore = moveAmount.y;

            moveAmount = (cam.transform.forward * Input.GetAxisRaw("Vertical")) + (cam.transform.right * Input.GetAxisRaw("Horizontal"));
            moveAmount.y = 0f;
            moveAmount = moveAmount.normalized;

            if (moveAmount.magnitude > .1f)
            {
                if (moveAmount != Vector3.zero)
                {
                    Quaternion newRot = Quaternion.LookRotation(moveAmount);
                    transform.rotation = Quaternion.Slerp(transform.rotation, newRot, rotateSpeed * Time.deltaTime);
                }
            }

            moveAmount.y = yStore;

            // --- JUMP LOGIC ---
            if (characterCon)
            {
                if (characterCon.isGrounded && Input.GetButtonDown("Jump"))
                {
                    moveAmount.y = jumpForce;
                    AudioManager.instance.PlaySFXPitched(11);

                    if (jumpParticle != null && feetPosition != null)
                    {
                        Instantiate(jumpParticle, feetPosition.position, Quaternion.identity);
                    }
                }
            }

            // Movement & Knockback
            Vector3 normalMove = new Vector3(moveAmount.x * moveSpeed, moveAmount.y, moveAmount.z * moveSpeed);
            characterCon.Move((normalMove + knockback) * Time.deltaTime);

            if (knockback.magnitude > 0.2f)
            {
                knockback = Vector3.Lerp(knockback, Vector3.zero, 1f * Time.deltaTime);
            }

            // --- LANDING LOGIC ---
            if (!wasGrounded && characterCon.isGrounded)
            {
                if (landParticle != null && feetPosition != null)
                {
                    Instantiate(landParticle, feetPosition.position, Quaternion.identity);
                }
            }
            wasGrounded = characterCon.isGrounded;

            // 3. Update Animations
            float moveVel = new Vector3(moveAmount.x, 0f, moveAmount.z).magnitude * moveSpeed;
            anim.SetFloat("speed", moveVel);
            // Play the Carry animation if we have Key OR Paddle OR Bubble Gun
            anim.SetBool("hasKey", hasKey || hasPaddle || hasBubbleGun);
            anim.SetBool("isGrounded", characterCon.isGrounded);
            anim.SetFloat("yVel", moveAmount.y);
        }
    }

    // --- CUSTOM MECHANICS ---

    public void Bounce()
    {
        moveAmount.y = bounceForce;
        characterCon.Move(Vector3.up * bounceForce * Time.deltaTime);
    }

    public void Bounce(float force)
    {
        moveAmount.y = force;
        // Important: We reset 'yStore' implicitly in Update, but setting velocity here handles the launch
    }

    public void Bouncer()
    {
        moveAmount.y = 50;
        characterCon.Move(Vector3.up * bounceForce * Time.deltaTime);
    }

    public void Knockback(Vector3 direction, float force)
    {
        direction.y = 0;
        if (direction.magnitude < 0.1f)
        {
            direction = -transform.forward;
        }

        knockback = direction.normalized * force;
        moveAmount.y = 5f;
    }

    public void LaunchPlayer(Vector3 velocity)
    {
        moveAmount.y = velocity.y;
        knockback = new Vector3(velocity.x, 0f, velocity.z);
    }

    // --- COLLISION LOGIC (BUBBLES & SEESAWS) ---
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // 1. CHECK FOR BUBBLE INTERACTION
        if (hit.gameObject.tag == "Bubble")
        {
            // CHECK COOLDOWN: Only bounce if we haven't bounced recently (0.2s)
            if (bubbleCooldown <= 0)
            {
                // CHECK ANGLE: Only bounce if landing ON TOP (not hitting the side)
                if (hit.normal.y > 0.5f)
                {
                    BubblePlatform bubble = hit.gameObject.GetComponent<BubblePlatform>();

                    if (bubble != null)
                    {
                        // A. Trigger the Jiggle/Pop animation
                        bubble.TriggerBubble();

                        // B. BOUNCE THE PLAYER UP (Crucial for climbing!)
                        Bounce(bubble.bounceForce);

                        // C. Play Jump Sound
                        AudioManager.instance.PlaySFXPitched(11); // Use your jump sound index

                        // D. Reset Cooldown
                        bubbleCooldown = 0.2f;
                    }
                }
            }
        }

        // 2. CHECK FOR RIGIDBODY PUSHING (Keep this for Crates/Seesaws)
        Rigidbody body = hit.collider.attachedRigidbody;

        if (body == null || body.isKinematic) return;

        if (hit.moveDirection.y < -0.3f)
        {
            Vector3 pushForce = new Vector3(0, -40f, 0);
            body.AddForceAtPosition(pushForce, hit.point, ForceMode.Impulse);
        }
    }
}