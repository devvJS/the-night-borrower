using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerFlashlight : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;

    private GameObject flashlightObject;
    private Light flashlightLight;
    private InputAction flashlightAction;
    private bool isOn;

    public bool IsOn => isOn;

    private void Awake()
    {
        if (cameraTransform == null)
        {
            cameraTransform = GetComponentInChildren<Camera>().transform;
        }

        // Create Spot Light as child of camera — inherits direction automatically
        flashlightObject = new GameObject("FlashlightLight");
        flashlightObject.transform.SetParent(cameraTransform, false);

        flashlightLight = flashlightObject.AddComponent<Light>();
        flashlightLight.type = LightType.Spot;
        flashlightLight.range = GameConstants.FlashlightRange;
        flashlightLight.spotAngle = GameConstants.FlashlightSpotAngle;
        flashlightLight.innerSpotAngle = GameConstants.FlashlightInnerSpotAngle;
        flashlightLight.intensity = GameConstants.FlashlightIntensity;
        flashlightLight.color = Color.white;
        flashlightLight.shadows = LightShadows.Soft;

        flashlightObject.SetActive(false);

        flashlightAction = new InputAction("Flashlight", InputActionType.Button, "<Keyboard>/f");
    }

    private void OnEnable()
    {
        flashlightAction.Enable();
    }

    private void OnDisable()
    {
        flashlightAction.Disable();
    }

    private void OnDestroy()
    {
        flashlightAction?.Dispose();
    }

    private void Update()
    {
        if (flashlightAction.WasPressedThisFrame())
        {
            isOn = !isOn;
            flashlightObject.SetActive(isOn);
        }
    }
}
