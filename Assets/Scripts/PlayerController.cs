using UnityEngine;

[RequireComponent(typeof(PlayerScore))]
[RequireComponent(typeof(DrawShape))]
[RequireComponent(typeof(ResizeShape))]
public class PlayerController : MonoBehaviour
{
    // Components
    Camera mainCamera;
    ResizeShape resizeShape;
    PlayerScore playerScore;

    // Scaling variables
    private bool isKeyBeingHeld = false;
    private float maxScale = 4.0f;
    private float originalScale;

    // Movement variables
    [SerializeField]
    private float teleportSpeed = 20f; // High speed for teleportation

    [SerializeField]
    private float continuousSpeed = 5f; // Slower speed for continuous movement

    private Vector3? targetPosition = null; // Nullable Vector3 to store the target position

    public bool checkKey { get; private set; }

    // Movement state enumeration
    private enum MovementState
    {
        Idle,
        Teleporting,
        MovingContinuously
    }

    private MovementState currentState = MovementState.Idle;

    private Vector3 movementDirection = Vector3.zero; // Direction of movement
    // Reference to GameManager
    private GameManager gameManager;

    void Start()
    {
        resizeShape = GetComponent<ResizeShape>();
        playerScore = GetComponent<PlayerScore>();

        gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameManager not found in the scene.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCamera();
        HandleInput();
        HandleScaling();
        HandleMovement();
    }

    private void HandleScaling()
    {
        if (isKeyBeingHeld)
        {
            resizeShape.SetScale(maxScale);
        }
        else
        {
            resizeShape.SetScale(null);
        }
    }

    private void HandleInput()
    {
        if (Input.anyKeyDown || Input.anyKey)
        {
            isKeyBeingHeld = true;
        }
        else
        {
            isKeyBeingHeld = false;
        }
    }

    private void HandleMovement()
    {
        switch (currentState)
        {
            case MovementState.Teleporting:
                if (targetPosition.HasValue)
                {
                    // Move towards the target position at teleportSpeed
                    transform.position = Vector3.MoveTowards(transform.position, targetPosition.Value, teleportSpeed * Time.deltaTime);

                    // Check if the player has reached the target position
                    if (Vector3.Distance(transform.position, targetPosition.Value) < 0.001f)
                    {
                        transform.position = targetPosition.Value; // Snap to target position
                        targetPosition = null; // Clear the target position
                        currentState = MovementState.MovingContinuously; // Switch to continuous movement
                    }
                }
                break;

            case MovementState.MovingContinuously:
                // Move continuously in the movementDirection at continuousSpeed
                transform.position += movementDirection * continuousSpeed * Time.deltaTime;
                break;

            case MovementState.Idle:
            default:
                // Do nothing
                break;
        }
    }

    private void TransformToSize(ResizeShape targetResizeShape)
    {
        float scale = targetResizeShape.originalOuterRadius / resizeShape.originalOuterRadius;
        resizeShape.OverrideShape(scale);
    }
    
    public void TeleportTo(Vector3 position, ResizeShape targetResizeShape)
    {
        playerScore.ComputeEating(position);
        targetPosition = new Vector3(position.x, position.y);
        // Calculate direction from current position to target
        movementDirection = (targetPosition.Value - transform.position).normalized;
        // Set state to Teleporting
        currentState = MovementState.Teleporting;
        TransformToSize(targetResizeShape);
    }

    void UpdateCamera()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        if (mainCamera != null)
        {
            mainCamera.transform.position = new Vector3(transform.position.x, transform.position.y, mainCamera.transform.position.z);
        }
    }

    public void SetGetKey(bool value)
    {
        checkKey = value;
    }

    public void Die()
    {
        // Play death sounds and particles
        // deathAudio.Play(); // Uncomment when AudioSource is added
        // deathParticles.Play(); // Uncomment when ParticleSystem is added

        // Notify GameManager
        playerScore.FinishStreak();
        if (gameManager != null)
        {
            gameManager.scoreManager.SaveScore();
            gameManager.PlayerDied();
        }

        // Destroy the player object
        Destroy(gameObject);
    }
}
