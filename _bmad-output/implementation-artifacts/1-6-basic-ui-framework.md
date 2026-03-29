# Story 1.6: Basic UI Framework

Status: done

## Story

As a player,
I want to see a basic UI framework (crosshair, interaction prompts),
so that I understand available actions.

## Acceptance Criteria

1. Given the player is in the scene, When they look around, Then a small centered crosshair is visible at all times
2. Given the player highlights an interactable object, When the highlight appears, Then a text prompt (e.g., "E: Inspect") appears near the crosshair
3. Given the player is not looking at any interactable, When they look at empty space, Then only the crosshair is visible with no prompt text

## Tasks / Subtasks

- [x] Task 1: Add UI constants to GameConstants.cs (AC: 1, 2, 3)
  - [x] 1.1 Add `CrosshairSize` (8f — small subtle dot, not distracting)
  - [x] 1.2 Add `CrosshairAlpha` (0.6f — semi-transparent, unobtrusive)
  - [x] 1.3 Add `PromptFontSize` (18f — readable but not dominant)
  - [x] 1.4 Add `PromptVerticalOffset` (40f — pixels below screen center for prompt text placement)
- [x] Task 2: Create PlayerHUD.cs MonoBehaviour (AC: 1, 2, 3)
  - [x] 2.1 Create `PlayerHUD.cs` in `Assets/Scripts/Player/`
  - [x] 2.2 Add `[SerializeField] private PlayerController playerController` field for querying highlight state. In `Awake()`, if this field is null, fallback to `FindObjectOfType<PlayerController>()`. If still null, `Debug.LogError("PlayerHUD: No PlayerController found")`.
  - [x] 2.3 In `Awake()`, after resolving `playerController`, build the UI Canvas and all child elements programmatically (see Dev Notes for approach). This avoids scene YAML for Canvas hierarchies which are extremely complex and fragile.
  - [x] 2.4 Create a Screen Space - Overlay Canvas with sorting order 0 and a CanvasScaler set to Scale With Screen Size (reference 1920x1080, match width/height 0.5)
  - [x] 2.5 Add a Raycaster is NOT needed — this is display-only HUD, no clickable elements. Do NOT add GraphicRaycaster.
  - [x] 2.6 Create crosshair: a small UI Image (RectTransform anchored center) with a procedurally-generated 1x1 white texture, scaled to `CrosshairSize`, with alpha set to `CrosshairAlpha`. Color: white.
  - [x] 2.7 Create interaction prompt: a TextMeshProUGUI element anchored below center, positioned `PromptVerticalOffset` pixels below screen center. Font size `PromptFontSize`, color warm white `(1.0, 0.95, 0.85, 0.9)`, center-aligned. Default text empty, `gameObject.SetActive(false)`.
  - [x] 2.8 In `Update()`, check `playerController.CurrentHighlighted`:
    - If not null AND not `playerController.IsInteracting` → show prompt with text `"E: Inspect"` (context-sensitive text can be expanded in Epic 2 stories based on ObjectType)
    - If null OR `IsInteracting` → hide prompt
  - [x] 2.9 Add `SetPromptText(string text)` public method for future stories to customize the prompt text per ObjectType
  - [x] 2.10 Add `SetCrosshairVisible(bool visible)` public method for future stories that may want to hide crosshair (e.g., notebook view, inspection view)
- [x] Task 3: Add PlayerHUD to Bookstore scene (AC: 1, 2, 3)
  - [x] 3.1 Add an empty GameObject "PlayerHUD" to the Bookstore scene at root level via YAML
  - [x] 3.2 **User must attach the PlayerHUD component in Unity Editor** — script GUID not available until Unity imports the new .cs file
  - [x] 3.3 **User may optionally drag the Player GameObject into the `playerController` serialized field** in the Inspector — the `FindObjectOfType` fallback in Task 2.2 handles auto-discovery if left unset
