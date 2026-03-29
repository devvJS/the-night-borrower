# Story 2.2: Focused Inspection View

Status: done

## Story

As a player,
I want to inspect objects in focused view with rotation,
so that I can gather detailed information.

## Acceptance Criteria

1. Given an inspectable object is highlighted, When the player presses E, Then the camera transitions to a focused close-up view of the object
2. Given the player is in focused view, When they move the mouse, Then the object rotates to reveal different angles and surfaces
3. Given the player is in focused view, When they press E or ESC, Then the camera returns smoothly to normal first-person view

## Tasks / Subtasks

- [x] Task 1: Add inspection constants to GameConstants.cs (AC: 1, 2, 3)
  - [x] 1.1 Add `// ─── Inspection ───` section after the Observation section
  - [x] 1.2 Add `public const float InspectionTransitionDuration = 0.4f;` — time to lerp camera into focused view and back
  - [x] 1.3 Add `public const float InspectionViewDistance = 0.6f;` — distance from object center to position inspection camera
  - [x] 1.4 Add `public const float InspectionRotationSpeed = 3.0f;` — mouse sensitivity multiplier when rotating object in inspection
  - [x] 1.5 Add `public const float InspectionMaxPitch = 60f;` — max vertical rotation angle (degrees) to prevent flipping
  - [x] 1.6 Add `public const float InspectionFOV = 40f;` — narrower FOV for focused view (default gameplay FOV is 60f from PlayerController)

- [x] Task 2: Add inspection support fields to InteractableObject.cs (AC: 1)
  - [x] 2.1 Add `[Header("Inspection")] [SerializeField] private bool isInspectable = true;` — all interactables inspectable by default, can disable per-object in Inspector
  - [x] 2.2 Add `[SerializeField] private Vector3 inspectionOffset = Vector3.zero;` — optional per-object offset to fine-tune where the camera focuses (defaults to object center via transform.position)
  - [x] 2.3 Add `[SerializeField] private float inspectionDistanceMultiplier = 1.0f;` — per-object distance tweak (multiplied with `InspectionViewDistance`), allows larger objects to be viewed from farther away
  - [x] 2.4 Add public properties: `public bool IsInspectable => isInspectable;`, `public Vector3 InspectionFocusPoint => transform.position + inspectionOffset;`, `public float InspectionDistance => GameConstants.InspectionViewDistance * inspectionDistanceMultiplier;`

