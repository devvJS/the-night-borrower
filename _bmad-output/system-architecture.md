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
│   │   ├── NarrativeTriggerTracker.cs // Plain class, flag array + event subscriptions
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
    public bool[] narrativeTriggers;       // NarrativeTriggerTracker flag array
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
  → NarrativeTriggerTracker.GetSaveState() → narrativeTriggers[]
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
  → NarrativeTriggerTracker.LoadState(narrativeTriggers)
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
    Narrative   = 1 << 7,   // Trigger flags, reveals, echoes, env storytelling
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

#### Narrative

All narrative triggers log through a dedicated tracker that wraps the flag array. See **Narrative Trigger Debug Table** below for the full list.

```csharp
// In NarrativeTriggerTracker (see implementation pattern below):
DevLog.Log(LogCategory.Narrative,
    $"Trigger: {triggerId} fired (day={day}, phase={phase}, zone={zone})");

// On echo activation:
DevLog.Log(LogCategory.Narrative,
    $"Echo: {echoId} activated (parent={parentReveal}, day={day})");

// On failsafe firing:
DevLog.Warn(LogCategory.Narrative,
    $"Failsafe: {triggerId} forced (day={day}, reason={reason})");
```

### Narrative Trigger Debug Table

#### System-Critical Environmental Moments (E01–E06)

| Trigger ID | Debug Flag | Console Log Format | Overlay Text |
|---|---|---|---|
| E01 | `TRIGGER_E01_DISPLACEMENT_NOTICED` | `[Narrative] E01 fired: first displacement observed (day={day}, zone={zone}, object={objId})` | `E01 Displacement [DAY {d}]` |
| E02 | `TRIGGER_E02_CORRUPTION_NOTICED` | `[Narrative] E02 fired: first entry corruption (day={day}, entry={entryId})` | `E02 Corruption [DAY {d}]` |
| E03 | `TRIGGER_E03_LIGHT_WARNING` | `[Narrative] E03 fired: entity proximity flicker (day={day}, zone={zone}, entityState={state})` | `E03 LightWarn [DAY {d}]` |
| E04 | `TRIGGER_E04_ENTROPY_REVERT` | `[Narrative] E04 fired: organized shelf reverted (day={day}, zone={zone}, object={objId})` | `E04 Revert [DAY {d}]` |
| E05 | `TRIGGER_E05_CONTRADICTION` | `[Narrative] E05 fired: document contradicts entry (day={day}, doc={docId}, entry={entryId})` | `E05 Contradict [DAY {d}]` |
| E05-FS | `TRIGGER_E05_FAILSAFE` | `[Narrative] E05 failsafe: manufactured contradiction placed (day={day})` | `E05 FAILSAFE [DAY {d}]` |
| E06 | `TRIGGER_E06_LIGHT_DEGRADATION` | `[Narrative] E06 fired: first light failure (day={day}, zone={zone}, fixture={fixtureId})` | `E06 Degrade [DAY {d}]` |

#### Key Reveals (R1–R8)

| Trigger ID | Debug Flag | Console Log Format | Overlay Text |
|---|---|---|---|
| R1 | `TRIGGER_R1_FIRED` | `[Narrative] R1 fired: inventory losses revealed (day={day}, elderConvos={count})` | `R1 Inventory [DAY {d}]` |
| R1-FS | `TRIGGER_R1_FAILSAFE` | `[Narrative] R1 failsafe: Elder approached player (day=2, elderConvos=0)` | `R1 FAILSAFE [DAY 2]` |
| R2 | `TRIGGER_R2_FIRED` | `[Narrative] R2 fired: employee disappearance (day={day}, loreFound={count})` | `R2 Employee [DAY {d}]` |
| R3 | `TRIGGER_R3_FIRED` | `[Narrative] R3 fired: feeds on records (day={day}, nightsSurvived={n}, corrupted={c})` | `R3 Records [DAY {d}]` |
| R3-FS | `TRIGGER_R3_FAILSAFE` | `[Narrative] R3 failsafe: forced insight (day=5, corruption never observed)` | `R3 FAILSAFE [DAY 5]` |
| R4 | `TRIGGER_R4_FIRED` | `[Narrative] R4 fired: indigenous warnings (day={day}, puzzle=P06)` | `R4 Warnings [DAY {d}]` |
| R5 | `TRIGGER_R5_FIRED` | `[Narrative] R5 fired: Elder survived cycle (day={day}, elderConvos={count})` | `R5 Elder [DAY {d}]` |
| R5-FS | `TRIGGER_R5_FAILSAFE` | `[Narrative] R5 failsafe: Elder initiated conversation (day=5, elderConvos<3)` | `R5 FAILSAFE [DAY 5]` |
| R6 | `TRIGGER_R6_FIRED` | `[Narrative] R6 fired: containment revealed (day={day}, loreFound={count})` | `R6 Contain [DAY {d}]` |
| R6-FS | `TRIGGER_R6_FAILSAFE` | `[Narrative] R6 failsafe: threshold lowered (day={day}, newThreshold={t}, loreFound={count})` | `R6 FAILSAFE [DAY {d}]` |
| R7 | `TRIGGER_R7_FIRED` | `[Narrative] R7 fired: hiring deliberate (day={day}, puzzle=P10)` | `R7 Hiring [DAY {d}]` |
| R8 | `TRIGGER_R8_FIRED` | `[Narrative] R8 fired: entity bound to word (day={day}, quality={full|partial}, elderConvos={count})` | `R8 Final [{quality}] [DAY {d}]` |

