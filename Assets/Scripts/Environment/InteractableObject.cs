using UnityEngine;
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

    private Renderer objectRenderer;
    private Material materialInstance;
    private Coroutine fadeCoroutine;
    private bool isHighlighted;

    public string ObjectId => objectId;
    public new ObjectType Type => objectType;
    public bool IsHighlighted => isHighlighted;

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
        materialInstance.DisableKeyword("_EMISSION");
        isHighlighted = false;
        fadeCoroutine = null;
    }
}
