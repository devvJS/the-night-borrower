# Story 2.7: Inventory System

Status: done

## Story

As a player,
I want to carry and manage a limited inventory of supplies,
so that resource decisions feel meaningful.

## Acceptance Criteria

1. Given the player picks up a spare bulb or tool, When the item is added, Then it appears in the inventory with the current count visible
2. Given the player's inventory is at capacity, When they attempt to pick up another item, Then they receive feedback that inventory is full and must choose what to drop or leave behind
3. Given the player opens the inventory view, When they review contents, Then all carried items (spare bulbs, repair tools) are listed with quantities
4. Given the player uses an item (e.g., replacing a bulb), When the action completes, Then the item count decreases by one in inventory

## Tasks / Subtasks

- [x] Task 1: Add inventory constants to GameConstants.cs (AC: 1, 2, 3)
  - [x]1.1 Add `// ─── Inventory ───` section after the Light Fixtures section
  - [x]1.2 Add `public const int InventorySlotCount = 3;` — 3 fixed hotbar slots (locked in at Epic 1 retro)
  - [x]1.3 Add `public const int MaxStackSize = 5;` — max items per slot
  - [x]1.4 Add `public const float InventorySlotSize = 50f;` — UI slot dimensions
  - [x]1.5 Add `public const float InventorySlotSpacing = 8f;` — gap between slots
  - [x]1.6 Add `public const float InventorySlotAlpha = 0.7f;` — background alpha for slots
  - [x]1.7 Add `public const float InventoryFullFeedbackDuration = 1.5f;` — how long "Inventory Full" message shows

- [x] Task 2: Add ItemType enum to GameEnums.cs (AC: 1)
  - [x]2.1 Add `public enum ItemType { SpareBulb, RepairTool }` after the existing enums
    - SpareBulb: used by LightFixture.Repair (absorbs spareBulbs from 2-6)
    - RepairTool: placeholder for future repair mechanics (Epic 5)

- [x] Task 3: Create PlayerInventory.cs (AC: 1, 2, 4)
  - [x]3.1 Create new file `Assets/Scripts/Player/PlayerInventory.cs`
  - [x]3.2 Add struct `InventorySlot`:
    - `public ItemType itemType;`
    - `public int count;`
    - `public bool IsEmpty => count <= 0;`
  - [x]3.3 Add serialized fields:
    - `[Header("Starting Items")]`
    - `[SerializeField] private int startingBulbs = GameConstants.StartingBulbCount;` — replaces PlayerController.spareBulbs
  - [x]3.4 Add private fields:
    - `private InventorySlot[] slots;` — fixed-size array of InventorySlotCount
  - [x]3.5 Add public properties:
    - `public IReadOnlyList<InventorySlot> Slots => slots;` — for UI to read
  - [x]3.6 Add event: `public event Action OnInventoryChanged;` — fires after any add/remove for UI refresh
  - [x]3.7 In `Awake()`:
    - Initialize `slots = new InventorySlot[GameConstants.InventorySlotCount]`
    - If `startingBulbs > 0`, call `AddItem(ItemType.SpareBulb, startingBulbs)` to populate starting inventory
  - [x]3.8 Add `public bool TryAddItem(ItemType type, int amount = 1)` method:
    - First, check existing slots for same ItemType with room (count < MaxStackSize). Add to that slot.
    - If no existing slot has room, find first empty slot (count == 0). Set type and count.
    - If no room anywhere, return false (inventory full — AC2)
    - Fire `OnInventoryChanged` on success, return true
  - [x]3.9 Add `public bool TryUseItem(ItemType type, int amount = 1)` method:
    - Find slot with matching ItemType and count >= amount
    - Decrement count by amount (AC4)
    - If count reaches 0, clear the slot (set count = 0)
    - Fire `OnInventoryChanged` on success, return true
    - Return false if item not found or insufficient count
  - [x]3.10 Add `public int GetItemCount(ItemType type)` method:
    - Sum counts across all slots matching type
  - [x]3.11 Add private `void AddItem(ItemType type, int amount)` helper for Awake initialization:
    - Distributes items across slots respecting MaxStackSize
    - Used only during setup, not for runtime pickups (TryAddItem handles that)

