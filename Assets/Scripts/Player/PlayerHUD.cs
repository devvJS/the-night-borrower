using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;

    private InspectionSystem inspectionSystem;
    private InspectionUI inspectionUI;
    private GameObject promptObject;
    private TextMeshProUGUI promptText;
    private GameObject crosshairObject;
    private Texture2D crosshairTexture;
    private Sprite crosshairSprite;

    private bool wasShowingPrompt;

    public InspectionUI InspectionUI => inspectionUI;

    private void Awake()
    {
        if (playerController == null)
        {
            playerController = FindObjectOfType<PlayerController>();
        }

        if (playerController == null)
        {
            Debug.LogError("PlayerHUD: No PlayerController found");
            return;
        }

        inspectionSystem = playerController.GetComponent<InspectionSystem>();

        BuildHUD();
    }

    private void OnDestroy()
    {
        if (crosshairSprite != null)
        {
            Destroy(crosshairSprite);
        }
        if (crosshairTexture != null)
        {
            Destroy(crosshairTexture);
        }
    }

    private void Update()
    {
        if (playerController == null) return;

        bool shouldShowPrompt = playerController.CurrentHighlighted != null
                                && !playerController.IsInteracting
                                && (inspectionSystem == null || !inspectionSystem.IsInspecting);

        if (shouldShowPrompt && !wasShowingPrompt)
        {
            promptText.text = "E: Inspect";
            promptObject.SetActive(true);
            wasShowingPrompt = true;
        }
        else if (!shouldShowPrompt && wasShowingPrompt)
        {
            promptObject.SetActive(false);
            wasShowingPrompt = false;
        }
    }

    public void SetPromptText(string text)
    {
        promptText.text = text;
    }

    public void SetCrosshairVisible(bool visible)
    {
        crosshairObject.SetActive(visible);
    }

    private void BuildHUD()
    {
        // Create Canvas
        GameObject canvasObject = new GameObject("HUDCanvas");
        canvasObject.transform.SetParent(transform);

        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 0;

        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        // No GraphicRaycaster — this is display-only HUD

        // Create Crosshair
        crosshairObject = new GameObject("Crosshair");
        crosshairObject.transform.SetParent(canvasObject.transform, false);

        Image crosshairImage = crosshairObject.AddComponent<Image>();
        int texSize = 16;
        crosshairTexture = new Texture2D(texSize, texSize, TextureFormat.RGBA32, false);
        crosshairTexture.filterMode = FilterMode.Bilinear;
        float center = (texSize - 1) / 2f;
        float radius = center;
        for (int y = 0; y < texSize; y++)
        {
            for (int x = 0; x < texSize; x++)
            {
                float dist = Mathf.Sqrt((x - center) * (x - center) + (y - center) * (y - center));
                float alpha = Mathf.Clamp01(1f - (dist / radius));
                crosshairTexture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
            }
        }
        crosshairTexture.Apply();
        crosshairSprite = Sprite.Create(
            crosshairTexture,
            new Rect(0, 0, texSize, texSize),
            new Vector2(0.5f, 0.5f)
        );
        crosshairImage.sprite = crosshairSprite;
        crosshairImage.color = new Color(1f, 1f, 1f, GameConstants.CrosshairAlpha);

        RectTransform crosshairRect = crosshairObject.GetComponent<RectTransform>();
        crosshairRect.anchorMin = new Vector2(0.5f, 0.5f);
        crosshairRect.anchorMax = new Vector2(0.5f, 0.5f);
        crosshairRect.pivot = new Vector2(0.5f, 0.5f);
        crosshairRect.anchoredPosition = Vector2.zero;
        crosshairRect.sizeDelta = new Vector2(GameConstants.CrosshairSize, GameConstants.CrosshairSize);

        // Create Interaction Prompt
        promptObject = new GameObject("InteractionPrompt");
        promptObject.transform.SetParent(canvasObject.transform, false);

        promptText = promptObject.AddComponent<TextMeshProUGUI>();
        promptText.fontSize = GameConstants.PromptFontSize;
        promptText.color = new Color(1.0f, 0.95f, 0.85f, 0.9f);
        promptText.alignment = TextAlignmentOptions.Center;

        RectTransform promptRect = promptObject.GetComponent<RectTransform>();
        promptRect.anchorMin = new Vector2(0.5f, 0.5f);
        promptRect.anchorMax = new Vector2(0.5f, 0.5f);
        promptRect.pivot = new Vector2(0.5f, 0.5f);
        promptRect.anchoredPosition = new Vector2(0, -GameConstants.PromptVerticalOffset);
        promptRect.sizeDelta = new Vector2(300, 50);

        promptObject.SetActive(false);

        // Create Inspection UI
        inspectionUI = gameObject.AddComponent<InspectionUI>();
        inspectionUI.Initialize(canvas);
    }
}