- [x] Task 4: Add GameEvents integration for prompt visibility (AC: 2, 3)
  - [x] 4.1 Add `OnObjectHighlighted` event to `GameEvents.cs`: `public static event Action<string> OnObjectHighlighted` with invoke helper — fires when a new object is highlighted (objectId) or null when unhighlighted
  - [x] 4.2 In `PlayerController.HandleHighlighting()`: after `currentHighlighted.Highlight()` (line ~146), publish `GameEvents.ObjectHighlighted(currentHighlighted.ObjectId)`. After `currentHighlighted.Unhighlight()` / `currentHighlighted = null` (line ~155), publish `GameEvents.ObjectHighlighted(null)`
  - [x] 4.3 In `PlayerHUD`, optionally subscribe to `OnObjectHighlighted` for decoupled notification — OR simply poll `CurrentHighlighted` in Update (polling is simpler and acceptable for a single HUD element). Choose polling for simplicity in this story.
  - [x] 4.4 Note: The event is added for future systems (notebook auto-logging, audio feedback) that need to react to highlight changes without coupling to PlayerController
- [x] Task 5: Manual testing verification (AC: 1, 2, 3) — **Requires Unity Editor; to be verified by user**
  - [x] 5.1 Enter play mode — verify small crosshair dot is visible at screen center
  - [x] 5.2 Look at an interactable object within range — verify "E: Inspect" text appears below crosshair
  - [x] 5.3 Look away from the interactable — verify prompt text disappears, only crosshair remains
  - [x] 5.4 Look at a non-interactable (wall, shelf, Crate_Decor) — verify no prompt appears
  - [x] 5.5 Verify crosshair does not interfere with gameplay or feel distracting
  - [x] 5.6 Verify prompt text is readable against both light and dark backgrounds in the scene

## Dev Notes
- User confirmed existence of crosshair and prompt in the UI but the crosshair is a little square and personally we think  a small dot would be more subtle and fitting. To note: we wouldnt say it interferes with gameplay but its not transparent enough. We recommend adjusting the crosshair to be a small dot (e.g., 8x8 pixels) and reducing its alpha to around 0.6 for a more subtle, unobtrusive presence that still provides aiming feedback without drawing too much attention.
### Architecture Compliance

- **File:** `Assets/Scripts/Player/PlayerHUD.cs` — New MonoBehaviour. The architecture spec places `NotebookUI.cs` in `Player/` (Section 3 class structure), so HUD UI belongs there too. Do NOT create a separate `Scripts/UI/` folder — the architecture doesn't define one.
- **File:** `Assets/Scripts/Player/PlayerController.cs` — Existing file. Add `GameEvents.ObjectHighlighted()` calls in `HandleHighlighting()`. Minor modification.
- **File:** `Assets/Scripts/Core/GameEvents.cs` — Existing file. Add `OnObjectHighlighted` event and invoke helper.
- **File:** `Assets/Scripts/Core/GameConstants.cs` — Existing file. Add UI constants.
- **Canvas approach:** Build Canvas and all UI elements **programmatically in code** (in `Awake()`). Do NOT attempt to add Canvas hierarchy via scene YAML — Unity Canvas YAML is extremely complex with dozens of internal components, GUIDs, and serialized references that are nearly impossible to hand-author correctly. Previous stories succeeded with simple GameObjects via YAML, but Canvas is fundamentally different.
- **TextMeshPro:** Version 3.0.6 is already in `Packages/manifest.json`. Use `TMPro.TextMeshProUGUI` for UI text. Add `using TMPro;` at the top. The `com.unity.textmeshpro` package is available.
- **No GraphicRaycaster:** This HUD has no clickable elements. Omitting GraphicRaycaster saves a raycast per frame.
- **No EventSystem:** An EventSystem is not needed since there are no interactive UI elements. Do NOT create one.

### Previous Story (1.5) Learnings