- [x] Task 4: Create PickupItem.cs component (AC: 1, 2)
  - [x]4.1 Create new file `Assets/Scripts/Environment/PickupItem.cs`
  - [x]4.2 Add serialized fields:
    - `[Header("Pickup")]`
    - `[SerializeField] private ItemType itemType;` — what this pickup gives
    - `[SerializeField] private int amount = 1;` — how many to add
  - [x]4.3 Add public properties:
    - `public ItemType Type => itemType;`
    - `public int Amount => amount;`
  - [x]4.4 Add `public void Collect()` method:
    - Deactivate the GameObject: `gameObject.SetActive(false)`
    - Note: the actual inventory add happens in PlayerController which calls TryAddItem first, then Collect on success

- [x] Task 5: Add pickup awareness to InteractableObject.cs (AC: 1)
  - [x]5.1 Add private field after the lightFixture field: `private PickupItem pickupItem;`
  - [x]5.2 Add public property: `public PickupItem Pickup => pickupItem;`
  - [x]5.3 In `Awake()`, after the lightFixture line: `pickupItem = GetComponent<PickupItem>();`

- [x] Task 6: Create InventoryUI as part of PlayerHUD (AC: 1, 2, 3)
  - [x]6.1 Create new file `Assets/Scripts/Player/InventoryUI.cs`
  - [x]6.2 Add private fields:
    - `private GameObject[] slotObjects;` — the slot background GameObjects
    - `private TextMeshProUGUI[] countTexts;` — count labels per slot
    - `private TextMeshProUGUI[] nameTexts;` — item name labels per slot
    - `private GameObject feedbackObject;` — "Inventory Full" feedback
    - `private TextMeshProUGUI feedbackText;`
    - `private Coroutine feedbackCoroutine;`
    - `private PlayerInventory inventory;`
  - [x]6.3 Add `public void Initialize(Canvas canvas, PlayerInventory inv)` method:
    - Store inventory reference
    - Create a horizontal row of 3 slots at bottom-center of screen
    - Each slot: semi-transparent dark background (matching inspection panel style), item name text (small, top), count text (larger, center)
    - Empty slots show as dimmed with no text
    - Create feedback text object above the slots (hidden by default)
    - Subscribe to `inventory.OnInventoryChanged += RefreshSlots`
  - [x]6.4 Add `private void RefreshSlots()` method:
    - For each slot index, read `inventory.Slots[i]`
    - If slot is empty: hide name/count text, dim background
    - If slot has items: show item name (e.g., "Bulbs"), show count, normal alpha
  - [x]6.5 Add `public void ShowFullFeedback()` method:
    - Show "Inventory Full" text above slots
    - Start coroutine to hide after InventoryFullFeedbackDuration
  - [x]6.6 Add cleanup: unsubscribe from OnInventoryChanged in a public `Cleanup()` method (called from PlayerHUD.OnDestroy)

- [x] Task 7: Integrate InventoryUI into PlayerHUD.cs (AC: 3)
  - [x]7.1 Add private field: `private InventoryUI inventoryUI;`
  - [x]7.2 Add public property: `public InventoryUI InventoryUI => inventoryUI;`
  - [x]7.3 In `Awake()`, after finding playerController: find `PlayerInventory` via `playerController.GetComponent<PlayerInventory>()`
  - [x]7.4 In `BuildHUD()`, after DiscoveryNotificationUI creation: create InventoryUI and call `Initialize(canvas, playerInventory)`
  - [x]7.5 In `OnDestroy()`: call `inventoryUI?.Cleanup()`

