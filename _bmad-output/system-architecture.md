# The Night Borrower - System Architecture

**Generated from Systems Spec:** 2026-03-28
**Author:** Dakota
**Status:** Draft — Awaiting Review
**Design Constraint:** Minimal coupling, solo developer maintainability, no over-engineering.

---

## 1. System Dependency Map

### Event Flow Diagram

```
┌─────────────────────────────────────────────────────────────────────┐
│                         GameEventBus                                │
│  (static C# event hub — all systems publish/subscribe through it)  │
└────┬──────────┬──────────┬──────────┬──────────┬────────────────────┘
     │          │          │          │          │
     ▼          ▼          ▼          ▼          ▼
┌─────────┐ ┌─────────┐ ┌─────────┐ ┌─────────┐ ┌─────────┐
│DayNight │ │Environ- │ │Notebook │ │ Entity  │ │ Puzzle  │
│ Cycle   │ │  ment   │ │ System  │ │ System  │ │ System  │
│ Manager │ │ Manager │ │ Manager │ │ Manager │ │ Manager │
└────┬────┘ └────┬────┘ └────┬────┘ └────┬────┘ └────┬────┘
     │          │          │          │          │
     │          ▼          ▼          ▼          ▼
     │     ┌──────────────────────────────────────┐
     │     │           Ending Tracker              │
     │     │  (passive listener — no outbound      │
     │     │   events, just accumulates scores)    │
     │     └──────────────────────────────────────┘
     │
     └──────► SaveManager (listens to phase transitions → triggers autosave)
```

### Dependency Direction (who depends on whom)

```
DayNightCycle ──────► Nothing (pure clock, drives everything)
     │
     │  publishes: PhaseChanged
     ▼
EnvironmentManager ──► DayNightCycle (reads current phase)
     │
     │  publishes: ObjectDisplaced, LightFailed, LightRepaired,
     │             ZoneStabilityChanged, RecordCorrupted
     ▼
NotebookManager ───► EnvironmentManager (reads object data for auto-entries)
     │
     │  publishes: EntryCreated, EntryCorrupted, ManualNoteCreated
     │
EntityManager ─────► EnvironmentManager (reads zone stability/lighting)
     │                DayNightCycle (reads phase for state transitions)
     │
     │  publishes: EntityStateChanged, EntityEnteredZone,
     │             PlayerDetected, PlayerCaught
     │
PuzzleManager ─────► NotebookManager (checks entry prerequisites)
     │                EnvironmentManager (reads/writes object state)
     │
     │  publishes: PuzzleSolved, PuzzleFailed, AreaUnlocked
     │
EndingTracker ─────► Listens to everything, publishes nothing
SaveManager ───────► Reads all manager state on save, writes on load
```

### Key Dependency Rules

1. **DayNightCycle depends on nothing.** It is the heartbeat. All systems react to it.
2. **EnvironmentManager is the shared data layer.** Most systems read zone/object state from it.
3. **EntityManager never writes directly to NotebookManager.** It publishes `PlayerCaught`, and NotebookManager listens to apply corruption. This keeps them decoupled.
4. **EndingTracker is read-only.** It subscribes to events and tallies variables. Nothing depends on it until the final ending selection call.
5. **No circular dependencies.** Event flow is strictly top-down with the event bus breaking direct references.

---

## 2. Game Event Bus Architecture

### Design

A **static C# class** with `System.Action`-based events. No message queuing, no priority system, no generic `EventArgs` inheritance trees. Just typed static events.

```csharp
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

    // ─── Invoke helpers (null-safe) ───
    public static void PhaseChanged(GamePhase old, GamePhase next)
        => OnPhaseChanged?.Invoke(old, next);

    public static void DayStarted(int day) => OnDayStarted?.Invoke(day);
    public static void NightStarted(int day) => OnNightStarted?.Invoke(day);
    public static void NightEnded(int day, bool survived)
        => OnNightEnded?.Invoke(day, survived);

    // ... same pattern for all events
}
```

### Event Publishing Sources