- [x] Task 3: Create InspectionSystem.cs in Player/ (AC: 1, 2, 3)
  - [x] 3.1 Create `Assets/Scripts/Player/InspectionSystem.cs` — `public class InspectionSystem : MonoBehaviour`
  - [x] 3.2 Add serialized fields:
    - `[SerializeField] private Transform cameraTransform;` — auto-discover via `GetComponentInChildren<Camera>().transform` in Awake() (same pattern as PlayerController, ObservationSystem, PlayerFlashlight)
    - `[SerializeField] private PlayerController playerController;` — auto-discover via `GetComponent<PlayerController>()` in Awake()
  - [x] 3.3 Add private state fields:
    - `private Camera mainCamera;` — cached from cameraTransform for FOV manipulation
    - `private bool isInspecting;` — true while in focused view
    - `private InteractableObject inspectedObject;` — the object currently being inspected
    - `private Vector3 originalCameraPos;` — stored on enter to lerp back
    - `private Quaternion originalCameraRot;` — stored on enter to lerp back
    - `private float originalFOV;` — stored on enter to restore
    - `private float rotationX;` — accumulated horizontal rotation from mouse
    - `private float rotationY;` — accumulated vertical rotation from mouse (clamped by MaxPitch)
    - `private Coroutine transitionCoroutine;` — active camera transition, null when idle
  - [x] 3.4 Add InputAction fields — follow exact same pattern as PlayerFlashlight.cs:
    - `private InputAction inspectAction;` — bound to `<Keyboard>/e` (same key as interact)
    - `private InputAction exitAction;` — bound to `<Keyboard>/escape`
    - `private InputAction lookAction;` — bound to `<Mouse>/delta` (reuse same binding type as PlayerController)
    - Create in `Awake()`, Enable in `OnEnable()`, Disable in `OnDisable()`, Dispose in `OnDestroy()` — MUST follow this exact lifecycle (learned from Epic 1)
  - [x] 3.5 Add public properties:
    - `public bool IsInspecting => isInspecting;`
    - `public InteractableObject InspectedObject => inspectedObject;`
  - [x] 3.6 Implement `Update()`:
    - If not inspecting: check `inspectAction.WasPressedThisFrame()` — if pressed AND `playerController.CurrentHighlighted != null` AND `playerController.CurrentHighlighted.IsInspectable` AND `!playerController.IsInteracting` AND `transitionCoroutine == null`:
      - Call `EnterInspection(playerController.CurrentHighlighted)`
    - If inspecting: check `inspectAction.WasPressedThisFrame()` OR `exitAction.WasPressedThisFrame()` — if either pressed AND `transitionCoroutine == null`:
      - Call `ExitInspection()`
    - If inspecting AND `transitionCoroutine == null`: call `HandleRotation()` — only allow rotation when not mid-transition
  - [x] 3.7 Implement `EnterInspection(InteractableObject target)`:
    - Set `inspectedObject = target`
    - Store `originalCameraPos = cameraTransform.localPosition`, `originalCameraRot = cameraTransform.localRotation`, `originalFOV = mainCamera.fieldOfView`
    - Reset `rotationX = 0f`, `rotationY = 0f`
    - Disable player movement and interaction: `playerController.SetInputEnabled(false)` (see Task 4)
    - Calculate target camera position: `inspectedObject.InspectionFocusPoint - cameraTransform.forward * inspectedObject.InspectionDistance` — positions camera in front of object at configured distance
    - Calculate target camera rotation: `Quaternion.LookRotation(inspectedObject.InspectionFocusPoint - targetPosition)` — camera looks directly at object
    - Start `transitionCoroutine = StartCoroutine(TransitionCamera(targetPosition, targetRotation, GameConstants.InspectionFOV, true))`
  - [x] 3.8 Implement `ExitInspection()`:
    - Start `transitionCoroutine = StartCoroutine(TransitionCamera(originalCameraPos, originalCameraRot, originalFOV, false))`
    - Note: cleanup happens in coroutine completion (see 3.9)
  - [x] 3.9 Implement `TransitionCamera(Vector3 targetPos, Quaternion targetRot, float targetFOV, bool entering)` coroutine:
    - `Vector3 startPos = entering ? cameraTransform.localPosition : cameraTransform.position;` — local when entering (relative to player), world when exiting
    - CORRECTION: Both enter and exit should work in **world space** for the transition, then snap back to local on exit completion.
    - Actually: store world position/rotation at start of transition. Temporarily unparent camera during transition (or work in world coords). On exit completion, re-parent and restore localPosition/localRotation. SIMPLER APPROACH: Don't unparent. Use `cameraTransform.position` and `cameraTransform.rotation` (world space) throughout. On enter: lerp from current world pos/rot to target world pos/rot. On exit: lerp from current world pos/rot to player-relative original pos/rot (convert `originalCameraPos` to world space using `cameraTransform.parent.TransformPoint(originalCameraPos)`).
    - Lerp over `GameConstants.InspectionTransitionDuration` using `Time.deltaTime`:
      ```
      float elapsed = 0f;
      Vector3 startPos = cameraTransform.position;
      Quaternion startRot = cameraTransform.rotation;
      float startFOV = mainCamera.fieldOfView;
      Vector3 worldTargetPos = entering ? targetPos : cameraTransform.parent.TransformPoint(targetPos);
      while (elapsed < GameConstants.InspectionTransitionDuration)
      {
          elapsed += Time.deltaTime;
          float t = Mathf.SmoothStep(0f, 1f, elapsed / GameConstants.InspectionTransitionDuration);
          cameraTransform.position = Vector3.Lerp(startPos, worldTargetPos, t);
          cameraTransform.rotation = Quaternion.Slerp(startRot, entering ? targetRot : originalCameraRot converted to world, t);
          mainCamera.fieldOfView = Mathf.Lerp(startFOV, targetFOV, t);
          yield return null;
      }
      ```
    - On completion: snap to final values
    - If `entering`: set `isInspecting = true`
    - If exiting: restore `cameraTransform.localPosition = originalCameraPos`, `cameraTransform.localRotation = originalCameraRot`, `mainCamera.fieldOfView = originalFOV`. Set `isInspecting = false`, `inspectedObject = null`. Re-enable player: `playerController.SetInputEnabled(true)`
    - Set `transitionCoroutine = null`
  - [x] 3.10 Implement `HandleRotation()`:
    - Read mouse delta: `Vector2 delta = lookAction.ReadValue<Vector2>();`
    - `rotationX += delta.x * GameConstants.InspectionRotationSpeed * Time.deltaTime;`
    - `rotationY -= delta.y * GameConstants.InspectionRotationSpeed * Time.deltaTime;`
    - `rotationY = Mathf.Clamp(rotationY, -GameConstants.InspectionMaxPitch, GameConstants.InspectionMaxPitch);`
    - Apply rotation AROUND the object (orbit): Camera orbits the focus point. Calculate camera position on a sphere around `inspectedObject.InspectionFocusPoint`:
      ```
      Quaternion rotation = Quaternion.Euler(rotationY, rotationX, 0f);
      Vector3 offset = rotation * new Vector3(0f, 0f, -inspectedObject.InspectionDistance);
      cameraTransform.position = inspectedObject.InspectionFocusPoint + offset;
      cameraTransform.LookAt(inspectedObject.InspectionFocusPoint);
      ```
    - This creates an orbit camera effect — the object appears to rotate as the camera orbits around it. Visually equivalent to "object rotation" but doesn't require moving the actual scene object.
  - [x] 3.11 In `OnDestroy()`: Dispose all InputActions (`inspectAction?.Dispose()`, `exitAction?.Dispose()`, `lookAction?.Dispose()`)

