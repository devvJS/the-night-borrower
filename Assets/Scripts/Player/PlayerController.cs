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

    [Header("Physics")]
    [SerializeField] private float gravity = GameConstants.Gravity;

    private CharacterController characterController;
    private Transform cameraTransform;

    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool isSprinting;
    private float verticalVelocity;
    private float cameraPitch;

    // Input Actions
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction sprintAction;

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
    }

    private void OnDisable()
    {
        moveAction.Disable();
        lookAction.Disable();
        sprintAction.Disable();
    }

    private void OnDestroy()
    {
        moveAction?.Dispose();
        lookAction?.Dispose();
        sprintAction?.Dispose();
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