| Event | Published By | When |
|---|---|---|
| `OnPhaseChanged` | `DayNightCycleManager` | Phase timer expires or forced transition |
| `OnDayStarted` | `DayNightCycleManager` | Recovery → Day transition |
| `OnNightStarted` | `DayNightCycleManager` | Unease → Night transition |
| `OnNightEnded` | `DayNightCycleManager` | Night → Recovery transition |
| `OnObjectDisplaced` | `EnvironmentManager` | Entropy tick or entity interaction |
| `OnObjectRestored` | `EnvironmentManager` | Player organizes object |
| `OnLightFailed` | `EnvironmentManager` | Degradation reaches 1.0 |
| `OnLightRepaired` | `EnvironmentManager` | Player uses bulb/parts |
| `OnZoneStabilityChanged` | `EnvironmentManager` | After any zone state recalculation |
| `OnRecordCorrupted` | `EnvironmentManager` | Entity corrupts a record object |
| `OnAreaUnlocked` | `EnvironmentManager` | Day transition unlock check passes |
| `OnEntryCreated` | `NotebookManager` | Auto-log trigger or manual note |
| `OnEntryCorrupted` | `NotebookManager` | Entity corruption applied to entry |
| `OnManualNoteCreated` | `NotebookManager` | Player saves a manual note |
| `OnCrossReferenceUsed` | `NotebookManager` | Player navigates a related entry link |
| `OnEntityStateChanged` | `EntityManager` | State machine transition |
| `OnEntityEnteredZone` | `EntityManager` | Entity moves to new zone |
| `OnPlayerDetected` | `EntityManager` | Detection roll succeeds |
| `OnPlayerCaught` | `EntityManager` | Escalation reaches player |
| `OnPuzzleSolved` | `PuzzleManager` | Puzzle completion validated |
| `OnPuzzleFailed` | `PuzzleManager` | Incorrect attempt |
| `OnPatternIdentified` | `PuzzleManager` | Player correctly identifies pattern |
| `OnPlayerEnteredZone` | `PlayerController` | Zone trigger collider entered |
| `OnObjectInspected` | `PlayerController` | Player presses E on inspectable |

### Event Listeners

| Listener | Subscribes To | Reaction |
|---|---|---|
| **EnvironmentManager** | `OnPhaseChanged` | Apply entropy, entity interference, supply replenishment |
| | `OnPlayerCaught` | Apply night failure penalties |
| **NotebookManager** | `OnObjectInspected` | Create auto-entry if first inspection |
| | `OnObjectDisplaced` | Create observation entry ("Something moved in...") |
| | `OnPuzzleSolved` | Create pattern/event entry |
| | `OnPlayerCaught` | Apply entry corruption (1-3 entries) |
| | `OnAreaUnlocked` | Create record entry |
| | `OnLightFailed` | Create observation entry (first time per zone) |
| | `OnNightEnded` | Create survival summary entry |
| **EntityManager** | `OnPhaseChanged` | IDLE ↔ OBSERVING transitions |
| | `OnZoneStabilityChanged` | Recalculate patrol target |
| | `OnPlayerEnteredZone` | Update player tracking for detection |
| **PuzzleManager** | `OnObjectRestored` | Check book arrangement puzzle progress |
| | `OnEntryCreated` | Check cross-reference puzzle prerequisites |
| | `OnPhaseChanged` | Enable/disable entity puzzle interference |
| | `OnDayStarted` | Unlock day-gated puzzles |
| **EndingTracker** | `OnEntryCreated` | Increment relevant counters |
| | `OnEntryCorrupted` | Recalculate knowledgeScore |
| | `OnManualNoteCreated` | Increment manualNotesCount |
| | `OnCrossReferenceUsed` | Increment crossReferencesUsed |
| | `OnObjectRestored` | Increment totalOrganized |
| | `OnLightRepaired` | Increment totalRepairs |
| | `OnZoneStabilityChanged` | Update averageStability |
| | `OnNightEnded` | Update nightsSurvived / nightsFailed |
| | `OnPuzzleSolved` | Increment puzzlesSolved |
| | `OnPatternIdentified` | Increment patternsIdentified |
| | `OnPlayerCaught` | Increment totalEntityEncounters |
| | `OnEntityStateChanged` | Track entity behavior learning |
| **SaveManager** | `OnDayStarted` | Autosave |
| | `OnNightEnded` | Autosave |

---

## 3. Unity Class Structure

### Project Layout

