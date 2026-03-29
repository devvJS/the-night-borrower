# Story 1.8: Flashlight Toggle

Status: done

## Story

As a player,
I want to toggle the flashlight with F,
so that I have a portable light source.

## Acceptance Criteria

1. Given the flashlight is off, When the player presses F, Then the flashlight activates and casts a directional light cone from the player's position
2. Given the flashlight is on, When the player presses F, Then the flashlight deactivates and the light cone disappears
3. Given the flashlight is on, When the player looks around, Then the light cone follows camera direction with no visible lag

## Tasks / Subtasks

- [x] Task 1: Add flashlight constants to GameConstants.cs (AC: 1, 2, 3)
  - [x] 1.1 Add `// ─── Flashlight ───` section after the Save section
  - [x] 1.2 Add `public const float FlashlightRange = 15f;` — how far the spot light reaches
  - [x] 1.3 Add `public const float FlashlightSpotAngle = 45f;` — cone width in degrees (standard flashlight feel)
  - [x] 1.4 Add `public const float FlashlightIntensity = 1.5f;` — bright enough to illuminate but not wash out the scene
  - [x] 1.5 Add `public const float FlashlightInnerSpotAngle = 25f;` — inner cone for brighter center hotspot

- [x] Task 2: Create PlayerFlashlight.cs in Player/ (AC: 1, 2, 3)
  - [x] 2.1 Create `Assets/Scripts/Player/PlayerFlashlight.cs` — `public class PlayerFlashlight : MonoBehaviour`
  - [x] 2.2 Add `[SerializeField] private Transform cameraTransform;` — the camera transform to parent the light to. In `Awake()`, if null, find via `GetComponentInChildren<Camera>().transform` (same pattern as PlayerController).
  - [x] 2.3 Create a Spot Light programmatically in `Awake()`:
    - Create `new GameObject("FlashlightLight")`, parent it to `cameraTransform`
    - Add `Light` component, set `type = LightType.Spot`
    - Set `range = GameConstants.FlashlightRange`
    - Set `spotAngle = GameConstants.FlashlightSpotAngle`
    - Set `innerSpotAngle = GameConstants.FlashlightInnerSpotAngle`
    - Set `intensity = GameConstants.FlashlightIntensity`
    - Set `color = Color.white` (neutral — will be tuned in polish epics)
    - Set `shadows = LightShadows.Soft` for realistic shadow casting
    - Position at `Vector3.zero` local, rotation at `Quaternion.identity` local — inherits camera direction automatically (AC3: no lag)
    - Set `gameObject.SetActive(false)` — flashlight starts OFF
  - [x] 2.4 Add InputAction for F key: `new InputAction("Flashlight", InputActionType.Button, "<Keyboard>/f")`
  - [x] 2.5 Enable/Disable/Dispose InputAction in OnEnable/OnDisable/OnDestroy — same lifecycle as PlayerController
  - [x] 2.6 In `Update()`: if `flashlightAction.WasPressedThisFrame()` → toggle the light GameObject active state. Store `private bool isOn;` field.
  - [x] 2.7 Add `public bool IsOn => isOn;` property for future systems (HUD indicator in Story 1.6 cross-ref, night phase in Story 4.3)
  - [x] 2.8 Required usings: `using UnityEngine;`, `using UnityEngine.InputSystem;`

- [x] Task 3: Add PlayerFlashlight to Player GameObject in Bookstore scene (AC: 1, 2, 3)
  - [x] 3.1 The PlayerFlashlight component should be attached to the same Player GameObject that has PlayerController — it needs access to the child Camera. **User must attach the PlayerFlashlight component in Unity Editor** after import.
  - [x] 3.2 No scene YAML edit needed — the component needs the camera reference which is auto-discovered in Awake() via GetComponentInChildren<Camera>().

- [x] Task 4: Manual testing verification (AC: 1, 2, 3) — **Requires Unity Editor; to be verified by user**
  - [x] 4.1 Enter play mode — press F — verify a visible light cone appears casting forward from the player's camera
  - [x] 4.2 Press F again — verify the light cone disappears completely
  - [x] 4.3 With flashlight on, look around with mouse — verify the light cone tracks camera direction with zero visible lag
  - [x] 4.4 Walk into a darker area of the scene — verify the flashlight visibly illuminates surfaces and casts shadows
  - [x] 4.5 Rapid toggle (press F quickly several times) — verify no errors, light toggles cleanly each time

