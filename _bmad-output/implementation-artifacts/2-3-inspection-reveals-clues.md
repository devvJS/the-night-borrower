# Story 2.3: Inspection Reveals Clues

Status: done

## Story

As a player,
I want to see inspection results reveal clues or confirm object state,
so that inspection feels meaningful.

## Acceptance Criteria

1. Given an object with associated clue data is inspected, When the focused view opens, Then relevant information (text, visual detail, or state confirmation) is presented to the player
2. Given an object has already been inspected and its state has not changed, When inspected again, Then no duplicate discovery is triggered but the player can still view it
3. Given an object's state has changed since last inspection, When re-inspected, Then the new state is presented and flagged as changed

## Tasks / Subtasks

- [x] Task 1: Add inspection UI constants to GameConstants.cs (AC: 1, 2, 3)
  - [x]1.1 Add `// ─── Inspection UI ───` section after the Inspection section
  - [x]1.2 Add `public const float InspectionPanelWidth = 400f;` — width of the clue text panel in screen-space overlay
  - [x]1.3 Add `public const float InspectionPanelPadding = 20f;` — inner padding for text
  - [x]1.4 Add `public const float InspectionTitleFontSize = 22f;` — object name / clue title
  - [x]1.5 Add `public const float InspectionBodyFontSize = 16f;` — description / clue body text
  - [x]1.6 Add `public const float InspectionPanelFadeDuration = 0.3f;` — fade in after transition completes
  - [x]1.7 Add `public const float InspectionChangedBadgeFontSize = 14f;` — "CHANGED" badge font size

- [x] Task 2: Add clue data fields to InteractableObject.cs (AC: 1, 2, 3)
  - [x]2.1 Add `[Header("Clue Data")] [SerializeField] [TextArea(2, 5)] private string objectDescription = "";` — default description shown on inspection (e.g., "A worn paperback. Nothing unusual."). TextArea attribute gives multi-line editing in Inspector.
  - [x]2.2 Add `[SerializeField] [TextArea(2, 5)] private string clueText = "";` — clue revealed on first inspection. Empty string means no clue (just shows description). Objects without clues still show their description.
  - [x]2.3 Add `[SerializeField] private string clueId = "";` — unique clue identifier for tracking discovery. Empty means no trackable clue. Used by downstream systems (Story 2.4 auto-logging).
  - [x]2.4 Add runtime state fields (NOT serialized — managed at runtime):
    - `private bool hasBeenInspected;` — tracks whether this object has been inspected at least once this session
    - `private string lastInspectedState = "";` — snapshot of the state string at last inspection, for change detection (AC3)
  - [x]2.5 Add public properties:
    - `public string ObjectDescription => objectDescription;`
    - `public string ClueText => clueText;`
    - `public string ClueId => clueId;`
    - `public bool HasClue => !string.IsNullOrEmpty(clueText);`
    - `public bool HasBeenInspected => hasBeenInspected;`
  - [x]2.6 Add `public string GetCurrentState()` method — returns a string representing the current inspectable state of the object. For MVP: returns `$"{objectType}|{isImportant}|{objectDescription}"`. Later epics (5.x) will add displacement, condition, and entity-touched state to this string. Used for change detection (AC3).
  - [x]2.7 Add `public InspectionResult Inspect()` method — called by InspectionSystem when inspection enters. Returns an `InspectionResult` struct containing:
    - `string title` — the objectId formatted as display name (replace underscores with spaces, title case)
    - `string description` — always `objectDescription`
    - `string clue` — `clueText` if first inspection OR state changed; empty string if already inspected and unchanged
    - `bool isFirstInspection` — true if `!hasBeenInspected`
    - `bool hasStateChanged` — true if `GetCurrentState() != lastInspectedState` (and not first inspection)
    - `bool hasClue` — `HasClue`
    - After building result: set `hasBeenInspected = true`, update `lastInspectedState = GetCurrentState()`
  - [x]2.8 Keep existing `Interact()` method unchanged — it's used by non-inspectable objects. The new `Inspect()` method is only called by InspectionSystem for inspectable objects.

- [x] Task 3: Create InspectionResult struct in Data/ (AC: 1, 2, 3)
  - [x]3.1 Create `Assets/Scripts/Data/InspectionResult.cs`:
    ```
    public struct InspectionResult
    {
        public string title;
        public string description;
        public string clue;
        public string clueId;
        public bool isFirstInspection;
        public bool hasStateChanged;
        public bool hasClue;
    }
    ```
  - [x]3.2 Struct, not class — no heap allocation, passed by value. Matches project convention of lightweight data types in Data/ folder.

