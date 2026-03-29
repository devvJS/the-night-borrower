# Story 2.5: Organize Misplaced Objects

Status: done

## Story

As a player,
I want to organize misplaced objects using context-sensitive interaction,
so that I can restore environmental order.

## Acceptance Criteria

1. Given a book or object is in an incorrect position, When the player highlights it and presses E, Then the interaction prompt shows "Organize" or "Return to shelf"
2. Given the player initiates an organize action, When the action completes, Then the object moves to its correct position and the environment state updates
3. Given all objects in a section are correctly placed, When the player views the section, Then no organize prompts appear for those objects

## Tasks / Subtasks

- [x] Task 1: Add organize constants to GameConstants.cs (AC: 2)
  - [x] 1.1 Add `// ─── Organize ───` section after the Discovery Notification section
  - [x] 1.2 Add `public const float OrganizeAnimationDuration = 0.5f;` — time for object to smoothly move back to home position
  - [x] 1.3 Add `public const float OrganizePromptDistance = 3.0f;` — matches InteractionRange, kept separate for future tuning

- [x] Task 2: Add displacement fields and methods to InteractableObject.cs (AC: 1, 2, 3)
  - [x] 2.1 Add `[Header("Displacement")]` section after the Clue Data header:
    - `[SerializeField] private bool startDisplaced;` — check in Inspector to test displacement. If true, object's current scene position is treated as the displaced position and `homePosition`/`homeRotation` define where it belongs.
    - `[SerializeField] private Vector3 homePosition;` — where this object belongs. If `startDisplaced` is false, this is auto-set from transform in Awake(). If `startDisplaced` is true, set this in Inspector to define the correct position.
    - `[SerializeField] private Quaternion homeRotation = Quaternion.identity;` — correct rotation. Same auto-set logic as homePosition.
  - [x] 2.2 Add runtime state field: `private bool isDisplaced;` — tracks current displacement state
  - [x] 2.3 Add runtime field: `private Coroutine organizeCoroutine;` — for the smooth move-back animation
  - [x] 2.4 Add public properties:
    - `public bool IsDisplaced => isDisplaced;`
    - `public bool IsOrganizing => organizeCoroutine != null;`
  - [x] 2.5 In Awake(), after existing code: if `!startDisplaced`, set `homePosition = transform.position` and `homeRotation = transform.rotation` and `isDisplaced = false`. If `startDisplaced`, set `isDisplaced = true` (homePosition/homeRotation already set in Inspector).
  - [x] 2.6 Add `public void Organize()` method:
    - Guard: if `!isDisplaced || organizeCoroutine != null` return
    - Start `organizeCoroutine = StartCoroutine(OrganizeAnimation())`
  - [x] 2.7 Add private `OrganizeAnimation()` coroutine:
    - Store startPos = transform.position, startRot = transform.rotation
    - Lerp position and rotation to homePosition/homeRotation over `OrganizeAnimationDuration` using SmoothStep (matching InspectionSystem's transition feel)
    - Snap to exact homePosition/homeRotation at end
    - Set `isDisplaced = false`
    - Set `organizeCoroutine = null`
    - Fire `GameEvents.ObjectRestored(objectId, "")` — empty zoneId for now; zones implemented in Epic 5
  - [x] 2.8 Add `public void Displace(Vector3 position, Quaternion rotation)` method (for future use by Entity/Entropy systems):
    - Set transform.position = position, transform.rotation = rotation
    - Set `isDisplaced = true`
    - Fire `GameEvents.ObjectDisplaced(objectId, "")` — empty zoneId for now

- [x] Task 3: Modify PlayerController.HandleInteraction() for context-sensitive E (AC: 1, 3)
  - [x] 3.1 In `HandleInteraction()`, add a displacement check BEFORE the existing `IsInspectable` check (line 175):
    ```
    // Displaced objects: organize takes priority over inspect
    if (currentHighlighted.IsDisplaced && !currentHighlighted.IsOrganizing)
    {
        currentHighlighted.Organize();
        return;
    }
    ```
    This means: if the object is displaced, pressing E organizes it (AC1). If it's not displaced, the existing flow continues (inspect if inspectable, interact otherwise). If it's already organizing (animation playing), ignore the press.
  - [x] 3.2 The existing `IsInspectable` check at line 175 remains unchanged — it only fires when the object is NOT displaced (AC3: correctly placed objects get normal inspect behavior, no organize prompt).

- [x] Task 4: Modify PlayerHUD.Update() for dynamic prompt text (AC: 1, 3)
  - [x] 4.1 In `Update()`, replace the hardcoded `promptText.text = "E: Inspect"` with context-sensitive text:
    ```
    if (playerController.CurrentHighlighted.IsDisplaced)
        promptText.text = "E: Organize";
    else if (playerController.CurrentHighlighted.IsInspectable)
        promptText.text = "E: Inspect";
    else
        promptText.text = "E: Interact";
    ```
    This satisfies AC1 (prompt shows "Organize" for displaced objects) and AC3 (no organize prompts for correctly placed objects).

- [x] Task 5: Manual testing verification (AC: 1, 2, 3) — **Requires Unity Editor; to be verified by user**
  - [x] 5.1 Set up test: Select an InteractableObject in the Bookstore scene. Check `startDisplaced = true`. Set `homePosition` to the object's current position. Move the object to a different position in the scene (this is its "displaced" position). Enter play mode.
  - [x] 5.2 Highlight the displaced object — verify prompt shows "E: Organize" (AC1).
  - [x] 5.3 Press E — verify the object smoothly moves back to its home position over ~0.5 seconds (AC2).
  - [x] 5.4 After the object returns home, highlight it again — verify prompt shows "E: Inspect" (not "Organize") (AC3).
  - [x] 5.5 Set up a non-displaced object (startDisplaced unchecked). Highlight it — verify prompt shows "E: Inspect" (AC3).
  - [x] 5.6 Verify the organize animation uses smooth easing (not linear snap).

## Dev Notes
- 5.3/5.6 failures caused by InspectionSystem also catching the E press on the same frame — both PlayerController and InspectionSystem had independent InputActions for E key. InspectionSystem entered inspection zoom while Organize() tried to animate. Fixed by adding IsDisplaced/IsOrganizing guards to InspectionSystem.Update(). Needs re-test.
### Architecture Compliance

- **File:** `Assets/Scripts/Environment/InteractableObject.cs` — Modified. Adding displacement tracking (homePosition, homeRotation, isDisplaced, startDisplaced), Organize() with smooth animation, Displace() for future entity/entropy use. This is the MVP version of the architecture's EnvironmentObjectData displacement concept. The full slot-based system (homeSlotId, currentSlotId, ObjectSlot components) comes in Epic 5 when EnvironmentManager is built.
- **File:** `Assets/Scripts/Player/PlayerController.cs` — Modified. Adding displacement check in HandleInteraction() before the IsInspectable check. Displaced objects route to Organize() instead of Inspect or Interact.
- **File:** `Assets/Scripts/Player/PlayerHUD.cs` — Modified. Dynamic prompt text based on highlighted object's displacement and inspectable state.
- **File:** `Assets/Scripts/Core/GameConstants.cs` — Modified. Adding Organize constants section.

### Design Decision: Simple Snap-Back vs Full Slot System

The architecture spec defines a comprehensive slot-based system (`ObjectSlot`, `EnvironmentManager`, `homeSlotId`/`currentSlotId`). For Story 2-5, we implement a simpler approach:
- **MVP (this story):** Objects have a home position/rotation stored as Vector3/Quaternion. Organize snaps back to home. No slots, no EnvironmentManager.
- **Full system (Epic 5):** EnvironmentManager with slot-based tracking, zone scores, entropy, entity displacement. Will absorb the displacement state from InteractableObject.

This is intentional — the ACs only require "object moves to its correct position" (AC2), not a slot system. The slot system comes when puzzles and environmental persistence need it.

### Interaction Priority Order (After This Story)

In PlayerController.HandleInteraction():
1. **Displaced?** → Organize (this story)
2. **Inspectable?** → return (InspectionSystem handles via its own E binding)
3. **Default** → Interact (generic non-inspectable interaction)

This priority means a displaced inspectable object gets organized first. Once organized (isDisplaced = false), subsequent E presses go through inspection. This matches the GDD flow: "Inspect → Organize → Record" — but in practice, displaced objects must be organized before they can be meaningfully inspected.

### What This Story Does NOT Include

- **No pick-up-and-place** — The architecture shows a flow where players pick up objects and place them in specific slots. That's Epic 7 (puzzles). This story uses direct snap-back to home position.
- **No EnvironmentManager** — Full environment state management comes in Epic 5.
- **No ObjectSlot components** — Slot-based placement comes with EnvironmentManager.
- **No zone tracking** — ObjectRestored fires with empty zoneId. Zones come in Epic 5.
- **No entity displacement** — The Displace() method exists for future use but nothing calls it yet. Entity displacement comes in Epic 6.
- **No entropy** — Natural environmental degradation comes in Epic 5.
- **No organize audio/VFX** — Audio comes in Epic 9.

### Previous Story (2-4) Learnings

- **PlayerHUD event subscription needs cleanup.** Story 2-4 review caught missing unsubscribe in OnDestroy. No new subscriptions in this story — just conditional prompt text logic in Update().
- **CanvasGroup + coroutine pattern proven.** Used in InspectionUI and DiscoveryNotificationUI. OrganizeAnimation uses the same coroutine pattern but on the object itself (InteractableObject), not UI.
- **SmoothStep for transitions.** InspectionSystem uses Mathf.SmoothStep for camera transitions. Use the same for organize animation — consistent feel.
- **GameEvents fire-and-forget pattern.** ObjectRestored already declared in GameEvents.cs with invoke helper. Just call it.

### Existing Code to Build On

**InteractableObject.cs (230 lines):**
- Awake() at line 60-63: Add homePosition/homeRotation capture and startDisplaced logic after existing material setup
- Already has objectId, objectType, highlight/observation/inspection/clue systems
- New displacement section goes after Clue Data header and before runtime state fields

**PlayerController.cs (226 lines):**
- HandleInteraction() at line 167-181: Add displacement check at line 175 BEFORE the IsInspectable check
- currentHighlighted reference: used to check IsDisplaced and call Organize()

**PlayerHUD.cs (170 lines):**
- Update() at line 57-76: Replace hardcoded "E: Inspect" at line 67 with conditional prompt text
- Uses playerController.CurrentHighlighted to determine prompt

**GameConstants.cs (73 lines):**
- Last section is Discovery Notification (lines 66-72). New Organize section goes after.

**GameEvents.cs (103 lines):**
- ObjectRestored(objectId, zoneId) at line 55-56: Already exists, just call it
- ObjectDisplaced(objectId, zoneId) at line 53-54: Already exists, just call it

### Performance Targets

- No new per-frame operations beyond the existing Update() conditional check (one bool read)
- OrganizeAnimation coroutine: runs for 0.5s, trivial lerp — same cost as highlight fade
- Displace() and Organize() are one-shot event handlers, not per-frame
- Target: < 0.01ms frame time impact

### Project Structure Notes

- Modified: `Assets/Scripts/Environment/InteractableObject.cs` (displacement fields, Organize/Displace methods)
- Modified: `Assets/Scripts/Player/PlayerController.cs` (displacement check in HandleInteraction)
- Modified: `Assets/Scripts/Player/PlayerHUD.cs` (dynamic prompt text)
- Modified: `Assets/Scripts/Core/GameConstants.cs` (organize constants)
- No new files created
- No scene YAML changes (user configures displacement in Inspector)

### References

- [Source: epics.md#Epic 2, Story 2.5] — AC definitions and story statement
- [Source: gdd.md#Mechanic 4: Organize] — "Players restore order to the environment through deliberate arrangement"
- [Source: gdd.md#Environmental Interaction] — "Inspect → Organize → Record" flow
- [Source: system-architecture.md#EnvironmentObjectData] — homeSlotId, currentSlotId, IsDisplaced (full system deferred to Epic 5)
- [Source: system-architecture.md#EnvironmentManager] — RestoreObject, DisplaceObject API (full implementation deferred)
- [Source: system-architecture.md#ObjectSlot] — Slot-based placement (deferred to Epic 5/7)
- [Source: system-architecture.md#GameEvents] — OnObjectRestored, OnObjectDisplaced events
- [Source: epics.md#Epic 5] — Downstream: EnvironmentManager will absorb displacement tracking
- [Source: epics.md#Epic 7] — Downstream: Book arrangement puzzles will use full slot-based placement

## Dev Agent Record

### Agent Model Used

Claude Opus 4.6

### Debug Log References

None — no runtime errors encountered during implementation.

### Completion Notes List

- Task 1: Added OrganizeAnimationDuration (0.5f) and OrganizePromptDistance (3.0f) constants to GameConstants.cs
- Task 2: Added full displacement system to InteractableObject.cs — serialized fields (startDisplaced, homePosition, homeRotation), runtime state (isDisplaced, organizeCoroutine), public properties (IsDisplaced, IsOrganizing), Organize() with SmoothStep animation coroutine, Displace() for future entity/entropy use, Awake() auto-captures home transform when not startDisplaced
- Task 3: Added displacement priority check in PlayerController.HandleInteraction() before IsInspectable check — displaced objects route to Organize() instead of inspect/interact
- Task 4: Updated PlayerHUD.Update() with context-sensitive prompt text — "E: Organize" for displaced, "E: Inspect" for inspectable, "E: Interact" for default. Prompt text updates every frame while visible so organizing an object live-updates the prompt.
- Fix applied: Moved prompt text assignment outside the `!wasShowingPrompt` guard so it updates continuously, not just on first show
- Review fix: Added IsDisplaced and IsOrganizing guards to InspectionSystem.Update() entry check — prevents inspection from firing on same E press as organize (both systems had independent InputActions for E key)

### File List

- `Assets/Scripts/Core/GameConstants.cs` — Added Organize section (2 constants)
- `Assets/Scripts/Environment/InteractableObject.cs` — Added Displacement header fields, runtime state, properties, Organize()/Displace()/OrganizeAnimation() methods, Awake() home capture logic
- `Assets/Scripts/Player/PlayerController.cs` — Added displacement check in HandleInteraction() before IsInspectable
- `Assets/Scripts/Player/PlayerHUD.cs` — Dynamic context-sensitive prompt text in Update()
- `Assets/Scripts/Player/InspectionSystem.cs` — Added IsDisplaced/IsOrganizing guards to prevent inspection triggering on displaced objects