```
Assets/
├── Scripts/
│   ├── Core/
│   │   ├── GameEvents.cs              // Static event bus (Section 2)
│   │   ├── GameEnums.cs               // All shared enums in one file
│   │   └── GameConstants.cs           // Tuning values (speeds, thresholds, rates)
│   │
│   ├── Managers/
│   │   ├── DayNightCycleManager.cs    // MonoBehaviour, singleton
│   │   ├── EnvironmentManager.cs      // MonoBehaviour, singleton
│   │   ├── NotebookManager.cs         // MonoBehaviour, singleton
│   │   ├── EntityManager.cs           // MonoBehaviour, singleton
│   │   ├── PuzzleManager.cs           // MonoBehaviour, singleton
│   │   ├── EndingTracker.cs           // Plain class, no MonoBehaviour needed
│   │   └── SaveManager.cs            // MonoBehaviour, singleton
│   │
│   ├── Data/
│   │   ├── EnvironmentObjectData.cs   // Serializable data classes
│   │   ├── ZoneStateData.cs
│   │   ├── DayStateData.cs
│   │   ├── NotebookEntryData.cs
│   │   ├── ManualNoteData.cs
│   │   ├── PuzzleData.cs
│   │   ├── EndingVariablesData.cs
│   │   └── SaveData.cs               // Top-level save container
│   │
│   ├── Entity/
│   │   ├── EntityStateMachine.cs      // State machine logic
│   │   ├── EntityMovement.cs          // Pathfinding + speed
│   │   └── EntityDetection.cs         // Line-of-sight + lighting checks
│   │
│   ├── Environment/
│   │   ├── ObjectSlot.cs              // MonoBehaviour on slot GameObjects (edit-time only)
│   │   ├── InteractableObject.cs      // MonoBehaviour on scene objects
│   │   ├── LightFixture.cs            // MonoBehaviour on light objects
│   │   └── ZoneTrigger.cs             // Trigger collider for zone transitions
│   │
│   ├── Puzzles/
│   │   ├── PuzzleBase.cs              // Abstract base
│   │   ├── BookArrangementPuzzle.cs
│   │   ├── CrossReferencePuzzle.cs
│   │   ├── PatternPuzzle.cs
│   │   ├── CodePuzzle.cs
│   │   └── EnvironmentalPuzzle.cs
│   │
│   └── Player/
│       ├── PlayerController.cs        // Movement + interaction
│       └── NotebookUI.cs              // Tab menu, views, manual notes
```

### Shared Enums (GameEnums.cs)

```csharp
public enum GamePhase { Day, Unease, Night, Recovery }
public enum EntityState { Idle, Observing, Hunting, Escalating, Retreating }
public enum ObjectType { Book, Fixture, Prop, Record, Furniture, Switch }
public enum ObjectCondition { Normal, Degraded, Failed, Corrupted }
public enum EntryType { AutoDiscovery, AutoEvent, ManualNote }
public enum EntryCategory { Observation, Pattern, Clue, Record, Personal }
public enum PuzzleType { BookArrangement, CrossReference, Pattern, Code, Environmental }
public enum Difficulty { Simple, Moderate, Complex }
public enum SlotType { Shelf, Table, Wall, Floor, FixtureMount }
public enum ZoneId { MainFloor, Office, Storage, Basement, Apartment, Street, Alley, Forest, Rail }
```

### System Managers

#### DayNightCycleManager

```csharp
public class DayNightCycleManager : MonoBehaviour
{
    public static DayNightCycleManager Instance { get; private set; }

    [SerializeField] private float dayDuration = 300f;    // seconds
    [SerializeField] private float uneaseDuration = 120f;
    [SerializeField] private float nightDuration = 600f;
    [SerializeField] private float recoveryDuration = 60f;

    public int CurrentDay { get; private set; }
    public GamePhase CurrentPhase { get; private set; }
    public float PhaseTimeRemaining { get; private set; }

    // Core loop: Update() counts down PhaseTimeRemaining
    // When it hits zero → AdvancePhase()
    // AdvancePhase() sets new phase, publishes GameEvents.PhaseChanged()
    // Night 7 Recovery end → triggers EndingTracker.EvaluateEndings()
}
```

#### EnvironmentManager

```csharp
public class EnvironmentManager : MonoBehaviour
{
    public static EnvironmentManager Instance { get; private set; }

    private Dictionary<string, EnvironmentObjectData> objects;
    private Dictionary<ZoneId, ZoneStateData> zones;

    // Public API — Slot-Based
    public ZoneStateData GetZoneState(ZoneId zone);
    public void DisplaceObject(string objectId);       // Moves to random valid slot
    public void MoveObjectToSlot(string objectId, string slotId); // Explicit placement
    public void RestoreObject(string objectId);        // Snaps back to homeSlotId
    public bool IsSlotOccupied(string slotId);
    public string GetOccupant(string slotId);          // Returns objectId or null
    public List<string> GetEmptySlots(ZoneId zone, SlotType type); // For displacement
    public void DegradeFixture(string fixtureId);
    public void RepairFixture(string fixtureId);
    public void CorruptRecord(string objectId);

    // Internal
    private void ApplyNaturalEntropy(int day);         // Called on Day→Unease
    private void ApplyEntityInterference();            // Called on Unease→Night
    private void ApplyNightFailurePenalties();         // Called on PlayerCaught
    private void RecalculateZoneScores(ZoneId zone);   // After any object change
    private void CheckAreaUnlocks(int day);            // Called on Recovery→Day

    // Subscribe to: OnPhaseChanged, OnPlayerCaught
}
```