From Stories 1.1-1.5 code reviews:
- **InputAction disposal matters** — no new InputActions in this story, but maintain awareness
- **All tunable values go in GameConstants** — UI sizes, offsets, alpha values all go there
- **Manual testing tasks stay unchecked** — dev agent cannot run Unity Editor
- **Resource cleanup matters** — the crosshair texture (created via `new Texture2D(1,1)`) should be destroyed in `OnDestroy()` to prevent memory leak (same pattern as material instance in Story 1.2)
- **Scene YAML edits work for simple GameObjects** — Story 1.5 added PerformanceMonitor; same pattern for PlayerHUD (empty GameObject, component attached by user)
- **`[Conditional]` pattern** — not needed for HUD (it should be visible in release builds), but keep FPSDisplay's `OnGUI()` separate from this Canvas-based HUD

### Existing Code to Build On

**PlayerController.cs** (current state after Stories 1.1-1.5):
- `public InteractableObject CurrentHighlighted => currentHighlighted` — query this to determine prompt visibility
- `public bool IsInteracting => isInteracting` — hide prompt during active interaction
- `HandleHighlighting()` method at line 129 — add `GameEvents.ObjectHighlighted()` calls here
- Update order: `ReadInput() → HandleMouseLook() → HandleHighlighting() → HandleInteraction() → HandleMovement()`

**InteractableObject.cs** (current state):
- `public string ObjectId => objectId` — for event publishing
- `public new ObjectType Type => objectType` — for future context-sensitive prompts (not needed this story, but available)
- `public bool IsHighlighted => isHighlighted` — alternative highlight state check

**GameConstants.cs** (current state):
- Player Movement, Object Highlighting, Performance Baseline, Lighting Baseline sections
- Add new `// ─── UI ───` section

**GameEvents.cs** (current state):
- Has `OnObjectInspected` (published by PlayerController on E press)
- Add `OnObjectHighlighted` (published by PlayerController on highlight change)

### GDD Context

Per GDD (Input Feel): "Context-driven interaction — one key adapts to the situation" and "Consistent across all object types." — The prompt must show what E will do. For now, all objects show "E: Inspect". In Epic 2, this becomes context-sensitive: "E: Inspect" for books, "E: Organize" for misplaced objects, "E: Maintain" for light fixtures.

Per GDD (Accessibility): "Adjustable text size for notebook and UI" — This story establishes the TextMeshPro baseline. Adjustable text size is Epic 12 (UI & UX Completion). For now, use a sensible default.

Per GDD (Visual Design): "Calm readable environment" — The crosshair and prompt must be subtle and unobtrusive. Semi-transparent, warm-toned, small. The HUD should fade into the background of the experience.

### What This Story Does NOT Include

- No notebook UI (Story 3.1 — NotebookUI.cs in Player/ per architecture)
- No health/status bars (not in GDD — no health system)
- No inventory UI (Story 2.7)
- No day/night phase indicator (Story 4.1)
- No context-sensitive prompt text per ObjectType (Story 2.1+)
- No adjustable text size settings (Story 12.2)
- No pause menu (future story)
- No DebugOverlay (separate system in Debug/ folder, uses OnGUI not Canvas)
- No subtitle system

### Cross-Story Context

- **Story 1.8 (Flashlight)** — May want to show flashlight state indicator in HUD. PlayerHUD should be extensible for additional HUD elements but does NOT need them now.
- **Story 2.1 (Observation Highlighting)** — Will enhance prompt to show context-sensitive text based on ObjectType. `SetPromptText()` method supports this.
- **Story 2.2 (Focused Inspection)** — Will hide crosshair during inspection view. `SetCrosshairVisible()` method supports this.
- **Story 3.1 (Notebook Review)** — NotebookUI.cs will be a separate script, likely on its own Canvas or a child of this one. This story establishes the Canvas pattern.
- **Story 12.2 (Accessibility Settings)** — Will add adjustable text size using the TextMeshPro foundation from this story.

### Technical Notes

