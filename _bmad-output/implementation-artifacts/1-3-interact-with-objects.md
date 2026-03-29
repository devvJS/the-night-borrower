# Story 1.3: Interact with Objects

Status: done

## Story

As a player,
I want to press E to interact with highlighted objects,
so that I can engage with the environment.

## Acceptance Criteria

1. Given an object is highlighted, When the player presses E, Then the appropriate context-sensitive interaction fires (inspect, pick up, toggle, etc.)
2. Given no object is highlighted, When the player presses E, Then nothing happens and no error occurs
3. Given an interaction is in progress, When the player presses E again, Then the input is ignored until the current interaction completes

## Tasks / Subtasks

- [x] Task 1: Add interact InputAction to PlayerController.cs (AC: 1, 2)
  - [x] 1.1 Add `private InputAction interactAction` field alongside existing moveAction/lookAction/sprintAction
  - [x] 1.2 In `SetupInputActions()`, create: `interactAction = new InputAction("Interact", InputActionType.Button, "<Keyboard>/e")`
  - [x] 1.3 Enable/Disable in `OnEnable()`/`OnDisable()` matching existing pattern
  - [x] 1.4 Dispose in `OnDestroy()` matching existing pattern — CRITICAL: Story 1.1 review caught missing disposal as a leak
  - [x] 1.5 In `ReadInput()` or `Update()`, detect E press via `interactAction.WasPressedThisFrame()` (NOT `IsPressed()` — interaction is a single press, not hold) — used in HandleInteraction() directly
- [x] Task 2: Add interaction state tracking to PlayerController.cs (AC: 3)
  - [x] 2.1 Add `private bool isInteracting` field to prevent re-interaction during active interaction
  - [x] 2.2 Add `public bool IsInteracting => isInteracting` read-only property for UI queries (Story 1.6 will need this)
  - [x] 2.3 Guard interaction: if `isInteracting` is true, ignore E press entirely
- [x] Task 3: Implement HandleInteraction() in PlayerController.cs (AC: 1, 2, 3)
  - [x] 3.1 Add `HandleInteraction()` method, called in `Update()` AFTER `HandleHighlighting()` (highlighting must resolve first)
  - [x] 3.2 Check: if E not pressed this frame → return early
  - [x] 3.3 Check: if `isInteracting` → return early (AC3)
  - [x] 3.4 Check: if `currentHighlighted == null` → return early (AC2 — no highlighted object, nothing happens)
  - [x] 3.5 Set `isInteracting = true`
  - [x] 3.6 Call `currentHighlighted.Interact()` to trigger the object's interaction behavior
  - [x] 3.7 Publish `GameEvents.ObjectInspected(currentHighlighted.ObjectId)` — architecture spec requires PlayerController to publish this event
- [x] Task 4: Add Interact() method and callback to InteractableObject.cs (AC: 1)
  - [x] 4.1 Add `public event System.Action<InteractableObject> OnInteracted` event for external listeners
  - [x] 4.2 Add `public void Interact()` method that invokes the OnInteracted event
  - [x] 4.3 Interact() should log to console: `Debug.Log($"Interacted with {objectId} ({objectType})")` — placeholder until future stories add real behavior
  - [x] 4.4 Add `public void CompleteInteraction()` method that the interaction consumer calls when done — NOT added as separate method; completion is handled by PlayerController.OnInteractionComplete() directly. InteractableObject doesn't need to know about PlayerController's state.
- [x] Task 5: Wire up interaction completion in PlayerController.cs (AC: 3)
  - [x] 5.1 Add `public void OnInteractionComplete()` method that sets `isInteracting = false`
  - [x] 5.2 For now (placeholder): call `OnInteractionComplete()` immediately after `Interact()` since no real interaction flow exists yet — this means AC3's guard will effectively never block in this story, but the mechanism is in place
  - [x] 5.3 Add a comment noting that future stories (2.2 Focused Inspection, 2.5 Organization) will call `OnInteractionComplete()` when their interaction flow finishes
- [x] Task 6: Update Bookstore scene serialized data (AC: 1)
  - [x] 6.1 No new serialized fields added — interactionRange and interactableLayer were added in Story 1.2. No scene changes needed.
- [ ] Task 7: Manual testing verification (AC: 1, 2, 3) — **Requires Unity Editor; to be verified by user**
  - [x] 7.1 Enter play mode, look at an interactable object, press E — confirm console log "Interacted with {id} ({type})"
  - [x] 7.2 Look at empty space (no highlight), press E — confirm nothing happens, no errors in console
  - [x] 7.3 Look at non-interactable object (Crate_Decor, walls), press E — confirm nothing happens
  - [x] 7.4 Verify highlighting still works correctly after interaction (no regression from Story 1.2)
  - [x] 7.5 Rapid-press E on a highlighted object — confirm no double-interaction or errors

