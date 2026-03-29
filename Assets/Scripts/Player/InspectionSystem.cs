using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class InspectionSystem : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private PlayerController playerController;

    private Camera mainCamera;
    private bool isInspecting;
    private InteractableObject inspectedObject;
    private Vector3 originalCameraLocalPos;
    private Quaternion originalCameraLocalRot;
    private float originalFOV;
    private float rotationX;
    private float rotationY;
    private Coroutine transitionCoroutine;

    private ObservationSystem observationSystem;
    private PlayerFlashlight playerFlashlight;
    private PlayerHUD playerHUD;

    private InputAction inspectAction;
    private InputAction exitAction;
    private InputAction lookAction;

    public bool IsInspecting => isInspecting;
    public InteractableObject InspectedObject => inspectedObject;

    private void Awake()
    {
        if (cameraTransform == null)
            cameraTransform = GetComponentInChildren<Camera>().transform;

        mainCamera = cameraTransform.GetComponent<Camera>();

        if (playerController == null)
            playerController = GetComponent<PlayerController>();

        observationSystem = GetComponent<ObservationSystem>();
        playerFlashlight = GetComponent<PlayerFlashlight>();
        playerHUD = FindObjectOfType<PlayerHUD>();

        inspectAction = new InputAction("Inspect", InputActionType.Button, "<Keyboard>/e");
        exitAction = new InputAction("ExitInspection", InputActionType.Button, "<Keyboard>/escape");
        lookAction = new InputAction("InspectionLook", InputActionType.Value, "<Mouse>/delta");
    }

    private void OnEnable()
    {
        inspectAction.Enable();
        exitAction.Enable();
        lookAction.Enable();
    }

    private void OnDisable()
    {
        inspectAction.Disable();
        exitAction.Disable();
        lookAction.Disable();
    }

    private void OnDestroy()
    {
        inspectAction?.Dispose();
        exitAction?.Dispose();
        lookAction?.Dispose();
    }

    private void Update()
    {
        if (!isInspecting)
        {
            if (inspectAction.WasPressedThisFrame()
                && playerController.CurrentHighlighted != null
                && playerController.CurrentHighlighted.IsInspectable
                && !playerController.CurrentHighlighted.IsDisplaced
                && !playerController.CurrentHighlighted.IsOrganizing
                && !(playerController.CurrentHighlighted.Fixture != null
                     && !playerController.CurrentHighlighted.Fixture.IsFunctional)
                && !playerController.IsInteracting
                && transitionCoroutine == null)
            {
                EnterInspection(playerController.CurrentHighlighted);
            }
        }
        else
        {
            if ((inspectAction.WasPressedThisFrame() || exitAction.WasPressedThisFrame())
                && transitionCoroutine == null)
            {
                ExitInspection();
            }

            if (transitionCoroutine == null)
            {
                HandleRotation();
            }
        }
    }

    private void EnterInspection(InteractableObject target)
    {
        inspectedObject = target;
        originalCameraLocalPos = cameraTransform.localPosition;
        originalCameraLocalRot = cameraTransform.localRotation;
        originalFOV = mainCamera.fieldOfView;
        rotationX = 0f;
        rotationY = 0f;

        playerController.SetInputEnabled(false);

        if (observationSystem != null)
            observationSystem.enabled = false;
        if (playerFlashlight != null)
            playerFlashlight.enabled = false;

        Vector3 targetPos = inspectedObject.InspectionFocusPoint
                            - cameraTransform.forward * inspectedObject.InspectionDistance;
        Quaternion targetRot = Quaternion.LookRotation(
            inspectedObject.InspectionFocusPoint - targetPos);

        GameEvents.ObjectInspected(inspectedObject.ObjectId);

        InspectionResult result = inspectedObject.Inspect();

        if (result.isFirstInspection && result.hasClue && !string.IsNullOrEmpty(result.clueId))
        {
            GameEvents.ClueDiscovered(result.clueId, inspectedObject.ObjectId);
        }

        if (playerHUD != null)
            playerHUD.InspectionUI.Show(result);

        transitionCoroutine = StartCoroutine(
            TransitionCamera(targetPos, targetRot, GameConstants.InspectionFOV, true));
    }

    private void ExitInspection()
    {
        if (playerHUD != null)
            playerHUD.InspectionUI.Hide();

        Vector3 worldTargetPos = cameraTransform.parent.TransformPoint(originalCameraLocalPos);
        Quaternion worldTargetRot = cameraTransform.parent.rotation * originalCameraLocalRot;

        transitionCoroutine = StartCoroutine(
            TransitionCamera(worldTargetPos, worldTargetRot, originalFOV, false));
    }

    private IEnumerator TransitionCamera(
        Vector3 targetPos, Quaternion targetRot, float targetFOV, bool entering)
    {
        Vector3 startPos = cameraTransform.position;
        Quaternion startRot = cameraTransform.rotation;
        float startFOV = mainCamera.fieldOfView;
        float elapsed = 0f;

        while (elapsed < GameConstants.InspectionTransitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f,
                elapsed / GameConstants.InspectionTransitionDuration);

            cameraTransform.position = Vector3.Lerp(startPos, targetPos, t);
            cameraTransform.rotation = Quaternion.Slerp(startRot, targetRot, t);
            mainCamera.fieldOfView = Mathf.Lerp(startFOV, targetFOV, t);
            yield return null;
        }

        cameraTransform.position = targetPos;
        cameraTransform.rotation = targetRot;
        mainCamera.fieldOfView = targetFOV;

        if (entering)
        {
            isInspecting = true;
        }
        else
        {
            cameraTransform.localPosition = originalCameraLocalPos;
            cameraTransform.localRotation = originalCameraLocalRot;
            mainCamera.fieldOfView = originalFOV;
            isInspecting = false;
            inspectedObject = null;

            playerController.SetInputEnabled(true);

            if (observationSystem != null)
                observationSystem.enabled = true;
            if (playerFlashlight != null)
                playerFlashlight.enabled = true;
        }

        transitionCoroutine = null;
    }

    private void HandleRotation()
    {
        Vector2 delta = lookAction.ReadValue<Vector2>();
        rotationX += delta.x * GameConstants.InspectionRotationSpeed * Time.deltaTime;
        rotationY -= delta.y * GameConstants.InspectionRotationSpeed * Time.deltaTime;
        rotationY = Mathf.Clamp(rotationY,
            -GameConstants.InspectionMaxPitch, GameConstants.InspectionMaxPitch);

        Quaternion rotation = Quaternion.Euler(rotationY, rotationX, 0f);
        Vector3 offset = rotation * new Vector3(0f, 0f, -inspectedObject.InspectionDistance);
        cameraTransform.position = inspectedObject.InspectionFocusPoint + offset;
        cameraTransform.LookAt(inspectedObject.InspectionFocusPoint);
    }
}
