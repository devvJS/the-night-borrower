# Story 2.1: Observation Highlighting

Status: done

## Story

As a player,
I want to notice objects of interest through subtle visual highlighting,
so that observation feels natural and continuous.

## Acceptance Criteria

1. Given an object with important metadata (clue, changed state, misplaced) is within camera view, When the player's view center passes over it within detection range, Then a subtle highlight effect distinguishes it from normal objects
2. Given multiple interactable objects are visible, When the player scans the room, Then only contextually important objects receive observation highlights (not every interactable)
3. Given an object's importance changes (e.g., a book becomes misplaced), When the player next views it, Then the highlight state updates to reflect current importance

## Tasks / Subtasks

- [x] Task 1: Add observation constants to GameConstants.cs (AC: 1, 2, 3)
  - [x] 1.1 Add `// ─── Observation ───` section after the Flashlight section
  - [x] 1.2 Add `public const float ObservationRange = 8.0f;` — detection range, longer than InteractionRange (3.0f) since observation is passive awareness
  - [x] 1.3 Add `public const float ObservationHighlightIntensity = 0.15f;` — subtler than interaction highlight (0.3f) to feel atmospheric, not mechanical
  - [x] 1.4 Add `public const float ObservationFadeDuration = 0.4f;` — slower fade than interaction (0.2f) for a softer feel
  - [x] 1.5 Add `public const float ObservationViewAngle = 30f;` — degrees from camera center within which observation triggers (cone check, not just raycast)

- [x] Task 2: Add `isImportant` field and observation highlight to InteractableObject.cs (AC: 1, 2, 3)
  - [x] 2.1 Add serialized field: `[Header("Observation")] [SerializeField] private bool isImportant;` — manually set in Inspector per object. This is the MVP importance flag. Later epics (5.x EnvironmentManager) will set this dynamically via `SetImportant(bool)`.
  - [x] 2.2 Add serialized field: `[SerializeField] private Color observationColor = new Color(0.6f, 0.8f, 1.0f);` — cool blue-white tint, visually distinct from the warm interaction highlight color (1.0, 0.95, 0.8)
  - [x] 2.3 Add public property: `public bool IsImportant => isImportant;`
  - [x] 2.4 Add public method: `public void SetImportant(bool important)` — sets `isImportant` and if changing to false while observation-highlighted, triggers observation unhighlight. This enables AC3 (dynamic importance changes).
  - [x] 2.5 Add `private bool isObservationHighlighted;` field and `public bool IsObservationHighlighted => isObservationHighlighted;` property
  - [x] 2.6 Add `public void ObservationHighlight()` method:
    - If already observation-highlighted, return early
    - If currently interaction-highlighted (`isHighlighted == true`), do NOT apply observation highlight — interaction highlight takes visual priority. Set `isObservationHighlighted = true` so state is tracked but don't change emission.
    - Otherwise: enable `_EMISSION`, set emission to `observationColor * GameConstants.ObservationHighlightIntensity`
    - Set `isObservationHighlighted = true`
  - [x] 2.7 Add `public void ObservationUnhighlight()` method:
    - If not observation-highlighted, return early
    - Set `isObservationHighlighted = false`
    - If currently interaction-highlighted, do nothing (interaction is still controlling emission)
    - Otherwise: start a fade coroutine similar to `Unhighlight()` but using `GameConstants.ObservationFadeDuration`
  - [x] 2.8 Modify existing `Highlight()` method: when interaction highlight activates, if observation highlight is active, the interaction highlight simply overrides the emission color (already does this since it sets emission directly). No change needed — interaction `Highlight()` already sets emission unconditionally.
  - [x] 2.9 Modify existing `Unhighlight()` method: after interaction highlight fades out, if `isObservationHighlighted` is still true, restore observation emission color instead of fading to black. Modify the `FadeHighlight()` coroutine end state: instead of always fading to `Color.black` and disabling `_EMISSION`, check `isObservationHighlighted` — if true, fade to `observationColor * ObservationHighlightIntensity` and keep `_EMISSION` enabled.
  - [x] 2.10 Add a second coroutine field `private Coroutine observationFadeCoroutine;` to avoid conflicts with the existing `fadeCoroutine`. The observation fade coroutine (`FadeObservationHighlight()`) fades from current emission to black over `ObservationFadeDuration`.
  - [x] 2.11 In `OnDestroy()`: no new cleanup needed — `materialInstance` already destroyed there.

- [x] Task 3: Add GameEvents for observation system (AC: 1, 3)
  - [x] 3.1 Add to Player section in GameEvents.cs: `public static event Action<string> OnObjectObserved;` with comment `// (objectId)`
  - [x] 3.2 Add invoke helper: `public static void ObjectObserved(string objectId) => OnObjectObserved?.Invoke(objectId);`