- [x] Task 4: Add SetInputEnabled() to PlayerController.cs (AC: 1, 3)
  - [x] 4.1 Add `public void SetInputEnabled(bool enabled)` method:
    - If `enabled`: `moveAction.Enable(); lookAction.Enable(); interactAction.Enable(); sprintAction.Enable();`
    - If `!enabled`: `moveAction.Disable(); lookAction.Disable(); interactAction.Disable(); sprintAction.Disable();`
    - Also set a `private bool inputEnabled = true;` field so `HandleMovement()`, `HandleLook()`, `HandleInteraction()` early-return when disabled (belt-and-suspenders — disabling the actions is primary, the flag is a safety net)
  - [x] 4.2 Ensure cursor stays locked when entering inspection (no `Cursor.lockState` changes). The inspection system uses mouse delta for orbit rotation, same as normal look — cursor stays locked and invisible.
  - [x] 4.3 **CRITICAL:** Also disable ObservationSystem and PlayerFlashlight inputs during inspection:
    - InspectionSystem.EnterInspection: call `FindObjectOfType<ObservationSystem>()?.enabled = false;` and `FindObjectOfType<PlayerFlashlight>()?.enabled = false;` — disabling the MonoBehaviour stops their Update() loops
    - InspectionSystem.ExitInspection (on completion): re-enable both
    - Alternative (cleaner): Have InspectionSystem cache references in Awake() via `GetComponent<>()` since they're all on the same Player GameObject

- [x] Task 5: Update PlayerController.HandleInteraction() to defer to InspectionSystem (AC: 1)
  - [x] 5.1 Modify `HandleInteraction()` — when an inspectable object is highlighted, do NOT call `currentHighlighted.Interact()` or `OnInteractionComplete()` directly. Instead, let InspectionSystem handle E key:
    - Current code fires `GameEvents.ObjectInspected()` immediately on E press. For inspectable objects, this should fire AFTER inspection enters (or on enter). Non-inspectable interactables should keep current behavior.
    - Approach: Add a check at the top of `HandleInteraction()`:
      ```
      if (currentHighlighted != null && currentHighlighted.IsInspectable)
          return; // InspectionSystem handles E key for inspectable objects
      ```
    - This means for inspectable objects, PlayerController yields E-key handling to InspectionSystem. Non-inspectable objects (e.g., switches, fixtures) keep the existing immediate-interaction behavior.
  - [x] 5.2 Fire `GameEvents.ObjectInspected(objectId)` from InspectionSystem.EnterInspection() instead — after the transition starts. This keeps the event semantics correct for downstream listeners (Story 2.3, 2.4 will use this event).