#### NotebookManager

```csharp
public class NotebookManager : MonoBehaviour
{
    public static NotebookManager Instance { get; private set; }

    private List<NotebookEntryData> entries;
    private List<ManualNoteData> manualNotes;

    // Public API
    public void CreateAutoEntry(EntryType type, EntryCategory cat,
                                string title, string body,
                                string zone, string sourceObj);
    public void CreateManualNote(string body, EntryCategory cat,
                                 string linkedEntryId = null);
    public void CorruptEntries(ZoneId zone, int count);
    public List<NotebookEntryData> GetEntries(/* filter params */);
    public float GetKnowledgeScore();

    // Subscribe to: OnObjectInspected, OnObjectDisplaced, OnPuzzleSolved,
    //               OnPlayerCaught, OnAreaUnlocked, OnLightFailed, OnNightEnded
}
```

#### EntityManager

```csharp
public class EntityManager : MonoBehaviour
{
    public static EntityManager Instance { get; private set; }

    public EntityState CurrentState { get; private set; }

    [SerializeField] private EntityStateMachine stateMachine;
    [SerializeField] private EntityMovement movement;
    [SerializeField] private EntityDetection detection;

    // State machine delegates to sub-components:
    //   stateMachine → decides transitions based on zone data
    //   movement → handles pathfinding at current speed
    //   detection → handles line-of-sight + lighting checks

    // Subscribe to: OnPhaseChanged, OnZoneStabilityChanged, OnPlayerEnteredZone
}
```

#### PuzzleManager

```csharp
public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance { get; private set; }

    private Dictionary<string, PuzzleData> puzzles;
    private HashSet<string> solvedPuzzles;

    // Public API
    public bool IsPuzzleAvailable(string puzzleId);    // Check day + prereqs
    public bool IsPuzzleSolved(string puzzleId);
    public void CompletePuzzle(string puzzleId);
    public void FailPuzzle(string puzzleId);
    public int GetSolvedCount();

    // Subscribe to: OnObjectRestored, OnEntryCreated, OnPhaseChanged, OnDayStarted
}
```

#### EndingTracker

```csharp
public class EndingTracker
{
    // Not a MonoBehaviour — just a data accumulator.
    // Instantiated by a manager or GameManager on startup.

    public EndingVariablesData Variables { get; private set; }

    public void SubscribeToEvents();   // Called once at init
    public void UnsubscribeFromEvents();

    public int EvaluateEnding();       // Returns ending number 1-10
    // Checks conditions in priority order, returns first match (default: 5)
}
```

### Data Models (all [System.Serializable])

```csharp
[System.Serializable]
public class SaveData
{
    public DayStateData dayState;
    public List<ZoneStateData> zones;
    public List<EnvironmentObjectData> changedObjects;  // Delta only
    public List<NotebookEntryData> entries;
    public List<ManualNoteData> manualNotes;
    public List<string> solvedPuzzles;
    public EndingVariablesData endingVars;
    public EntitySaveData entityState;
    public PlayerSaveData playerState;
}
```

```csharp
[System.Serializable]
public class EnvironmentObjectData
{
    public string id;
    public ObjectType objectType;
    public ZoneId zoneId;
    public string homeSlotId;       // Where this object belongs
    public string currentSlotId;    // Where this object currently is
    public bool IsDisplaced => homeSlotId != currentSlotId;  // Derived, not stored
    public ObjectCondition condition;
    public float degradationLevel;
    public bool hasBeenInspected;
    public int lastInspectedDay;
    public int interactionCount;
    public bool entityTouched;
    public int entityTouchDay;
}
```

```csharp
public enum SlotType { Shelf, Table, Wall, Floor, FixtureMount }
```

```csharp
[System.Serializable]
public class ZoneStateData
{
    public ZoneId zoneId;
    public bool isUnlocked;
    public int unlockDay;
    public float stabilityScore;
    public float lightingScore;
    public float orderScore;
    public int totalFixtures;
    public int functionalFixtures;
    public int totalObjects;
    public int displacedObjects;
    public int corruptedRecords;
}
```

```csharp
[System.Serializable]
public class NotebookEntryData
{
    public string id;
    public EntryType entryType;
    public EntryCategory category;
    public string title;
    public string body;
    public int dayCreated;
    public GamePhase phaseCreated;
    public ZoneId sourceLocation;
    public string sourceObject;
    public bool isCorrupted;
    public string corruptedBody;
    public List<string> relatedEntries;
    public bool isRead;
    public bool isPinned;
}
```

