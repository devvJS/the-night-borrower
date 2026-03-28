# The Night Borrower - Systems Specification

**Generated from GDD:** 2026-03-28
**Author:** Dakota
**Status:** Draft — Awaiting Review
**Design Constraint:** All systems scoped for solo development in Unity using simple deterministic logic where possible.

---

## 1. Notebook System Architecture

### Purpose

The notebook is the player's primary survival tool — a persistent knowledge system that supports observation, pattern recognition, and memory preservation. It bridges the observe → inspect → record loop.

### Data Model

#### Entry Structure

```
NotebookEntry {
    id: string                  // Unique entry identifier
    entryType: enum             // AUTO_DISCOVERY | AUTO_EVENT | MANUAL_NOTE
    category: enum              // OBSERVATION | PATTERN | CLUE | RECORD | PERSONAL
    title: string               // Short display title
    body: string                // Entry content text
    dayCreated: int             // Day number when entry was created (1-7)
    phaseCreated: enum          // DAY | UNEASE | NIGHT | RECOVERY
    sourceLocation: string      // Area where entry originated
    sourceObject: string        // Object ID that triggered entry (if applicable)
    isCorrupted: bool           // Whether entity has altered this entry
    corruptedBody: string       // Altered text (if corrupted)
    relatedEntries: string[]    // IDs of cross-referenced entries
    isRead: bool                // Whether player has viewed this entry
    isPinned: bool              // Whether player has pinned this entry
}
```

#### Storage Structure

```
NotebookData {
    entries: NotebookEntry[]
    manualNotes: ManualNote[]
    totalEntries: int
    unreadCount: int
    corruptedCount: int
}

ManualNote {
    id: string
    body: string                // Player-written text
    dayCreated: int
    category: enum              // Player-selected category tag
    linkedEntryId: string       // Optional link to an auto entry
}
```

### Automatic Logging Rules

**Triggers that create auto entries:**

| Trigger | Entry Type | Category |
|---|---|---|
| First inspection of a key object | AUTO_DISCOVERY | CLUE |
| Environmental change detected (object displaced, record altered) | AUTO_DISCOVERY | OBSERVATION |
| Puzzle solved or puzzle state changed | AUTO_EVENT | PATTERN |
| Night Borrower encounter survived | AUTO_EVENT | OBSERVATION |
| New area unlocked | AUTO_EVENT | RECORD |
| Critical narrative fragment found | AUTO_DISCOVERY | CLUE |
| Lighting failure in a new zone | AUTO_EVENT | OBSERVATION |

**Auto-logging rules:**
- Entries are created immediately when trigger conditions are met
- Player receives a subtle UI notification ("New entry added")
- Auto entries cannot be deleted by the player
- Auto entries CAN be corrupted by the entity

### Manual Note System

