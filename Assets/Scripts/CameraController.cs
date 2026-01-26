using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target;
    public Vector3 pivotOffset = new Vector3(0f, 1.5f, 0f);

    [Header("Movement Settings")]
    public float moveSpeed = 15f;
    public float rotateSpeed = 2.0f;
    public float minViewAngle = -45f;
    public float maxViewAngle = 45f;
    public bool invertY = false;



    [Header("Pipe / Fixed View Settings")]
    public float offsetSmoothSpeed = 5f;
    private Vector3 targetOffset;
    private bool useMouseControl = true;

    // Internal Variables
    private float horizontalAngle;
    private float verticalAngle;
    private float currentDistance;
    private Vector3 defaultOffset;

    // --- NEW: BOAT MODE ---
    private bool isBoatMode = false;

    [HideInInspector]
    public Transform endCamPos;

    private void Start()
    {
        if (target == null)
        {
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player != null) target = player.transform;
        }

        if (target != null)
        {
            defaultOffset = transform.position - target.position;
            currentDistance = defaultOffset.magnitude;
            horizontalAngle = transform.eulerAngles.y;
            verticalAngle = transform.eulerAngles.x;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // Level End
        if (endCamPos != null)
        {
            transform.position = Vector3.Lerp(transform.position, endCamPos.position, moveSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, endCamPos.rotation, moveSpeed * Time.deltaTime);
            return;
        }

        // Pipe Mode
        if (!useMouseControl)
        {
            Vector3 desiredPosition = target.position + targetOffset;
            transform.position = Vector3.Lerp(transform.position, desiredPosition, offsetSmoothSpeed * Time.deltaTime);
            transform.LookAt(target.position + pivotOffset);
        }
        else // Normal & Boat Mode
        {
            // --- BOAT ROTATION LOGIC ---
            if (isBoatMode)
            {
                // Smoothly rotate behind the boat
                horizontalAngle = Mathf.LerpAngle(horizontalAngle, target.eulerAngles.y, 3f * Time.deltaTime);
                horizontalAngle += Input.GetAxis("Mouse X") * rotateSpeed; // Optional mouse adjust
            }
            else
            {
                // Standard Control
                float mouseX = Input.GetAxis("Mouse X") * rotateSpeed;
                horizontalAngle += mouseX;
            }

            float mouseY = Input.GetAxis("Mouse Y") * rotateSpeed;
            if (invertY) verticalAngle += mouseY;
            else verticalAngle -= mouseY;
            verticalAngle = Mathf.Clamp(verticalAngle, minViewAngle, maxViewAngle);

            Quaternion rotation = Quaternion.Euler(verticalAngle, horizontalAngle, 0);
            Vector3 desiredPosition = (target.position + pivotOffset) - (rotation * Vector3.forward * currentDistance);

            transform.position = Vector3.Lerp(transform.position, desiredPosition, moveSpeed * Time.deltaTime);
            transform.LookAt(target.position + pivotOffset);
        }
    }

    // --- EXTERNAL FUNCTIONS ---


    [Header("Level Select Specifics")]
    public bool isLevelSelect = false; // Check this ONLY in the Level Select scene!
    public float mapDistance = 15f;
    public float mapHeight = 45f;

    public void SnapToTarget()
    {
        if (target == null)
        {
            target = FindObjectOfType<PlayerController>().transform;
        }

        // --- THE LOGIC SPLIT ---

        if (isLevelSelect)
        {
            // MODE A: LEVEL SELECT (Force High View)
            currentDistance = mapDistance;
            verticalAngle = mapHeight;
            targetOffset = Vector3.zero; // Reset any pipe offsets
        }
        else
        {
            // MODE B: NORMAL GAMEPLAY (Calculate Natural Distance)
            // If distance is 0 (first load), calculate it based on where you placed the camera
            if (currentDistance == 0)
            {
                currentDistance = (transform.position - target.position).magnitude;
            }

            // Keep the angle dynamic or set a default gameplay angle
            if (verticalAngle == 0) verticalAngle = 10f;
        }

        // -----------------------

        // Standard Math (Same for both)
        horizontalAngle = target.eulerAngles.y;

        Quaternion rotation = Quaternion.Euler(verticalAngle, horizontalAngle, 0);

        Vector3 actualPivot = pivotOffset;
        if (actualPivot.y == 0) actualPivot = new Vector3(0, 1.5f, 0);

        transform.position = (target.position + actualPivot) - (rotation * Vector3.forward * currentDistance);
        transform.rotation = rotation;
    }

    public void ChangeOffset(Vector3 newOffset) { useMouseControl = false; targetOffset = newOffset; }
    public void ResetOffset() { useMouseControl = true; horizontalAngle = target.eulerAngles.y; verticalAngle = 20f; }

    public void SetBoatMode(bool active)
    {
        isBoatMode = active;
        if (active) { currentDistance = 20f; verticalAngle = 25f; }
        else { currentDistance = defaultOffset.magnitude; }
    }
}