```csharp
[System.Serializable]
public class EndingVariablesData
{
    public float knowledgeScore;
    public int manualNotesCount;
    public int crossReferencesUsed;
    public float averageStability;
    public int totalRepairs;
    public int totalOrganized;
    public int nightsSurvived;
    public int nightsFailed;
    public int totalEntityEncounters;
    public int puzzlesSolved;
    public int patternsIdentified;
    public int loreFragmentsFound;
    public int npcConversations;
    public int elderConversations;
    public int criticalReveals;
    public int entityBehaviorLearned;
    public int entityRecordsPreserved;
}
```

### SaveManager

```csharp
public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    private const string SaveFileName = "nightborrower_save.json";

    public void Save()
    {
        // 1. Build SaveData by reading each manager's current state
        // 2. JsonUtility.ToJson(saveData)
        // 3. File.WriteAllText(Application.persistentDataPath + "/" + SaveFileName, json)
    }

    public bool Load()
    {
        // 1. Read JSON from disk
        // 2. JsonUtility.FromJson<SaveData>(json)
        // 3. Push data into each manager via their LoadState() methods
        // 4. Return false if no save file exists
    }

    public bool HasSave()
    {
        return File.Exists(Application.persistentDataPath + "/" + SaveFileName);
    }

    // Subscribe to: OnDayStarted (autosave), OnNightEnded (autosave)
}
```

**Save flow:**
```
SaveManager.Save()
  → EnvironmentManager.GetSaveState()   → changedObjects[] + zones[]
  → NotebookManager.GetSaveState()      → entries[] + manualNotes[]
  → PuzzleManager.GetSaveState()        → solvedPuzzles[]
  → EndingTracker.Variables              → endingVars
  → DayNightCycleManager state          → dayState
  → EntityManager.GetSaveState()        → entityState
  → PlayerController state              → playerState
  → Serialize to JSON → Write to disk
```

**Load flow:**
```
SaveManager.Load()
  → Read JSON → Deserialize to SaveData
  → DayNightCycleManager.LoadState(dayState)
  → EnvironmentManager.LoadState(zones, changedObjects)
  → NotebookManager.LoadState(entries, manualNotes)
  → PuzzleManager.LoadState(solvedPuzzles)
  → EndingTracker.LoadState(endingVars)
  → EntityManager.LoadState(entityState)
  → PlayerController.LoadState(playerState)
```

---

## Design Notes

### Why Static Events Over a Generic Event System

- **No allocations.** Static `Action` events don't box, don't allocate message objects, don't need lookup dictionaries.
- **Compile-time safety.** Typo in an event name = compiler error. String-keyed event systems fail silently.
- **Easy to trace.** "Find All References" on any event shows every publisher and subscriber. No runtime indirection.
- **Trade-off accepted:** Adding a new event type requires editing `GameEvents.cs`. For a solo dev on a 7-day game cycle, this is a feature, not a bug — it keeps the event surface visible in one file.

### Why Singletons for Managers

- There is exactly one of each system. No need for dependency injection, service locators, or Zenject.
- `Instance` is set in `Awake()`, nulled in `OnDestroy()`. Standard Unity pattern.
- Managers subscribe to events in `OnEnable()`, unsubscribe in `OnDisable()`. Prevents dangling references.

### Slot System: Scene Setup and Puzzle Example

#### ObjectSlot (Scene Component)

```csharp
// Placed on empty GameObjects in the scene editor.
// Defines where objects CAN exist. Not serialized to save data.
public class ObjectSlot : MonoBehaviour
{
    [Header("Slot Identity")]
    public string slotId;               // e.g. "main_shelfA_03"
    public ZoneId zone;
    public SlotType slotType;

    [Header("Accepted Objects")]
    public ObjectType[] acceptsTypes;   // What can go here

    // Runtime (not serialized)
    public string OccupantId { get; set; }
    public bool IsOccupied => OccupantId != null;

    // Editor convenience: auto-generate slotId from hierarchy path
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(slotId))
            slotId = gameObject.name.ToLower().Replace(" ", "_");
    }
}
```

#### Book Arrangement Puzzle Example (P01: Shelf Restoration)

Scene setup: Main floor has shelf A with 8 slots (`main_shelfA_01` through `main_shelfA_08`). Five books belong on this shelf. During entropy, some books get displaced to other shelves or tables.

