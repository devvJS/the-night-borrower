using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class InventoryUI : MonoBehaviour
{
    private GameObject[] slotObjects;
    private Image[] slotBackgrounds;
    private TextMeshProUGUI[] countTexts;
    private TextMeshProUGUI[] nameTexts;
    private GameObject feedbackObject;
    private TextMeshProUGUI feedbackText;
    private Coroutine feedbackCoroutine;
    private PlayerInventory inventory;

    public void Initialize(Canvas canvas, PlayerInventory inv)
    {
        inventory = inv;

        float slotSize = GameConstants.InventorySlotSize;
        float spacing = GameConstants.InventorySlotSpacing;
        int slotCount = GameConstants.InventorySlotCount;
        float totalWidth = slotCount * slotSize + (slotCount - 1) * spacing;

        // Container anchored at bottom-center
        GameObject container = new GameObject("InventoryHotbar");
        container.transform.SetParent(canvas.transform, false);

        RectTransform containerRect = container.AddComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(0.5f, 0f);
        containerRect.anchorMax = new Vector2(0.5f, 0f);
        containerRect.pivot = new Vector2(0.5f, 0f);
        containerRect.anchoredPosition = new Vector2(0, 20f);
        containerRect.sizeDelta = new Vector2(totalWidth, slotSize);

        slotObjects = new GameObject[slotCount];
        slotBackgrounds = new Image[slotCount];
        countTexts = new TextMeshProUGUI[slotCount];
        nameTexts = new TextMeshProUGUI[slotCount];

        float startX = -totalWidth / 2f + slotSize / 2f;

        for (int i = 0; i < slotCount; i++)
        {
            // Slot background
            GameObject slot = new GameObject($"Slot_{i}");
            slot.transform.SetParent(container.transform, false);

            Image bg = slot.AddComponent<Image>();
            bg.color = new Color(0.1f, 0.1f, 0.12f, GameConstants.InventorySlotAlpha);

            RectTransform slotRect = slot.GetComponent<RectTransform>();
            slotRect.anchorMin = new Vector2(0.5f, 0.5f);
            slotRect.anchorMax = new Vector2(0.5f, 0.5f);
            slotRect.pivot = new Vector2(0.5f, 0.5f);
            slotRect.anchoredPosition = new Vector2(startX + i * (slotSize + spacing), 0);
            slotRect.sizeDelta = new Vector2(slotSize, slotSize);

            // Item name text (small, top of slot)
            GameObject nameObj = new GameObject("Name");
            nameObj.transform.SetParent(slot.transform, false);

            TextMeshProUGUI nameText = nameObj.AddComponent<TextMeshProUGUI>();
            nameText.fontSize = 10f;
            nameText.color = new Color(0.7f, 0.7f, 0.7f, 0.9f);
            nameText.alignment = TextAlignmentOptions.Center;
            nameText.text = "";

            RectTransform nameRect = nameObj.GetComponent<RectTransform>();
            nameRect.anchorMin = new Vector2(0f, 0.6f);
            nameRect.anchorMax = new Vector2(1f, 1f);
            nameRect.offsetMin = Vector2.zero;
            nameRect.offsetMax = Vector2.zero;

            // Count text (larger, center of slot)
            GameObject countObj = new GameObject("Count");
            countObj.transform.SetParent(slot.transform, false);

            TextMeshProUGUI countText = countObj.AddComponent<TextMeshProUGUI>();
            countText.fontSize = 18f;
            countText.color = new Color(1.0f, 0.95f, 0.85f, 0.9f);
            countText.alignment = TextAlignmentOptions.Center;
            countText.text = "";

            RectTransform countRect = countObj.GetComponent<RectTransform>();
            countRect.anchorMin = new Vector2(0f, 0f);
            countRect.anchorMax = new Vector2(1f, 0.65f);
            countRect.offsetMin = Vector2.zero;
            countRect.offsetMax = Vector2.zero;

            slotObjects[i] = slot;
            slotBackgrounds[i] = bg;
            nameTexts[i] = nameText;
            countTexts[i] = countText;
        }

        // Feedback text (above hotbar, hidden by default)
        feedbackObject = new GameObject("InventoryFeedback");
        feedbackObject.transform.SetParent(container.transform, false);

        feedbackText = feedbackObject.AddComponent<TextMeshProUGUI>();
        feedbackText.fontSize = 16f;
        feedbackText.color = new Color(1.0f, 0.5f, 0.4f, 0.9f);
        feedbackText.alignment = TextAlignmentOptions.Center;
        feedbackText.text = "Inventory Full";

        RectTransform feedbackRect = feedbackObject.GetComponent<RectTransform>();
        feedbackRect.anchorMin = new Vector2(0.5f, 1f);
        feedbackRect.anchorMax = new Vector2(0.5f, 1f);
        feedbackRect.pivot = new Vector2(0.5f, 0f);
        feedbackRect.anchoredPosition = new Vector2(0, 8f);
        feedbackRect.sizeDelta = new Vector2(200f, 30f);

        feedbackObject.SetActive(false);

        inventory.OnInventoryChanged += RefreshSlots;
        RefreshSlots();
    }

    private void RefreshSlots()
    {
        for (int i = 0; i < slotObjects.Length; i++)
        {
            var slot = inventory.Slots[i];

            if (slot.IsEmpty)
            {
                nameTexts[i].text = "";
                countTexts[i].text = "";
                slotBackgrounds[i].color = new Color(0.1f, 0.1f, 0.12f,
                    GameConstants.InventorySlotAlpha * 0.4f);
            }
            else
            {
                nameTexts[i].text = GetItemDisplayName(slot.itemType);
                countTexts[i].text = slot.count.ToString();
                slotBackgrounds[i].color = new Color(0.15f, 0.15f, 0.18f,
                    GameConstants.InventorySlotAlpha);
            }
        }
    }

    public void ShowFullFeedback()
    {
        if (feedbackCoroutine != null)
            StopCoroutine(feedbackCoroutine);
        feedbackCoroutine = StartCoroutine(FeedbackLifecycle());
    }

    public void Cleanup()
    {
        if (inventory != null)
            inventory.OnInventoryChanged -= RefreshSlots;
    }

    private IEnumerator FeedbackLifecycle()
    {
        feedbackObject.SetActive(true);
        yield return new WaitForSeconds(GameConstants.InventoryFullFeedbackDuration);
        feedbackObject.SetActive(false);
        feedbackCoroutine = null;
    }

    private static string GetItemDisplayName(ItemType type)
    {
        switch (type)
        {
            case ItemType.SpareBulb: return "Bulbs";
            case ItemType.RepairTool: return "Tools";
            default: return "Item";
        }
    }
}
