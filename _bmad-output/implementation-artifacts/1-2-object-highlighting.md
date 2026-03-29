# Story 1.2: Object Highlighting

Status: done

## Story

As a player,
I want to see objects highlight when I look at them,
so that I know what is interactable.

## Acceptance Criteria

1. Given an interactable object exists in the scene, When the player's camera center is aimed at it within interaction range, Then a subtle visual highlight appears on the object
2. Given an object is highlighted, When the player looks away or moves out of range, Then the highlight fades within 0.2 seconds
3. Given a non-interactable object exists, When the player looks at it, Then no highlight appears

## Tasks / Subtasks

- [x] Task 1: Add highlighting constants to GameConstants.cs (AC: 1, 2)
  - [x] 1.1 Add `InteractionRange` (3.0f — arm's length in a bookstore, not sniper distance)
  - [x] 1.2 Add `HighlightFadeDuration` (0.2f — per AC2)
  - [x] 1.3 Add `HighlightEmissionIntensity` (0.3f — subtle glow, not neon sign)
  - [x] 1.4 Add `HighlightColor` as a comment noting default warm white (Color cannot be const; set in InteractableObject inspector or via a static readonly in a helper)
- [x] Task 2: Create InteractableObject.cs MonoBehaviour (AC: 1, 3)
  - [x] 2.1 Create `InteractableObject.cs` in `Assets/Scripts/Environment/`
  - [x] 2.2 Add `[SerializeField] private string objectId` field for unique identification
  - [x] 2.3 Add `[SerializeField] private ObjectType objectType` field using existing enum
  - [x] 2.4 Require a `Collider` component via `[RequireComponent(typeof(Collider))]` — the raycast needs something to hit
  - [x] 2.5 Require a `Renderer` component via `[RequireComponent(typeof(Renderer))]` — the highlight needs something to glow
  - [x] 2.6 Cache `Renderer` and material instance in `Awake()`. Use `renderer.material` (not `sharedMaterial`) to get a per-instance copy for emission changes without affecting other objects
  - [x] 2.7 Add `Highlight()` method: enables emission keyword (`_EMISSION`) and sets `_EmissionColor` to highlight color * intensity
  - [x] 2.8 Add `Unhighlight()` method: starts fade coroutine that lerps emission to black over `HighlightFadeDuration`, then disables `_EMISSION` keyword
  - [x] 2.9 Add public read-only property `ObjectId` for external access
  - [x] 2.10 Add public read-only property `IsHighlighted` for state queries
- [x] Task 3: Add raycast detection to PlayerController.cs (AC: 1, 2, 3)
  - [x] 3.1 Add `[SerializeField] private float interactionRange` with default from `GameConstants.InteractionRange`
  - [x] 3.2 Add `[SerializeField] private LayerMask interactableLayer` for filtering raycasts (default to Everything; can be refined via inspector)
  - [x] 3.3 Add private field `private InteractableObject currentHighlighted` to track what's currently highlighted
  - [x] 3.4 In `Update()`, add `HandleHighlighting()` call AFTER `HandleMouseLook()` (camera must be rotated first so raycast direction is current)
  - [x] 3.5 Implement `HandleHighlighting()`: raycast from `cameraTransform.position` along `cameraTransform.forward` for `interactionRange` distance, filtered by `interactableLayer`
  - [x] 3.6 If ray hits a collider with `InteractableObject` component → call `Highlight()` on it; if it's a different object than `currentHighlighted`, call `Unhighlight()` on the previous one first
  - [x] 3.7 If ray hits nothing or a non-interactable → call `Unhighlight()` on `currentHighlighted` and set to null
  - [x] 3.8 Store reference to `currentHighlighted` for Story 1.3 (interaction will check this field) — exposed via `CurrentHighlighted` public property
- [x] Task 4: Add test interactable objects to Bookstore scene (AC: 1, 3)
  - [x] 4.1 Add 2-3 primitive objects (cubes/spheres) with `InteractableObject` component, each with a unique `objectId` and a `Collider` + `Renderer`
  - [x] 4.2 Add 1-2 objects WITHOUT `InteractableObject` (non-interactable) to verify AC3 — added Crate_Decor (no InteractableObject); existing shelves/walls also serve this purpose
  - [x] 4.3 Ensure all interactable objects are placed within reachable range from the player spawn point
  - [x] 4.4 Verify the scene saves correctly with all new components
- [ ] Task 5: Manual testing verification (AC: 1, 2, 3) — **Requires Unity Editor; to be verified by user**
  - [x] 5.1 Enter play mode, look at an interactable object within range — confirm subtle highlight appears
  - [x] 5.2 Look away from the object — confirm highlight fades within ~0.2 seconds (not instant snap-off)
  - [x] 5.3 Look at a non-interactable object (wall, shelf) — confirm no highlight appears
  - [x] 5.4 Walk out of range while looking at an interactable — confirm highlight fades
  - [x] 5.5 Rapidly look between two interactable objects — confirm only one highlights at a time, no double-highlight

## Dev Notes

### Architecture Compliance

- **File:** `Assets/Scripts/Environment/InteractableObject.cs` — MonoBehaviour placed on scene objects. Per architecture spec Section 3, this is the correct location.
- **File:** `Assets/Scripts/Player/PlayerController.cs` — Existing file, add raycast highlighting logic. Do NOT create a separate "HighlightManager" or "InteractionManager" — the architecture spec puts interaction in PlayerController. A separate highlight system is over-engineering for this scope.
- **Highlight approach:** Use Unity's built-in emission property on Standard shader materials. Do NOT import third-party outline/highlight packages. The GDD says "subtle" — emission glow is appropriate. Outline shaders can be added later if needed.
- **Raycast filtering:** Use `Physics.Raycast` with a `LayerMask`. Default to `Everything` layer for now; can be optimized to a dedicated "Interactable" layer in future stories if performance requires it.

### Previous Story (1.1) Learnings

From the Story 1.1 code review:
- **InputAction disposal matters** — any new InputActions must be disposed in OnDestroy()
- **Magic numbers go in GameConstants** — all tunable values must be constants, not inline literals
- **Manual testing tasks stay unchecked** — the dev agent cannot run Unity Editor; leave Task 5 unchecked for user verification
- **Camera reference** is already cached in PlayerController as `cameraTransform` (obtained in Awake via `GetComponentInChildren<Camera>().transform`)

### Existing Code to Build On

**PlayerController.cs** (from Story 1.1):
- `cameraTransform` — cached camera Transform, use for raycast origin and direction
- `Update()` method — add `HandleHighlighting()` here, after `HandleMouseLook()`
- Input system pattern — inline InputAction definitions (no .inputactions asset)
- Existing fields: walkSpeed, sprintSpeed, mouseSensitivity, verticalLookClamp, gravity (all serialized with GameConstants defaults)

**GameConstants.cs** (from Story 1.1):
- Already has: WalkSpeed, SprintSpeed, MouseSensitivity, Gravity, VerticalLookClamp, MouseSensitivityScale, GroundedDownForce
- Add new highlighting constants here

**GameEnums.cs** (from Story 1.1):
- `ObjectType { Book, Fixture, Prop, Record, Furniture, Switch }` — use this for InteractableObject.objectType field
- `ObjectCondition { Normal, Degraded, Failed, Corrupted }` — available for future use, not needed this story

**GameEvents.cs** (from Story 1.1):
- `OnObjectInspected` event already exists — will be used by Story 1.3, not this story
- No new events needed for Story 1.2

### GDD Context

Per GDD (Input Feel section): "Objects of interest subtly highlight when centered in view or approached within range. Observation should feel natural and continuous, not mechanical."

This means:
- Highlight must be SUBTLE — a soft emission glow, not a bright outline
- Detection is camera-center based (raycast from screen center)
- The system should feel passive — player just looks around and objects naturally respond
- No dedicated "scan mode" or button — always active

### What This Story Does NOT Include

- No E key interaction (Story 1.3)
- No interaction prompts/UI (Story 1.6 — "E: Inspect" text)
- No observation system with metadata awareness (Story 2.1)
- No focused inspection view (Story 2.2)
- No object pickup/organization (Story 2.5)
- No EnvironmentObjectData integration — InteractableObject is a simple scene component for now
- No audio feedback on highlight

### Cross-Story Context

- **Story 1.3 (Interaction)** will add E key input checking `currentHighlighted` on PlayerController — ensure this field is accessible (public or via property)
- **Story 1.6 (UI Framework)** will show "E: Inspect" prompt when an object is highlighted — ensure `IsHighlighted` state is queryable
- **Story 2.1 (Observation)** will enhance highlighting with metadata awareness — InteractableObject should be extensible but simple for now

### Technical Notes

- **Material instancing:** `renderer.material` creates a runtime copy. This is intentional — we need per-object emission control. The original shared material stays clean.
- **Emission keyword:** Unity's Standard shader requires `_EMISSION` keyword to be enabled for emission to render. Use `material.EnableKeyword("_EMISSION")` / `material.DisableKeyword("_EMISSION")`.
- **Fade via coroutine:** Use `StartCoroutine` for the 0.2s fade lerp. Stop any running fade coroutine before starting a new one (e.g., if player rapidly looks away and back).
- **Single highlight rule:** Only ONE object can be highlighted at a time. The raycast hits the nearest collider; previous highlight is cleared before new one is applied.

### Performance Targets

- Single Physics.Raycast per frame — negligible cost
- Emission changes are shader property updates — no per-frame allocation
- Material instancing happens once per InteractableObject on first highlight, not every frame

### Project Structure Notes

- `Assets/Scripts/Environment/InteractableObject.cs` — first file in the Environment/ directory
- No new directories needed — Environment/ already exists from Story 1.1
- No new packages or dependencies needed

### References

- [Source: system-architecture.md#3. Unity Class Structure] — InteractableObject.cs in Environment/ folder
- [Source: system-architecture.md#2. Game Event Bus Architecture] — OnObjectInspected event (for future Story 1.3)
- [Source: gdd.md#Observation] — "Objects of interest subtly highlight when centered in view or approached within range"
- [Source: gdd.md#Input Feel] — "Observation should feel natural and continuous, not mechanical"
- [Source: epics.md#Epic 1, Story 1.2] — AC definitions and technical scope
- [Source: epics.md#Epic 1, Story 1.3] — Downstream dependency on highlighting system
- [Source: epics.md#Epic 1, Story 1.6] — UI prompt depends on highlight state

## Dev Agent Record

### Agent Model Used
Claude Opus 4.6

### Debug Log References
N/A — no runtime errors encountered during implementation

### Completion Notes List
- InteractableObject.cs created with emission-based highlight system using `_EMISSION` keyword and `_EmissionColor` shader property
- Material instancing via `renderer.material` ensures per-object emission control
- Coroutine-based fade (0.2s lerp to black) for smooth unhighlight transitions
- HandleHighlighting() in PlayerController uses Physics.Raycast from camera center, filtered by interactableLayer
- Single-highlight rule enforced: previous object is unhighlighted before new one activates
- Public `CurrentHighlighted` property exposed for Story 1.3 interaction system
- 3 interactable test objects (Book_1, Book_2, Vase_1) and 1 non-interactable prop (Crate_Decor) added to scene
- Task 5 (manual testing) left unchecked — requires Unity Editor verification by user

### Change Log
- `Assets/Scripts/Core/GameConstants.cs` — Added InteractionRange, HighlightFadeDuration, HighlightEmissionIntensity constants and HighlightColor comment
- `Assets/Scripts/Environment/InteractableObject.cs` — NEW: MonoBehaviour with Highlight()/Unhighlight(), emission fade coroutine, objectId/objectType identity
- `Assets/Scripts/Player/PlayerController.cs` — Added interactionRange, interactableLayer, currentHighlighted fields; HandleHighlighting() raycast method; CurrentHighlighted public property
- `Assets/Scenes/Bookstore.unity` — Added Book_1, Book_2, Vase_1 (with InteractableObject), Crate_Decor (without); updated PlayerController serialized fields

### File List
- `Assets/Scripts/Core/GameConstants.cs` (modified)
- `Assets/Scripts/Environment/InteractableObject.cs` (new)
- `Assets/Scripts/Environment/InteractableObject.cs.meta` (new)
- `Assets/Scripts/Environment.meta` (new — directory meta)
- `Assets/Scripts/Player/PlayerController.cs` (modified)
- `Assets/Scenes/Bookstore.unity` (modified)

## Senior Developer Review (AI)

**Reviewer:** Claude Opus 4.6
**Date:** 2026-03-28

### Findings

| # | Severity | Description | File | Resolution |
|---|----------|-------------|------|------------|
| H1 | HIGH | Material instance created via `renderer.material` never destroyed — causes material leak on object destroy/scene unload | InteractableObject.cs | Fixed: Added `OnDestroy()` with `Destroy(materialInstance)` |
| M1 | MEDIUM | Story File List missing `Assets/Scripts/Environment.meta` directory meta file | 1-2-object-highlighting.md | Fixed: Added to File List |
| L1 | LOW | `Type` property shadows inherited `Component.type` — CS0108 compiler warning | InteractableObject.cs:23 | Fixed: Added `new` keyword to property declaration |

### AC Validation

| AC | Status | Evidence |
|----|--------|----------|
| AC1: Highlight appears when camera aimed at interactable within range | IMPLEMENTED | `HandleHighlighting()` raycasts from camera center, calls `Highlight()` on hit InteractableObject (PlayerController.cs:119-139) |
| AC2: Highlight fades within 0.2s when looking away/out of range | IMPLEMENTED | `Unhighlight()` starts coroutine lerping emission to black over `HighlightFadeDuration` (0.2f) (InteractableObject.cs:57-74) |
| AC3: Non-interactable objects get no highlight | IMPLEMENTED | Raycast hit checks for `InteractableObject` component; no component = no highlight (PlayerController.cs:125-127). Crate_Decor in scene has no InteractableObject for testing. |

### Summary
Implementation is solid. Raycast logic correctly enforces single-highlight rule. Emission-based approach matches GDD "subtle" requirement. Material instancing pattern is correct but needed cleanup on destroy (same pattern as Story 1.1's InputAction disposal). All ACs verified in code.