#### Core Narrative Environmental Moments (E07–E11)

| Trigger ID | Debug Flag | Console Log Format | Overlay Text |
|---|---|---|---|
| E07 | `TRIGGER_E07_PHOTO_INSPECTED` | `[Narrative] E07 fired: photo inspected (day={day}, scratchLevel={level}/7)` | `E07 Photo [DAY {d}]` |
| E08 | `TRIGGER_E08_CABINET_OPENED` | `[Narrative] E08 fired: employee cabinet opened (day={day})` | `E08 Cabinet [DAY {d}]` |
| E09 | `TRIGGER_E09_TALLY_INSPECTED` | `[Narrative] E09 fired: tally marks inspected (day={day})` | `E09 Tally [DAY {d}]` |
| E10 | `TRIGGER_E10_APARTMENT_ENTERED` | `[Narrative] E10 fired: apartment belongings seen (day={day})` | `E10 Apartment [DAY {d}]` |
| E11 | `TRIGGER_E11_HANDPRINT_INSPECTED` | `[Narrative] E11 fired: entity handprint inspected (day={day})` | `E11 Handprint [DAY {d}]` |

#### Echo Activations

| Trigger ID | Debug Flag | Console Log Format | Overlay Text |
|---|---|---|---|
| R1-A | `ECHO_R1A_SHELF_COUNT` | `[Narrative] Echo R1-A: shelf label inspected post-reveal (day={day})` | `~R1a` |
| R1-B | `ECHO_R1B_CUSTOMER_LINE` | `[Narrative] Echo R1-B: customer Hemingway line delivered (day={day})` | `~R1b` |
| R2-A | `ECHO_R2A_NAME_TAG` | `[Narrative] Echo R2-A: name tag inspected post-reveal (day={day})` | `~R2a` |
| R2-B | `ECHO_R2B_NOTEBOOK_LINK` | `[Narrative] Echo R2-B: lore→R2 notebook links added (day={day}, linked={count})` | `~R2b` |
| R3-A | `ECHO_R3A_BLANK_PAGES` | `[Narrative] Echo R3-A: blank book inspected post-reveal (day={day})` | `~R3a` |
| R3-B | `ECHO_R3B_ENTITY_LINGER` | `[Narrative] Echo R3-B: entity shelf linger activated (day={day})` | `~R3b` |
| R4-A | `ECHO_R4A_FOUNDATION` | `[Narrative] Echo R4-A: foundation symbol inspected (day={day})` | `~R4a` |
| R5-A | `ECHO_R5A_TALLY_ELDER` | `[Narrative] Echo R5-A: tally marks re-inspected, Elder handwriting recognized (day={day})` | `~R5a` |
| R5-B | `ECHO_R5B_ELDER_PREFIX` | `[Narrative] Echo R5-B: Elder "you look at me different" prefix delivered (day={day})` | `~R5b` |
| R6-A | `ECHO_R6A_RESTORE_TEXT` | `[Narrative] Echo R6-A: one-time restore text variant used (day={day})` | `~R6a` |
| R6-B | `ECHO_R6B_NOTEBOOK_LINK` | `[Narrative] Echo R6-B: incidents→R6 notebook links added (day={day}, linked={count})` | `~R6b` |
| R7-A | `ECHO_R7A_SIGN_REMOVED` | `[Narrative] Echo R7-A: help wanted sign removed (day={day})` | `~R7a` |
| R7-B | `ECHO_R7B_CUSTOMER_LINE` | `[Narrative] Echo R7-B: customer "old man asked about you" line delivered (day={day})` | `~R7b` |
| R8-A | `ECHO_R8A_ENDING_QUALITY` | `[Narrative] Echo R8-A: ending text quality={full|partial} (reveals={count})` | `~R8a` |

