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
}
