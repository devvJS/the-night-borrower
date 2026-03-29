using System.Diagnostics;

public static class DevLog
{
    public static LogCategory ActiveCategories = LogCategory.All;

    [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
    public static void Log(LogCategory category, string message)
    {
        if ((ActiveCategories & category) == 0) return;
        UnityEngine.Debug.Log($"[{category}] {message}");
    }

    [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
    public static void Warn(LogCategory category, string message)
    {
        if ((ActiveCategories & category) == 0) return;
        UnityEngine.Debug.LogWarning($"[{category}] {message}");
    }
}
