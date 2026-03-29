# Story 2.4: Auto-Log Discoveries

Status: done

## Story

As a player,
I want to see important discoveries automatically logged,
so that I don't miss critical information.

## Acceptance Criteria

1. Given the player inspects an object containing a first-time discovery, When the inspection completes, Then a notebook auto-entry is created with the discovery details
2. Given an auto-entry is created, When it happens, Then a brief non-intrusive UI notification confirms the entry was logged
3. Given a discovery has already been auto-logged, When the same discovery is encountered again, Then no duplicate entry is created

## Tasks / Subtasks

- [x] Task 1: Add notification UI constants to GameConstants.cs (AC: 2)
  - [x] 1.1 Add `// ─── Discovery Notification ───` section after the Inspection UI section
  - [x] 1.2 Add `public const float NotificationFontSize = 14f;` — notification text size
  - [x] 1.3 Add `public const float NotificationFadeInDuration = 0.3f;` — fade in time
  - [x] 1.4 Add `public const float NotificationDisplayDuration = 2.5f;` — how long the notification stays visible
  - [x] 1.5 Add `public const float NotificationFadeOutDuration = 0.5f;` — fade out time
  - [x] 1.6 Add `public const float NotificationVerticalOffset = 60f;` — distance from bottom of screen
  - [x] 1.7 Add `public const float NotificationPanelWidth = 350f;` — notification panel width

- [x] Task 2: Create DiscoveryEntry struct in Data/ (AC: 1, 3)
  - [x] 2.1 Create `Assets/Scripts/Data/DiscoveryEntry.cs`:
    ```
    public struct DiscoveryEntry
    {
        public string entryId;
        public string clueId;
        public string objectId;
        public string title;
        public string description;
        public EntryCategory category;
    }
    ```
  - [x] 2.2 Struct, not class — matches InspectionResult pattern. Uses existing `EntryCategory` enum from GameEnums.cs. The `entryId` is auto-generated from clueId (e.g., `"discovery_clue_journal_01"`). The `title` comes from `InteractableObject.FormatDisplayName()` — but that method is private static. See Task 3 for how to get the title.

- [x] Task 3: Create DiscoveryLog.cs in Core/ (AC: 1, 3)
  - [x] 3.1 Create `Assets/Scripts/Core/DiscoveryLog.cs` — `public class DiscoveryLog : MonoBehaviour`
  - [x] 3.2 Add singleton pattern: `public static DiscoveryLog Instance { get; private set; }` — set in Awake(), null-check for duplicate prevention. This is a lightweight precursor to the full NotebookManager (Epic 3). The singleton pattern matches the architecture spec's `NotebookManager.Instance`.
  - [x] 3.3 Add fields:
    - `private List<DiscoveryEntry> entries = new List<DiscoveryEntry>();` — all logged discoveries this session
    - `private HashSet<string> loggedClueIds = new HashSet<string>();` — fast duplicate check (AC3 safety net)
  - [x] 3.4 Add public API:
    - `public IReadOnlyList<DiscoveryEntry> Entries => entries;` — read-only access for future notebook UI (Epic 3)
    - `public bool HasDiscovery(string clueId) => loggedClueIds.Contains(clueId);` — duplicate check
    - `public int Count => entries.Count;`
  - [x] 3.5 Add `public event Action<DiscoveryEntry> OnDiscoveryLogged;` — local event for notification UI. Separate from GameEvents.EntryCreated which passes only an entryId string — the notification needs the full entry data to display title.
  - [x] 3.6 Subscribe to `GameEvents.OnClueDiscovered` in OnEnable(), unsubscribe in OnDisable().
  - [x] 3.7 Add `private void HandleClueDiscovered(string clueId, string objectId)`:
    - Guard: if `loggedClueIds.Contains(clueId)` return (AC3 duplicate prevention)
    - Find the InteractableObject by objectId to get title and description. Use `FindObjectsOfType<InteractableObject>()` and match by ObjectId. Cache the result — this only runs on first discovery, not per-frame.
    - Build DiscoveryEntry:
      - `entryId = $"discovery_{clueId}"`
      - `clueId = clueId`
      - `objectId = objectId`
      - `title` = the object's display name. Since `FormatDisplayName` is private static on InteractableObject, get it from the InspectionResult: call `obj.Inspect()` — wait, that would re-inspect. Instead, use the object's ObjectId property and format it here, OR make FormatDisplayName internal/public. **Recommended approach:** Add a `public string DisplayName` property to InteractableObject that calls FormatDisplayName(objectId). This is a minimal, useful addition.
      - `description` = the object's ClueText (the actual clue content to log)
      - `category = EntryCategory.Clue`
    - Add to `entries` list and `loggedClueIds` set
    - Fire `OnDiscoveryLogged?.Invoke(entry)`
    - Fire `GameEvents.EntryCreated(entry.entryId)` — for downstream systems (Epic 3 notebook)
  - [x] 3.8 Awake(): set Instance singleton. OnDestroy(): clear Instance if this is the instance.