### Narrative Trigger Implementation Pattern

A single tracker class manages all narrative flags. Not a MonoBehaviour — instantiated by a game manager, subscribes to events.

```csharp
public class NarrativeTriggerTracker
{
    // ─── Flag Storage ───
    // One bool per trigger. Serialized in save data.
    private bool[] triggerFired = new bool[32];  // E01-E06=0-5, R1-R8=6-13, E07-E11=14-18, echoes=19-31+

    // Readable constants for indexing
    private const int E01 = 0, E02 = 1, E03 = 2, E04 = 3, E05 = 4, E06 = 5;
    private const int R1 = 6, R2 = 7, R3 = 8, R4 = 9, R5 = 10, R6 = 11, R7 = 12, R8 = 13;
    private const int E07 = 14, E08 = 15, E09 = 16, E10 = 17, E11 = 18;
    // Echoes: 19-31 (R1A=19, R1B=20, R2A=21, ...)

    // ─── Public API ───
    public bool HasFired(int triggerId) => triggerFired[triggerId];

    public void Fire(int triggerId, string zone = "", string detail = "")
    {
        if (triggerFired[triggerId]) return;  // Already fired — skip
        triggerFired[triggerId] = true;

        int day = DayNightCycleManager.Instance.CurrentDay;
        var phase = DayNightCycleManager.Instance.CurrentPhase;

        DevLog.Log(LogCategory.Narrative,
            $"Trigger: {GetName(triggerId)} fired (day={day}, phase={phase}, zone={zone}, detail={detail})");

        GameEvents.NarrativeTriggerFired(triggerId);
    }

    public void FireFailsafe(int triggerId, string reason)
    {
        if (triggerFired[triggerId]) return;
        triggerFired[triggerId] = true;

        int day = DayNightCycleManager.Instance.CurrentDay;

        DevLog.Warn(LogCategory.Narrative,
            $"Failsafe: {GetName(triggerId)} forced (day={day}, reason={reason})");

        GameEvents.NarrativeTriggerFired(triggerId);
    }

    public void FireEcho(int echoId, int parentReveal)
    {
        if (triggerFired[echoId]) return;
        if (!triggerFired[parentReveal]) return;  // Parent hasn't fired — no echo
        triggerFired[echoId] = true;

        int day = DayNightCycleManager.Instance.CurrentDay;

        DevLog.Log(LogCategory.Narrative,
            $"Echo: {GetName(echoId)} activated (parent={GetName(parentReveal)}, day={day})");
    }

    // ─── Event Subscriptions ───
    public void SubscribeToEvents()
    {
        GameEvents.OnObjectDisplaced += CheckE01;
        GameEvents.OnEntryCorrupted += CheckE02;
        GameEvents.OnEntityEnteredZone += CheckE03;
        GameEvents.OnObjectDisplaced += CheckE04;
        GameEvents.OnLightFailed += CheckE06;
        GameEvents.OnPuzzleSolved += CheckPuzzleReveals;
        GameEvents.OnDayStarted += CheckFailsafes;
        GameEvents.OnDayStarted += CheckEchoActivations;
        GameEvents.OnObjectInspected += CheckInspectionTriggers;
    }

    // ─── Example Handlers ───
    private void CheckE01(string objId, string zoneId)
    {
        // E01: First displacement noticed (Day 2+)
        if (DayNightCycleManager.Instance.CurrentDay >= 2)
            Fire(E01, zoneId, objId);
    }

    private void CheckE04(string objId, string zoneId)
    {
        // E04: Object that player previously restored is displaced again
        var obj = EnvironmentManager.Instance.GetObject(objId);
        if (obj != null && obj.interactionCount > 0)  // Player has touched this before
            Fire(E04, zoneId, objId);
    }

    private void CheckFailsafes(int day)
    {
        // R1 failsafe: Elder hasn't been spoken to by Day 2
        if (day == 2 && !triggerFired[R1])
            FireFailsafe(R1, "elderConversations == 0 at Day 2 start");

        // E05 failsafe: No contradiction experienced by Day 4
        if (day == 4 && !triggerFired[E05])
            FireFailsafe(E05, "manufactured contradiction placed");

        // R5 failsafe: Elder conversations < 3 by Day 5
        if (day == 5 && !triggerFired[R5])
        {
            int convos = EndingTracker.Instance.Variables.elderConversations;
            if (convos < 3)
                FireFailsafe(R5, $"elderConversations={convos} < 3 at Day 5");
        }

        // R6 failsafe: Lore threshold adjustment
        if (day == 6 && !triggerFired[R6])
        {
            int lore = EndingTracker.Instance.Variables.loreFragmentsFound;
            if (lore >= 15)
            {
                Fire(R6, "", $"threshold lowered, lore={lore}");
            }
        }
        if (day == 7 && !triggerFired[R6])
        {
            FireFailsafe(R6, "forced insight at Day 7 start");
        }
    }

    private void CheckEchoActivations(int day)
    {
        // Echoes activate the day AFTER their parent reveal
        // Environment echoes: activate pre-placed inactive props
        // NPC echoes: flag NPC system to use conditional line variant
        // Notebook echoes: add relatedEntries links

        // Example: R1 echoes
        if (triggerFired[R1] && day >= 2)
        {
            // R1-A: Activate shelf label prop
            EnvironmentManager.Instance.ActivateEchoProp("r1a_shelf_label");
            // R1-B is handled by NPC system checking echo flag before line delivery
        }
    }

    // ─── Save/Load ───
    public bool[] GetSaveState() => triggerFired;
    public void LoadState(bool[] saved) => triggerFired = saved;

    // ─── Name Lookup (for debug logs) ───
    private string GetName(int id) => id switch
    {
        0 => "E01", 1 => "E02", 2 => "E03", 3 => "E04", 4 => "E05", 5 => "E06",
        6 => "R1", 7 => "R2", 8 => "R3", 9 => "R4", 10 => "R5", 11 => "R6", 12 => "R7", 13 => "R8",
        14 => "E07", 15 => "E08", 16 => "E09", 17 => "E10", 18 => "E11",
        19 => "R1-A", 20 => "R1-B", 21 => "R2-A", 22 => "R2-B",
        23 => "R3-A", 24 => "R3-B", 25 => "R4-A",
        26 => "R5-A", 27 => "R5-B", 28 => "R6-A", 29 => "R6-B",
        30 => "R7-A", 31 => "R7-B",
        _ => $"UNKNOWN_{id}"
    };
}
```

