using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed = GameConstants.WalkSpeed;
    [SerializeField] private float sprintSpeed = GameConstants.SprintSpeed;

    [Header("Mouse Look")]
    [SerializeField] private float mouseSensitivity = GameConstants.MouseSensitivity;
    [SerializeField] private float verticalLookClamp = GameConstants.VerticalLookClamp;

    [Header("Interaction")]
    [SerializeField] private float interactionRange = GameConstants.InteractionRange;
    [SerializeField] private LayerMask interactableLayer = ~0;

    [Header("Physics")]
    [SerializeField] private float gravity = GameConstants.Gravity;

    private CharacterController characterController;
    private Transform cameraTransform;

    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool isSprinting;
    private float verticalVelocity;
    private float cameraPitch;
    private InteractableObject currentHighlighted;

    private bool isInteracting;

    public InteractableObject CurrentHighlighted => currentHighlighted;
    public bool IsInteracting => isInteracting;

    // Input Actions
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction sprintAction;
    private InputAction interactAction;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        cameraTransform = GetComponentInChildren<Camera>().transform;

        SetupInputActions();
    }

    private void OnEnable()
    {
        moveAction.Enable();
        lookAction.Enable();
        sprintAction.Enable();
        interactAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        lookAction.Disable();
        sprintAction.Disable();
        interactAction.Disable();
    }

    private void OnDestroy()
    {
        moveAction?.Dispose();
        lookAction?.Dispose();
        sprintAction?.Dispose();
        interactAction?.Dispose();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        ReadInput();
        HandleMouseLook();
        HandleHighlighting();
        HandleInteraction();
        HandleMovement();
    }

    private void SetupInputActions()
    {
        moveAction = new InputAction("Move", InputActionType.Value, binding: null);
        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");

        lookAction = new InputAction("Look", InputActionType.Value, "<Mouse>/delta");

        sprintAction = new InputAction("Sprint", InputActionType.Button, "<Keyboard>/leftShift");

        interactAction = new InputAction("Interact", InputActionType.Button, "<Keyboard>/e");
    }

    private void ReadInput()
    {
        moveInput = moveAction.ReadValue<Vector2>();
        lookInput = lookAction.ReadValue<Vector2>();
        isSprinting = sprintAction.IsPressed();
    }

    private void HandleMouseLook()
    {
        // Mouse delta is already per-frame from the Input System — no deltaTime needed.
        // Sensitivity of 0.1 means ~0.1 degrees per pixel of mouse movement.
        float mouseX = lookInput.x * mouseSensitivity * GameConstants.MouseSensitivityScale;
        float mouseY = lookInput.y * mouseSensitivity * GameConstants.MouseSensitivityScale;

        // Horizontal rotation on player transform
        transform.Rotate(Vector3.up, mouseX);

        // Vertical rotation on camera with clamping
        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, -verticalLookClamp, verticalLookClamp);
        cameraTransform.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
    }

    private void HandleHighlighting()
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactionRange, interactableLayer))
        {
            InteractableObject interactable = hit.collider.GetComponent<InteractableObject>();

            if (interactable != null)
            {
                if (interactable != currentHighlighted)
                {
                    if (currentHighlighted != null)
                    {
                        currentHighlighted.Unhighlight();
                    }
                    currentHighlighted = interactable;
                    currentHighlighted.Highlight();
                }
                return;
            }
        }

        // Ray hit nothing or a non-interactable
        if (currentHighlighted != null)
        {
            currentHighlighted.Unhighlight();
            currentHighlighted = null;
        }
    }

    private void HandleInteraction()
    {
        if (!interactAction.WasPressedThisFrame()) return;
        if (isInteracting) return;
        if (currentHighlighted == null) return;

        isInteracting = true;
        currentHighlighted.Interact();
        GameEvents.ObjectInspected(currentHighlighted.ObjectId);

        // No real interaction flow yet — complete immediately.
        // Future stories (2.2 Focused Inspection, 2.5 Organization) will call
        // OnInteractionComplete() when their interaction flow finishes.
        OnInteractionComplete();
    }

    public void OnInteractionComplete()
    {
        isInteracting = false;
    }

    private void HandleMovement()
    {
        // Gravity
        if (characterController.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = GameConstants.GroundedDownForce;
        }
        verticalVelocity += gravity * Time.deltaTime;

        // Movement relative to camera facing
        float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;
        Vector3 moveDirection = transform.right * moveInput.x + transform.forward * moveInput.y;
        Vector3 velocity = moveDirection * currentSpeed;
        velocity.y = verticalVelocity;

        characterController.Move(velocity * Time.deltaTime);
    }
}
