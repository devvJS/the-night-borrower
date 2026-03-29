using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class DiscoveryNotificationUI : MonoBehaviour
{
    private GameObject notificationPanel;
    private TextMeshProUGUI notificationText;
    private CanvasGroup panelCanvasGroup;
    private Coroutine notificationCoroutine;

    public void Initialize(Canvas canvas)
    {
        // Panel container — anchored to bottom-left
        notificationPanel = new GameObject("DiscoveryNotification");
        notificationPanel.transform.SetParent(canvas.transform, false);

        RectTransform panelRect = notificationPanel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0f, 0f);
        panelRect.anchorMax = new Vector2(0f, 0f);
        panelRect.pivot = new Vector2(0f, 0f);
        panelRect.anchoredPosition = new Vector2(20f, GameConstants.NotificationVerticalOffset);
        panelRect.sizeDelta = new Vector2(GameConstants.NotificationPanelWidth, 40f);

        // Semi-transparent dark background
        Image bgImage = notificationPanel.AddComponent<Image>();
        bgImage.color = new Color(0f, 0f, 0f, 0.6f);

        panelCanvasGroup = notificationPanel.AddComponent<CanvasGroup>();
        panelCanvasGroup.alpha = 0f;

        // Notification text — soft gold
        GameObject textObj = new GameObject("NotificationText");
        textObj.transform.SetParent(notificationPanel.transform, false);

        notificationText = textObj.AddComponent<TextMeshProUGUI>();
        notificationText.fontSize = GameConstants.NotificationFontSize;
        notificationText.color = new Color(1.0f, 0.9f, 0.6f, 1.0f);
        notificationText.alignment = TextAlignmentOptions.MidlineLeft;
        notificationText.enableWordWrapping = true;

        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.pivot = new Vector2(0.5f, 0.5f);
        textRect.anchoredPosition = Vector2.zero;
        textRect.sizeDelta = new Vector2(-20f, 0f);

        notificationPanel.SetActive(false);
    }

    public void ShowNotification(DiscoveryEntry entry)
    {
        notificationText.text = $"Discovery logged: {entry.title}";

        if (notificationCoroutine != null)
            StopCoroutine(notificationCoroutine);

        notificationCoroutine = StartCoroutine(NotificationLifecycle());
    }

    private IEnumerator NotificationLifecycle()
    {
        panelCanvasGroup.alpha = 0f;
        notificationPanel.SetActive(true);

        // Fade in
        float elapsed = 0f;
        while (elapsed < GameConstants.NotificationFadeInDuration)
        {
            elapsed += Time.deltaTime;
            panelCanvasGroup.alpha = Mathf.Clamp01(
                elapsed / GameConstants.NotificationFadeInDuration);
            yield return null;
        }
        panelCanvasGroup.alpha = 1f;

        // Hold
        yield return new WaitForSeconds(GameConstants.NotificationDisplayDuration);

        // Fade out
        elapsed = 0f;
        while (elapsed < GameConstants.NotificationFadeOutDuration)
        {
            elapsed += Time.deltaTime;
            panelCanvasGroup.alpha = Mathf.Clamp01(
                1f - elapsed / GameConstants.NotificationFadeOutDuration);
            yield return null;
        }

        panelCanvasGroup.alpha = 0f;
        notificationPanel.SetActive(false);
        notificationCoroutine = null;
    }
}