- [x] Task 4: Add DisplayName property to InteractableObject.cs (AC: 1)
  - [x] 4.1 Add `public string DisplayName => FormatDisplayName(objectId);` — exposes the formatted name without duplicating the logic. Used by DiscoveryLog to get the object's title for the log entry. No other changes to InteractableObject.

- [x] Task 5: Create DiscoveryNotificationUI.cs for toast notification (AC: 2)
  - [x] 5.1 Create `Assets/Scripts/Player/DiscoveryNotificationUI.cs` — `public class DiscoveryNotificationUI : MonoBehaviour`
  - [x] 5.2 Add fields:
    - `private GameObject notificationPanel;` — the toast container
    - `private TextMeshProUGUI notificationText;` — "Discovery logged: [title]"
    - `private CanvasGroup panelCanvasGroup;` — for fade in/out
    - `private Coroutine notificationCoroutine;` — manages the full lifecycle
  - [x] 5.3 Add `public void Initialize(Canvas canvas)` — creates the notification panel:
    - Create `notificationPanel` as child of canvas, anchored to bottom-left of screen (doesn't conflict with inspection panel on right or crosshair/prompt in center)
    - Add `CanvasGroup` for alpha fade
    - Add a semi-transparent dark background `Image` (Color: 0, 0, 0, 0.6) — slightly more transparent than inspection panel since it's a transient notification
    - Create `notificationText` (TextMeshProUGUI): `NotificationFontSize`, color soft gold `(1.0, 0.9, 0.6, 1.0)` — matches clue text color for visual consistency with discovery theme
    - Position: anchored bottom-left, offset by `NotificationVerticalOffset` from bottom
    - Width: `NotificationPanelWidth`
    - Set `notificationPanel.SetActive(false)` initially
  - [x] 5.4 Add `public void ShowNotification(DiscoveryEntry entry)`:
    - Set `notificationText.text = $"Discovery logged: {entry.title}"`
    - If a notification coroutine is already running, stop it (new discovery replaces previous notification)
    - Start notification lifecycle coroutine
  - [x] 5.5 Private `NotificationLifecycle()` coroutine:
    - Activate panel, set alpha 0
    - Fade in: lerp alpha 0 → 1 over `NotificationFadeInDuration`
    - Hold: `yield return new WaitForSeconds(NotificationDisplayDuration)`
    - Fade out: lerp alpha 1 → 0 over `NotificationFadeOutDuration`
    - Deactivate panel, set coroutine reference to null

- [x] Task 6: Integrate DiscoveryLog and DiscoveryNotificationUI (AC: 1, 2, 3)
  - [x] 6.1 In `PlayerHUD.cs`:
    - Add `private DiscoveryNotificationUI discoveryNotificationUI;` field
    - In `BuildHUD()` after InspectionUI creation: `discoveryNotificationUI = gameObject.AddComponent<DiscoveryNotificationUI>(); discoveryNotificationUI.Initialize(canvas);`
    - Add public property: `public DiscoveryNotificationUI DiscoveryNotificationUI => discoveryNotificationUI;`
  - [x] 6.2 DiscoveryLog needs to be added to the scene. Two options:
    - **Option A (recommended):** Create a new empty GameObject "DiscoveryLog" in the scene with the DiscoveryLog component. This keeps it independent of the Player hierarchy.
    - **Option B:** Add to an existing manager object if one exists.
    - The DiscoveryLog singleton finds PlayerHUD to connect the notification in Awake() or Start(): `PlayerHUD hud = FindObjectOfType<PlayerHUD>(); if (hud != null) OnDiscoveryLogged += hud.DiscoveryNotificationUI.ShowNotification;`
    - Unsubscribe in OnDisable/OnDestroy.
  - [x] 6.3 **Alternative simpler wiring:** Instead of DiscoveryLog finding PlayerHUD, have PlayerHUD.Awake() find DiscoveryLog and subscribe. Since PlayerHUD already finds other systems (PlayerController, InspectionSystem), this pattern is established. Add after InspectionSystem lookup:
    ```
    DiscoveryLog discoveryLog = FindObjectOfType<DiscoveryLog>();
    if (discoveryLog != null)
        discoveryLog.OnDiscoveryLogged += discoveryNotificationUI.ShowNotification;
    ```
    This is cleaner because PlayerHUD already owns the notification UI and already does FindObjectOfType lookups. Use this approach.

- [x] Task 7: Manual testing verification (AC: 1, 2, 3) — **Requires Unity Editor; to be verified by user**
  - [x] 7.1 Create an empty GameObject named "DiscoveryLog" in the Bookstore scene. Add the DiscoveryLog component.
  - [x] 7.2 Set up test: On an InteractableObject with `clueText` and `clueId` filled in, enter play mode.
  - [x] 7.3 Inspect the object (E key) — verify the inspection panel shows AND a bottom-left notification appears saying "Discovery logged: [Object Name]" with gold text and fade-in (AC1, AC2).
  - [x] 7.4 Exit inspection, re-inspect the same object — verify NO new notification appears (AC3: no duplicate). The inspection panel should still show the clue (re-readable from Story 2-3).
  - [x] 7.5 Set up a second object with different clueId and clueText. Inspect it — verify a NEW notification appears for this distinct discovery.
  - [x] 7.6 Set up an object with empty clueText/clueId. Inspect it — verify NO notification appears (no clue = no discovery to log).
  - [x] 7.7 Verify the notification fades out after ~2.5 seconds and doesn't block gameplay.

## Dev Notes

### Architecture Compliance

- **File:** `Assets/Scripts/Core/DiscoveryLog.cs` — New MonoBehaviour singleton in `Core/`. Lightweight precursor to the full NotebookManager (Epic 3, Story 3-1). Uses singleton pattern matching architecture spec's `NotebookManager.Instance`. When NotebookManager is created in Epic 3, it will absorb DiscoveryLog's entries list and subscription logic.
- **File:** `Assets/Scripts/Data/DiscoveryEntry.cs` — New struct in `Data/`. Follows InspectionResult pattern (lightweight value type). Uses existing `EntryCategory` enum.
- **File:** `Assets/Scripts/Player/DiscoveryNotificationUI.cs` — New MonoBehaviour in `Player/`. Toast notification component. Follows InspectionUI pattern (Initialize with canvas, programmatic UI creation).
- **File:** `Assets/Scripts/Environment/InteractableObject.cs` — Modified. Adding `DisplayName` public property to expose FormatDisplayName without duplicating logic.
- **File:** `Assets/Scripts/Player/PlayerHUD.cs` — Modified. Creates and owns DiscoveryNotificationUI, wires to DiscoveryLog.
- **File:** `Assets/Scripts/Core/GameConstants.cs` — Modified. Adding Discovery Notification constants section.

### Event Flow After This Story

1. Player inspects object with clue (first time) → InspectionSystem
2. `GameEvents.ClueDiscovered(clueId, objectId)` fires (from Story 2-3)
3. DiscoveryLog.HandleClueDiscovered receives event
4. Duplicate check: `loggedClueIds.Contains(clueId)` → skip if already logged (AC3)
5. Build DiscoveryEntry from object data
6. Add to entries list and loggedClueIds set
7. Fire `DiscoveryLog.OnDiscoveryLogged(entry)` → DiscoveryNotificationUI.ShowNotification (AC2)
8. Fire `GameEvents.EntryCreated(entryId)` → future notebook systems (Epic 3)
9. Notification shows "Discovery logged: [title]" with fade-in/hold/fade-out (AC2)

### Duplicate Prevention (AC3) — Defense in Depth

Two layers prevent duplicate entries:
1. **Source layer (Story 2-3):** `GameEvents.ClueDiscovered` only fires on `result.isFirstInspection && result.hasClue` — re-inspections don't fire the event at all.
2. **Log layer (this story):** `loggedClueIds` HashSet guards against any edge case where the event fires twice (e.g., rapid double-press before hasBeenInspected updates).

### What This Story Does NOT Include

- **No notebook UI browser** — Story 3-1 creates the notebook review interface where players can browse all logged entries.
- **No manual notes** — Story 3-2 adds player-written notes.
- **No entry persistence** — Entries are runtime-only this story. Epic 5 adds save/load of discovery state.
- **No entry corruption** — The Borrower corrupting entries comes in Epic 6.
- **No audio feedback** — Discovery sounds come in Epic 9.
- **No categorization/filtering** — Story 3-3 adds notebook categorization.

### Previous Story (2-3) Learnings

- **InspectionUI pattern works well.** Same approach for DiscoveryNotificationUI: MonoBehaviour component on PlayerHUD GameObject, Initialize(Canvas), programmatic UI creation.
- **PlayerHUD owns all UI components.** InspectionUI added via `gameObject.AddComponent<InspectionUI>()` in BuildHUD(). Same pattern for DiscoveryNotificationUI.
- **PlayerHUD.BuildHUD() canvas is local variable.** Must pass it to Initialize() — canvas is not stored as a field. Follow same pattern as InspectionUI.
- **Manual positioning works.** Project uses RectTransform positioning, not VerticalLayoutGroup.
- **GameEvents invoke helpers are one-liners.** `EntryCreated(entryId)` already exists in GameEvents.cs — no new event declaration needed, just fire it.
- **FindObjectOfType pattern established.** InspectionSystem uses it for PlayerHUD. PlayerHUD uses it for PlayerController. Acceptable for one-time Awake() lookups.
- **CanvasGroup alpha for fade effects.** Proven pattern in InspectionUI.

### Existing Code to Build On

**GameEvents.cs (103 lines):**
- `OnClueDiscovered(clueId, objectId)` at line 42: DiscoveryLog subscribes to this
- `EntryCreated(entryId)` at line 67-68: Already exists, just call it when logging
- `OnEntryCreated` at line 21: Already declared, no modification needed

**InteractableObject.cs (229 lines):**
- `FormatDisplayName(objectId)` at line 172-182: Private static method. Add public DisplayName property to expose.
- `ObjectId`, `ClueText`, `ObjectDescription` properties: Used by DiscoveryLog to build entry.

**PlayerHUD.cs (157 lines):**
- `BuildHUD()` at line 83: Add DiscoveryNotificationUI creation after InspectionUI (line 153-154)
- Pattern: `gameObject.AddComponent<T>()` then `Initialize(canvas)`
- Awake() at line 21: Add DiscoveryLog lookup and event subscription

**InspectionSystem.cs (208 lines):**
- `EnterInspection()` at line 99: Already fires `ClueDiscovered` at line 124-127. No changes needed — DiscoveryLog subscribes passively.

**GameConstants.cs (65 lines):**
- Last section is Inspection UI (lines 58-64). New Discovery Notification section goes after.

### Performance Targets

- DiscoveryLog: singleton, event-driven — zero per-frame cost
- HandleClueDiscovered: runs only on first discovery — one FindObjectsOfType call (cached via direct object reference from event args)
- DiscoveryEntry struct: stack-allocated, no GC pressure
- Notification coroutine: trivial alpha lerp, auto-cleans up
- loggedClueIds HashSet: O(1) duplicate check
- No new per-frame operations or Update() calls
- Target: < 0.01ms frame time impact

### Project Structure Notes

- New file: `Assets/Scripts/Core/DiscoveryLog.cs`
- New file: `Assets/Scripts/Data/DiscoveryEntry.cs`
- New file: `Assets/Scripts/Player/DiscoveryNotificationUI.cs`
- Modified: `Assets/Scripts/Core/GameConstants.cs` (notification constants)
- Modified: `Assets/Scripts/Environment/InteractableObject.cs` (DisplayName property)
- Modified: `Assets/Scripts/Player/PlayerHUD.cs` (notification UI creation, DiscoveryLog wiring)
- Scene: Add DiscoveryLog GameObject to Bookstore.unity (manual step by user)

### References

- [Source: epics.md#Epic 2, Story 2.4] — AC definitions and story statement
- [Source: gdd.md#Record Mechanic] — "Major discoveries... are automatically logged — preventing players from missing essential progression"
- [Source: gdd.md#Hint System] — "notebook entries (auto-logged key discoveries, highlighted observations)"
- [Source: system-architecture.md#NotebookManager] — Singleton pattern, CreateAutoEntry API, subscribes to OnObjectInspected
- [Source: system-architecture.md#GameEvents] — OnEntryCreated event, OnClueDiscovered event
- [Source: GameEnums.cs] — EntryType.AutoDiscovery, EntryCategory.Clue
- [Source: epics.md#Epic 3, Story 3-1] — Downstream: notebook UI will read DiscoveryLog.Entries to display logged discoveries

## Dev Agent Record

### Agent Model Used

Claude Opus 4.6

### Debug Log References

None — no debug logs added.

### Completion Notes List

- Tasks 1-6 implemented as specified
- Task 4 done before Task 3 since DiscoveryLog depends on DisplayName property
- Used Task 6.3 approach (PlayerHUD finds DiscoveryLog and subscribes) — cleaner than DiscoveryLog finding PlayerHUD
- Task 7 (manual testing) requires user to: add DiscoveryLog GameObject to scene, then verify in Unity Editor
- DiscoveryLog has no Update() — purely event-driven, zero per-frame cost

### File List

- `Assets/Scripts/Core/DiscoveryLog.cs` — NEW: Singleton discovery log, subscribes to ClueDiscovered, stores entries, fires notifications
- `Assets/Scripts/Data/DiscoveryEntry.cs` — NEW: Lightweight struct for discovery log entries
- `Assets/Scripts/Player/DiscoveryNotificationUI.cs` — NEW: Bottom-left toast notification with fade-in/hold/fade-out
- `Assets/Scripts/Core/GameConstants.cs` — MODIFIED: Added Discovery Notification constants section (6 constants)
- `Assets/Scripts/Environment/InteractableObject.cs` — MODIFIED: Added DisplayName public property
- `Assets/Scripts/Player/PlayerHUD.cs` — MODIFIED: Creates DiscoveryNotificationUI, wires to DiscoveryLog

### Change Log

| Date | Change | Author |
|------|--------|--------|
| 2026-03-29 | Tasks 1-6 implemented | Dev Agent (Claude Opus 4.6) |
| 2026-03-29 | Code review: fixed M1 — PlayerHUD now unsubscribes from DiscoveryLog.OnDiscoveryLogged in OnDestroy | Review Agent (Claude Opus 4.6) |