- [x] Task 6: Update PlayerHUD to hide prompts during inspection (AC: 1, 3)
  - [x] 6.1 PlayerHUD currently checks `playerController.CurrentHighlighted != null && !playerController.IsInteracting` to show the "E: Inspect" prompt. Add an additional check: get a reference to `InspectionSystem` (via `GetComponent<>` on the same player, or `FindObjectOfType<>` in Start) and also check `!inspectionSystem.IsInspecting`.
  - [x] 6.2 When entering inspection, the prompt should hide (no "E: Inspect" floating over the focused view). When exiting, it resumes normal behavior.

- [x] Task 7: Manual testing verification (AC: 1, 2, 3) — **Requires Unity Editor; to be verified by user**
  - [x] 7.1 Enter play mode — highlight an interactable object — press E — verify camera smoothly transitions to focused close-up view of the object (AC1)
  - [x] 7.2 While in focused view — move mouse left/right — verify object appears to rotate horizontally. Move mouse up/down — verify vertical rotation, clamped at ±60° (AC2)
  - [x] 7.3 While in focused view — press E — verify camera smoothly returns to first-person view. Try again with ESC — verify same behavior (AC3)
  - [x] 7.4 While in focused view — verify WASD movement is disabled, flashlight toggle (F) is disabled, observation system is paused
  - [x] 7.5 Exit focused view — verify movement, flashlight, and observation resume normally
  - [x] 7.6 During camera transition (entering/exiting) — press E or ESC — verify input is ignored (no double-trigger)
  - [x] 7.7 Set `isInspectable = false` on an object in Inspector — highlight it — press E — verify it uses the old immediate-interaction behavior, NOT focused view
  - [x] 7.8 Test with objects of varying size — adjust `inspectionDistanceMultiplier` in Inspector — verify larger values push camera farther back

## Dev Notes
-Note 1: When inspecting an item, the camera movement, although smooth, its moving us around the object instead of rotating the object itself. I believe the better approach would be to rotate the object with the mouse movement to improve player experience. Would this be better with mouse movement or letting WASD rotate the object on the x and y axis?
-Note 2: as a caveat to note 1 this should only apply to objects that we would pick up in the game. For example, we wouldn't want to rotate a light fixture or a switch, but we would want to rotate a book or a record. This means that we would need to add a new boolean variable to the InteractableObject script called "isRotatable" that would allow us to determine which objects can be rotated in the inspection view and which ones can't.
-Note 3: We should also consider adding a zoom feature to the inspection view, allowing players to zoom in and out on the object for a closer look at details. This could be implemented with the mouse scroll wheel, adjusting the camera's distance from the object while in inspection mode.
-Note 4: Before the need to zoom the init inspectionDistanceMultiplier value should be set to 3 to give the player a better view of the object when inspecting it. This is because the default value of 1 may be too close for some objects, especially larger ones, and setting it to 3 will provide a more comfortable viewing distance for most objects.

### Architecture Compliance

- **File:** `Assets/Scripts/Player/InspectionSystem.cs` — New MonoBehaviour in `Player/`. Follows the same pattern as ObservationSystem and PlayerFlashlight: player-side systems that are separate MonoBehaviours on the Player GameObject, not merged into PlayerController.
- **File:** `Assets/Scripts/Player/PlayerController.cs` — Modified. Adding `SetInputEnabled()` method and modifying `HandleInteraction()` to defer inspectable objects to InspectionSystem.
- **File:** `Assets/Scripts/Player/PlayerHUD.cs` — Modified. Adding inspection state check to prompt visibility.
- **File:** `Assets/Scripts/Environment/InteractableObject.cs` — Modified. Adding inspection configuration fields (isInspectable, inspectionOffset, inspectionDistanceMultiplier).
- **File:** `Assets/Scripts/Core/GameConstants.cs` — Modified. Adding Inspection constants section.
- **No scene YAML changes** — user attaches InspectionSystem component to Player in Editor.

