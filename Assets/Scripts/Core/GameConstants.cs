public static class GameConstants
{
    // ─── Player Movement ───
    public const float WalkSpeed = 3.5f;
    public const float SprintSpeed = 5.5f;
    public const float MouseSensitivity = 2.0f;
    public const float Gravity = -9.81f;
    public const float VerticalLookClamp = 80f;
    public const float MouseSensitivityScale = 0.1f;
    public const float GroundedDownForce = -2f;

    // ─── Object Highlighting ───
    public const float InteractionRange = 3.0f;
    public const float HighlightFadeDuration = 0.2f;
    public const float HighlightEmissionIntensity = 0.3f;
    // HighlightColor: default warm white (1.0, 0.95, 0.8) — set via InteractableObject inspector since Color cannot be const

    // ─── Performance Baseline ───
    public const int TargetFPS = 60;
    public const int MinimumFPS = 30;
    public const float MaxLoadTimeSeconds = 5.0f;
    public const float FPSLogIntervalSeconds = 10.0f;

    // ─── Lighting Baseline ───
    public const float AmbientLightIntensity = 0.15f;
    public const float CeilingLightIntensity = 1.2f;
    public const float DeskLampIntensity = 0.6f;
    public const float CeilingLightRange = 7.0f;

    // ─── UI ───
    public const float CrosshairSize = 8f;
    public const float CrosshairAlpha = 0.6f;
    public const float PromptFontSize = 18f;
    public const float PromptVerticalOffset = 40f;

    // ─── Save ───
    public const string SaveFileName = "nightborrower_save.json";

    // ─── Flashlight ───
    public const float FlashlightRange = 15f;
    public const float FlashlightSpotAngle = 45f;
    public const float FlashlightInnerSpotAngle = 25f;
    public const float FlashlightIntensity = 1.5f;

    // ─── Observation ───
    public const float ObservationRange = 8.0f;
    public const float ObservationHighlightIntensity = 0.35f;
    public const float ObservationFadeDuration = 0.4f;
    public const float ObservationViewAngle = 30f;

    // ─── Inspection ───
    public const float InspectionTransitionDuration = 0.4f;
    public const float InspectionViewDistance = 0.6f;
    public const float InspectionRotationSpeed = 3.0f;
    public const float InspectionMaxPitch = 60f;
    public const float InspectionFOV = 40f;
}