```csharp
// In PuzzleManager or a BookArrangementPuzzle component:
public class BookArrangementPuzzle : PuzzleBase
{
    // Set in inspector — the book IDs that must be correctly placed
    [SerializeField] private string[] requiredBookIds;

    public override bool CheckCompletion()
    {
        var env = EnvironmentManager.Instance;
        foreach (string bookId in requiredBookIds)
        {
            var obj = env.GetObject(bookId);
            if (obj.IsDisplaced) return false;  // homeSlotId != currentSlotId
        }
        return true;
    }
}

// Player interaction flow:
// 1. Player inspects displaced book → sees "This doesn't belong here"
// 2. Player picks up book (E key) → book enters "held" state
// 3. Player walks to correct shelf slot → slot highlights as valid
// 4. Player places book (E key) → MoveObjectToSlot(bookId, slotId)
// 5. If slotId == homeSlotId → book snaps into place, subtle positive feedback
// 6. After each placement → CheckCompletion() → if true, publish OnPuzzleSolved

// Entity displacement during night:
// 1. EntityManager triggers EnvironmentManager.DisplaceObject(bookId)
// 2. DisplaceObject() picks random empty SHELF slot in same zone
// 3. Book snaps to new slot → player must re-organize next day
```

#### Cross-Reference Puzzle Example (P03: Receipt Discrepancy)

```csharp
// No slot logic needed — this puzzle type checks notebook state only.
public class CrossReferencePuzzle : PuzzleBase
{
    [SerializeField] private string[] requiredEntryIds;  // Entries player must have

    public override bool CheckCompletion()
    {
        var notebook = NotebookManager.Instance;
        foreach (string entryId in requiredEntryIds)
        {
            var entry = notebook.GetEntry(entryId);
            if (entry == null || !entry.isRead) return false;
        }
        return true;  // Player has read all required entries
    }
}
```

#### How Slot Data Flows Through Save/Load

**What gets saved** (per object, delta-compressed):
```json
{ "id": "book_welcome_03", "currentSlotId": "main_shelfB_02", "condition": 0, "hasBeenInspected": true }
```

**What does NOT get saved:**
- `homeSlotId` — baked into scene data (never changes)
- `objectType`, `zoneId` — baked into scene data
- Slot world positions — baked into scene `ObjectSlot` transforms
- Objects at their home slot — excluded by delta compression

**On load:**
1. All objects start at their `homeSlotId` (scene default)
2. SaveManager reads changed objects from JSON
3. For each entry: `EnvironmentManager.MoveObjectToSlot(id, currentSlotId)`
4. Objects snap to their saved slot positions

This means a fresh game with no displacement has **zero** object entries in the save file.

---

## 4. Debug and Logging Framework

### Design Principle

One static class. Compile-stripped in release builds. No log files, no log levels beyond on/off per category. This is a **playtest tool**, not a production logging pipeline.

### Logging Categories

```csharp
[System.Flags]
public enum LogCategory
{
    None        = 0,
    Cycle       = 1 << 0,   // Day/night phase transitions
    Environment = 1 << 1,   // Object displacement, light failure, zone stability
    Entity      = 1 << 2,   // State machine transitions, detection, zone movement
    Notebook    = 1 << 3,   // Entry creation, corruption, manual notes
    Puzzle      = 1 << 4,   // Solve/fail, prerequisite checks, entity interference
    Save        = 1 << 5,   // Save/load operations, file size, timing
    Ending      = 1 << 6,   // Variable accumulation, threshold checks
    All         = ~0
}
```

Flags enum so categories combine: `LogCategory.Entity | LogCategory.Environment` logs both while silencing everything else.

### Implementation

```csharp
public static class DevLog
{
    // Toggle per category — set via debug overlay or inspector
    public static LogCategory ActiveCategories = LogCategory.All;

    // Master kill switch — strips all logging in release
    [System.Diagnostics.Conditional("UNITY_EDITOR"),
     System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
    public static void Log(LogCategory cat, string message)
    {
        if ((ActiveCategories & cat) == 0) return;
        UnityEngine.Debug.Log($"[{cat}] {message}");
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR"),
     System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
    public static void Warn(LogCategory cat, string message)
    {
        if ((ActiveCategories & cat) == 0) return;
        UnityEngine.Debug.LogWarning($"[{cat}] {message}");
    }
}
```

**Key decisions:**
- `[Conditional]` attributes mean the compiler **removes all call sites** in release builds. Zero runtime cost. No `#if` blocks scattered through game code.
- No log-to-file. Unity's console is sufficient for solo dev. If a tester needs logs, they can run a development build and Unity captures console output automatically.
- No timestamps in the log string. Unity's console already timestamps every entry.

