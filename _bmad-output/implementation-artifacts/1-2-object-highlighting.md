# Story 1.2: Object Highlighting

Status: ready-for-dev

## Story

As a player,
I want to see objects highlight when I look at them,
so that I know what is interactable.

## Acceptance Criteria

1. Given an interactable object exists in the scene, When the player's camera center is aimed at it within interaction range, Then a subtle visual highlight appears on the object
2. Given an object is highlighted, When the player looks away or moves out of range, Then the highlight fades within 0.2 seconds
3. Given a non-interactable object exists, When the player looks at it, Then no highlight appears

## Tasks / Subtasks

- [ ] Task 1: Add highlighting constants to GameConstants.cs (AC: 1, 2)
  - [ ] 1.1 Add `InteractionRange` (3.0f — arm's length in a bookstore, not sniper distance)
  - [ ] 1.2 Add `HighlightFadeDuration` (0.2f — per AC2)
  - [ ] 1.3 Add `HighlightEmissionIntensity` (0.3f — subtle glow, not neon sign)
  - [ ] 1.4 Add `HighlightColor` as a comment noting default warm white (Color cannot be const; set in InteractableObject inspector or via a static readonly in a helper)
- [ ] Task 2: Create InteractableObject.cs MonoBehaviour (AC: 1, 3)
  - [ ] 2.1 Create `InteractableObject.cs` in `Assets/Scripts/Environment/`
  - [ ] 2.2 Add `[SerializeField] private string objectId` field for unique identification
  - [ ] 2.3 Add `[SerializeField] private ObjectType objectType` field using existing enum
  - [ ] 2.4 Require a `Collider` component via `[RequireComponent(typeof(Collider))]` — the raycast needs something to hit
  - [ ] 2.5 Require a `Renderer` component via `[RequireComponent(typeof(Renderer))]` — the highlight needs something to glow
  - [ ] 2.6 Cache `Renderer` and material instance in `Awake()`. Use `renderer.material` (not `sharedMaterial`) to get a per-instance copy for emission changes without affecting other objects
  - [ ] 2.7 Add `Highlight()` method: enables emission keyword (`_EMISSION`) and sets `_EmissionColor` to highlight color * intensity
  - [ ] 2.8 Add `Unhighlight()` method: starts fade coroutine that lerps emission to black over `HighlightFadeDuration`, then disables `_EMISSION` keyword
  - [ ] 2.9 Add public read-only property `ObjectId` for external access
  - [ ] 2.10 Add public read-only property `IsHighlighted` for state queries
- [ ] Task 3: Add raycast detection to PlayerController.cs (AC: 1, 2, 3)
  - [ ] 3.1 Add `[SerializeField] private float interactionRange` with default from `GameConstants.InteractionRange`
  - [ ] 3.2 Add `[SerializeField] private LayerMask interactableLayer` for filtering raycasts (default to Everything; can be refined via inspector)
  - [ ] 3.3 Add private field `private InteractableObject currentHighlighted` to track what's currently highlighted
  - [ ] 3.4 In `Update()`, add `HandleHighlighting()` call AFTER `HandleMouseLook()` (camera must be rotated first so raycast direction is current)
  - [ ] 3.5 Implement `HandleHighlighting()`: raycast from `cameraTransform.position` along `cameraTransform.forward` for `interactionRange` distance, filtered by `interactableLayer`
  - [ ] 3.6 If ray hits a collider with `InteractableObject` component → call `Highlight()` on it; if it's a different object than `currentHighlighted`, call `Unhighlight()` on the previous one first
  - [ ] 3.7 If ray hits nothing or a non-interactable → call `Unhighlight()` on `currentHighlighted` and set to null
  - [ ] 3.8 Store reference to `currentHighlighted` for Story 1.3 (interaction will check this field)
- [ ] Task 4: Add test interactable objects to Bookstore scene (AC: 1, 3)
  - [ ] 4.1 Add 2-3 primitive objects (cubes/spheres) with `InteractableObject` component, each with a unique `objectId` and a `Collider` + `Renderer`
  - [ ] 4.2 Add 1-2 objects WITHOUT `InteractableObject` (non-interactable) to verify AC3 — the existing shelves/walls serve this purpose, but add at least one clearly distinct non-interactable prop for testing
  - [ ] 4.3 Ensure all interactable objects are placed within reachable range from the player spawn point
  - [ ] 4.4 Verify the scene saves correctly with all new components
- [ ] Task 5: Manual testing verification (AC: 1, 2, 3) — **Requires Unity Editor; to be verified by user**
  - [ ] 5.1 Enter play mode, look at an interactable object within range — confirm subtle highlight appears
  - [ ] 5.2 Look away from the object — confirm highlight fades within ~0.2 seconds (not instant snap-off)
  - [ ] 5.3 Look at a non-interactable object (wall, shelf) — confirm no highlight appears
  - [ ] 5.4 Walk out of range while looking at an interactable — confirm highlight fades
  - [ ] 5.5 Rapidly look between two interactable objects — confirm only one highlights at a time, no double-highlight

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

### Debug Log References

### Completion Notes List

### Change Log

### File List