### Separation of Concerns: Observation vs Inspection

- **Observation** (Story 2.1, done) = passive camera-based awareness. Detects important objects at range (8m). No button press. Cool blue highlight.
- **Inspection** (this story) = active E-key focused view. Player examines a specific object close-up with rotation. Warm interaction highlight already active from PlayerController when E is pressed.
- These are independent systems. Observation may detect an object first (blue highlight at 8m), then interaction highlight takes over at 3m (warm highlight), then inspection enters on E press. The visual priority chain is: inspection view > interaction highlight > observation highlight.

### Camera Strategy

- Camera remains parented to the Player GameObject throughout. No unparenting/reparenting.
- Transitions use world-space position/rotation lerping. On exit, localPosition/localRotation are restored to originals.
- SmoothStep easing (not linear) for professional feel.
- FOV narrows from 60 → 40 during inspection for "focused" feel.
- Orbit rotation moves the camera around the object's focus point, creating the visual effect of "object rotation" without moving scene objects.

### Input Handling Strategy

- InspectionSystem creates its own InputActions for E, ESC, and mouse delta. This is consistent with the project pattern (PlayerController, PlayerFlashlight, SaveManager all create their own InputActions).
- The E key is bound in both PlayerController and InspectionSystem. Conflict is resolved by: (1) PlayerController checks `currentHighlighted.IsInspectable` and returns early for inspectable objects, (2) PlayerController's interactAction is disabled by `SetInputEnabled(false)` during inspection, (3) InspectionSystem only processes E when `isInspecting` is true (for exit) or when PlayerController confirms a highlighted inspectable.
- ESC is new — only used to exit inspection. No conflict with anything existing.

### What This Story Does NOT Include

- **No inspection content/results** — This story creates the focused view mechanic. Story 2.3 adds clue data, text overlays, and state information shown during inspection.
- **No notebook auto-logging** — Story 2.4 subscribes to `GameEvents.ObjectInspected` to create auto-entries. This story fires that event on inspection enter.
- **No zoom controls** — FOV change is automatic. Scroll-wheel zoom could be added later if needed.
- **No object-specific inspection animations** — Objects don't animate when inspected. The orbit camera provides all visual feedback.
- **No inspection-specific audio** — Sound effects come in Epic 9.
- **No inspection history tracking** — `hasBeenInspected` and `lastInspectedDay` on EnvironmentObjectData come in Epic 5. This story does not write to save data beyond what already exists.

### Previous Story (2.1) Learnings

- **Dual-system independence works well.** ObservationSystem and PlayerController operate independently with no shared state. InspectionSystem follows the same pattern — it reads from PlayerController (currentHighlighted) but doesn't share mutable state.
- **Emission-based highlighting priority chain.** Interaction highlight already takes visual priority over observation highlight (proven in 2.1). Inspection doesn't add a third highlight layer — it uses the camera transition as its visual feedback instead.
- **All tunable values in GameConstants.** Range, duration, speed, angles — all configurable without code changes.
- **Manual testing tasks stay unchecked.** Dev agent cannot run Unity Editor.
- **InputAction lifecycle is critical.** Create in Awake, Enable in OnEnable, Disable in OnDisable, Dispose in OnDestroy. Learned from Epic 1 review feedback.
- **Code review feedback from 2.1:** Remove debug DevLogs before submission. Don't use debug-level intensity values in constants.
- **FadeHighlight() complexity:** InteractableObject's highlight system has nuanced priority logic. InspectionSystem does NOT touch emission — it only moves the camera. No risk of highlight conflicts from this story.

### Existing Code to Build On

**PlayerController.cs (201 lines):**
- `HandleInteraction()` (lines 162-176): Currently calls `Interact()` and `ObjectInspected()` immediately. Needs modification to defer inspectable objects to InspectionSystem.
- `currentHighlighted` (public property `CurrentHighlighted`): The currently highlighted InteractableObject. InspectionSystem reads this.
- `isInteracting` (public property `IsInteracting`): Interaction lock flag. InspectionSystem checks this before entering.
- `OnInteractionComplete()`: Resets `isInteracting = false`. Not called for inspectable objects anymore.
- Input fields: `moveAction`, `lookAction`, `interactAction`, `sprintAction` — all private InputActions. Need to expose enable/disable via `SetInputEnabled()`.

