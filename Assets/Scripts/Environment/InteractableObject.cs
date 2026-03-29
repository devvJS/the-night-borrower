using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Renderer))]
public class InteractableObject : MonoBehaviour
{
    [Header("Identity")]
    [SerializeField] private string objectId;
    [SerializeField] private ObjectType objectType;

    [Header("Highlight")]
    [SerializeField] private Color highlightColor = new Color(1.0f, 0.95f, 0.8f);
    [SerializeField] private float emissionIntensity = GameConstants.HighlightEmissionIntensity;

    [Header("Observation")]
    [SerializeField] private bool isImportant;
    [SerializeField] private Color observationColor = new Color(0.6f, 0.8f, 1.0f);

    [Header("Inspection")]
    [SerializeField] private bool isInspectable = true;
    [SerializeField] private Vector3 inspectionOffset = Vector3.zero;
    [SerializeField] private float inspectionDistanceMultiplier = 3.0f;

    [Header("Clue Data")]
    [SerializeField] [TextArea(2, 5)] private string objectDescription = "";
    [SerializeField] [TextArea(2, 5)] private string clueText = "";
    [SerializeField] private string clueId = "";

    private bool hasBeenInspected;
    private string lastInspectedState = "";

    private Renderer objectRenderer;
    private Material materialInstance;
    private Coroutine fadeCoroutine;
    private Coroutine observationFadeCoroutine;
    private bool isHighlighted;
    private bool isObservationHighlighted;

    public string ObjectId => objectId;
    public new ObjectType Type => objectType;
    public bool IsHighlighted => isHighlighted;
    public bool IsImportant => isImportant;
    public bool IsObservationHighlighted => isObservationHighlighted;
    public bool IsInspectable => isInspectable;
    public Vector3 InspectionFocusPoint => transform.position + inspectionOffset;
    public float InspectionDistance => GameConstants.InspectionViewDistance * inspectionDistanceMultiplier;
    public string ObjectDescription => objectDescription;
    public string ClueText => clueText;
    public string ClueId => clueId;
    public bool HasClue => !string.IsNullOrEmpty(clueText);
    public bool HasBeenInspected => hasBeenInspected;

    public event Action<InteractableObject> OnInteracted;

    private static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");

    private void Awake()
    {
        objectRenderer = GetComponent<Renderer>();
        materialInstance = objectRenderer.material;
    }

    private void OnDestroy()
    {
        if (materialInstance != null)
        {
            Destroy(materialInstance);
        }
    }

    public void SetImportant(bool important)
    {
        if (isImportant == important) return;
        isImportant = important;
        if (!important && isObservationHighlighted)
        {
            ObservationUnhighlight();
        }
    }

    public void Highlight()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }

        isHighlighted = true;
        materialInstance.EnableKeyword("_EMISSION");
        materialInstance.SetColor(EmissionColorId, highlightColor * emissionIntensity);
    }

    public void Unhighlight()
    {
        if (!isHighlighted) return;

        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        fadeCoroutine = StartCoroutine(FadeHighlight());
    }

    public void ObservationHighlight()
    {
        if (isObservationHighlighted) return;
        isObservationHighlighted = true;

        if (isHighlighted) return;

        if (observationFadeCoroutine != null)
        {
            StopCoroutine(observationFadeCoroutine);
            observationFadeCoroutine = null;
        }

        materialInstance.EnableKeyword("_EMISSION");
        materialInstance.SetColor(EmissionColorId, observationColor * GameConstants.ObservationHighlightIntensity);
    }

    public void ObservationUnhighlight()
    {
        if (!isObservationHighlighted) return;
        isObservationHighlighted = false;

        if (isHighlighted) return;

        if (observationFadeCoroutine != null)
        {
            StopCoroutine(observationFadeCoroutine);
        }
        observationFadeCoroutine = StartCoroutine(FadeObservationHighlight());
    }

    public string GetCurrentState()
    {
        return $"{objectType}|{isImportant}|{objectDescription}";
    }

    public InspectionResult Inspect()
    {
        string currentState = GetCurrentState();
        bool isFirst = !hasBeenInspected;
        bool stateChanged = !isFirst && currentState != lastInspectedState;

        var result = new InspectionResult
        {
            title = FormatDisplayName(objectId),
            description = objectDescription,
            clue = HasClue ? clueText : "",
            clueId = clueId,
            isFirstInspection = isFirst,
            hasStateChanged = stateChanged,
            hasClue = HasClue
        };

        hasBeenInspected = true;
        lastInspectedState = currentState;

        return result;
    }

    public void Interact()
    {
        Debug.Log($"Interacted with {objectId} ({objectType})");
        OnInteracted?.Invoke(this);
    }

    private static string FormatDisplayName(string id)
    {
        if (string.IsNullOrEmpty(id)) return "Unknown Object";
        var parts = id.Split('_');
        for (int i = 0; i < parts.Length; i++)
        {
            if (parts[i].Length > 0)
                parts[i] = char.ToUpper(parts[i][0]) + parts[i].Substring(1);
        }
        return string.Join(" ", parts);
    }

    private IEnumerator FadeHighlight()
    {
        Color startColor = materialInstance.GetColor(EmissionColorId);
        float elapsed = 0f;

        while (elapsed < GameConstants.HighlightFadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / GameConstants.HighlightFadeDuration;
            materialInstance.SetColor(EmissionColorId, Color.Lerp(startColor, Color.black, t));
            yield return null;
        }

        materialInstance.SetColor(EmissionColorId, Color.black);
        isHighlighted = false;
        fadeCoroutine = null;

        if (isObservationHighlighted)
        {
            materialInstance.EnableKeyword("_EMISSION");
            materialInstance.SetColor(EmissionColorId, observationColor * GameConstants.ObservationHighlightIntensity);
        }
        else
        {
            materialInstance.DisableKeyword("_EMISSION");
        }
    }

    private IEnumerator FadeObservationHighlight()
    {
        Color startColor = materialInstance.GetColor(EmissionColorId);
        float elapsed = 0f;

        while (elapsed < GameConstants.ObservationFadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / GameConstants.ObservationFadeDuration;
            materialInstance.SetColor(EmissionColorId, Color.Lerp(startColor, Color.black, t));
            yield return null;
        }

        materialInstance.SetColor(EmissionColorId, Color.black);
        materialInstance.DisableKeyword("_EMISSION");
        observationFadeCoroutine = null;
    }
}
