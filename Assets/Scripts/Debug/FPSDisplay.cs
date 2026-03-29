#if UNITY_EDITOR || DEVELOPMENT_BUILD
using UnityEngine;

public class FPSDisplay : MonoBehaviour
{
    private float[] frameTimes;
    private int frameIndex;
    private int frameSamples;
    private float frameTimeSum;
    private float minFPS = float.MaxValue;
    private float maxFPS;
    private float currentFPS;

    private float lastLogTime;
    private bool visible = true;

    private float awakeTime;
    private float loadTime = -1f;
    private float loadTimeDisplayRemaining;

    private GUIStyle cachedStyle;

    private void Awake()
    {
        awakeTime = Time.realtimeSinceStartup;
        int sampleCount = Mathf.CeilToInt(0.5f / (1f / GameConstants.TargetFPS));
        frameTimes = new float[sampleCount];
#if UNITY_EDITOR
        visible = true;
#else
        visible = false;
#endif
    }

    private void Update()
    {
        if (loadTime < 0f)
        {
            loadTime = Time.realtimeSinceStartup - awakeTime;
            loadTimeDisplayRemaining = 5f;
            DevLog.Log(LogCategory.Performance, $"Scene ready: {loadTime:F2}s");
        }

        float dt = Time.unscaledDeltaTime;
        frameTimeSum -= frameTimes[frameIndex];
        frameTimes[frameIndex] = dt;
        frameTimeSum += dt;
        frameIndex = (frameIndex + 1) % frameTimes.Length;
        if (frameSamples < frameTimes.Length) frameSamples++;

        currentFPS = frameSamples / frameTimeSum;
        if (frameSamples >= frameTimes.Length)
        {
            if (currentFPS < minFPS) minFPS = currentFPS;
            if (currentFPS > maxFPS) maxFPS = currentFPS;
        }

        if (loadTimeDisplayRemaining > 0f)
            loadTimeDisplayRemaining -= Time.unscaledDeltaTime;

        if (Time.realtimeSinceStartup - lastLogTime >= GameConstants.FPSLogIntervalSeconds)
        {
            lastLogTime = Time.realtimeSinceStartup;
            DevLog.Log(LogCategory.Performance, $"FPS: avg={currentFPS:F0} min={minFPS:F0} max={maxFPS:F0}");
        }

        if (Input.GetKeyDown(KeyCode.F2))
            visible = !visible;
    }

    private void OnGUI()
    {
        if (!visible) return;

        Color color;
        if (currentFPS >= GameConstants.TargetFPS)
            color = Color.green;
        else if (currentFPS >= GameConstants.MinimumFPS)
            color = Color.yellow;
        else
            color = Color.red;

        if (cachedStyle == null)
        {
            cachedStyle = new GUIStyle
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold
            };
        }
        cachedStyle.normal.textColor = color;

        float y = 10f;
        GUI.Label(new Rect(10, y, 300, 20), $"FPS: {currentFPS:F0}  Min: {minFPS:F0}  Max: {maxFPS:F0}", cachedStyle);

        if (loadTimeDisplayRemaining > 0f)
        {
            y += 20f;
            cachedStyle.normal.textColor = Color.white;
            GUI.Label(new Rect(10, y, 300, 20), $"Load: {loadTime:F2}s", cachedStyle);
        }
    }

    public void ResetMinMax()
    {
        minFPS = float.MaxValue;
        maxFPS = 0f;
    }
}
#endif