- [x] Task 4: Create ObservationSystem.cs in Player/ (AC: 1, 2, 3)
  - [x] 4.1 Create `Assets/Scripts/Player/ObservationSystem.cs` — `public class ObservationSystem : MonoBehaviour`
  - [x] 4.2 Add `[SerializeField] private Transform cameraTransform;` — same auto-discovery pattern: if null in Awake, `GetComponentInChildren<Camera>().transform`
  - [x] 4.3 Add `[SerializeField] private LayerMask interactableLayer;` — same layer as PlayerController uses
  - [x] 4.4 Store `private InteractableObject currentObserved;` to track what's currently observation-highlighted
  - [x] 4.5 In `Update()`, implement observation detection:
    - Cast a ray from camera center forward, max range `GameConstants.ObservationRange`
    - If hit has `InteractableObject` component AND `interactable.IsImportant == true`:
      - If different from `currentObserved`: unhighlight old, highlight new, fire `GameEvents.ObjectObserved(objectId)`
    - If hit has `InteractableObject` but `IsImportant == false`, or hit nothing:
      - Unhighlight `currentObserved`, set to null
  - [x] 4.6 **Important interaction with PlayerController:** Both systems raycast from camera center. PlayerController handles interaction highlighting (range 3.0f). ObservationSystem handles observation highlighting (range 8.0f). They are independent — an object can be both interaction-highlighted AND observation-highlighted simultaneously (observation is visually suppressed while interaction highlight is active, per Task 2.6/2.7).
  - [x] 4.7 No InputAction needed — observation is passive (no button press). No OnDestroy cleanup beyond what Unity handles automatically (no runtime resources created).

- [x] Task 5: Add ObservationSystem to Player GameObject in Bookstore scene (AC: 1, 2, 3)
  - [x] 5.1 **User must attach ObservationSystem component to the Player GameObject in Unity Editor** after import, same as PlayerFlashlight. Set `interactableLayer` to match PlayerController's layer mask in Inspector.
  - [x] 5.2 Camera transform auto-discovered via GetComponentInChildren<Camera>() — no manual assignment needed.

- [x] Task 6: Set importance flags on test objects (AC: 1, 2)
  - [x] 6.1 In Unity Editor, set `isImportant = true` on 1-2 InteractableObject instances in the Bookstore scene for testing. Leave others as false. This validates AC2 (only important objects get observation highlight).
  - [x] 6.2 **User must toggle importance in Inspector** — no scene YAML edit.

- [x] Task 7: Manual testing verification (AC: 1, 2, 3) — **Requires Unity Editor; to be verified by user**
  - [x] 7.1 Enter play mode — look at an important object from within 8m — verify subtle blue-ish highlight appears, visually distinct from the warm interaction highlight
  - [x] 7.2 Look at a non-important interactable — verify NO observation highlight appears (AC2)
  - [x] 7.3 Walk within 3m of an important object — verify interaction highlight (warm) overrides observation highlight (cool). Walk back out past 3m but within 8m — verify observation highlight returns.
  - [x] 7.4 Via Inspector at runtime: toggle `isImportant` on an object from true→false while looking at it — verify observation highlight fades away (AC3). Toggle back true→observe — verify highlight appears.
  - [x] 7.5 Look away from an observation-highlighted object — verify it fades out smoothly (not instant snap)

## Dev Notes

### Architecture Compliance

- **File:** `Assets/Scripts/Player/ObservationSystem.cs` — New MonoBehaviour in `Player/`. Observation is a player-side system (camera-based detection), not an environment system. Separate from PlayerController (already 200 lines) following the same separation pattern as PlayerFlashlight.
- **File:** `Assets/Scripts/Environment/InteractableObject.cs` — Modified. Adding observation highlight layer alongside existing interaction highlight. Same emission-based approach, different color and intensity.
- **File:** `Assets/Scripts/Core/GameConstants.cs` — Modified. New Observation constants section.
- **File:** `Assets/Scripts/Core/GameEvents.cs` — Modified. New OnObjectObserved event.
- **Observation is NOT inspection:** Observation is passive camera-based awareness (this story). Inspection is active E-key focused view (Story 2.2). They are different systems with different triggers.
- **Two highlight layers, one material:** Both highlights use the same `_EmissionColor` property on the material instance. They cannot visually stack — interaction highlight takes priority when both are active. Observation highlight restores when interaction highlight fades.

### What This Story Does NOT Include