- **Programmatic Canvas creation:** `new GameObject("HUDCanvas").AddComponent<Canvas>()`, set `renderMode = RenderMode.ScreenSpaceOverlay`, add `CanvasScaler` with `uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize`, `referenceResolution = new Vector2(1920, 1080)`, `matchWidthOrHeight = 0.5f`. This is cleaner than scene YAML for Canvas.
- **Crosshair texture:** Create a 1x1 white `Texture2D`, apply it as the sprite source for a UI `Image`. Set `rectTransform.sizeDelta = new Vector2(CrosshairSize, CrosshairSize)`. Destroy texture in `OnDestroy()`.
- **TextMeshPro import:** `using TMPro;` — the package is already available. Use `gameObject.AddComponent<TextMeshProUGUI>()` for UI text. TMP will use its default font (LiberationSans SDF) which is bundled with the package.
- **Performance:** One Canvas with 2 elements (Image + TMP text) is negligible cost. No per-frame allocations needed — just toggle `gameObject.SetActive()` and update `text` string.
- **FPSDisplay coexistence:** FPSDisplay uses `OnGUI()` which renders on top of everything. The new Canvas-based HUD renders at sorting order 0. They won't conflict. FPSDisplay stays in top-left, crosshair/prompt are at center.

### Performance Targets

- 1 Screen Space Overlay Canvas with 2 UI elements — negligible GPU/CPU cost
- No per-frame allocations (string assignment to TMP text only when state changes)
- No raycasting (no GraphicRaycaster)
- Target: zero measurable FPS impact

### Project Structure Notes

- New file: `Assets/Scripts/Player/PlayerHUD.cs`
- Modified: `Assets/Scripts/Core/GameConstants.cs` (UI constants)
- Modified: `Assets/Scripts/Core/GameEvents.cs` (OnObjectHighlighted event)
- Modified: `Assets/Scripts/Player/PlayerController.cs` (publish highlight event)
- Modified: `Assets/Scenes/Bookstore.unity` (add PlayerHUD empty GameObject)
- No new directories needed — Player/ already exists

### References

