using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [Header("Launch Settings")]
    public float launchForce = 10f;          
    public float gravityAfterLaunch = 1f;
    public float minDragDistance = 1f;    // Minimum distance required to launch
    public float maxDragDistance = 5f;    // Maximum drag distance for max force
    public float minLaunchForce = 5f;     // Minimum force applied
    public float maxLaunchForce = 20f;    // Maximum force applied
    public float forceMultiplier = 5f;
    public Vector2 initialPosition;

    [Header("Visual Feedback")]
    public LineRenderer aimingLine;           // Reference to LineRenderer
    public Color lineColor = Color.white;

    private Rigidbody2D rb;                  
    private Vector2 launchDirection;
    private Vector2 initialMousePosition;
    private bool isDragging = false;         
    private bool isLaunched = false;
    private float currentDragDistance;   
    private float calculatedLaunchForce;

    private bool hasCollectedLetter;
    private float lastCollectionTime = 0f;
    private float collectionCooldown = 0.5f;

    private GameManager gameManager;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D component missing from the ball.");
        }
        rb.gravityScale = 0f;
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;

        // Initialize LineRenderer
        if (aimingLine == null)
        {
            aimingLine = GetComponent<LineRenderer>();
            if (aimingLine == null)
            {
                Debug.LogError("LineRenderer component missing from the ball.");
            }
        }

        aimingLine.positionCount = 0; // Hide the line initially
        aimingLine.startColor = lineColor;
        aimingLine.endColor = lineColor;
        aimingLine.startWidth = 0.05f;
        aimingLine.endWidth = 0.05f;

        initialPosition = gameObject.transform.position;

        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameManager not found in the scene.");
        }

        hasCollectedLetter = false;
    }

    void Update()
    {
        if (isLaunched || (gameManager != null && !gameManager.canShoot)) // Check if shooting is disabled
            return;

        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            initialMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            aimingLine.positionCount = 2;
        }

        if (isDragging)
        {
            Vector2 currentMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 dragVector = currentMousePosition - initialMousePosition;
            launchDirection = -dragVector.normalized; // Negative to launch towards drag direction

            currentDragDistance = dragVector.magnitude;

            // Clamp the drag distance
            float clampedDragDistance = Mathf.Clamp(currentDragDistance, minDragDistance, maxDragDistance);

            // Map drag distance to launch force
            calculatedLaunchForce = Mathf.Lerp(minLaunchForce, maxLaunchForce, (clampedDragDistance - minDragDistance) / (maxDragDistance - minDragDistance));

            Vector3 startPoint = transform.position;
            Vector3 endPoint = transform.position + (Vector3)(launchDirection * calculatedLaunchForce * 0.1f); // Scale down for visualization
            aimingLine.SetPosition(0, startPoint);
            aimingLine.SetPosition(1, endPoint);
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            LaunchBall();
            isDragging = false;
            isLaunched = true;

            aimingLine.positionCount = 0;
        }
    }

    /// <summary>
    /// Launches the ball in the calculated direction and enables gravity.
    /// </summary>
    void LaunchBall()
    {
        rb.isKinematic = false;

        rb.gravityScale = gravityAfterLaunch;

        rb.velocity = launchDirection * calculatedLaunchForce;
        
        hasCollectedLetter = false;

        // Optional: Add torque or other forces if needed
        // rb.AddTorque(5f, ForceMode2D.Impulse);
    }

    public void ResetBall()
    {
        isLaunched = false;
        rb.isKinematic = true;
        rb.gravityScale = 0f;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        // Optionally, reset the ball's position
        transform.position = initialPosition;
        hasCollectedLetter = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Reset"))
        {
            ResetBall();
        }
    }

    public bool CanCollectLetter()
    {
        return Time.time - lastCollectionTime > collectionCooldown;
    }

    public void MarkLetterAsCollected()
    {
        hasCollectedLetter = true;
        lastCollectionTime = Time.time; // Update the last collection time
    }
}