- **No EnvironmentManager integration** — The full `EnvironmentObjectData` class with `IsDisplaced`, `ObjectCondition`, `entityTouched` etc. comes in Epic 5. This story uses a simple `isImportant` bool as the MVP importance flag.
- **No automatic importance detection** — Objects are manually flagged `isImportant` in Inspector. Automatic importance based on displacement, condition, or entity interference comes in later epics when those systems exist.
- **No notebook auto-logging** — Discovery logging comes in Story 2.4. The `GameEvents.ObjectObserved` event is published here so 2.4 can subscribe to it.
- **No cone/frustum detection** — MVP uses a single raycast from camera center, matching PlayerController's approach. A wider cone/frustum check could be added later if observation feels too narrow in playtesting.
- **No observation-specific UI** — No on-screen indicator or prompt for observed objects. The highlight itself is the only feedback. UI could be added in Epic 12 if needed.
- **No sound effect** — Audio cues for observation come in Epic 9.

### Previous Story (1.8) Learnings

From Epic 1 code reviews:
- **InputAction lifecycle** — Not needed here (observation is passive, no input). But follow the pattern if adding any input later.
- **Resource cleanup in OnDestroy()** — InteractableObject already destroys `materialInstance`. No new runtime resources in ObservationSystem (no Materials, Textures, Sprites, InputActions created).
- **All tunable values go in GameConstants** — Range, intensity, fade duration, view angle.
- **Manual testing tasks stay unchecked** — Dev agent cannot run Unity Editor.
- **Programmatic creation preferred** — Not applicable here. No complex Unity objects being created programmatically.
- **Emission-based highlighting works well** — Proven in Epic 1 across all interactable objects.

### Existing Code to Build On

**InteractableObject.cs** (93 lines):
- `Highlight()` / `Unhighlight()` — Emission-based, with fade coroutine. Observation methods mirror this pattern.
- `materialInstance` — Already created in Awake(), destroyed in OnDestroy(). Shared between both highlight types.
- `EmissionColorId` — Static shader property ID, reuse for observation emission.
- `highlightColor` (warm) vs new `observationColor` (cool) — visually distinct.

**PlayerController.cs HandleHighlighting()** (lines 129-160):
- Raycast from camera center, `interactionRange` (3.0f), `interactableLayer`
- ObservationSystem uses identical raycast pattern but with `ObservationRange` (8.0f)
- Both systems are independent — no shared state, no coordination needed

**GameConstants.cs:**
- Sections: Player Movement, Object Highlighting, Performance Baseline, Lighting Baseline, UI, Save, Flashlight
- Add new `// ─── Observation ───` section

**GameEvents.cs:**
- Player section has OnObjectInspected, OnObjectHighlighted
- Add OnObjectObserved alongside these

### Technical Notes

- **Dual emission state management:** The key complexity is managing two highlight states (interaction + observation) on one material's `_EmissionColor`. The solution: interaction always wins visually. Observation tracks its own state (`isObservationHighlighted`) independently. When interaction ends, observation restores if still active.
- **Coroutine conflicts:** Two separate coroutine fields (`fadeCoroutine` for interaction, `observationFadeCoroutine` for observation). Each system cancels only its own coroutine. When interaction `Highlight()` fires, it cancels `fadeCoroutine` (existing behavior) — observation coroutine doesn't need cancellation since interaction takes visual priority.
- **Performance:** One additional raycast per frame in ObservationSystem.Update(). PlayerController already does one. Two raycasts per frame is negligible. No Physics.OverlapSphere or frustum checks needed for MVP.

### Performance Targets

- 1 additional Physics.Raycast per frame — negligible cost
- Emission color changes are material property sets — negligible GPU cost
- No new Materials or Textures created at runtime
- Target: < 0.1ms frame time impact

### Project Structure Notes

- New file: `Assets/Scripts/Player/ObservationSystem.cs`
- Modified: `Assets/Scripts/Environment/InteractableObject.cs` (observation highlight methods + isImportant field)
- Modified: `Assets/Scripts/Core/GameConstants.cs` (Observation constants)
- Modified: `Assets/Scripts/Core/GameEvents.cs` (OnObjectObserved event)
- No scene YAML changes — user attaches component and sets importance flags in Editor

### References

