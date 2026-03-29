using System;

public static class GameEvents
{
    // ─── Day/Night Cycle ───
    public static event Action<GamePhase, GamePhase> OnPhaseChanged;       // (oldPhase, newPhase)
    public static event Action<int> OnDayStarted;                           // (dayNumber)
    public static event Action<int> OnNightStarted;                         // (dayNumber)
    public static event Action<int, bool> OnNightEnded;                     // (dayNumber, survived)

    // ─── Environment ───
    public static event Action<string, string> OnObjectDisplaced;           // (objectId, zoneId)
    public static event Action<string, string> OnObjectRestored;            // (objectId, zoneId)
    public static event Action<string, string> OnLightFailed;               // (fixtureId, zoneId)
    public static event Action<string, string> OnLightRepaired;             // (fixtureId, zoneId)
    public static event Action<string, float> OnZoneStabilityChanged;       // (zoneId, newScore)
    public static event Action<string> OnRecordCorrupted;                   // (objectId)
    public static event Action<string> OnAreaUnlocked;                      // (zoneId)

    // ─── Notebook ───
    public static event Action<string> OnEntryCreated;                      // (entryId)
    public static event Action<string> OnEntryCorrupted;                    // (entryId)
    public static event Action<string> OnManualNoteCreated;                 // (noteId)
    public static event Action OnCrossReferenceUsed;

    // ─── Entity ───
    public static event Action<EntityState, EntityState> OnEntityStateChanged; // (oldState, newState)
    public static event Action<string> OnEntityEnteredZone;                 // (zoneId)
    public static event Action OnPlayerDetected;
    public static event Action OnPlayerCaught;

    // ─── Puzzles ───
    public static event Action<string> OnPuzzleSolved;                      // (puzzleId)
    public static event Action<string> OnPuzzleFailed;                      // (puzzleId)
    public static event Action<string> OnPatternIdentified;                 // (patternId)

    // ─── Player ───
    public static event Action<string> OnPlayerEnteredZone;                 // (zoneId)
    public static event Action<string> OnObjectInspected;                   // (objectId)
    public static event Action<string> OnObjectHighlighted;                 // (objectId or null when unhighlighted)
    public static event Action<string> OnObjectObserved;                    // (objectId)

    // ─── Invoke helpers (null-safe) ───
    public static void PhaseChanged(GamePhase oldPhase, GamePhase newPhase)
        => OnPhaseChanged?.Invoke(oldPhase, newPhase);

    public static void DayStarted(int day) => OnDayStarted?.Invoke(day);
    public static void NightStarted(int day) => OnNightStarted?.Invoke(day);
    public static void NightEnded(int day, bool survived)
        => OnNightEnded?.Invoke(day, survived);

    public static void ObjectDisplaced(string objectId, string zoneId)
        => OnObjectDisplaced?.Invoke(objectId, zoneId);
    public static void ObjectRestored(string objectId, string zoneId)
        => OnObjectRestored?.Invoke(objectId, zoneId);
    public static void LightFailed(string fixtureId, string zoneId)
        => OnLightFailed?.Invoke(fixtureId, zoneId);
    public static void LightRepaired(string fixtureId, string zoneId)
        => OnLightRepaired?.Invoke(fixtureId, zoneId);
    public static void ZoneStabilityChanged(string zoneId, float newScore)
        => OnZoneStabilityChanged?.Invoke(zoneId, newScore);
    public static void RecordCorrupted(string objectId)
        => OnRecordCorrupted?.Invoke(objectId);
    public static void AreaUnlocked(string zoneId)
        => OnAreaUnlocked?.Invoke(zoneId);

    public static void EntryCreated(string entryId)
        => OnEntryCreated?.Invoke(entryId);
    public static void EntryCorrupted(string entryId)
        => OnEntryCorrupted?.Invoke(entryId);
    public static void ManualNoteCreated(string noteId)
        => OnManualNoteCreated?.Invoke(noteId);
    public static void CrossReferenceUsed()
        => OnCrossReferenceUsed?.Invoke();

    public static void EntityStateChanged(EntityState oldState, EntityState newState)
        => OnEntityStateChanged?.Invoke(oldState, newState);
    public static void EntityEnteredZone(string zoneId)
        => OnEntityEnteredZone?.Invoke(zoneId);
    public static void PlayerDetected()
        => OnPlayerDetected?.Invoke();
    public static void PlayerCaught()
        => OnPlayerCaught?.Invoke();

    public static void PuzzleSolved(string puzzleId)
        => OnPuzzleSolved?.Invoke(puzzleId);
    public static void PuzzleFailed(string puzzleId)
        => OnPuzzleFailed?.Invoke(puzzleId);
    public static void PatternIdentified(string patternId)
        => OnPatternIdentified?.Invoke(patternId);

    public static void PlayerEnteredZone(string zoneId)
        => OnPlayerEnteredZone?.Invoke(zoneId);
    public static void ObjectInspected(string objectId)
        => OnObjectInspected?.Invoke(objectId);
    public static void ObjectHighlighted(string objectId)
        => OnObjectHighlighted?.Invoke(objectId);
    public static void ObjectObserved(string objectId)
        => OnObjectObserved?.Invoke(objectId);
}
