using UnityEngine;

[RequireComponent(typeof(DrawShape))]
[RequireComponent(typeof(ResizeShape))]
public class PlayerController : MonoBehaviour
{
    // Components
    Camera mainCamera;
    DrawShape drawShape;
    ResizeShape resizeShape;

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

    // Movement state enumeration
    private enum MovementState
    {
        Idle,
        Teleporting,
        MovingContinuously
    }

    private MovementState currentState = MovementState.Idle;

    private Vector3 movementDirection = Vector3.zero; // Direction of movement

    void Start()
    {
        resizeShape = GetComponent<ResizeShape>();
        drawShape = GetComponent<DrawShape>();
        drawShape.SetPolygon(12, 0.5f, 0.4f, false, EntityType.Player);
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
}
