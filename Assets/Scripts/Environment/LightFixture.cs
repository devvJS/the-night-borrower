using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Light))]
public class LightFixture : MonoBehaviour
{
    [Header("Fixture")]
    [SerializeField] private string fixtureId;
    [SerializeField] private bool startBroken;

    private Light fixtureLight;
    private bool isFunctional;
    private float baseIntensity;
    private Coroutine repairCoroutine;

    public string FixtureId => fixtureId;
    public bool IsFunctional => isFunctional;
    public bool IsRepairing => repairCoroutine != null;

    private void Awake()
    {
        fixtureLight = GetComponent<Light>();
        baseIntensity = fixtureLight.intensity;

        if (startBroken)
        {
            fixtureLight.intensity = 0f;
            fixtureLight.enabled = false;
            isFunctional = false;
        }
        else
        {
            isFunctional = true;
        }
    }

    public void Repair()
    {
        if (isFunctional || repairCoroutine != null) return;
        repairCoroutine = StartCoroutine(RepairAnimation());
    }

    public void Break()
    {
        fixtureLight.intensity = 0f;
        fixtureLight.enabled = false;
        isFunctional = false;
        GameEvents.LightFailed(fixtureId, "");
    }

    private IEnumerator RepairAnimation()
    {
        fixtureLight.enabled = true;
        fixtureLight.intensity = 0f;
        float elapsed = 0f;

        while (elapsed < GameConstants.BulbReplaceAnimationDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f,
                elapsed / GameConstants.BulbReplaceAnimationDuration);
            fixtureLight.intensity = Mathf.Lerp(0f, baseIntensity, t);
            yield return null;
        }

        fixtureLight.intensity = baseIntensity;
        isFunctional = true;
        repairCoroutine = null;
        GameEvents.LightRepaired(fixtureId, "");
    }
}