## Dev Notes
- User confirms all working flashlight functionality. The flashlight is a child of the camera, so it tracks perfectly with no lag. The F key toggles it on and off, and it casts a visible cone of light with soft shadows. All AC are met.
- One minor note: When getting close to walls, the flashlight cone appears as a big bright white circle on the wall due to the innerSpotAngle creating a bright hotspot. This is expected behavior and can be tuned in future polish epics if needed. Overall, the implementation is nice but true flashlight behavior when approaching a wall may require additional tuning of the inner/outer spot angles and intensity.
### Architecture Compliance

- **File:** `Assets/Scripts/Player/PlayerFlashlight.cs` — New MonoBehaviour in `Player/`. The architecture places player-related scripts in `Player/` (PlayerController.cs, PlayerHUD.cs). A separate script keeps flashlight logic decoupled from PlayerController (which is already 200 lines with movement, look, highlighting, and interaction).
- **File:** `Assets/Scripts/Core/GameConstants.cs` — Existing file. Add Flashlight constants section.
- **No separate Flashlight class in architecture:** The architecture defines `LightFixture.cs` in `Environment/` for scene-based degradable lights. The player flashlight is fundamentally different — it's a player equipment item, not an environment fixture. It does NOT use LightFixture.cs.
- **Light as child of Camera:** By parenting the Spot Light to the camera transform, it inherits camera rotation automatically every frame. This is the standard Unity approach for flashlight — zero lag, zero per-frame code needed for tracking (AC3).
- **Programmatic Light creation:** Same approach as PlayerHUD programmatic Canvas — avoids scene YAML complexity for components with many serialized fields.

### What This Story Does NOT Include

- **No battery/power mechanic** — GDD describes flashlight as always-available toggle. No duration limits, degradation, or power consumption. Battery mechanics are not in any epic.
- **No HUD indicator** — Flashlight on/off state is not shown in HUD. Could be added to PlayerHUD in future stories. The `IsOn` property supports this.
- **No sound effects** — Click/toggle audio comes in Epic 9 (Atmosphere & Tension Systems).
- **No interaction with zone stability** — Per architecture, only maintained fixtures contribute to zone lighting scores. Player flashlight is personal equipment.
- **No night-specific behavior** — Flashlight works the same in all phases. Night phase reduced visibility (Story 4.3) makes it more useful but doesn't change its behavior.
- **No save/load of flashlight state** — The placeholder save system (Story 1.7) doesn't track flashlight. Flashlight resets to off on scene load. Future save expansion may persist this.

### Previous Story (1.7) Learnings

From Stories 1.1-1.7 code reviews:
- **InputAction lifecycle is critical** — Enable in OnEnable, Disable in OnDisable, Dispose in OnDestroy. Proven pattern across PlayerController and SaveManager.
- **Resource cleanup in OnDestroy()** — Dispose InputActions. The programmatic Light GameObject is a child of camera and will be destroyed with the player hierarchy — no manual cleanup needed.
- **All tunable values go in GameConstants** — Flashlight range, angle, intensity all go there.
- **DontDestroyOnLoad pattern** — Not needed for PlayerFlashlight since it's attached to the Player GameObject which lives in the scene. SaveManager needed it because it's a standalone singleton.
- **Programmatic creation preferred for complex Unity objects** — Light component has many properties; setting them in code is clearer and more maintainable than scene YAML.
- **Manual testing tasks stay unchecked** — Dev agent cannot run Unity Editor.
- **Sprite/Material leak pattern** — Not applicable here. The Light component is a standard Unity component, not a runtime-created resource.

### Existing Code to Build On

**PlayerController.cs** (current state after Stories 1.1-1.7):
- `cameraTransform = GetComponentInChildren<Camera>().transform` — same discovery pattern for PlayerFlashlight
- InputAction pattern: inline `new InputAction()`, Enable/Disable/Dispose lifecycle
- `WasPressedThisFrame()` for single-press detection
- 200 lines — do NOT add flashlight logic here; use separate component

**PlayerHUD.cs** (current state):
- `SetCrosshairVisible(bool)` — available for future stories that may hide crosshair during flashlight-specific UI
- No modification needed this story

**GameConstants.cs** (current state):
- Sections: Player Movement, Object Highlighting, Performance Baseline, Lighting Baseline, UI, Save
- Add new `// ─── Flashlight ───` section

**GameEnums.cs** (current state):
- No new enums needed for this story

### Technical Notes

- **Spot Light:** Unity's `LightType.Spot` creates a cone of light. `spotAngle` is the outer cone angle, `innerSpotAngle` is the bright center. `range` controls how far it reaches. `shadows = LightShadows.Soft` gives realistic shadow edges.
- **Parenting to camera:** `flashlightLight.transform.SetParent(cameraTransform, false)` with `worldPositionStays=false` ensures local position/rotation are relative to camera. With local position (0,0,0) and rotation identity, the light points exactly where the camera looks.
- **Performance:** One additional Spot Light with soft shadows. Impact depends on shadow resolution and number of shadow-casting objects in range. For the current simple scene, this is negligible. Epic 15 (Performance Optimization) will profile if needed.
- **No flicker/sway effects:** This is a basic toggle. Visual polish (subtle sway, warm-up delay, flicker) belongs in Epic 13 (Audio & Visual Finalization).