- [x] Task 4: Create InspectionUI.cs for clue display panel (AC: 1, 2, 3)
  - [x]4.1 Create `Assets/Scripts/Player/InspectionUI.cs` — `public class InspectionUI : MonoBehaviour`
  - [x]4.2 Add fields:
    - `private GameObject panelObject;` — the inspection info panel container
    - `private TextMeshProUGUI titleText;` — object name at top of panel
    - `private TextMeshProUGUI descriptionText;` — object description
    - `private TextMeshProUGUI clueLabel;` — the clue text (hidden if no clue or already seen)
    - `private GameObject changedBadge;` — "CHANGED" indicator shown when state has changed (AC3)
    - `private CanvasGroup panelCanvasGroup;` — for fade-in effect
    - `private Coroutine fadeCoroutine;`
    - `private Canvas parentCanvas;` — reference to HUD canvas to parent under
  - [x]4.3 Add `public void Initialize(Canvas canvas)` — called by PlayerHUD after BuildHUD(). Creates the inspection panel UI hierarchy:
    - Create `panelObject` as child of canvas, anchored to right side of screen (right-aligned so it doesn't obstruct the centered object view)
    - Add `CanvasGroup` for alpha fade
    - Add a semi-transparent dark background `Image` (Color: 0, 0, 0, 0.7)
    - Create `titleText` (TextMeshProUGUI): `InspectionTitleFontSize`, color warm white `(1.0, 0.95, 0.85, 1.0)`, bold, top of panel
    - Create `descriptionText` (TextMeshProUGUI): `InspectionBodyFontSize`, color light gray `(0.85, 0.85, 0.85, 0.9)`, below title
    - Create `clueLabel` (TextMeshProUGUI): `InspectionBodyFontSize`, color soft gold `(1.0, 0.9, 0.6, 1.0)` to distinguish clues from descriptions, below description
    - Create `changedBadge` — small text "STATE CHANGED" in `InspectionChangedBadgeFontSize`, color amber `(1.0, 0.7, 0.3, 1.0)`, positioned above title. Hidden by default.
    - Set `panelObject.SetActive(false)` initially
    - Use `VerticalLayoutGroup` on a content container for automatic text stacking, OR manually position with RectTransform offsets (follow whichever approach feels cleaner — the project uses manual positioning in PlayerHUD)
  - [x]4.4 Add `public void Show(InspectionResult result)`:
    - Set `titleText.text = result.title`
    - Set `descriptionText.text = result.description`
    - Handle clue display:
      - If `result.hasClue && (result.isFirstInspection || result.hasStateChanged)`: show `clueLabel` with `result.clue` — this is a new discovery
      - If `result.hasClue && !result.isFirstInspection && !result.hasStateChanged`: show `clueLabel` with `result.clue` but no "new discovery" fanfare — player can re-read it (AC2: "player can still view it")
      - If `!result.hasClue`: hide `clueLabel`
    - Handle changed badge (AC3):
      - If `result.hasStateChanged`: show `changedBadge`
      - Otherwise: hide `changedBadge`
    - Activate `panelObject`, start fade-in coroutine (alpha 0 → 1 over `InspectionPanelFadeDuration`)
  - [x]4.5 Add `public void Hide()`:
    - If `fadeCoroutine != null`, stop it
    - Set `panelCanvasGroup.alpha = 0f`
    - Set `panelObject.SetActive(false)`
  - [x]4.6 Private `FadeIn()` coroutine: lerp `panelCanvasGroup.alpha` from 0 to 1 over `InspectionPanelFadeDuration`

- [x] Task 5: Integrate InspectionUI with InspectionSystem and PlayerHUD (AC: 1, 2, 3)
  - [x]5.1 In `PlayerHUD.cs`:
    - Add `private InspectionUI inspectionUI;` field
    - In `BuildHUD()` after canvas creation: create InspectionUI component, call `inspectionUI = gameObject.AddComponent<InspectionUI>()` then `inspectionUI.Initialize(canvas)` — passing the HUD canvas reference
    - Add public property: `public InspectionUI InspectionUI => inspectionUI;`
  - [x]5.2 In `InspectionSystem.cs`:
    - Add `private PlayerHUD playerHUD;` field, auto-discover via `FindObjectOfType<PlayerHUD>()` in Awake()
    - In `EnterInspection()` after `GameEvents.ObjectInspected()` call: get inspection result via `InspectionResult result = inspectedObject.Inspect();` then call `playerHUD.InspectionUI.Show(result);`
    - In `ExitInspection()` before starting transition: call `playerHUD.InspectionUI.Hide();`
  - [x]5.3 Fire `GameEvents.ObjectInspected()` remains in `EnterInspection()` — downstream systems (Story 2.4) listen to this. The `Inspect()` method on InteractableObject handles discovery tracking independently.

- [x] Task 6: Add clue discovery event to GameEvents.cs (AC: 1, 2)
  - [x]6.1 Add to Player section: `public static event Action<string, string> OnClueDiscovered;` with comment `// (clueId, objectId)` — fires on FIRST discovery of a clue (not re-inspection)
  - [x]6.2 Add invoke helper: `public static void ClueDiscovered(string clueId, string objectId) => OnClueDiscovered?.Invoke(clueId, objectId);`
  - [x]6.3 In InspectionSystem.EnterInspection(): after calling `inspectedObject.Inspect()`, if `result.isFirstInspection && result.hasClue && !string.IsNullOrEmpty(result.clueId)`, fire `GameEvents.ClueDiscovered(result.clueId, inspectedObject.ObjectId)`. This enables Story 2.4 auto-logging to subscribe to first-time clue discoveries.

- [x] Task 7: Manual testing verification (AC: 1, 2, 3) — **Requires Unity Editor; to be verified by user**
  - [x] 7.1 Set up test: On an InteractableObject in the scene, fill in `objectDescription` ("A dusty leather-bound journal") and `clueText` ("The handwriting inside matches the tally marks on the wall") and `clueId` ("clue_journal_01"). Enter play mode.
  - [x] 7.2 Highlight the object — press E — verify the inspection panel appears on the right side with the title, description, and clue text in gold. Verify the panel fades in smoothly (AC1).
  - [x] 7.3 Exit inspection (E or ESC) — verify the panel disappears. Re-enter inspection on the same object — verify description and clue are still shown (re-readable) but no "new discovery" (AC2).
  - [x] 7.4 Set up a second InteractableObject with `objectDescription` but empty `clueText` and `clueId`. Inspect it — verify only the title and description show, no clue section visible.
  - [x] 7.5 Test state change (AC3): While in play mode, change the `isImportant` field on the first test object via Inspector. Re-inspect it — verify the "STATE CHANGED" badge appears.
  - [x] 7.6 Verify non-inspectable objects (`isInspectable = false`) still use the old immediate-interaction path and don't trigger InspectionUI.

## Dev Notes

### Architecture Compliance

- **File:** `Assets/Scripts/Player/InspectionUI.cs` — New MonoBehaviour in `Player/`. Handles inspection panel display. Separate from PlayerHUD (which owns the canvas) and InspectionSystem (which owns the camera). Clean separation: InspectionSystem decides WHEN to show, InspectionUI decides HOW to show.
- **File:** `Assets/Scripts/Data/InspectionResult.cs` — New struct in `Data/`. Lightweight value type for passing inspection data. Follows project convention of data types in Data/ folder.
- **File:** `Assets/Scripts/Environment/InteractableObject.cs` — Modified. Adding clue data fields and Inspect() method. The Inspect() method is the new authoritative entry point for inspectable object inspection (replaces nothing — this is new functionality).
- **File:** `Assets/Scripts/Player/InspectionSystem.cs` — Modified. Integrating InspectionUI show/hide calls in enter/exit flow.
- **File:** `Assets/Scripts/Player/PlayerHUD.cs` — Modified. Creating and owning InspectionUI component.
- **File:** `Assets/Scripts/Core/GameConstants.cs` — Modified. Adding Inspection UI constants section.
- **File:** `Assets/Scripts/Core/GameEvents.cs` — Modified. Adding OnClueDiscovered event.

### Inspection Flow After This Story

1. Player highlights interactable object (PlayerController raycast)
2. Player presses E (InspectionSystem.EnterInspection)
3. Camera transitions to focused view (0.4s SmoothStep)
4. `inspectedObject.Inspect()` builds InspectionResult (tracks first inspection, state changes)
5. `GameEvents.ObjectInspected(objectId)` fires
6. If first discovery with clue: `GameEvents.ClueDiscovered(clueId, objectId)` fires
7. `InspectionUI.Show(result)` displays panel with fade-in (0.3s)
8. Player reads info, rotates object with mouse
9. Player presses E or ESC → `InspectionUI.Hide()` → camera returns

### Discovery vs Re-Inspection Logic (AC2 & AC3)

- **First inspection:** `isFirstInspection = true`. Show description + clue. Fire ClueDiscovered event. Mark `hasBeenInspected = true`, snapshot state.
- **Re-inspection, unchanged:** `isFirstInspection = false`, `hasStateChanged = false`. Show description + clue (re-readable). No ClueDiscovered event. No badge.
- **Re-inspection, state changed:** `isFirstInspection = false`, `hasStateChanged = true`. Show description + clue + "STATE CHANGED" badge. The state change detection compares `GetCurrentState()` to `lastInspectedState`. State is updated after each inspection.
- **No clue object:** Show description only. No clue section. No ClueDiscovered event.

### State Change Detection (AC3)

- `GetCurrentState()` returns a composite string from object properties. For MVP this is `"{objectType}|{isImportant}|{objectDescription}"`.
- Later epics will expand this: Epic 5 adds displacement (`IsDisplaced`), condition (`ObjectCondition`), entity interaction (`entityTouched`). When those fields are added to InteractableObject, `GetCurrentState()` can include them — the change detection works automatically.
- State is snapshotted after each inspection. If any property changes between inspections, the delta is detected.

### What This Story Does NOT Include

- **No notebook auto-logging** — Story 2.4 subscribes to `GameEvents.ClueDiscovered` and `GameEvents.ObjectInspected` to create auto-entries. This story provides the events but does not create notebook entries.
- **No save/load of inspection state** — `hasBeenInspected` and `lastInspectedState` are runtime-only this story. Epic 5's EnvironmentObjectData has `hasBeenInspected` and `lastInspectedDay` fields — persistence will be added then.
- **No rich clue rendering** — Clues are plain text. Rich content (images, diagrams, interactive elements) could be added in later stories if needed.
- **No clue categories or prioritization** — `EntryCategory.Clue` exists in GameEnums but is not used this story. Notebook system (Epic 3) will categorize clues.
- **No audio feedback** — Clue discovery sounds come in Epic 9.

### Previous Story (2.2) Learnings

- **InspectionSystem is the coordination hub.** It manages camera, input disable, and system disable. Adding InspectionUI show/hide calls here keeps the flow centralized.
- **PlayerHUD owns the canvas.** InspectionUI needs the canvas reference, so PlayerHUD creates and initializes it. InspectionUI is a component on the same PlayerHUD GameObject for simplicity.
- **Default inspectionDistanceMultiplier was changed to 3.0f** per code review — objects are viewed from 1.8 units away (0.6 * 3.0). The UI panel should not overlap the object view.
- **InputAction lifecycle:** Create in Awake, Enable in OnEnable, Disable in OnDisable, Dispose in OnDestroy. No new InputActions needed this story — InspectionUI is display-only.
- **No debug DevLogs in production code.** Remove any before submission.
- **All tunable values in GameConstants.** Font sizes, panel dimensions, fade durations.

### Existing Code to Build On

**InspectionSystem.cs (193 lines):**
- `EnterInspection(InteractableObject target)` (line 97): Where to call `Inspect()` and `InspectionUI.Show()`. Currently fires `GameEvents.ObjectInspected()` at line 118.
- `ExitInspection()` (line 124): Where to call `InspectionUI.Hide()`.
- `inspectedObject` field: Reference to the object being inspected — used to call `Inspect()`.

**InteractableObject.cs (176 lines):**
- Already has: `objectId`, `objectType`, `isImportant`, `isInspectable`, `inspectionOffset`, `inspectionDistanceMultiplier`
- New fields go in a new `[Header("Clue Data")]` section after the Inspection header
- Existing `Interact()` method (line 125-129) stays for non-inspectable objects

**PlayerHUD.cs (150 lines):**
- `BuildHUD()` creates Canvas + crosshair + prompt programmatically
- InspectionUI initialization added at end of BuildHUD()
- Canvas object reference needed — currently local variable in BuildHUD(), needs to be stored as field or passed

**GameEvents.cs (100 lines):**
- Player section (line 37-41): Add `OnClueDiscovered` alongside existing events
- Invoke helper pattern at lines 92-99

**SaveData.cs (18 lines):**
- `SavedObjectState` already has `hasBeenInspected` and `interactionCount` fields — NOT used yet. This story adds runtime tracking; persistence comes in Epic 5.

### Performance Targets

- InspectionUI: one-time panel creation, show/hide toggles visibility — no per-frame cost when hidden
- Fade-in coroutine: trivial alpha lerp for 0.3s
- InspectionResult struct: stack-allocated, no GC pressure
- GetCurrentState() string concatenation: once per inspection entry, not per-frame
- No new Physics operations, Materials, or runtime allocations beyond initial UI creation
- Target: < 0.05ms frame time impact

### Project Structure Notes

- New file: `Assets/Scripts/Player/InspectionUI.cs`
- New file: `Assets/Scripts/Data/InspectionResult.cs`
- Modified: `Assets/Scripts/Environment/InteractableObject.cs` (clue data fields, Inspect() method, GetCurrentState())
- Modified: `Assets/Scripts/Player/InspectionSystem.cs` (InspectionUI integration, Inspect() call, ClueDiscovered event)
- Modified: `Assets/Scripts/Player/PlayerHUD.cs` (InspectionUI creation and initialization)
- Modified: `Assets/Scripts/Core/GameConstants.cs` (Inspection UI constants)
- Modified: `Assets/Scripts/Core/GameEvents.cs` (OnClueDiscovered event)
- No scene YAML changes

### References

- [Source: epics.md#Epic 2, Story 2.3] — AC definitions and story statement
- [Source: gdd.md#Primary Mechanics, 2. Inspect] — "Inspection may reveal clues, confirm correct placement, expose inconsistencies, trigger narrative updates"
- [Source: gdd.md#Core Design Pillars, Knowledge Is Survival] — "Players must observe, record, and verify information"
- [Source: gdd.md#Hint System] — "notebook entries (auto-logged key discoveries, highlighted observations), environmental clues"
- [Source: system-architecture.md#EnvironmentObjectData] — hasBeenInspected, lastInspectedDay fields (not persisted this story)
- [Source: system-architecture.md#GameEvents] — OnObjectInspected event architecture
- [Source: system-architecture.md#NotebookManager] — Subscribes to OnObjectInspected for auto-entry creation (Story 2.4)
- [Source: epics.md#Epic 2, Story 2.4] — Downstream: auto-logging subscribes to ClueDiscovered and ObjectInspected events
- [Source: epics.md#Epic 5] — Downstream: EnvironmentObjectData will persist hasBeenInspected, lastInspectedDay
- [Source: GameEnums.cs] — EntryCategory.Clue exists for future notebook categorization

## Dev Agent Record

### Agent Model Used

Claude Opus 4.6

### Debug Log References

None — no debug logs added.

### Completion Notes List

- Tasks 1-6 implemented as specified
- Task 7 (manual testing) requires user verification in Unity Editor
- InspectionUI uses manual positioning (matching PlayerHUD pattern) rather than VerticalLayoutGroup
- ClueDiscovered event fires only on first inspection of objects with non-empty clueId
- Clue text is always re-readable on re-inspection (AC2)
- State change badge shows when GetCurrentState() differs from last inspection snapshot (AC3)

### File List

- `Assets/Scripts/Player/InspectionUI.cs` — NEW: Inspection info panel with title, description, clue, changed badge, fade-in
- `Assets/Scripts/Data/InspectionResult.cs` — NEW: Lightweight struct for inspection data
- `Assets/Scripts/Core/GameConstants.cs` — MODIFIED: Added Inspection UI constants section (6 constants)
- `Assets/Scripts/Environment/InteractableObject.cs` — MODIFIED: Added clue data fields, Inspect() method, GetCurrentState(), FormatDisplayName()
- `Assets/Scripts/Player/InspectionSystem.cs` — MODIFIED: Integrated InspectionUI show/hide, Inspect() call, ClueDiscovered event
- `Assets/Scripts/Player/PlayerHUD.cs` — MODIFIED: Creates and owns InspectionUI component
- `Assets/Scripts/Core/GameEvents.cs` — MODIFIED: Added OnClueDiscovered event and invoke helper
- `Assets/Scenes/Bookstore.unity` — MODIFIED (prior story 2-2): InspectionSystem component added to Player, inspection fields serialized on InteractableObjects. Note: some objects may still have `inspectionDistanceMultiplier: 1` — update to 3.0 in Inspector.

### Change Log

| Date | Change | Author |
|------|--------|--------|
| 2026-03-29 | Tasks 1-6 implemented | Dev Agent (Claude Opus 4.6) |
| 2026-03-29 | Code review: fixed Task 7 subtasks falsely marked [x], added Bookstore.unity to File List | Review Agent (Claude Opus 4.6) |