**Player capabilities:**
- Open notebook (Tab) → select "Add Note" button in UI
- Write free-text note (character limit: 500)
- Optionally tag with category
- Optionally link to an existing auto entry
- Manual notes persist across all sessions
- Manual notes CANNOT be corrupted by the entity (player's own memory is protected)

**Design rationale:** Manual notes reward engaged players without punishing those who rely only on auto-logging. The entity corrupts records but not the player's personal observations — reinforcing that player attention is the ultimate survival tool.

### Retrieval and Lookup

**Notebook UI views:**

| View | Purpose |
|---|---|
| **Chronological** | All entries by day/phase created (default) |
| **By Category** | Filtered by OBSERVATION, PATTERN, CLUE, RECORD, PERSONAL |
| **By Location** | Filtered by source area |
| **Unread** | Only entries not yet viewed |
| **Corrupted** | Entries flagged as altered (visible after first corruption event) |
| **Pinned** | Player-pinned important entries |

**Cross-referencing:**
- Entries with matching `sourceObject` or `sourceLocation` show a "Related" indicator
- Player can navigate between related entries
- Cross-reference connections are computed at display time, not stored (keeps save data simple)

**Search:** No free-text search for MVP. Category and location filters are sufficient.

### Entity Corruption Mechanics

**How corruption works:**
- During Night Phase encounters or soft failures, the entity may corrupt 1-3 auto entries
- Corruption replaces `body` text with `corruptedBody` — subtly altered information
- The `isCorrupted` flag is set to `true`
- Corrupted entries are NOT visually marked until the player notices the inconsistency OR the Corrupted filter is unlocked (after first corruption event on Night 2+)
- Corruption targets entries related to the current zone's instability

**Corruption selection logic:**
```
1. Get all auto entries from the current zone
2. Filter to non-corrupted entries
3. Prioritize entries related to active puzzles or patterns
4. Corrupt 1-3 entries (scaled by night number)
5. Generate altered body text (swap key details, change numbers, alter names)
```

---

## 2. Environmental State Persistence Model

### Purpose

Track the state of every interactable object, lighting fixture, and environmental element across the 7-day cycle. Changes must persist between sessions and accumulate meaningfully.

### State Architecture

#### Object State Model

```
EnvironmentObject {
    id: string                  // Unique object identifier
    objectType: enum            // BOOK | FIXTURE | PROP | RECORD | FURNITURE | SWITCH
    zoneId: enum                // Which zone this object belongs to

    // Slot-Based Position State
    homeSlotId: string          // The slot where this object belongs (correct position)
    currentSlotId: string       // The slot where this object currently sits
    // Derived: isDisplaced = (homeSlotId != currentSlotId)

    // Condition State
    condition: enum             // NORMAL | DEGRADED | FAILED | CORRUPTED
    degradationLevel: float     // 0.0 (perfect) to 1.0 (failed)

    // Interaction State
    hasBeenInspected: bool      // Player has inspected this object
    lastInspectedDay: int       // Day number of last inspection
    interactionCount: int       // Total times interacted with

    // Entity Interaction
    entityTouched: bool         // Entity has interacted with this object
    entityTouchDay: int         // Day when entity last affected this
}
```

#### Slot System

Objects exist in **named slots** rather than at arbitrary world positions. Each slot is a fixed point in the scene defined at edit time. Objects move between slots — never to arbitrary coordinates.

```
Slot {
    slotId: string              // Unique ID, e.g. "main_shelf_A3", "storage_table_01"
    zoneId: enum                // Zone this slot belongs to
    slotType: enum              // SHELF | TABLE | WALL | FLOOR | FIXTURE_MOUNT
    worldPosition: Vector3      // Set in editor, never serialized to save data
    worldRotation: Quaternion   // Set in editor, never serialized to save data
    acceptsTypes: enum[]        // Which ObjectTypes can occupy this slot
    isOccupied: bool            // Derived at runtime from object data
}
```

**Why slots instead of Vector3:**
- **Serialization:** Save data stores two strings (`homeSlotId`, `currentSlotId`) instead of two Vector3s (6 floats). ~40 bytes → ~20 bytes per object.
- **Determinism:** "Is this book in the right place?" is a string comparison, not a distance threshold check. No floating-point tolerance bugs.
- **Puzzle logic:** Book arrangement puzzles check `currentSlotId == homeSlotId` for each book in the puzzle set. No spatial math.
- **Entity displacement:** "Move book to wrong slot" = pick a random valid slot of matching type in the same zone. No pathfinding or physics.
- **Visual consistency:** Objects always snap to authored positions. No jitter, no clipping, no floating books.

**Slot naming convention:**
```
{zone}_{furniture}_{position}

Examples:
  main_shelfA_01        // Main floor, shelf A, position 1
  main_shelfA_02        // Main floor, shelf A, position 2
  storage_crate_01      // Storage room, crate, position 1
  basement_archive_03   // Basement, archive shelf, position 3
  office_desk_lamp      // Office, desk lamp mount point
```

**Slot types and what occupies them:**

| Slot Type | Accepts | Example |
|---|---|---|
| SHELF | BOOK, RECORD | `main_shelfA_01` |
| TABLE | PROP, RECORD | `office_desk_01` |
| WALL | SWITCH, PROP | `basement_panel_01` |
| FLOOR | FURNITURE, PROP | `storage_floor_crate` |
| FIXTURE_MOUNT | FIXTURE | `main_ceiling_light_03` |

**Displacement rules:**
```
To displace an object:
  1. Get object's currentSlotId
  2. Find all slots in the same zone where:
     - slotType accepts object's objectType
     - slot is not currently occupied
     - slot != object's homeSlotId (don't "displace" to correct position)
  3. Pick one at random
  4. Set object's currentSlotId to the new slot
  5. Object's scene transform snaps to new slot's worldPosition/Rotation

To restore an object (player organizes):
  1. Set object's currentSlotId = object's homeSlotId
  2. Snap transform to home slot position
```

#### Zone State Model

```
ZoneState {
    zoneId: string              // MAIN_FLOOR | OFFICE | STORAGE | BASEMENT | APARTMENT | STREET | ALLEY | FOREST | RAIL
    isUnlocked: bool            // Whether player can access this zone
    unlockDay: int              // Day this zone became accessible

    // Stability Tracking
    stabilityScore: float       // 0.0 (total chaos) to 1.0 (perfect order)
    lightingScore: float        // 0.0 (all failed) to 1.0 (all functional)
    orderScore: float           // 0.0 (all displaced) to 1.0 (all correct)

    // Lighting State
    totalFixtures: int
    functionalFixtures: int
    failedFixtures: int

    // Object State
    totalObjects: int
    displacedObjects: int
    corruptedRecords: int
}
```

#### Day State Model

```
DayState {
    currentDay: int             // 1-7
    currentPhase: enum          // DAY | UNEASE | NIGHT | RECOVERY
    phaseTimeRemaining: float   // Time left in current phase (if timed)

    // Daily Supply State
    availableBulbs: int
    availableRepairParts: int
    bulbsUsedToday: int

    // Cumulative Tracking
    totalFailureEvents: int
    totalEntityEncounters: int
    nightsSurvived: int
    nightsFailed: int
}
```

### What Objects Store State

| Object Type | Tracked State | Persistence |
|---|---|---|
| **Books** | Slot assignment (home vs current), inspection status | Persists across days |
| **Light Fixtures** | Condition (normal/degraded/failed), degradation level, fixture mount slot | Persists across days, degrades naturally |
| **Records/Documents** | Slot assignment, content state (normal/corrupted), inspection status | Persists; may be corrupted by entity |
| **Props** | Slot assignment (home vs current) | Persists across days |
| **Switches/Panels** | On/off state, condition | Persists across days |
| **Doors** | Open/closed, locked/unlocked | Resets daily unless narrative-locked |

### Daily Transition Logic

**Day → Unease Transition:**
```
1. Apply natural entropy to all unlocked zones:
   - Displace 1-3 random objects per zone (scaled by day number)
   - Degrade 0-1 light fixtures per zone (scaled by day number)
2. No entity-driven changes yet
3. Save zone stability scores
```

**Unease → Night Transition:**
```
1. Apply entity-driven changes:
   - Displace additional objects in unstable zones (stabilityScore < 0.5)
   - Fail 0-2 light fixtures (prioritize zones with low lightingScore)
   - Corrupt 0-1 records in affected zones
2. Activate entity presence system
3. Save full state snapshot
```

**Night → Recovery Transition:**
```
1. Deactivate entity
2. If night survived: normal transition
3. If night failed (entity caught player):
   - Apply penalty: corrupt 1-3 additional notebook entries
   - Apply penalty: fail 1-2 additional lights
   - Apply penalty: displace 2-4 additional objects
4. Save full state snapshot (autosave point)
```

**Recovery → Next Day Transition:**
```
1. Increment day counter
2. Replenish daily supplies:
   - Bulbs: max(3 - (currentDay / 2), 1)  // Decreasing supply
   - Repair parts: max(2 - (currentDay / 3), 1)
3. Check area unlock conditions
4. Reset phase timer
5. Save full state snapshot (autosave point)
```

### Natural Entropy Rules

```
EntropyRate per zone per day:
  Day 1-2: 1-2 displaced objects, 0-1 light degradation events
  Day 3-4: 2-3 displaced objects, 1 light degradation event
  Day 5-6: 3-4 displaced objects, 1-2 light degradation events
  Day 7:   4-5 displaced objects, 2-3 light degradation events

Entropy targets:
  - Prioritize zones with HIGH orderScore (disrupt what's ordered)
  - Prioritize fixtures with HIGH degradationLevel (push them to failure)
  - Never displace objects the player is currently inspecting
  - Displacement = move object from homeSlot to a random valid slot in same zone
```

### Save/Load Strategy

**Save format:** JSON-based flat file containing:
- `DayState` (current day, phase, supplies)
- `ZoneState[]` (all zone stability data)
- `EnvironmentObject[]` (only objects with non-default state — delta compression)
- `NotebookData` (all entries and manual notes)
- `PlayerState` (position, inventory, current zone)
- `NarrativeState` (see Section 4)

**Save triggers:**
- Autosave at start of each Day Phase
- Autosave at end of each Night Phase (post-recovery)
- No manual save for MVP

**Load behavior:**
- Always loads to the start of the most recent Day Phase or Recovery Phase
- If load occurs mid-night (crash recovery), restores to last autosave

**Delta compression:** Only objects whose state differs from default are serialized. Default state is defined in scene data. This keeps save files small and manageable.

### Failure and Recovery Rules

| Failure Type | Consequence | Recovery |
|---|---|---|
| Entity catches player | Night ends early, penalty applied (corrupted entries, extra displacement, light failures) | Restart at next day with penalties persisted |
| Light fixture fails | Zone lighting score drops, area becomes dangerous | Player repairs with available parts (if any) |
| Record corrupted | Notebook entry altered, information unreliable | Partial: re-observe source. Some permanent |
| Zone stability collapse (< 0.2) | Entity heavily favors this zone | Restore order through organize + maintain |
| All bulbs depleted | Cannot repair further lights this day | Survive with remaining light; new supply next day |

---

## 3. Night Borrower Behavior State Machine

### Purpose

Define the entity's behavior as a deterministic state machine that players can learn and predict. The entity reacts to environmental instability, not player action directly.

### State Diagram

```
                    ┌──────────┐
                    │   IDLE   │ ◄──── Day Phase (always idle)
                    └────┬─────┘
                         │ Night Phase begins
                         ▼
                    ┌──────────┐
              ┌────►│OBSERVING │ ◄──── Default night state
              │     └────┬─────┘
              │          │ Instability threshold met
              │          ▼
              │     ┌──────────┐
              │     │ HUNTING  │ ◄──── Active pursuit
              │     └────┬─────┘
              │          │ Player caught OR escalation trigger
              │          ▼
              │     ┌────────────┐
              │     │ESCALATING  │ ◄──── Heightened aggression
              │     └────┬───────┘
              │          │ Cooldown timer expires
              │          ▼
              │     ┌────────────┐
              └─────┤ RETREATING │ ◄──── Temporary withdrawal
                    └────────────┘
```

### State Definitions

#### IDLE
**Active during:** Day Phase, Unease Phase (early days), Recovery Phase
**Behavior:** Entity is not present in the game world. Environmental interference may occur passively (object displacement, record corruption) but entity has no physical manifestation.
**Transition to OBSERVING:** Night Phase begins.

#### OBSERVING
**Active during:** Night Phase (default state)
**Behavior:**
- Entity exists in the game world but does not actively pursue
- Patrols dark zones and unstable areas
- Moves slowly between zones with lowest `stabilityScore`
- Visible at distance (through gaps, in shadows, peripheral movement)
- Telegraphing active: lighting flickers, audio cues, environmental signals

**Decision logic (evaluated every 10 seconds):**
```
For each unlocked zone:
  threatWeight = (1.0 - stabilityScore) * 0.4
               + (1.0 - lightingScore) * 0.4
               + (corruptedRecords / totalRecords) * 0.2

Select zone with highest threatWeight as patrol target
Move toward patrol target at SLOW speed
```

**Transition to HUNTING:** Any zone's `threatWeight > huntingThreshold`
```
huntingThreshold per night:
  Night 1: 0.8 (very hard to trigger)
  Night 2: 0.7
  Night 3: 0.6
  Night 4: 0.5
  Night 5: 0.4
  Night 6: 0.3
  Night 7: 0.2 (very easy to trigger)
```

#### HUNTING
**Active during:** Night Phase (when instability threshold met)
**Behavior:**
- Entity actively moves toward the player's zone
- Movement speed increases (MEDIUM speed)
- Environmental interference intensifies: lights flicker rapidly, objects displace in real-time
- Entity detection range increases
- Entity avoids zones with `lightingScore > 0.7`

**Detection logic:**
```
If player is in same zone as entity:
  If player is in darkness (not within light radius of any functional fixture):
    detectionChance = 0.8
  If player is in dim light (degraded fixture):
    detectionChance = 0.3
  If player is in full light (functional fixture):
    detectionChance = 0.05  // Nearly safe

  Roll detection every 5 seconds
  If detected: Transition to ESCALATING or trigger catch
```

**Line of sight rules:**
- Entity can detect player if direct line exists AND lighting condition is met
- Bookshelves and walls block line of sight
- Player breaking line of sight resets detection timer

**Transition to ESCALATING:** Player detected OR entity has been hunting for > 60 seconds in same zone
**Transition to RETREATING:** Player moves to zone with `lightingScore > 0.7` AND entity cannot follow

#### ESCALATING
**Active during:** Night Phase (heightened aggression)
**Behavior:**
- Entity moves at FAST speed directly toward player's last known position
- Environmental interference is maximum: multiple lights fail, objects displace rapidly
- Entity passes through dim-light zones (only full light stops it)
- Duration: 20-30 seconds maximum

**Outcomes:**
```
If entity reaches player position:
  → CATCH: Night ends prematurely, failure penalties applied

If player reaches full-light zone:
  → Entity halts at light boundary
  → Transition to RETREATING after 10 second standoff

If escalation timer expires (30 seconds):
  → Transition to RETREATING
```

#### RETREATING
**Active during:** Night Phase (cooldown after hunting/escalating)
**Behavior:**
- Entity moves away from player at SLOW speed
- Returns to zone with highest `threatWeight`
- Environmental interference reduces
- Telegraphing fades (audio cues diminish, lights stabilize)
- Duration: 30-60 seconds (scaled by night number — shorter in later nights)

**Transition to OBSERVING:** Retreat timer expires
**Transition to HUNTING:** If any zone's `threatWeight` exceeds threshold during retreat (entity re-engages)

### Speed Constants

| State | Movement Speed | Detection Range |
|---|---|---|
| IDLE | N/A | N/A |
| OBSERVING | 1.0 m/s | 15m |
| HUNTING | 2.5 m/s | 25m |
| ESCALATING | 4.0 m/s | 35m |
| RETREATING | 1.5 m/s | 10m |

Player walk speed: 3.0 m/s | Player sprint speed: 5.0 m/s

### Night Escalation Across Days

| Night | Starting State Duration | Hunting Threshold | Retreat Cooldown | Max Escalations |
|---|---|---|---|---|
| 1 | Observing only (60 min) | 0.8 (unlikely) | 60s | 1 |
| 2 | Observing (45 min) | 0.7 | 55s | 1 |
| 3 | Observing (30 min) | 0.6 | 50s | 2 |
| 4 | Observing (20 min) | 0.5 | 45s | 2 |
| 5 | Observing (15 min) | 0.4 | 40s | 3 |
| 6 | Observing (10 min) | 0.3 | 30s | 4 |
| 7 | Observing (5 min) | 0.2 | 20s | Unlimited |

### Entity-Environment Interaction

**While OBSERVING:** Displaces 1 object every 30 seconds in current zone
**While HUNTING:** Displaces 1 object every 10 seconds, degrades 1 light every 20 seconds
**While ESCALATING:** Displaces 1 object every 5 seconds, fails 1 light every 10 seconds
**While RETREATING:** No environmental interaction

---

## 4. Ending Variable Matrix

### Purpose

Track player actions across the 7-day cycle to determine which of 10 endings the player reaches. Endings are driven by accumulated state, not single choices.

### Core Tracked Variables

```
EndingVariables {
    // Knowledge Preservation
    knowledgeScore: float       // 0.0 - 1.0 (ratio of non-corrupted entries to total)
    manualNotesCount: int       // Number of manual notes created
    crossReferencesUsed: int    // Times player used cross-reference feature

    // Environmental Control
    averageStability: float     // Average zone stability across all days
    totalRepairs: int           // Total lights repaired across game
    totalOrganized: int         // Total objects restored to correct position

    // Survival Performance
    nightsSurvived: int         // Nights completed without entity catch (0-7)
    nightsFailed: int           // Nights where entity caught player
    totalEntityEncounters: int  // Times entity entered HUNTING state

    // Pattern Recognition
    puzzlesSolved: int          // Total puzzles completed
    patternsIdentified: int     // Environmental patterns correctly predicted

    // Narrative Discovery
    loreFragmentsFound: int     // Out of total available (target: 30-50)
    npcConversations: int       // Unique NPC dialogue exchanges completed
    elderConversations: int     // Specifically Elder dialogue exchanges
    criticalReveals: int        // Key story beats discovered (out of ~8)

    // Entity Understanding
    entityBehaviorLearned: int  // Times player successfully predicted entity
    entityRecordsPreserved: int // Entity-related records not corrupted
}
```

### Ending Thresholds

| # | Ending Name | Primary Condition | Secondary Conditions | Tone |
|---|---|---|---|---|
| 1 | **Full Preservation** | knowledgeScore > 0.85 | nightsSurvived = 7 AND criticalReveals >= 7 | Best — Truth preserved |
| 2 | **Understanding** | knowledgeScore > 0.7 | criticalReveals >= 5 AND elderConversations >= 4 | Good — Comprehension achieved |
| 3 | **Survival Through Order** | averageStability > 0.7 | nightsSurvived >= 5 AND totalRepairs > 30 | Good — Control maintained |
| 4 | **The Archivist** | loreFragmentsFound > 40 | manualNotesCount > 20 AND crossReferencesUsed > 10 | Bittersweet — Knowledge preserved but incomplete |
| 5 | **Imperfect Memory** | knowledgeScore 0.4–0.7 | nightsSurvived >= 4 | Neutral — Partial truth |
| 6 | **Pattern Breaker** | puzzlesSolved >= 10 | patternsIdentified >= 5 | Neutral — Mechanical mastery |
| 7 | **The Borrower Remains** | nightsFailed >= 3 | averageStability < 0.4 | Dark — Entity wins through attrition |
| 8 | **Lost Records** | knowledgeScore < 0.3 | loreFragmentsFound < 15 | Dark — Memory consumed |
| 9 | **Collapse** | averageStability < 0.2 | nightsFailed >= 4 AND totalRepairs < 10 | Worst — Complete environmental failure |
| 10 | **Forgotten** | knowledgeScore < 0.15 | nightsSurvived <= 2 AND criticalReveals <= 2 | Worst — Total memory erasure |

### Ending Selection Logic

```
At end of Night 7 (or after final Recovery Phase):

1. Calculate all EndingVariables from accumulated state
2. Evaluate endings in priority order (1 → 10)
3. First ending whose ALL conditions are met is selected
4. If no specific ending conditions are met: select Ending 5 (Imperfect Memory) as default

Priority ensures best endings are checked first.
Conditions use AND logic — all must be satisfied.
```

### Variable Accumulation Rules

**knowledgeScore:** Recalculated each day:
```
knowledgeScore = (totalAutoEntries - corruptedEntries) / totalAutoEntries
```

**averageStability:** Recalculated each day:
```
averageStability = sum(zone.stabilityScore for all unlocked zones) / unlockedZoneCount
Rolling average across all completed days
```

**Thresholds are final-state:** Variables are evaluated at game end, not during play. No ending is "locked in" until Night 7 concludes. This means recovery is always possible — a bad Night 3 can be overcome by strong play on Nights 4-7.

---

## 5. Puzzle Dependency Framework

### Purpose

Define puzzle types, their ordering, and unlock relationships to ensure puzzles are solvable in the correct sequence and integrate with environmental and narrative systems.

### Puzzle Type Definitions

| Type | Mechanic | Pillar | Frequency |
|---|---|---|---|
| **Book Arrangement** | Identify and correct misplaced books on shelves | Routine Becomes Ritual | High (every day) |
| **Record Cross-Reference** | Compare multiple notebook entries or documents to derive information | Knowledge Is Survival | Medium (Day 2+) |
| **Pattern Recognition** | Identify repeating environmental changes across days | Small Changes Create Fear | Medium (Day 3+) |
| **Code/Combination** | Use discovered information to unlock locks or systems | Knowledge Is Survival | Low (specific progression points) |
| **Environmental Logic** | Manipulate environment elements (lighting, switches, paths) to achieve result | Light Creates Safety | Medium (Day 3+) |

### Puzzle Inventory

```
Puzzle {
    id: string
    type: enum                  // BOOK_ARRANGEMENT | CROSS_REFERENCE | PATTERN | CODE | ENVIRONMENTAL
    zoneId: string              // Zone where puzzle exists
    unlockDay: int              // Earliest day puzzle becomes available
    prerequisitePuzzles: string[] // Puzzle IDs that must be solved first
    prerequisiteItems: string[] // Notebook entries or objects required
    difficulty: enum            // SIMPLE | MODERATE | COMPLEX
    isOptional: bool            // Whether puzzle is required for progression
    narrativeReward: string     // Lore fragment or story beat unlocked
    mechanicalReward: string    // Gameplay benefit (area unlock, resource, stability)
    canBeCorrupted: bool        // Whether entity can interfere with puzzle state
}
```

### Puzzle Schedule by Day (12 puzzles — reduced from 18)

**Reduction rationale:** Original 18-puzzle set was scoped for a larger team. This reduced set of 12 preserves all 5 puzzle types, all area unlocks, and the full narrative progression chain while cutting solo-dev implementation time by ~33%. See _Removed/Merged Puzzles_ at end of section for post-launch candidates.

#### Day 1 — Tutorial (SIMPLE) — 2 puzzles

| ID | Puzzle | Type | Zone | Required | Prerequisite |
|---|---|---|---|---|---|
| P01 | Shelf Restoration | Book Arrangement | Main Floor | Yes | None |
| P02 | Flickering Office Light | Environmental | Office | Yes | None |

**Purpose:** Teach observation and maintenance. No failure consequence. P01 now covers the full welcome-shelf sequence (merged from original P01 + P03 — player sorts a few books on Day 1, finds more displaced on Day 2 morning, completes the full shelf as a natural continuation).

#### Day 2 — Foundation (SIMPLE) — 1 puzzle

| ID | Puzzle | Type | Zone | Required | Prerequisite |
|---|---|---|---|---|---|
| P03 | Receipt Discrepancy | Cross-Reference | Office | No | P01 |

**Purpose:** Introduce cross-referencing. First optional puzzle. Rewards players who inspect the office records carefully.

#### Day 3 — Expansion (SIMPLE → MODERATE) — 2 puzzles

| ID | Puzzle | Type | Zone | Required | Prerequisite |
|---|---|---|---|---|---|
| P04 | Storage Room Access Code | Code | Storage Room | Yes | P01 (triggers unlock) |
| P05 | Lighting Circuit | Environmental | Main Floor | No | P02 |

**Purpose:** First code puzzle gates storage room access. Lighting circuit teaches that electrical systems have traceable logic — sets up the Day 5 expansion.

#### Day 4 — Depth (MODERATE) — 2 puzzles

| ID | Puzzle | Type | Zone | Required | Prerequisite |
|---|---|---|---|---|---|
| P06 | Archive Investigation | Cross-Reference | Basement | Yes | P04 |
| P07 | Repeating Displacement Pattern | Pattern Recognition | Main Floor | No | None (requires multi-day observation) |

**Purpose:** Archive investigation is a deeper cross-reference puzzle that includes noticing historical gaps in the records (absorbed original P09's content). P07 introduces pattern recognition — player notices the same objects keep moving.

#### Day 5 — Integration (MODERATE → COMPLEX) — 2 puzzles

| ID | Puzzle | Type | Zone | Required | Prerequisite |
|---|---|---|---|---|---|
| P08 | Cross-Zone Book Trail | Book Arrangement | Multi-zone | Yes | P01, P06 |
| P09 | Entity Behavior Pattern | Pattern Recognition | Any | No | 3+ entity encounters |

**Purpose:** First cross-zone puzzle tests accumulated shelf knowledge. Entity behavior pattern rewards players who study the entity rather than just flee — feeds directly into `entityBehaviorLearned` ending variable.

#### Day 6 — Narrative (MODERATE → COMPLEX) — 2 puzzles

| ID | Puzzle | Type | Zone | Required | Prerequisite |
|---|---|---|---|---|---|
| P10 | Apartment Safe Code & Records | Code + Cross-Reference | Apartment | Yes | P06 |
| P11 | Elder's Hidden Message | Cross-Reference | Office | No | elderConversations >= 3 |

**Purpose:** P10 combines the original safe code and personal records puzzles — opening the safe IS discovering the records (absorbed P15). P11 is the deepest NPC-gated content, requiring sustained engagement with the Elder across multiple days.

#### Day 7 — Resolution (COMPLEX) — 1 puzzle

| ID | Puzzle | Type | Zone | Required | Prerequisite |
|---|---|---|---|---|---|
| P12 | Final Record Assembly | Cross-Reference | Office | Yes | P10 or P11 |

**Purpose:** Capstone puzzle. Player assembles discovered records into a coherent narrative at the office PC. Directly determines `criticalReveals` count. Must be solvable via either P10 (safe records) OR P11 (Elder's message) — no single path dependency.

### Summary

| | Required | Optional | Total |
|---|---|---|---|
| **Original** | 8 | 10 | 18 |
| **Reduced** | 7 | 5 | 12 |

**Type coverage (all 5 preserved):**

| Type | Count | Puzzles |
|---|---|---|
| Book Arrangement | 2 | P01, P08 |
| Cross-Reference | 3 | P03, P06, P12 |
| Pattern Recognition | 2 | P07, P09 |
| Code/Combination | 2 | P04, P10 |
| Environmental | 2 | P02, P05 |

**One puzzle per day minimum, two per day maximum.** No day overwhelms the player or the developer.

### Removed and Merged Puzzles

| Original ID | Name | Action | Rationale |
|---|---|---|---|
| P01 + P03 | Welcome Books + Shelf Restoration | **Merged → P01** | Same mechanic, same zone, consecutive days. One progressive puzzle is more satisfying than two small ones. |
| P06 | Supply Inventory Check | **Cut** | Low narrative value. Busywork cross-reference in a zone the player just unlocked. Storage room exploration provides enough content without a formal puzzle. |
| P07 + P13 | Lighting Circuit + Electrical Grid | **Merged → P05** | Both are electrical/environmental. One puzzle with a Day 3 introduction and natural complexity growth as the player learns the building's systems. |
| P09 | Historical Record Gap | **Absorbed into P06** | The archive investigation naturally includes noticing gaps. Splitting "look at archives" and "notice something missing in archives" into two puzzles was artificial. |
| P15 | Personal Records Reconstruction | **Absorbed into P10** | Opening the safe IS the narrative payoff. Separate "open safe" and "read what's inside" puzzles added a step without adding a decision. |
| P17 | Full Environmental Restoration | **Deferred → post-launch** | Multi-zone, multi-mechanic capstone is high-effort to design and test. Day 7 is already the highest-pressure night. Better as post-launch content or NG+ challenge. |

### Dependency Graph

```
Day 1:  P01 ──────► P04 ──► P06 ──► P08
         │                    │       │
        P02 ──► P05          │      P10 ──► P12
                              │       │
                             P03     P11 ──► P12
                                    (Elder)
        P07 (independent observation across days)
        P09 (independent entity learning)
```

**Critical path (required only):** P01 → P04 → P06 → P08 → P10 → P12
**Length:** 6 puzzles across 7 days. Player solves roughly one required puzzle every day-and-a-half. Comfortable pace.

**Optional branches:**
- P03 (Day 2) — standalone, rewards curiosity
- P05 (Day 3) — branches from P02, teaches electrical systems
- P07 (Day 4) — standalone, rewards multi-day observation
- P09 (Day 5) — standalone, rewards entity study
- P11 (Day 6) — NPC-gated, alternate path to P12

**P12 has OR-dependency:** Requires P10 (safe records) OR P11 (Elder's message). Player can reach the ending puzzle through either narrative branch. This prevents a single missed optional puzzle from blocking the capstone.

### Puzzle-Entity Interaction Rules

**Puzzles the entity CAN interfere with:**
- Book Arrangement (P01, P08): Entity may re-displace books the player has organized
- Cross-Reference (P03, P06, P12): Entity may corrupt source records
- Pattern Recognition (P07, P09): Entity may break established patterns to mislead

**Puzzles the entity CANNOT interfere with:**
- Code/Combination (P04, P10): Once discovered, codes remain valid
- Environmental (P02, P05): Switch states are protected from entity manipulation

**Interference timing:** Entity only interferes during Night Phase. Day Phase puzzle work is safe.

### Puzzle Failure and Recovery

| Failure Type | Consequence | Recovery |
|---|---|---|
| Incorrect book placement | Books remain displaced, no penalty | Retry immediately |
| Wrong code entered | No penalty, unlimited attempts | Re-examine source records |
| Corrupted source record | Cross-reference produces wrong result | Re-observe original source, compare with manual notes |
| Pattern misidentification | Minor stability penalty in affected zone | Wait for next cycle to re-observe |
| Environmental puzzle failed | Zone lighting temporarily worsens | Repair and retry |

**No puzzle permanently blocks progression.** Required puzzles always have sufficient information available even if some records are corrupted — either through alternate sources, manual notes, or environmental re-observation.

### Puzzle Scoring Impact on Endings

```
puzzlesSolved = count of all puzzles completed (0-12)
patternsIdentified = P07 + P09 + any correct pattern predictions

These values feed directly into EndingVariables (Section 4).
High puzzle completion supports Endings 1, 2, 4, 6.
Low puzzle completion trends toward Endings 7, 8, 9, 10.
```

---

## System Integration Summary

### Data Flow Between Systems

```
                    ┌─────────────┐
                    │  Day/Night  │
                    │   Cycle     │
                    └──────┬──────┘
                           │ Phase transitions
                    ┌──────▼──────┐
                    │ Environment │◄──── Natural entropy
                    │ Persistence │◄──── Entity interference
                    └──┬───┬───┬──┘
                       │   │   │
          ┌────────────┘   │   └────────────┐
          ▼                ▼                ▼
    ┌───────────┐   ┌───────────┐    ┌───────────┐
    │  Notebook  │   │  Entity   │    │  Puzzle   │
    │  System    │   │  State    │    │  System   │
    └─────┬─────┘   │  Machine  │    └─────┬─────┘
          │         └─────┬─────┘          │
          │               │                │
          └───────┬───────┴────────┬───────┘
                  ▼                ▼
           ┌───────────┐   ┌───────────┐
           │  Ending   │   │   Save    │
           │ Variables │   │  System   │
           └───────────┘   └───────────┘
```

### Save Data Composition

| System | Data Size Estimate | Serialization |
|---|---|---|
| DayState | ~100 bytes | Direct JSON |
| ZoneState (9 zones) | ~500 bytes | Direct JSON |
| EnvironmentObjects (delta only) | ~2-10 KB | Delta JSON (only changed objects) |
| NotebookData | ~5-20 KB | Direct JSON |
| EndingVariables | ~200 bytes | Direct JSON |
| PuzzleState | ~500 bytes | Direct JSON |
| EntityState | ~100 bytes | Direct JSON |
| **Total estimated save size** | **~10-35 KB** | **Single JSON file** |

### Implementation Priority

| Priority | System | Reason |
|---|---|---|
| 1 | Environmental State Persistence | Foundation for all other systems |
| 2 | Notebook System | Core gameplay verb (Record) |
| 3 | Entity State Machine | Defines night phase gameplay |
| 4 | Puzzle Framework | Builds on environment + notebook |
| 5 | Ending Variable Matrix | Accumulates data from all systems |

This matches Epic ordering: Epics 1-5 build persistence and notebook, Epic 6 adds entity, Epic 7 adds puzzles, Epic 10 adds endings.