## Dev Notes
- in 7.1, the console log confirms that the Interact() method on the InteractableObject is firing correctly and the event is being published. One note that is relevent. When rapid firing E it fires the event every single time, no errors but registering the event every press of E
### Architecture Compliance

- **File:** `Assets/Scripts/Player/PlayerController.cs` — Existing file. Architecture spec says PlayerController handles "Movement + interaction". Add interaction logic here. Do NOT create a separate InteractionManager or InteractionSystem.
- **File:** `Assets/Scripts/Environment/InteractableObject.cs` — Existing file. Add `Interact()` method and completion callback. Keep it simple — real interaction behaviors come in later stories.
- **Event:** `GameEvents.ObjectInspected(objectId)` — Architecture spec Section 2 event table says PlayerController publishes this "when player presses E on inspectable." The event and invoke helper already exist in `GameEvents.cs` — just call `GameEvents.ObjectInspected(objectId)`.

### Previous Story (1.2) Learnings

From the Story 1.2 code review:
- **Resource cleanup on destroy** — Story 1.2 caught a material leak (same pattern as Story 1.1's InputAction leak). Any new InputAction must be disposed in `OnDestroy()`.
- **`new` keyword for property shadowing** — `InteractableObject.Type` needed `new` to avoid CS0108. Watch for similar warnings on any new properties.
- **Manual testing tasks stay unchecked** — dev agent cannot run Unity Editor; leave Task 7 unchecked.
- **Highlight system is complete** — `HandleHighlighting()` runs in `Update()`, tracks `currentHighlighted`, and `CurrentHighlighted` property is exposed publicly.

### Existing Code to Build On

**PlayerController.cs** (current state after Stories 1.1 + 1.2):
- `private InteractableObject currentHighlighted` — the currently highlighted object (null if none)
- `public InteractableObject CurrentHighlighted => currentHighlighted` — public accessor
- `HandleHighlighting()` — runs in `Update()` after `HandleMouseLook()`, before `HandleMovement()`
- Input pattern: inline `InputAction` definitions in `SetupInputActions()`, Enable/Disable in `OnEnable()`/`OnDisable()`, Dispose in `OnDestroy()`
- Existing actions: `moveAction` (2DVector composite), `lookAction` (Mouse/delta), `sprintAction` (leftShift button)

**InteractableObject.cs** (current state after Story 1.2):
- `public string ObjectId => objectId` — unique identifier, needed for `GameEvents.ObjectInspected()`
- `public new ObjectType Type => objectType` — object type enum
- `public bool IsHighlighted => isHighlighted` — highlight state
- `Highlight()` / `Unhighlight()` — emission control methods
- `OnDestroy()` — cleans up material instance
- No interaction methods yet — this story adds them

**GameEvents.cs** (from Story 1.1):
- `public static event Action<string> OnObjectInspected` — already defined
- `public static void ObjectInspected(string objectId)` — null-safe invoke helper already exists
- No new events needed for Story 1.3

**GameConstants.cs** (current state):
- No new constants needed for Story 1.3 — interaction is a binary press, no tunable values

### What This Story Does NOT Include

- No focused inspection view / camera zoom (Story 2.2)
- No object pickup or organization (Story 2.5)
- No "E: Inspect" UI prompt (Story 1.6)
- No interaction audio feedback
- No context-sensitive interaction types beyond a placeholder log — AC1 says "appropriate context-sensitive interaction fires" but for this story, the only action is a debug log. Real context-sensitive behavior (inspect, pick up, toggle) comes in Epic 2 stories.
- No EnvironmentObjectData integration
- No interaction animation or camera movement

### Cross-Story Context

- **Story 1.6 (UI Framework)** will show "E: Inspect" prompt when an object is highlighted — will query `CurrentHighlighted` and `IsInteracting`
- **Story 2.2 (Focused Inspection)** will add a real inspection camera view triggered by `Interact()` — will call `OnInteractionComplete()` when inspection ends
- **Story 2.5 (Organize Objects)** will add pickup/place logic triggered by `Interact()` based on `ObjectType`
- **Story 2.6 (Maintain Light Fixtures)** will add repair interaction for `ObjectType.Fixture`

### GDD Context

Per GDD (Inspect mechanic): "Pressing E on a highlighted object enters focused view with rotation/zoom where applicable." — This story lays the E key foundation. The focused view itself is Story 2.2.

Per GDD (Input Feel): "Context-driven interaction — one key adapts to the situation" and "Consistent across all object types" — The single E key must work for all ObjectTypes. Differentiation happens via `objectType` in future stories.

Per GDD (Interaction Philosophy): "Observe → Inspect → Record flows as: Natural noticing → Intentional examination → Memory preservation." — Story 1.2 covers Observe, this story covers the Inspect trigger, Record comes in Epic 3.

### Technical Notes

- **WasPressedThisFrame vs IsPressed**: Use `WasPressedThisFrame()` for the E key — interaction is a discrete press, not a continuous hold. `IsPressed()` would fire every frame while held.
- **Interaction guard**: The `isInteracting` bool prevents re-triggering. For this story it resets immediately (since there's no real interaction flow yet), but the mechanism must be in place for Story 2.2 to use.
- **Event order**: `Interact()` on the object fires first, then `GameEvents.ObjectInspected()` publishes globally. This lets the object handle its local state before the event bus notifies managers.

### Project Structure Notes

- No new files created — only modifying existing PlayerController.cs and InteractableObject.cs
- No new directories needed
- No new packages or dependencies needed

### References

- [Source: system-architecture.md#2. Game Event Bus Architecture] — OnObjectInspected published by PlayerController
- [Source: system-architecture.md#3. Unity Class Structure] — PlayerController handles "Movement + interaction"
- [Source: gdd.md#Inspect] — "Pressing E on a highlighted object enters focused view"
- [Source: gdd.md#Input Feel] — "Context-driven interaction — one key adapts to the situation"
- [Source: gdd.md#Controls] — E key mapped to "Interact (context-sensitive)"
- [Source: epics.md#Epic 1, Story 1.3] — AC definitions and technical scope
- [Source: epics.md#Epic 1, Story 1.2] — Upstream dependency (highlighting system)
- [Source: epics.md#Epic 1, Story 1.6] — Downstream dependency (UI prompt)

## Dev Agent Record

### Agent Model Used
Claude Opus 4.6

### Debug Log References
N/A — no runtime errors encountered during implementation

### Completion Notes List
- Added `interactAction` InputAction for E key with full lifecycle (setup, enable, disable, dispose)
- Uses `WasPressedThisFrame()` for discrete press detection, not `IsPressed()`
- `HandleInteraction()` runs in Update after HandleHighlighting with 3-tier guard: not pressed → isInteracting → no highlight
- `isInteracting` bool + `IsInteracting` property ready for Story 1.6 UI queries
- `Interact()` on InteractableObject logs to console and fires `OnInteracted` event for external listeners
- `GameEvents.ObjectInspected(objectId)` published per architecture spec event table
- `OnInteractionComplete()` resets `isInteracting` — called immediately for now, ready for future stories to defer
- No scene changes needed — no new serialized fields beyond what Story 1.2 added
- Task 7 (manual testing) left unchecked — requires Unity Editor verification by user

### Change Log
- `Assets/Scripts/Player/PlayerController.cs` — Added interactAction InputAction, isInteracting state, HandleInteraction() method, OnInteractionComplete() method, IsInteracting property
- `Assets/Scripts/Environment/InteractableObject.cs` — Added Interact() method with Debug.Log, OnInteracted event

### File List
- `Assets/Scripts/Player/PlayerController.cs` (modified)
- `Assets/Scripts/Environment/InteractableObject.cs` (modified)

## Senior Developer Review (AI)

**Reviewer:** Claude Opus 4.6
**Date:** 2026-03-28

### Findings

Clean review — no issues found.

### AC Validation

| AC | Status | Evidence |
|----|--------|----------|
| AC1: E on highlighted object fires interaction | IMPLEMENTED | `HandleInteraction()` calls `Interact()` + publishes `GameEvents.ObjectInspected()` (PlayerController.cs:160-174). User confirmed via console log in 7.1. |
| AC2: E with no highlight does nothing | IMPLEMENTED | Guard `if (currentHighlighted == null) return` at line 164. User confirmed in 7.2/7.3. |
| AC3: E during interaction ignored | IMPLEMENTED | Guard `if (isInteracting) return` at line 163. Mechanism in place; currently resets immediately since no real interaction flow exists. Will block when Story 2.2 defers `OnInteractionComplete()`. |

### Summary
Implementation is clean and minimal. Follows established patterns from Stories 1.1/1.2 exactly (InputAction lifecycle, guard pattern). No new issues introduced. User's observation about rapid E firing every press is expected — the `isInteracting` guard is architecturally correct but won't visibly block until a future story introduces a deferred interaction flow.
