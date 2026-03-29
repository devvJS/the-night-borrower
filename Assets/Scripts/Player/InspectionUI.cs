using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class InspectionUI : MonoBehaviour
{
    private GameObject panelObject;
    private TextMeshProUGUI titleText;
    private TextMeshProUGUI descriptionText;
    private TextMeshProUGUI clueLabel;
    private GameObject changedBadge;
    private TextMeshProUGUI changedBadgeText;
    private CanvasGroup panelCanvasGroup;
    private Coroutine fadeCoroutine;
    private Canvas parentCanvas;

    public void Initialize(Canvas canvas)
    {
        parentCanvas = canvas;

        // Panel container — anchored to right side of screen
        panelObject = new GameObject("InspectionPanel");
        panelObject.transform.SetParent(canvas.transform, false);

        RectTransform panelRect = panelObject.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(1f, 0f);
        panelRect.anchorMax = new Vector2(1f, 1f);
        panelRect.pivot = new Vector2(1f, 0.5f);
        panelRect.anchoredPosition = Vector2.zero;
        panelRect.sizeDelta = new Vector2(GameConstants.InspectionPanelWidth, 0f);

        // Semi-transparent dark background
        Image bgImage = panelObject.AddComponent<Image>();
        bgImage.color = new Color(0f, 0f, 0f, 0.7f);

        panelCanvasGroup = panelObject.AddComponent<CanvasGroup>();
        panelCanvasGroup.alpha = 0f;

        // Content area with padding
        float padding = GameConstants.InspectionPanelPadding;
        float yOffset = -padding;

        // Changed badge — positioned at top, hidden by default
        changedBadge = new GameObject("ChangedBadge");
        changedBadge.transform.SetParent(panelObject.transform, false);

        changedBadgeText = changedBadge.AddComponent<TextMeshProUGUI>();
        changedBadgeText.text = "STATE CHANGED";
        changedBadgeText.fontSize = GameConstants.InspectionChangedBadgeFontSize;
        changedBadgeText.color = new Color(1.0f, 0.7f, 0.3f, 1.0f);
        changedBadgeText.fontStyle = FontStyles.Bold;
        changedBadgeText.alignment = TextAlignmentOptions.TopLeft;

        RectTransform badgeRect = changedBadge.GetComponent<RectTransform>();
        badgeRect.anchorMin = new Vector2(0f, 1f);
        badgeRect.anchorMax = new Vector2(1f, 1f);
        badgeRect.pivot = new Vector2(0f, 1f);
        badgeRect.anchoredPosition = new Vector2(padding, yOffset);
        badgeRect.sizeDelta = new Vector2(-padding * 2f, 24f);

        changedBadge.SetActive(false);

        // Title text — warm white, bold
        GameObject titleObj = new GameObject("TitleText");
        titleObj.transform.SetParent(panelObject.transform, false);

        titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.fontSize = GameConstants.InspectionTitleFontSize;
        titleText.color = new Color(1.0f, 0.95f, 0.85f, 1.0f);
        titleText.fontStyle = FontStyles.Bold;
        titleText.alignment = TextAlignmentOptions.TopLeft;
        titleText.enableWordWrapping = true;

        RectTransform titleRect = titleObj.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0f, 1f);
        titleRect.anchorMax = new Vector2(1f, 1f);
        titleRect.pivot = new Vector2(0f, 1f);
        titleRect.anchoredPosition = new Vector2(padding, yOffset - 30f);
        titleRect.sizeDelta = new Vector2(-padding * 2f, 40f);

        // Description text — light gray
        GameObject descObj = new GameObject("DescriptionText");
        descObj.transform.SetParent(panelObject.transform, false);

        descriptionText = descObj.AddComponent<TextMeshProUGUI>();
        descriptionText.fontSize = GameConstants.InspectionBodyFontSize;
        descriptionText.color = new Color(0.85f, 0.85f, 0.85f, 0.9f);
        descriptionText.alignment = TextAlignmentOptions.TopLeft;
        descriptionText.enableWordWrapping = true;

        RectTransform descRect = descObj.GetComponent<RectTransform>();
        descRect.anchorMin = new Vector2(0f, 1f);
        descRect.anchorMax = new Vector2(1f, 1f);
        descRect.pivot = new Vector2(0f, 1f);
        descRect.anchoredPosition = new Vector2(padding, yOffset - 80f);
        descRect.sizeDelta = new Vector2(-padding * 2f, 80f);

        // Clue text — soft gold
        GameObject clueObj = new GameObject("ClueText");
        clueObj.transform.SetParent(panelObject.transform, false);

        clueLabel = clueObj.AddComponent<TextMeshProUGUI>();
        clueLabel.fontSize = GameConstants.InspectionBodyFontSize;
        clueLabel.color = new Color(1.0f, 0.9f, 0.6f, 1.0f);
        clueLabel.alignment = TextAlignmentOptions.TopLeft;
        clueLabel.enableWordWrapping = true;

        RectTransform clueRect = clueObj.GetComponent<RectTransform>();
        clueRect.anchorMin = new Vector2(0f, 1f);
        clueRect.anchorMax = new Vector2(1f, 1f);
        clueRect.pivot = new Vector2(0f, 1f);
        clueRect.anchoredPosition = new Vector2(padding, yOffset - 170f);
        clueRect.sizeDelta = new Vector2(-padding * 2f, 80f);

        clueObj.SetActive(false);

        panelObject.SetActive(false);
    }

    public void Show(InspectionResult result)
    {
        titleText.text = result.title;
        descriptionText.text = result.description;

        // Clue display: show if object has a clue (always re-readable per AC2)
        if (result.hasClue)
        {
            clueLabel.text = result.clue;
            clueLabel.gameObject.SetActive(true);
        }
        else
        {
            clueLabel.gameObject.SetActive(false);
        }

        // Changed badge (AC3)
        changedBadge.SetActive(result.hasStateChanged);

        panelCanvasGroup.alpha = 0f;
        panelObject.SetActive(true);

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeIn());
    }

    public void Hide()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }

        panelCanvasGroup.alpha = 0f;
        panelObject.SetActive(false);
    }

    private IEnumerator FadeIn()
    {
        float elapsed = 0f;

        while (elapsed < GameConstants.InspectionPanelFadeDuration)
        {
            elapsed += Time.deltaTime;
            panelCanvasGroup.alpha = Mathf.Clamp01(
                elapsed / GameConstants.InspectionPanelFadeDuration);
            yield return null;
        }

        panelCanvasGroup.alpha = 1f;
        fadeCoroutine = null;
    }
}