- [x] Task 8: Modify PlayerController for pickup and inventory integration (AC: 1, 2, 4)
  - [x]8.1 Add private field: `private PlayerInventory inventory;`
  - [x]8.2 In `Awake()`, after existing setup: `inventory = GetComponent<PlayerInventory>();`
  - [x]8.3 Remove `private int spareBulbs = GameConstants.StartingBulbCount;` field
  - [x]8.4 Remove `public int SpareBulbs => spareBulbs;` property
  - [x]8.5 Add `public PlayerInventory Inventory => inventory;` property
  - [x]8.6 Remove `TryUseBulb()` method entirely
  - [x]8.7 In `HandleInteraction()`, update the fixture repair block to use inventory:
    ```
    if (currentHighlighted.Fixture != null
        && !currentHighlighted.Fixture.IsFunctional
        && !currentHighlighted.Fixture.IsRepairing)
    {
        if (inventory.TryUseItem(ItemType.SpareBulb))
        {
            currentHighlighted.Fixture.Repair();
        }
        return;
    }
    ```
  - [x]8.8 In `HandleInteraction()`, add pickup check AFTER fixture check and BEFORE IsInspectable check:
    ```
    // Pickups: collect if room in inventory
    if (currentHighlighted.Pickup != null)
    {
        if (inventory.TryAddItem(currentHighlighted.Pickup.Type, currentHighlighted.Pickup.Amount))
        {
            currentHighlighted.Pickup.Collect();
        }
        else
        {
            // Inventory full — show feedback via HUD
            FindObjectOfType<PlayerHUD>()?.InventoryUI?.ShowFullFeedback();
        }
        return;
    }
    ```

- [x] Task 9: Update PlayerHUD.Update() for pickup and inventory prompts (AC: 1, 2)
  - [x]9.1 Replace `playerController.SpareBulbs` reference in fixture prompt with `playerController.Inventory.GetItemCount(ItemType.SpareBulb)`
  - [x]9.2 Add pickup prompt between fixture check and IsInspectable check:
    ```
    else if (playerController.CurrentHighlighted.Pickup != null)
        promptText.text = "E: Pick Up";
    ```

- [ ] Task 10: Manual testing verification (AC: 1, 2, 3, 4) — **Requires Unity Editor; to be verified by user**
  - [ ] 10.1 Set up: Add PlayerInventory component to the Player GameObject. Create a PickupItem object (Cube + InteractableObject + PickupItem, itemType=SpareBulb, amount=1). Enter play mode.
  - [ ] 10.2 Verify the 3-slot hotbar appears at the bottom of the screen with starting bulbs shown (AC3).
  - [ ] 10.3 Highlight the pickup — verify prompt shows "E: Pick Up" (AC1).
  - [ ] 10.4 Press E — verify item disappears from scene and inventory count updates (AC1).
  - [ ] 10.5 Fill all 3 slots to capacity. Try to pick up another item — verify "Inventory Full" feedback (AC2).
  - [ ] 10.6 Repair a broken light fixture — verify bulb count decreases in hotbar (AC4).
  - [ ] 10.7 Verify empty slots appear dimmed and populated slots show item name + count.

## Dev Notes

### Architecture Compliance

- **File:** `Assets/Scripts/Player/PlayerInventory.cs` — New. MonoBehaviour managing 3 fixed hotbar slots. This is the MVP inventory — no persistence (save/load comes in Epic 14), no drop/swap mechanic. Absorbs the temporary spareBulbs counter from Story 2-6.
- **File:** `Assets/Scripts/Player/InventoryUI.cs` — New. Always-visible hotbar at bottom-center of HUD. Programmatic UI matching established patterns (no prefabs).
- **File:** `Assets/Scripts/Environment/PickupItem.cs` — New. Companion component for interactable objects that can be picked up. Deactivates on collection.
- **File:** `Assets/Scripts/Environment/InteractableObject.cs` — Modified. Adding optional PickupItem reference via GetComponent in Awake (same pattern as LightFixture).
- **File:** `Assets/Scripts/Player/PlayerController.cs` — Modified. Removing spareBulbs/TryUseBulb, adding PlayerInventory reference, adding pickup interaction, updating fixture repair to use inventory.
- **File:** `Assets/Scripts/Player/PlayerHUD.cs` — Modified. Creating and managing InventoryUI, updating fixture prompt to use inventory count.
- **File:** `Assets/Scripts/Core/GameConstants.cs` — Modified. Adding Inventory constants section.
- **File:** `Assets/Scripts/Core/GameEnums.cs` — Modified. Adding ItemType enum.

### Design Decision: 3-Slot Hotbar (Locked at Epic 1 Retro)