### Log Message Format

```
[Category] Subject: What happened (key=value, key=value)
```

All log calls follow this pattern. Subject is the system or object acting. Parenthetical contains only the state that changed.

### What Each System Logs

#### Cycle

```csharp
// In DayNightCycleManager.AdvancePhase():
DevLog.Log(LogCategory.Cycle,
    $"Phase: {oldPhase} → {newPhase} (day={CurrentDay})");

// In DayNightCycleManager on Recovery→Day:
DevLog.Log(LogCategory.Cycle,
    $"Day started: {CurrentDay} (bulbs={available}, parts={parts})");
```

#### Entity

```csharp
// In EntityStateMachine on transition:
DevLog.Log(LogCategory.Entity,
    $"State: {oldState} → {newState} (zone={currentZone}, threat={threatWeight:F2})");

// In EntityDetection on detection roll:
DevLog.Log(LogCategory.Entity,
    $"Detection: roll in {zone} (chance={chance:F2}, lighting={lightLevel}, result={hit})");

// In EntityMovement on zone change:
DevLog.Log(LogCategory.Entity,
    $"Moved: {fromZone} → {toZone} (speed={speed:F1}, target={patrolTarget})");
```

#### Environment

```csharp
// In EnvironmentManager.DisplaceObject():
DevLog.Log(LogCategory.Environment,
    $"Displaced: {objectId} (from={oldSlot}, to={newSlot}, zone={zone})");

// In EnvironmentManager.RecalculateZoneScores():
DevLog.Log(LogCategory.Environment,
    $"Zone: {zoneId} (stability={score:F2}, lighting={light:F2}, order={order:F2})");

// In EnvironmentManager.ApplyNaturalEntropy():
DevLog.Log(LogCategory.Environment,
    $"Entropy: day {day} (displaced={count}, degraded={lightCount})");
```

#### Notebook

```csharp
// In NotebookManager.CreateAutoEntry():
DevLog.Log(LogCategory.Notebook,
    $"Entry: {title} (type={type}, cat={category}, zone={zone})");

// In NotebookManager.CorruptEntries():
DevLog.Warn(LogCategory.Notebook,
    $"Corrupted: {count} entries in {zone} (targets={string.Join(",", ids)})");
```

#### Puzzle

```csharp
// In PuzzleManager.CompletePuzzle():
DevLog.Log(LogCategory.Puzzle,
    $"Solved: {puzzleId} (type={type}, day={day}, total={solvedCount}/12)");

// In PuzzleBase.CheckCompletion() when prerequisites missing:
DevLog.Log(LogCategory.Puzzle,
    $"Blocked: {puzzleId} (missing={string.Join(",", missing)})");
```

#### Save

```csharp
// In SaveManager.Save():
DevLog.Log(LogCategory.Save,
    $"Saved: {fileName} (size={bytes}B, objects={changedCount}, entries={entryCount}, ms={elapsed})");

// In SaveManager.Load():
DevLog.Log(LogCategory.Save,
    $"Loaded: {fileName} (day={day}, phase={phase}, objects={count})");
```

#### Ending

```csharp
// In EndingTracker on any variable change (log sparingly — only on significant events):
DevLog.Log(LogCategory.Ending,
    $"Variable: {varName} → {newValue} (source={eventName})");

// In EndingTracker.EvaluateEnding():
DevLog.Log(LogCategory.Ending,
    $"Ending: #{result} {endingName} (knowledge={k:F2}, stability={s:F2}, " +
    $"survived={n}, puzzles={p}, reveals={r})");
```

### Integration with GameEvents

Rather than adding log calls inside every manager, a single `DebugEventLogger` subscribes to the event bus and logs cross-system flow. This keeps debug code out of game logic entirely.

```csharp
public class DebugEventLogger : MonoBehaviour
{
    // Only exists in development builds.
    // Attach to a "Debug" GameObject that is stripped from release.

    void OnEnable()
    {
        GameEvents.OnPhaseChanged += LogPhase;
        GameEvents.OnEntityStateChanged += LogEntity;
        GameEvents.OnObjectDisplaced += LogDisplace;
        GameEvents.OnPuzzleSolved += LogPuzzle;
        GameEvents.OnPlayerCaught += LogCaught;
        // ... subscribe to all events worth tracing
    }

    void OnDisable()
    {
        GameEvents.OnPhaseChanged -= LogPhase;
        GameEvents.OnEntityStateChanged -= LogEntity;
        GameEvents.OnObjectDisplaced -= LogDisplace;
        GameEvents.OnPuzzleSolved -= LogPuzzle;
        GameEvents.OnPlayerCaught -= LogCaught;
    }

    void LogPhase(GamePhase old, GamePhase next)
        => DevLog.Log(LogCategory.Cycle, $"Phase: {old} → {next}");

    void LogEntity(EntityState old, EntityState next)
        => DevLog.Log(LogCategory.Entity, $"State: {old} → {next}");

    void LogDisplace(string objId, string zoneId)
        => DevLog.Log(LogCategory.Environment, $"Displaced: {objId} (zone={zoneId})");

    void LogPuzzle(string puzzleId)
        => DevLog.Log(LogCategory.Puzzle, $"Solved: {puzzleId}");

    void LogCaught()
        => DevLog.Warn(LogCategory.Entity, "Player caught");
}
```