### Integration with GameEvents

Add one new event to the event bus:

```csharp
// In GameEvents.cs — add to Narrative section
public static event Action<int> OnNarrativeTriggerFired;  // (triggerId)
public static void NarrativeTriggerFired(int id) => OnNarrativeTriggerFired?.Invoke(id);
```

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
        GameEvents.OnNarrativeTriggerFired += LogNarrative;
        // ... subscribe to all events worth tracing
    }

    void OnDisable()
    {
        GameEvents.OnPhaseChanged -= LogPhase;
        GameEvents.OnEntityStateChanged -= LogEntity;
        GameEvents.OnObjectDisplaced -= LogDisplace;
        GameEvents.OnPuzzleSolved -= LogPuzzle;
        GameEvents.OnPlayerCaught -= LogCaught;
        GameEvents.OnNarrativeTriggerFired -= LogNarrative;
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

    void LogNarrative(int triggerId)
        => DevLog.Log(LogCategory.Narrative, $"Trigger fired: id={triggerId}");
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
        if (Input.GetKeyDown(KeyCode.F2)) activePanel = (activePanel + 1) % 5;
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
            case 4: DrawNarrativePanel(); break;
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

**Panel 4 — Narrative** (top-left)
```
SYSTEM-CRITICAL     E01[✓] E02[✓] E03[✓] E04[ ] E05[FS] E06[✓]
REVEALS             R1[✓]  R2[✓]  R3[✓]  R4[ ]  R5[ ]   R6[ ]  R7[ ] R8[ ]
CORE NARRATIVE      E07[✓] E08[ ] E09[✓] E10[ ] E11[ ]
ECHOES              ~R1a[✓] ~R1b[✓] ~R2a[ ] ~R2b[✓] ~R3a[✓] ~R3b[✓]
---
Last fired: E03 (Day 3, Night, MainFloor)
Failsafes: E05 forced Day 4
Pending:    R4 (needs P06) | R5 (needs elderConvos>=3, current=2)
```
`[✓]` = fired, `[ ]` = pending, `[FS]` = fired via failsafe

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