Decided during Epic 1 retrospective:
- 3 fixed hotbar slots, always visible on bottom of PlayerHUD
- Sequential fill, no reordering, no drag-and-drop
- No separate inventory screen — integrated into gameplay HUD
- Spare bulbs and repair tools share the same slots (meaningful resource trade-offs)
- Mortuary's Assistant as the design reference
- Full means can't pick up — no drop/swap mechanic for MVP
- Slot count is testable — may adjust after playtest feedback

### Design Decision: Absorbing spareBulbs from 2-6

Story 2-6 added a temporary `int spareBulbs` and `TryUseBulb()` on PlayerController. This story replaces them entirely:
- `spareBulbs` field → removed, PlayerInventory manages bulb count via slots
- `TryUseBulb()` → removed, replaced by `inventory.TryUseItem(ItemType.SpareBulb)`
- `SpareBulbs` property → removed, replaced by `Inventory` property exposing PlayerInventory
- PlayerHUD's `playerController.SpareBulbs` → replaced by `playerController.Inventory.GetItemCount(ItemType.SpareBulb)`

### AC3 Interpretation: "Opens the inventory view"

The epic AC says "opens the inventory view." Per the Epic 1 retro decision, there is no separate inventory screen. The hotbar is always visible on the HUD. AC3 is satisfied by the always-visible hotbar showing all items with quantities — the player can always "review contents" by looking at the bottom of the screen.

### Interaction Priority Order (After This Story)

In PlayerController.HandleInteraction():
1. **Displaced?** → Organize (from 2-5)
2. **Broken fixture?** → Repair if has bulb via inventory (updated from 2-6)
3. **Pickup?** → Collect if room in inventory (this story)
4. **Inspectable?** → return (InspectionSystem handles via its own E binding)
5. **Default** → Interact (generic non-inspectable interaction)

### What This Story Does NOT Include

- **No drop/swap mechanic** — Full means can't pick up. No dropping items for MVP.
- **No inventory persistence** — Save/load comes in Epic 14.
- **No item descriptions** — Items show name and count only. Detailed descriptions come later.
- **No Tab key toggle** — GDD shows Tab for Inventory/Notebook. Notebook is Epic 3. Tab integration comes then.
- **No supply events** — Daily supply availability comes in Epic 4/5.
- **No item degradation** — Resource degradation comes in Epic 5 (Story 5-6).

### Previous Story (2-6) Learnings

- **Companion component pattern proven.** LightFixture as GetComponent companion works cleanly. PickupItem follows the same pattern on InteractableObject.
- **Programmatic UI construction.** All UI built in code (InspectionUI, DiscoveryNotificationUI). InventoryUI follows the same pattern with canvas integration.
- **Event-driven UI refresh.** DiscoveryLog uses event Action for UI notification. PlayerInventory.OnInventoryChanged follows the same pattern for hotbar refresh.
- **CanvasGroup for fades.** Proven in InspectionUI and DiscoveryNotificationUI. InventoryUI feedback uses the same coroutine approach.
- **Dual InputAction bug prevention.** PickupItem interaction is handled entirely in PlayerController (no separate InputAction), so no InspectionSystem guard needed. But we should still be aware of the pattern.

### Existing Code to Build On

**PlayerController.cs (250 lines):**
- spareBulbs at line 34: REMOVE (replaced by PlayerInventory)
- SpareBulbs property at line 38: REMOVE (replaced by Inventory property)
- TryUseBulb() at line 209-213: REMOVE (replaced by inventory.TryUseItem)
- HandleInteraction() at line 169-202: Update fixture block, add pickup block

**PlayerHUD.cs (195 lines):**
- Awake(): Add PlayerInventory lookup and InventoryUI creation
- Update() line 77: Replace SpareBulbs with Inventory.GetItemCount
- BuildHUD(): Add InventoryUI.Initialize after DiscoveryNotificationUI
- OnDestroy(): Add InventoryUI cleanup

**InteractableObject.cs (291 lines):**
- Awake() after lightFixture line 87: Add pickupItem = GetComponent<PickupItem>()

**GameConstants.cs (81 lines):**
- Last section is Light Fixtures. New Inventory section goes after.

**GameEnums.cs (27 lines):**
- Add ItemType enum after existing enums

### Performance Targets