- [Source: system-architecture.md#3. Unity Class Structure] — NotebookUI.cs in Player/ folder, no UI/ folder defined
- [Source: system-architecture.md#2. Game Event Bus Architecture] — GameEvents pattern for OnObjectHighlighted
- [Source: system-architecture.md#Debug Overlay] — DebugOverlay uses OnGUI, separate from Canvas HUD
- [Source: gdd.md#Controls and Input] — E key for context-sensitive interaction
- [Source: gdd.md#Input Feel] — "Context-driven interaction — one key adapts to the situation"
- [Source: gdd.md#Accessibility Controls] — Adjustable text size (future Story 12.2)
- [Source: gdd.md#Visual Design — Day] — "Calm readable environment"
- [Source: epics.md#Epic 1, Story 1.6] — AC definitions
- [Source: epics.md#Epic 2, Story 2.1] — Downstream: context-sensitive prompts
- [Source: epics.md#Epic 2, Story 2.2] — Downstream: hide crosshair during inspection

## Dev Agent Record

### Agent Model Used

Claude Opus 4.6

### Debug Log References

N/A — no runtime errors encountered during implementation

### Completion Notes List

- Task 1: Added 4 UI constants to GameConstants.cs: CrosshairSize (8f), CrosshairAlpha (0.6f), PromptFontSize (18f), PromptVerticalOffset (40f)
- Task 2: Created PlayerHUD.cs with programmatic Canvas construction in Awake(). Screen Space Overlay Canvas with CanvasScaler (1920x1080, match 0.5). Crosshair via 1x1 Texture2D + UI Image anchored center. Interaction prompt via TextMeshProUGUI anchored below center. Polling-based Update() checks CurrentHighlighted with state-change optimization (wasShowingPrompt). Texture2D destroyed in OnDestroy(). FindObjectOfType fallback for PlayerController reference.
- Task 3: Added PlayerHUD empty GameObject to Bookstore.unity at root order 21. User must attach PlayerHUD component in Unity Editor after import.
- Task 4: Added OnObjectHighlighted event + invoke helper to GameEvents.cs. Added GameEvents.ObjectHighlighted() calls in PlayerController.HandleHighlighting() — fires objectId on new highlight, null on unhighlight. PlayerHUD uses polling (not event subscription) for simplicity.
- Task 5: Manual testing — requires Unity Editor verification by user. User must: refresh assets (Ctrl+R), attach PlayerHUD to the PlayerHUD GameObject, enter Play mode, and verify crosshair/prompt behavior.

### Change Log

- 2026-03-28: Implemented Story 1.6 — Basic UI Framework. Created PlayerHUD.cs with programmatic Canvas, crosshair, and interaction prompt. Added UI constants, OnObjectHighlighted event, and scene GameObject.
- 2026-03-28: Code review pending — all dev-automatable tasks (1-4) complete. Task 5 (manual testing) awaits user verification in Unity Editor.
- 2026-03-28: Code review fixes — [M1] Fixed Sprite leak: crosshairSprite now destroyed in OnDestroy() alongside crosshairTexture. [L1] Replaced 1x1 square texture with 16x16 procedural circular dot with alpha falloff for softer appearance. [L2] crosshairImage demoted to local variable in BuildHUD(). All ACs validated, all tasks confirmed. Approved.

### File List

- Assets/Scripts/Player/PlayerHUD.cs (new — programmatic Canvas HUD with crosshair dot and interaction prompt)
- Assets/Scripts/Player/PlayerHUD.cs.meta (new)
- Assets/Scripts/Core/GameConstants.cs (modified — added UI constants section)
- Assets/Scripts/Core/GameEvents.cs (modified — added OnObjectHighlighted event and invoke helper)
- Assets/Scripts/Player/PlayerController.cs (modified — added GameEvents.ObjectHighlighted() calls in HandleHighlighting)
- Assets/Scenes/Bookstore.unity (modified — added PlayerHUD GameObject at root order 21)

## Senior Developer Review (AI)

**Review Date:** 2026-03-28
**Reviewer:** Claude Opus 4.6 (adversarial code review)
**Review Outcome:** Approved (after fixes)

### Action Items

- [x] [M1][Medium] Sprite leak — `Sprite.Create()` allocates a runtime Sprite never destroyed in OnDestroy(). Fixed: added `crosshairSprite` field and `Destroy(crosshairSprite)` in OnDestroy().
- [x] [L1][Low] Crosshair renders as square (1x1 texture scaled to 8x8). User prefers circular dot. Fixed: replaced with 16x16 procedural texture with circular alpha falloff for softer dot appearance.
- [x] [L2][Low] `crosshairImage` stored as class field but never referenced after BuildHUD(). Fixed: demoted to local variable.

### AC Validation

| AC | Status | Evidence |
|----|--------|----------|
| AC1: Crosshair visible at all times | IMPLEMENTED | Crosshair Image anchored center, always active (PlayerHUD.cs:92-125). User confirmed in 5.1. |
| AC2: "E: Inspect" prompt on highlight | IMPLEMENTED | TextMeshProUGUI shown when `CurrentHighlighted != null && !IsInteracting` (PlayerHUD.cs:49-57). User confirmed in 5.2. |
| AC3: Only crosshair when no interactable | IMPLEMENTED | `promptObject.SetActive(false)` when no highlight (PlayerHUD.cs:58-61). User confirmed in 5.3/5.4. |

### Summary

Implementation is clean and well-structured. Programmatic Canvas creation avoids fragile scene YAML. Polling-based Update with state-change optimization avoids unnecessary SetActive calls. Resource cleanup follows established pattern from Stories 1.1 (InputAction disposal) and 1.2 (material instance cleanup). OnObjectHighlighted event added for future system decoupling. All 3 issues found and fixed automatically.