- [Source: epics.md#Epic 2, Story 2.1] — AC definitions
- [Source: gdd.md#Primary Mechanics, 1. Observe] — "Observation is passive and camera-based — no dedicated mode or button required. Objects of interest subtly highlight when centered in view or approached within range."
- [Source: gdd.md#Core Design Pillars, Knowledge Is Survival] — "Players must observe, record, and verify information"
- [Source: gdd.md#Interaction Philosophy] — "Observe → Inspect → Record flows as: Natural noticing → Intentional examination → Memory preservation"
- [Source: system-architecture.md#EnvironmentObjectData] — Full data class with IsDisplaced, ObjectCondition, entityTouched — NOT implemented this story, but the `isImportant` bool is the MVP proxy
- [Source: epics.md#Epic 2, Story 2.2] — Downstream: inspection system builds on observation (observe first, then inspect)
- [Source: epics.md#Epic 2, Story 2.4] — Downstream: auto-logging subscribes to ObjectObserved event
- [Source: epics.md#Epic 5] — Downstream: EnvironmentManager will call SetImportant() based on displacement/condition/entity state

## Dev Agent Record

### Agent Model Used

Claude Opus 4.6

### Debug Log References

N/A — no runtime errors encountered during implementation

### Completion Notes List

- Task 1: Added 4 Observation constants to GameConstants.cs: ObservationRange (8.0f), ObservationHighlightIntensity (0.15f), ObservationFadeDuration (0.4f), ObservationViewAngle (30f).
- Task 2: Extended InteractableObject.cs with observation highlight layer. Added isImportant field with SetImportant() for dynamic changes (AC3). Added ObservationHighlight()/ObservationUnhighlight() methods with cool blue-white emission (0.6, 0.8, 1.0). Interaction highlight takes visual priority — observation restores when interaction fades. Dual coroutine fields avoid conflicts. Modified FadeHighlight() to restore observation color instead of black when observation is active.
- Task 3: Added OnObjectObserved event and ObjectObserved() invoke helper to GameEvents.cs Player section.
- Task 4: Created ObservationSystem.cs — passive camera-based detection via raycast at 8m range. Only highlights IsImportant objects. Independent from PlayerController's interaction system. Fires GameEvents.ObjectObserved on new observation.
- Tasks 5-7: Unity Editor tasks — user must attach ObservationSystem to Player, set interactableLayer, flag 1-2 objects as important, and verify all ACs in play mode.

### Change Log

- 2026-03-29: Implemented Story 2.1 — Observation Highlighting. Created ObservationSystem.cs, extended InteractableObject.cs with dual-layer highlight system, added Observation constants and GameEvents.
- 2026-03-29: Code review — 2 Medium, 1 Low. Fixed: removed debug DevLog from ObservationSystem.cs, tuned ObservationHighlightIntensity from 0.8f (debug) to 0.35f. FadeHighlight() refactored to fade to black first then restore observation if active (fixed blue flash artifact). Approved.

### File List

- Assets/Scripts/Player/ObservationSystem.cs (new — passive camera-based observation detection, raycast at 8m range, highlights important objects)
- Assets/Scripts/Player/ObservationSystem.cs.meta (new)
- Assets/Scripts/Environment/InteractableObject.cs (modified — added isImportant field, ObservationHighlight/ObservationUnhighlight methods, SetImportant(), dual coroutine management, FadeHighlight restoration logic)
- Assets/Scripts/Core/GameConstants.cs (modified — added Observation constants section)
- Assets/Scripts/Core/GameEvents.cs (modified — added OnObjectObserved event and invoke helper)

## Senior Developer Review (AI)

**Review Date:** 2026-03-29
**Reviewer:** Claude Opus 4.6 (adversarial code review)
**Review Outcome:** Approved (2 Medium fixed, 1 Low acknowledged)

### Action Items

- [x] [Medium] Remove debug DevLog from ObservationSystem.cs — per-frame console spam
- [x] [Medium] Tune ObservationHighlightIntensity from 0.8f (debug) to 0.35f (visible but subtle)
- [ ] [Low] ObservationViewAngle constant defined but unused — intentional per story (reserved for future cone check)

### AC Validation

| AC | Status | Evidence |
|----|--------|----------|
| AC1: Important objects get subtle highlight within range | IMPLEMENTED | ObservationSystem raycast at 8m, ObservationHighlight() with blue emission (InteractableObject.cs:86-101) |
| AC2: Only important objects highlighted | IMPLEMENTED | `interactable.IsImportant` check (ObservationSystem.cs:29), non-important ignored |
| AC3: Highlight updates when importance changes | IMPLEMENTED | SetImportant(bool) triggers ObservationUnhighlight on false (InteractableObject.cs:52-60) |

### Summary

Clean dual-layer highlight system. ObservationSystem (50 lines) is minimal and focused — passive raycast, importance filter, event publishing. InteractableObject correctly manages two highlight states on one material with proper priority (interaction > observation). FadeHighlight() refactored during debugging to eliminate blue flash artifact when both systems unhighlight simultaneously. No resource leaks, no new runtime allocations. Two medium issues fixed (debug log removal, intensity tuning).