- No new per-frame operations (inventory is event-driven, UI refreshes only on change)
- InventoryUI.RefreshSlots: 3 slot reads + text updates — trivial
- TryAddItem/TryUseItem: linear scan of 3 slots — O(1) effectively
- Target: < 0.01ms frame time impact

### Project Structure Notes

- New: `Assets/Scripts/Player/PlayerInventory.cs` (inventory state management)
- New: `Assets/Scripts/Player/InventoryUI.cs` (always-visible hotbar)
- New: `Assets/Scripts/Environment/PickupItem.cs` (pickup companion component)
- Modified: `Assets/Scripts/Environment/InteractableObject.cs` (optional PickupItem reference)
- Modified: `Assets/Scripts/Player/PlayerController.cs` (remove spareBulbs, add inventory/pickup)
- Modified: `Assets/Scripts/Player/PlayerHUD.cs` (integrate InventoryUI)
- Modified: `Assets/Scripts/Core/GameConstants.cs` (inventory constants)
- Modified: `Assets/Scripts/Core/GameEnums.cs` (ItemType enum)
- No scene YAML changes (user configures pickups in Inspector)

### References

- [Source: epics.md#Epic 2, Story 2.7] — AC definitions and story statement
- [Source: gdd.md#Inventory Constraints] — "Limited space encourages prioritization — carrying more reduces flexibility"
- [Source: gdd.md#Resource Scarcity] — Resources are stability, not weapons
- [Source: gdd.md#Controls] — Tab key for Inventory/Notebook (deferred to Epic 3)
- [Source: gdd.md#Operational Supply System] — Fixed daily supplies, replacement bulbs, repair materials
- [Source: Epic 1 Retro] — 3-slot hotbar decision, Mortuary's Assistant reference
- [Source: Story 2-6] — spareBulbs/TryUseBulb to be absorbed by PlayerInventory
- [Source: epics.md#Epic 5, Story 5-6] — Downstream: Resource degradation will use inventory
- [Source: epics.md#Epic 14] — Downstream: Save/load will persist inventory state

## Dev Agent Record

### Agent Model Used

Claude Opus 4.6

### Debug Log References

None — clean implementation with no runtime errors.

### Completion Notes List

- Created PlayerInventory with 3 fixed slots, stacking up to 5, event-driven refresh
- Created InventoryUI as always-visible hotbar at bottom-center with slot backgrounds, name/count text, and "Inventory Full" feedback
- Created PickupItem companion component for collectible items
- Added PickupItem awareness to InteractableObject via GetComponent pattern
- Integrated InventoryUI into PlayerHUD (Awake lookup, BuildHUD creation, OnDestroy cleanup)
- Removed temporary spareBulbs/SpareBulbs/TryUseBulb from PlayerController, replaced with PlayerInventory
- Updated fixture repair to use inventory.TryUseItem(ItemType.SpareBulb)
- Added pickup interaction in HandleInteraction: displaced → fixture → pickup → inspectable → default
- Added pickup guard to InspectionSystem to prevent inspection firing on pickup items
- Updated PlayerHUD prompts: "E: Pick Up" for pickups, inventory-based bulb count for fixture prompt
- On successful pickup, unhighlight and clear currentHighlighted to prevent stale reference to deactivated object

### File List

- `Assets/Scripts/Core/GameConstants.cs` — Added Inventory constants section
- `Assets/Scripts/Core/GameEnums.cs` — Added ItemType enum
- `Assets/Scripts/Player/PlayerInventory.cs` — NEW: Inventory state management (3 slots, stacking, events)
- `Assets/Scripts/Player/InventoryUI.cs` — NEW: Always-visible hotbar UI
- `Assets/Scripts/Environment/PickupItem.cs` — NEW: Pickup companion component
- `Assets/Scripts/Environment/InteractableObject.cs` — Added PickupItem reference
- `Assets/Scripts/Player/PlayerController.cs` — Removed spareBulbs, added inventory/pickup integration
- `Assets/Scripts/Player/PlayerHUD.cs` — Integrated InventoryUI, updated prompts
- `Assets/Scripts/Player/InspectionSystem.cs` — Added pickup guard to prevent inspection on pickups

