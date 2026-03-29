# Story 2.6: Maintain Light Fixtures

Status: done

## Story

As a player,
I want to maintain light fixtures using context-sensitive interaction,
so that I can preserve safe zones.

## Acceptance Criteria

1. Given a light fixture has a burnt-out bulb, When the player highlights it, Then the prompt shows "Replace Bulb" (if the player has a spare) or "Needs Bulb" (if they don't)
2. Given the player has a spare bulb and initiates replacement, When the action completes, Then the fixture lights up, the spare bulb is consumed from inventory, and the zone lighting state updates
3. Given a fixture is functioning, When the player highlights it, Then no maintenance prompt appears

## Tasks / Subtasks

- [x] Task 1: Add light fixture constants to GameConstants.cs (AC: 2)
  - [x] 1.1 Add `// ─── Light Fixtures ───` section after the Organize section
  - [x] 1.2 Add `public const float BulbReplaceAnimationDuration = 0.8f;` — time for the light to fade in after replacement (slightly longer than organize for visual weight)
  - [x] 1.3 Add `public const int StartingBulbCount = 3;` — player starts with 3 spare bulbs (temporary; inventory system in 2-7 will manage this)

- [x] Task 2: Create LightFixture.cs component (AC: 1, 2, 3)
  - [x] 2.1 Create new file `Assets/Scripts/Environment/LightFixture.cs`
  - [x] 2.2 Add `[RequireComponent(typeof(Light))]` attribute
  - [x] 2.3 Add serialized fields:
    - `[Header("Fixture")]`
    - `[SerializeField] private string fixtureId;` — unique identifier for this fixture
    - `[SerializeField] private bool startBroken;` — check in Inspector to test broken state
  - [x] 2.4 Add private fields:
    - `private Light fixtureLight;` — cached Light component reference
    - `private bool isFunctional;` — current state
    - `private float baseIntensity;` — store original Light.intensity from scene for restoration
    - `private Coroutine repairCoroutine;` — for smooth light fade-in
  - [x] 2.5 Add public properties:
    - `public string FixtureId => fixtureId;`
    - `public bool IsFunctional => isFunctional;`
    - `public bool IsRepairing => repairCoroutine != null;`
  - [x] 2.6 In `Awake()`:
    - Cache `fixtureLight = GetComponent<Light>()`
    - Store `baseIntensity = fixtureLight.intensity`
    - If `startBroken`: set `fixtureLight.intensity = 0f`, `fixtureLight.enabled = false`, `isFunctional = false`
    - If `!startBroken`: set `isFunctional = true`
  - [x] 2.7 Add `public void Repair()` method:
    - Guard: if `isFunctional || repairCoroutine != null` return
    - Start `repairCoroutine = StartCoroutine(RepairAnimation())`
  - [x] 2.8 Add private `RepairAnimation()` coroutine:
    - Enable the Light: `fixtureLight.enabled = true`
    - Set `fixtureLight.intensity = 0f`
    - Lerp intensity from 0 to `baseIntensity` over `BulbReplaceAnimationDuration` using SmoothStep
    - Snap to exact `baseIntensity` at end
    - Set `isFunctional = true`
    - Set `repairCoroutine = null`
    - Fire `GameEvents.LightRepaired(fixtureId, "")` — empty zoneId for now; zones in Epic 5
  - [x] 2.9 Add `public void Break()` method (for future use by Entity/Entropy systems):
    - Set `fixtureLight.intensity = 0f`, `fixtureLight.enabled = false`
    - Set `isFunctional = false`
    - Fire `GameEvents.LightFailed(fixtureId, "")` — empty zoneId for now

- [x] Task 3: Add fixture awareness to InteractableObject.cs (AC: 1, 3)
  - [x] 3.1 Add private field after the organizeCoroutine field: `private LightFixture lightFixture;`
  - [x] 3.2 Add public property: `public LightFixture Fixture => lightFixture;`
  - [x] 3.3 In `Awake()`, after the displacement logic: `lightFixture = GetComponent<LightFixture>();`
    - This is null-safe — objects without LightFixture simply have `Fixture == null`

- [x] Task 4: Add simple bulb supply to PlayerController.cs (AC: 1, 2)
  - [x] 4.1 Add field after the `inputEnabled` field:
    - `private int spareBulbs = GameConstants.StartingBulbCount;`
  - [x] 4.2 Add public property: `public int SpareBulbs => spareBulbs;`
  - [x] 4.3 Add public method:
    ```
    public bool TryUseBulb()
    {
        if (spareBulbs <= 0) return false;
        spareBulbs--;
        return true;
    }
    ```
    Note: This is a temporary simple counter. Story 2-7 (Inventory System) will replace this with a proper inventory that tracks bulbs, tools, and capacity limits.

- [x] Task 5: Modify PlayerController.HandleInteraction() for fixture repair (AC: 1, 2, 3)
  - [x] 5.1 In `HandleInteraction()`, add a fixture check AFTER the displacement check and BEFORE the IsInspectable check:
    ```
    // Broken fixtures: repair takes priority over inspect
    if (currentHighlighted.Fixture != null
        && !currentHighlighted.Fixture.IsFunctional
        && !currentHighlighted.Fixture.IsRepairing)
    {
        if (TryUseBulb())
        {
            currentHighlighted.Fixture.Repair();
        }
        return; // Return even if no bulb — don't fall through to inspect
    }
    ```
    Priority order after this story: displaced → fixture repair → inspectable → default interact
  - [x] 5.2 The `return` after the fixture block ensures AC3: functioning fixtures (IsFunctional == true) skip this check entirely and proceed to inspect/interact normally.

- [x] Task 6: Modify PlayerHUD.Update() for fixture prompt text (AC: 1, 3)
  - [x] 6.1 In `Update()`, add fixture check AFTER the IsDisplaced check and BEFORE the IsInspectable check:
    ```
    if (playerController.CurrentHighlighted.IsDisplaced)
        promptText.text = "E: Organize";
    else if (playerController.CurrentHighlighted.Fixture != null
             && !playerController.CurrentHighlighted.Fixture.IsFunctional
             && !playerController.CurrentHighlighted.Fixture.IsRepairing)
    {
        if (playerController.SpareBulbs > 0)
            promptText.text = "E: Replace Bulb";
        else
            promptText.text = "Needs Bulb";
    }
    else if (playerController.CurrentHighlighted.IsInspectable)
        promptText.text = "E: Inspect";
    else
        promptText.text = "E: Interact";
    ```
    AC1: Shows "Replace Bulb" or "Needs Bulb" based on spare count. AC3: Functioning fixtures skip the check entirely and show normal prompts.

- [x] Task 7: Add fixture guards to InspectionSystem.cs (AC: 3)
  - [x] 7.1 Add a broken-fixture guard to the inspection entry check, matching the displacement guards pattern:
    ```
    && !(playerController.CurrentHighlighted.Fixture != null
         && !playerController.CurrentHighlighted.Fixture.IsFunctional)
    ```
    This prevents inspection from triggering on broken fixtures (same dual-InputAction issue fixed in 2-5 review).

- [x] Task 8: Manual testing verification (AC: 1, 2, 3) — **Requires Unity Editor; to be verified by user**
  - [x] 8.1 Set up test: Create a GameObject with Light, Collider, Renderer, InteractableObject (objectType=Fixture), and LightFixture components. Set `startBroken = true` on LightFixture. Set a unique `fixtureId`. Enter play mode.
  - [x] 8.2 Highlight the broken fixture — verify prompt shows "E: Replace Bulb" (AC1, with starting bulbs > 0).
  - [x] 8.3 Press E — verify the light smoothly fades in over ~0.8 seconds and the fixture is now lit (AC2).
  - [x] 8.4 After repair, highlight the fixture again — verify prompt shows "E: Inspect" (not maintenance prompt) (AC3).
  - [x] 8.5 Use up all spare bulbs (repair 3 fixtures). Highlight another broken fixture — verify prompt shows "Needs Bulb" (AC1).
  - [x] 8.6 Press E on a "Needs Bulb" fixture — verify nothing happens (no repair, no inspection).
  - [x] 8.7 Verify the repair animation uses smooth easing (not instant snap).

## Dev Notes

### Architecture Compliance

- **File:** `Assets/Scripts/Environment/LightFixture.cs` — New. MonoBehaviour for light fixture state and repair. Architecture spec defines this file at this path. MVP version — no degradation curve, no EnvironmentManager integration. Full fixture management (DegradeFixture, zone lighting scores) comes in Epic 5.
- **File:** `Assets/Scripts/Environment/InteractableObject.cs` — Modified. Adding optional LightFixture reference via GetComponent in Awake. Null-safe — non-fixture objects unaffected.
- **File:** `Assets/Scripts/Player/PlayerController.cs` — Modified. Adding fixture repair check in HandleInteraction() and temporary spareBulbs counter. The counter will be replaced by proper inventory in Story 2-7.
- **File:** `Assets/Scripts/Player/PlayerHUD.cs` — Modified. Dynamic prompt text for fixture states.
- **File:** `Assets/Scripts/Player/InspectionSystem.cs` — Modified. Adding broken-fixture guard to prevent inspection on broken fixtures (same pattern as displacement guards from 2-5).
- **File:** `Assets/Scripts/Core/GameConstants.cs` — Modified. Adding Light Fixtures constants section.

### Design Decision: Simple Bulb Counter vs Full Inventory

AC2 requires "spare bulb is consumed from inventory." Story 2-7 (Inventory System) hasn't been built yet. For this story:
- **MVP (this story):** Simple `int spareBulbs` on PlayerController with `TryUseBulb()`. No UI for bulb count (comes with inventory HUD in 2-7).
- **Full system (Story 2-7):** Proper inventory with capacity limits, item types, UI display. Will absorb the bulb counter.

This matches the pattern from 2-5 where we used simple homePosition instead of the full slot system.

### Design Decision: LightFixture as Companion Component

LightFixture is a separate MonoBehaviour on the same GameObject as InteractableObject, not a subclass or field extension. Reasons:
- Architecture spec lists it as a separate file (`Environment/LightFixture.cs`)
- Not all InteractableObjects are fixtures; companion component pattern keeps concerns separated
- InteractableObject discovers it via `GetComponent<LightFixture>()` in Awake — null when absent
- Follows Unity's composition-over-inheritance pattern

### Interaction Priority Order (After This Story)

In PlayerController.HandleInteraction():
1. **Displaced?** → Organize (from 2-5)
2. **Broken fixture?** → Repair if has bulb, else block (this story)
3. **Inspectable?** → return (InspectionSystem handles via its own E binding)
4. **Default** → Interact (generic non-inspectable interaction)

### What This Story Does NOT Include

- **No degradation curve** — Fixtures are binary (functional/broken). Gradual degradation (ObjectCondition.Degraded) comes in Epic 5.
- **No EnvironmentManager** — Zone lighting scores, DegradeFixture/RepairFixture API comes in Epic 5.
- **No bulb count UI** — Player can't see how many bulbs they have. Inventory HUD comes in 2-7.
- **No entity-caused failures** — Break() method exists for future use but nothing calls it yet. Entity light interference comes in Epic 6.
- **No repair audio/VFX** — Audio comes in Epic 9.
- **No power distribution** — Managing which fixtures get power comes in Epic 5.

### Previous Story (2-5) Learnings

- **Dual InputAction bug.** InspectionSystem has its own E key InputAction that fires in the same frame as PlayerController's. Story 2-5 review caught this — must add guards to InspectionSystem for any new interaction type. Task 7 handles this for fixtures.
- **Prompt text must update every frame.** Story 2-5 fixed a bug where prompt text only set on first show. Current PlayerHUD.Update() already updates text every frame — just add the new fixture conditional.
- **SmoothStep for animations.** Proven pattern across InspectionSystem camera, OrganizeAnimation, highlight fades. RepairAnimation uses the same approach.
- **Companion component pattern.** InteractableObject already uses GetComponent for Renderer/Collider. LightFixture follows the same discovery pattern.
- **GameEvents fire-and-forget.** LightRepaired/LightFailed already declared in GameEvents.cs with invoke helpers. Just call them.

### Existing Code to Build On

**InteractableObject.cs (287 lines):**
- Awake() at line 69-83: Add `lightFixture = GetComponent<LightFixture>()` after displacement logic
- Already has objectType field — fixtures use `ObjectType.Fixture`

**PlayerController.cs (233 lines):**
- HandleInteraction() at line 167-188: Add fixture check after displacement check (line 179) and before IsInspectable check (line 182)
- spareBulbs field goes with other private state fields around line 32

**PlayerHUD.cs (179 lines):**
- Update() at line 61-88: Add fixture conditional between IsDisplaced and IsInspectable checks

**InspectionSystem.cs (210 lines):**
- Update() at line 75-82: Add broken-fixture guard to entry conditions (same location as displacement guards)

**GameConstants.cs (77 lines):**
- Last section is Organize (lines 74-76). New Light Fixtures section goes after.

**GameEvents.cs (103 lines):**
- LightFailed(fixtureId, zoneId) at line 57-58: Already exists, just call it
- LightRepaired(fixtureId, zoneId) at line 59-60: Already exists, just call it

### Performance Targets

- No new per-frame operations beyond the existing Update() conditional check (one null check + one bool read)
- RepairAnimation coroutine: runs for 0.8s, single float lerp — trivial cost
- Break() and Repair() are one-shot event handlers, not per-frame
- GetComponent<LightFixture>() in Awake only — cached reference
- Target: < 0.01ms frame time impact

### Project Structure Notes

- New: `Assets/Scripts/Environment/LightFixture.cs` (fixture state and repair)
- Modified: `Assets/Scripts/Environment/InteractableObject.cs` (optional LightFixture reference)
- Modified: `Assets/Scripts/Player/PlayerController.cs` (fixture repair check, spareBulbs counter)
- Modified: `Assets/Scripts/Player/PlayerHUD.cs` (fixture prompt text)
- Modified: `Assets/Scripts/Player/InspectionSystem.cs` (broken-fixture guard)
- Modified: `Assets/Scripts/Core/GameConstants.cs` (light fixture constants)
- No scene YAML changes (user configures fixtures in Inspector)

### References

- [Source: epics.md#Epic 2, Story 2.6] — AC definitions and story statement
- [Source: gdd.md#Mechanic 5: Maintain] — "Players preserve environmental stability through upkeep — managing lighting sources"
- [Source: gdd.md#Pillar 4: Light Creates Safety] — "Maintaining lighting conditions is essential to navigation, observation, and survival"
- [Source: gdd.md#Resource Scarcity, Light Economy] — "Players maintain lighting by replacing burnt-out bulbs, repairing damaged fixtures"
- [Source: system-architecture.md#LightFixture.cs] — Listed in Environment/ folder
- [Source: system-architecture.md#GameEvents] — OnLightFailed, OnLightRepaired events
- [Source: system-architecture.md#EnvironmentManager] — DegradeFixture, RepairFixture API (full implementation deferred to Epic 5)
- [Source: system-architecture.md#ObjectType] — Fixture enum value already exists
- [Source: epics.md#Story 2.7] — Downstream: Inventory system will absorb spareBulbs counter
- [Source: epics.md#Epic 5] — Downstream: EnvironmentManager will absorb fixture state management

## Dev Agent Record

### Agent Model Used

Claude Opus 4.6

### Debug Log References

None — no runtime errors encountered during implementation.

### Completion Notes List

- Task 1: Added BulbReplaceAnimationDuration (0.8f) and StartingBulbCount (3) constants to GameConstants.cs
- Task 2: Created LightFixture.cs with RequireComponent(Light), serialized fixtureId/startBroken, Awake() caches Light and baseIntensity, Repair() with SmoothStep fade-in coroutine, Break() for future entity use, fires GameEvents.LightRepaired/LightFailed
- Task 3: Added LightFixture companion reference to InteractableObject via GetComponent in Awake(), exposed as Fixture property (null for non-fixtures)
- Task 4: Added temporary spareBulbs counter to PlayerController with TryUseBulb() method — to be replaced by inventory system in 2-7
- Task 5: Added fixture repair check in HandleInteraction() between displacement and inspectable checks — consumes bulb via TryUseBulb(), returns even without bulb to prevent fallthrough to inspect
- Task 6: Added fixture prompt text to PlayerHUD — "E: Replace Bulb" when has bulbs, "Needs Bulb" when empty, functioning fixtures show normal prompts
- Task 7: Added broken-fixture guard to InspectionSystem entry check — prevents dual-InputAction inspection trigger on broken fixtures

### File List

- `Assets/Scripts/Environment/LightFixture.cs` — New. Fixture state, Repair/Break methods, RepairAnimation coroutine
- `Assets/Scripts/Environment/InteractableObject.cs` — Added LightFixture field, Fixture property, GetComponent in Awake
- `Assets/Scripts/Player/PlayerController.cs` — Added spareBulbs counter, TryUseBulb(), fixture repair check in HandleInteraction
- `Assets/Scripts/Player/PlayerHUD.cs` — Added fixture prompt text conditionals
- `Assets/Scripts/Player/InspectionSystem.cs` — Added broken-fixture guard to inspection entry
- `Assets/Scripts/Core/GameConstants.cs` — Added Light Fixtures constants section