**Two logging layers:**
1. **DebugEventLogger** — catches all cross-system events automatically. Covers 80% of debugging needs without touching game code.
2. **Inline DevLog calls** — used sparingly inside managers for internal state that doesn't fire an event (e.g., entropy calculations, detection rolls, save timing).

### Debug Overlay

An in-game overlay toggled with **F1** (editor and development builds only). Not a full imgui system — just `OnGUI` text drawn over the game view.

```csharp
public class DebugOverlay : MonoBehaviour
{
    private bool showOverlay = false;
    private int activePanel = 0;  // Cycle with F2

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1)) showOverlay = !showOverlay;
        if (Input.GetKeyDown(KeyCode.F2)) activePanel = (activePanel + 1) % 4;
        if (Input.GetKeyDown(KeyCode.F3)) ToggleLogCategory();
    }

    void OnGUI()
    {
        if (!showOverlay) return;
        switch (activePanel)
        {
            case 0: DrawCyclePanel(); break;
            case 1: DrawEntityPanel(); break;
            case 2: DrawZonePanel(); break;
            case 3: DrawEndingPanel(); break;
        }
    }
}
```

**Panel 0 — Cycle** (top-left corner)
```
Day 4 | NIGHT | 08:23 remaining
Bulbs: 1 | Parts: 0
```

**Panel 1 — Entity** (top-left corner)
```
Entity: HUNTING → MainFloor
  threat: 0.72 | threshold: 0.50
  speed: 2.5 m/s | detect range: 25m
  player zone: Storage | lighting: 0.45
```

**Panel 2 — Zones** (top-left, scrollable with arrow keys)
```
MAIN_FLOOR  stab=0.38  light=0.45  order=0.32  [!]
OFFICE      stab=0.91  light=1.00  order=0.88
STORAGE     stab=0.55  light=0.60  order=0.50
BASEMENT    stab=0.22  light=0.30  order=0.15  [!!]
```
`[!]` = below 0.4, `[!!]` = below 0.25

**Panel 3 — Endings** (top-left)
```
knowledge=0.62  stability=0.48  survived=3/4
puzzles=6/12  reveals=4/8  notes=12
---
Current best: #5 Imperfect Memory
Next unlock:  #2 Understanding (need reveals>=5)
```

### Project Layout Addition

```
Assets/
├── Scripts/
│   ├── Core/
│   │   ├── GameEvents.cs
│   │   ├── GameEnums.cs
│   │   ├── GameConstants.cs
│   │   └── DevLog.cs              // Static logging class
│   │
│   ├── Debug/                     // Entire folder stripped from release builds
│   │   ├── DebugEventLogger.cs    // Auto-subscribes to GameEvents
│   │   └── DebugOverlay.cs        // F1 overlay panels
```

### What This Does NOT Include (Intentionally)

- **No log files.** Unity console captures output in dev builds. No file I/O overhead, no disk cleanup.
- **No log levels (Info/Debug/Warn/Error hierarchy).** Two methods: `Log` and `Warn`. Errors use Unity's built-in `Debug.LogError` which already breaks into the debugger.
- **No remote logging or analytics.** Solo dev playtesting doesn't need Sentry or Datadog.
- **No replay system.** Event logging is for reading, not replaying. A replay system is a separate feature with different requirements.
- **No performance profiling.** Unity Profiler already exists. Don't reimplement it.

---

### What This Architecture Does NOT Include (Intentionally)

- **No command pattern / undo system.** Not needed for this game's interaction model.
- **No object pooling.** Object count is fixed per scene. No spawning/despawning at runtime.
- **No ScriptableObject event channels.** Adds indirection without benefit for a single-scene game.
- **No ECS.** Traditional MonoBehaviour architecture is sufficient for the object counts in a single bookstore.
- **No generic messaging / reflection-based dispatch.** Static events are simpler and faster.