### Performance Targets

- 1 additional Spot Light — acceptable GPU cost for current scene complexity
- Zero per-frame code for light tracking (parented to camera)
- One `WasPressedThisFrame()` check per frame — negligible
- Target: < 1ms frame time impact

### Project Structure Notes

- New file: `Assets/Scripts/Player/PlayerFlashlight.cs`
- Modified: `Assets/Scripts/Core/GameConstants.cs` (Flashlight constants)
- No scene YAML changes — user attaches component in Editor
- No new directories needed — Player/ already exists

### References

- [Source: epics.md#Epic 1, Story 1.8] — AC definitions
- [Source: gdd.md#Controls and Input] — F key for flashlight toggle (medium frequency)
- [Source: gdd.md#Visual Design — Night] — Cold desaturated tones, reduced visibility, flashlight compensates
- [Source: gdd.md#Lighting as Core Mechanic] — Fixed fixtures are maintainable; flashlight is always-available player equipment
- [Source: system-architecture.md#3. Unity Class Structure] — Player/ folder for player scripts
- [Source: system-architecture.md#LightFixture.cs] — Environment lights, NOT player flashlight
- [Source: epics.md#Epic 4, Story 4.3] — Downstream: flashlight provides focused cone during night phase
- [Source: epics.md#Epic 8, Story 8.2] — Downstream: basement relies heavily on flashlight

## Dev Agent Record

### Agent Model Used

Claude Opus 4.6

### Debug Log References

N/A — no runtime errors encountered during implementation

### Completion Notes List

- Task 1: Added 4 Flashlight constants to GameConstants.cs: FlashlightRange (15f), FlashlightSpotAngle (45f), FlashlightInnerSpotAngle (25f), FlashlightIntensity (1.5f).
- Task 2: Created PlayerFlashlight.cs in Player/ with programmatic Spot Light creation in Awake(). Light parented to camera transform for automatic direction tracking (AC3). F key InputAction with full Enable/Disable/Dispose lifecycle. Toggle via WasPressedThisFrame() with isOn bool and SetActive(). Soft shadows enabled. Light starts off. Public IsOn property for future systems.
- Task 3: No scene YAML edit — user attaches PlayerFlashlight component to Player GameObject in Unity Editor. Camera transform auto-discovered via GetComponentInChildren<Camera>().
- Task 4: Manual testing — requires Unity Editor verification by user. User must: refresh assets (Ctrl+R), select Player GameObject, Add Component → PlayerFlashlight, enter Play mode, test F key toggle / camera tracking / shadow casting.

### Change Log

- 2026-03-28: Implemented Story 1.8 — Flashlight Toggle. Created PlayerFlashlight.cs with programmatic Spot Light, F key toggle, and camera-parented tracking. Added Flashlight constants to GameConstants.cs.
- 2026-03-28: Code review — clean review, no issues found. All ACs validated. Approved.

### File List

- Assets/Scripts/Player/PlayerFlashlight.cs (new — MonoBehaviour with programmatic Spot Light, F key toggle, camera-parented for zero-lag tracking)
- Assets/Scripts/Player/PlayerFlashlight.cs.meta (new)
- Assets/Scripts/Core/GameConstants.cs (modified — added Flashlight constants section)

## Senior Developer Review (AI)

**Review Date:** 2026-03-28
**Reviewer:** Claude Opus 4.6 (adversarial code review)
**Review Outcome:** Approved (clean)

### Action Items

None — clean review.

### AC Validation

| AC | Status | Evidence |
|----|--------|----------|
| AC1: F activates directional light cone | IMPLEMENTED | Spot Light with range/angle/intensity (PlayerFlashlight.cs:26-33), toggled via SetActive (line 60) |
| AC2: F deactivates light cone | IMPLEMENTED | isOn toggle → SetActive(false) (PlayerFlashlight.cs:59-60) |
| AC3: Light follows camera with no lag | IMPLEMENTED | Light parented to cameraTransform (line 24), inherits rotation automatically |

### Summary

Clean, minimal implementation. 65 lines with single responsibility. Spot Light programmatically created and parented to camera for zero-lag tracking. InputAction lifecycle follows established pattern (PlayerController, SaveManager). GameConstants for all tunables. Correctly separated from PlayerController. No issues found.