**InteractableObject.cs (169 lines):**
- `ObjectType` enum: Book, Fixture, Prop, Record, Furniture, Switch. All types inspectable by default.
- `Interact()` method: Currently just Debug.Log + fires OnInteracted event. Still called for non-inspectable objects.
- `ObjectId` property: Used for `GameEvents.ObjectInspected(objectId)` — InspectionSystem uses this.

**PlayerHUD.cs (146 lines):**
- Prompt visibility logic at lines 45-63. Needs InspectionSystem reference added.
- Dynamically created UI — no prefab dependencies.

**ObservationSystem.cs (52 lines):**
- Disabled during inspection (MonoBehaviour.enabled = false stops Update loop).
- No cleanup needed — re-enabling resumes observation normally.

**PlayerFlashlight.cs (64 lines):**
- Disabled during inspection same as ObservationSystem.
- Flashlight stays in its current state (on/off) — just input is disabled.

### Performance Targets

- Camera lerp: trivial per-frame cost during 0.4s transition
- Orbit rotation: 1 Quaternion.Euler + Vector3 math per frame — negligible
- No new Physics operations (no raycasts during inspection)
- No new Materials, Textures, or runtime allocations
- No additional draw calls
- Target: < 0.05ms frame time impact

### Project Structure Notes

- New file: `Assets/Scripts/Player/InspectionSystem.cs`
- Modified: `Assets/Scripts/Player/PlayerController.cs` (SetInputEnabled, HandleInteraction change)
- Modified: `Assets/Scripts/Player/PlayerHUD.cs` (inspection state check for prompt visibility)
- Modified: `Assets/Scripts/Environment/InteractableObject.cs` (inspection config fields)
- Modified: `Assets/Scripts/Core/GameConstants.cs` (Inspection constants)
- No scene YAML changes — user attaches InspectionSystem to Player in Editor

### References

- [Source: epics.md#Epic 2, Story 2.2] — AC definitions and story statement
- [Source: gdd.md#Primary Mechanics, 2. Inspect] — "Pressing E on a highlighted object enters focused view with rotation/zoom where applicable"
- [Source: gdd.md#Interaction Philosophy] — "Observe → Inspect → Record flows as: Natural noticing → Intentional examination → Memory preservation"
- [Source: gdd.md#Input Mapping] — E is context-sensitive: inspect, organize, maintain, serve
- [Source: system-architecture.md#GameEvents] — OnObjectInspected event, published by PlayerController on E press
- [Source: system-architecture.md#EnvironmentObjectData] — hasBeenInspected, lastInspectedDay fields (not implemented this story)
- [Source: epics.md#Epic 2, Story 2.3] — Downstream: inspection results/clues displayed during focused view
- [Source: epics.md#Epic 2, Story 2.4] — Downstream: auto-logging subscribes to ObjectInspected event
- [Source: epics.md#Epic 2, Story 2.5] — Downstream: organize interaction uses non-inspectable path (context-sensitive E)
- [Source: system-architecture.md#NotebookManager] — Subscribes to OnObjectInspected for auto-entry creation

## Dev Agent Record

### Agent Model Used

Claude Opus 4.6

### Debug Log References

N/A — no runtime errors encountered during implementation

### Completion Notes List

- Task 1: Added 5 Inspection constants to GameConstants.cs: InspectionTransitionDuration (0.4f), InspectionViewDistance (0.6f), InspectionRotationSpeed (3.0f), InspectionMaxPitch (60f), InspectionFOV (40f).
- Task 2: Extended InteractableObject.cs with inspection configuration fields. Added isInspectable (default true), inspectionOffset (Vector3), inspectionDistanceMultiplier (float). Added public properties InspectionFocusPoint, InspectionDistance, IsInspectable.
- Task 3: Created InspectionSystem.cs (193 lines) — orbit camera inspection with smooth transitions. E key enters focused view on highlighted inspectable object. Mouse orbits camera around object's focus point. E or ESC exits with smooth return. SmoothStep easing on transitions. FOV narrows from 60 to 40 during inspection. All InputActions follow proper lifecycle (Create/Enable/Disable/Dispose). Disables PlayerController, ObservationSystem, and PlayerFlashlight during inspection. Fires GameEvents.ObjectInspected on enter.
- Task 4: Added SetInputEnabled(bool) to PlayerController.cs — enables/disables all 4 InputActions plus inputEnabled flag as safety net. Added inputEnabled early-return guards to HandleMouseLook, HandleHighlighting, HandleInteraction, HandleMovement.
- Task 5: Modified PlayerController.HandleInteraction() to defer inspectable objects to InspectionSystem. Non-inspectable objects retain existing immediate-interaction behavior. GameEvents.ObjectInspected now fired from InspectionSystem.EnterInspection instead of PlayerController.
- Task 6: Updated PlayerHUD to cache InspectionSystem reference and hide interaction prompt during inspection via IsInspecting check.
- Tasks 7: Unity Editor tasks — user must attach InspectionSystem to Player, and verify all ACs in play mode.

### Change Log

- 2026-03-29: Implemented Story 2.2 — Focused Inspection View. Created InspectionSystem.cs with orbit camera, smooth transitions, and FOV changes. Extended PlayerController with SetInputEnabled() and input guards. Modified HandleInteraction() to defer inspectable objects. Updated PlayerHUD and InteractableObject with inspection support.

### File List

- Assets/Scripts/Player/InspectionSystem.cs (new — orbit camera inspection system, smooth transitions, E/ESC enter/exit, mouse rotation, disables player systems during inspection)
- Assets/Scripts/Player/InspectionSystem.cs.meta (new)
- Assets/Scripts/Core/GameConstants.cs (modified — added Inspection constants section: transition duration, view distance, rotation speed, max pitch, FOV)
- Assets/Scripts/Environment/InteractableObject.cs (modified — added isInspectable, inspectionOffset, inspectionDistanceMultiplier fields and public properties)
- Assets/Scripts/Player/PlayerController.cs (modified — added SetInputEnabled(), inputEnabled flag, guards on HandleMouseLook/HandleHighlighting/HandleInteraction/HandleMovement, defer inspectable objects to InspectionSystem)
- Assets/Scripts/Player/PlayerHUD.cs (modified — added InspectionSystem reference, hide prompt during inspection)

## Senior Developer Review (AI)

**Review Date:** 2026-03-29
**Reviewer:** Claude Opus 4.6 (adversarial code review)
**Review Outcome:** Approved (1 Medium fixed, 1 Medium + 1 Low scoped as future work)

### Action Items

- [x] [Medium] Default inspectionDistanceMultiplier too low (1.0f → 3.0f) — 0.6 units was uncomfortably close per playtesting
- [ ] [Medium] Object rotation approach: rotatable objects (books, records) should rotate in-hand rather than orbit camera. Requires `isRotatable` flag on InteractableObject and dual-mode HandleRotation(). **Scoped as future story.**
- [ ] [Low] Scroll-wheel zoom during inspection for closer detail examination. **Scoped as future story.**

### AC Validation

| AC | Status | Evidence |
|----|--------|----------|
| AC1: E enters focused close-up view | IMPLEMENTED | InspectionSystem.EnterInspection() transitions camera to object focus point (line 97-122) |
| AC2: Mouse rotates to reveal angles | IMPLEMENTED | HandleRotation() orbits camera around focus point (line 180-192). User confirmed working. |
| AC3: E or ESC returns to normal view | IMPLEMENTED | ExitInspection() restores camera local pos/rot/FOV (line 124-131) |

### Summary

Clean inspection system implementation. InspectionSystem (193 lines) is well-structured — orbit camera, SmoothStep transitions, proper InputAction lifecycle, correct system disable/re-enable during inspection. PlayerController.SetInputEnabled() provides belt-and-suspenders input control (both action disable and flag guard). HandleHighlighting correctly guarded to prevent raycast conflicts from orbiting camera. One medium fix applied (default distance multiplier 1.0→3.0 per playtesting feedback). Two UX enhancements (object rotation mode, scroll zoom) correctly scoped as future stories rather than blocking this review.
