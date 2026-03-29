# The Night Borrower - Development Epics

**Generated from GDD:** 2026-03-28
**Updated:** 2026-03-28 (acceptance criteria added, missing FR coverage inserted)
**Total Epics:** 16
**Total Stories:** 89
**Phases:** 5

---

## Epic Overview

| # | Epic Name | Phase | Dependencies | Est. Stories |
|---|---|---|---|---|
| 1 | Core Technical Foundation | 1 - Foundation | None | 8 |
| 2 | Observation & Interaction Systems | 1 - Foundation | Epic 1 | 7 |
| 3 | Notebook & Knowledge System | 1 - Foundation | Epic 2 | 6 |
| 4 | Day/Night Cycle Framework | 2 - Core Loop | Epic 3 | 5 |
| 5 | Environmental Persistence System | 2 - Core Loop | Epic 4 | 7 |
| 6 | First Horror Loop (Vertical Slice) | 2 - Core Loop | Epics 1–5 | 6 |
| 7 | Puzzle System Framework | 3 - Expansion | Epic 6 | 5 |
| 8 | Area Expansion System | 3 - Expansion | Epic 6 | 6 |
| 9 | Atmosphere & Tension Systems | 3 - Expansion | Epic 6 | 6 |
| 10 | Narrative & Ending System | 4 - Narrative | Phase 3 | 5 |
| 11 | NPC & Customer System | 3 - Expansion | Epic 6 | 5 |
| 12 | UI & UX Completion | 5 - Finalization | Phase 4 | 5 |
| 13 | Audio & Visual Finalization | 5 - Finalization | Phase 4 | 5 |
| 14 | Save System & Stability Pass | 5 - Finalization | Epic 12 | 4 |
| 15 | Polish & Performance Optimization | 5 - Finalization | Epic 14 | 5 |
| 16 | Steam Integration & Release Prep | 5 - Finalization | Epic 15 | 4 |

---

## Phase 1 — Foundation

---

## Epic 1: Core Technical Foundation

### Goal
Establish the technical baseline needed to support all gameplay systems.

### Scope
**Includes:** First-person player controller (WASD + mouse look), basic interaction system (E key context-sensitive), object highlighting system, basic bookstore scene structure, core lighting setup (baked + limited dynamic), initial UI framework, basic save/load placeholder, performance baseline testing.

**Excludes:** Full notebook system, day/night transitions, entity AI, NPC behavior, puzzle logic, audio systems beyond placeholder.

### Dependencies
None — this is the starting point.

### Deliverable
Player can walk through a simple bookstore scene, look around, and interact with highlighted objects.

### Stories

**1.1** As a player, I can move through the bookstore using WASD and mouse look so that I can explore the space
- **AC1:** Given the player is in the bookstore scene, When they press WASD keys, Then the player character moves in the corresponding direction relative to camera facing
- **AC2:** Given the player is moving, When they move the mouse, Then the camera rotates smoothly on X and Y axes with no visible jitter
- **AC3:** Given the player approaches a wall or solid object, When they continue moving toward it, Then the player is blocked by the collider and does not clip through

**1.2** As a player, I can see objects highlight when I look at them so that I know what is interactable
- **AC1:** Given an interactable object exists in the scene, When the player's camera center is aimed at it within interaction range, Then a subtle visual highlight appears on the object
- **AC2:** Given an object is highlighted, When the player looks away or moves out of range, Then the highlight fades within 0.2 seconds
- **AC3:** Given a non-interactable object exists, When the player looks at it, Then no highlight appears

**1.3** As a player, I can press E to interact with highlighted objects so that I can engage with the environment
- **AC1:** Given an object is highlighted, When the player presses E, Then the appropriate context-sensitive interaction fires (inspect, pick up, toggle, etc.)
- **AC2:** Given no object is highlighted, When the player presses E, Then nothing happens and no error occurs
- **AC3:** Given an interaction is in progress, When the player presses E again, Then the input is ignored until the current interaction completes

**1.4** As a player, I can see basic lighting in the bookstore so that the space feels grounded
- **AC1:** Given the bookstore scene loads, When the player looks around, Then baked lighting illuminates the main floor with warm amber tones and visible light sources
- **AC2:** Given a light fixture exists in the scene, When the player observes it, Then the light source and its shadow casting are visually consistent with the fixture's position
- **AC3:** Given the scene is fully lit, When the player walks through all accessible areas, Then no area is completely black or unlit unless intentionally designed as shadow

**1.5** As a developer, I can load the bookstore scene with acceptable performance so that the technical baseline is validated
- **AC1:** Given the bookstore scene with all base assets loaded, When profiled on target hardware (GTX 1060, i5, 8 GB RAM), Then frame rate is stable at 60 FPS or above
- **AC2:** Given the scene loads from the main menu, When the load completes, Then time from trigger to player control is under 5 seconds
- **AC3:** Given the player moves through all accessible areas, When profiled, Then no frame drops below 30 FPS occur

**1.6** As a player, I can see a basic UI framework (crosshair, interaction prompts) so that I understand available actions
- **AC1:** Given the player is in the scene, When they look around, Then a small centered crosshair is visible at all times
- **AC2:** Given the player highlights an interactable object, When the highlight appears, Then a text prompt (e.g., "E: Inspect") appears near the crosshair
- **AC3:** Given the player is not looking at any interactable, When they look at empty space, Then only the crosshair is visible with no prompt text

**1.7** As a developer, I can save and load basic game state so that persistence infrastructure exists
- **AC1:** Given the player has interacted with objects in the scene, When a save is triggered, Then a save file is written to disk containing current object states
- **AC2:** Given a valid save file exists, When a load is triggered, Then the game restores to the saved state with object positions and states matching
- **AC3:** Given no save file exists, When a load is triggered, Then the game handles the missing file gracefully without crashing

**1.8** As a player, I can toggle the flashlight with F so that I have a portable light source
- **AC1:** Given the flashlight is off, When the player presses F, Then the flashlight activates and casts a directional light cone from the player's position
- **AC2:** Given the flashlight is on, When the player presses F, Then the flashlight deactivates and the light cone disappears
- **AC3:** Given the flashlight is on, When the player looks around, Then the light cone follows camera direction with no visible lag

---

## Epic 2: Observation & Interaction Systems

### Goal
Build the core verbs that define moment-to-moment gameplay.

### Scope
**Includes:** Observation system (passive camera-based detection of important objects), inspection system (focused view with rotation/zoom), object tagging and metadata system (tracks object state, position, importance), early notebook UI framework, basic record logging system (auto-log discoveries), environmental interaction triggers (context-sensitive E for inspect, organize, maintain), player inventory system (carry spare bulbs, tools, limited space).

**Excludes:** Full notebook manual entry system, cross-referencing, puzzle integration, NPC interactions.

### Dependencies
Epic 1 (player controller, interaction system, scene structure).

### Deliverable
Player can observe environmental details, inspect objects in focused view, see discoveries logged automatically, and manage a limited inventory of supplies.

### Stories

**2.1** As a player, I can notice objects of interest through subtle visual highlighting so that observation feels natural
- **AC1:** Given an object with important metadata (clue, changed state, misplaced) is within camera view, When the player's view center passes over it within detection range, Then a subtle highlight effect distinguishes it from normal objects
- **AC2:** Given multiple interactable objects are visible, When the player scans the room, Then only contextually important objects receive observation highlights (not every interactable)
- **AC3:** Given an object's importance changes (e.g., a book becomes misplaced), When the player next views it, Then the highlight state updates to reflect current importance

**2.2** As a player, I can inspect objects in focused view with rotation so that I can gather detailed information
- **AC1:** Given an inspectable object is highlighted, When the player presses E, Then the camera transitions to a focused close-up view of the object
- **AC2:** Given the player is in focused view, When they move the mouse, Then the object rotates to reveal different angles and surfaces
- **AC3:** Given the player is in focused view, When they press E or ESC, Then the camera returns smoothly to normal first-person view

**2.3** As a player, I can see inspection results reveal clues or confirm object state so that inspection feels meaningful
- **AC1:** Given an object with associated clue data is inspected, When the focused view opens, Then relevant information (text, visual detail, or state confirmation) is presented to the player
- **AC2:** Given an object has already been inspected and its state has not changed, When inspected again, Then no duplicate discovery is triggered but the player can still view it
- **AC3:** Given an object's state has changed since last inspection, When re-inspected, Then the new state is presented and flagged as changed

**2.4** As a player, I can see important discoveries automatically logged so that I don't miss critical information
- **AC1:** Given the player inspects an object containing a first-time discovery, When the inspection completes, Then a notebook auto-entry is created with the discovery details
- **AC2:** Given an auto-entry is created, When it happens, Then a brief non-intrusive UI notification confirms the entry was logged
- **AC3:** Given a discovery has already been auto-logged, When the same discovery is encountered again, Then no duplicate entry is created

**2.5** As a player, I can organize misplaced objects using context-sensitive interaction so that I can restore environmental order
- **AC1:** Given a book or object is in an incorrect position, When the player highlights it and presses E, Then the interaction prompt shows "Organize" or "Return to shelf"
- **AC2:** Given the player initiates an organize action, When the action completes, Then the object moves to its correct position and the environment state updates
- **AC3:** Given all objects in a section are correctly placed, When the player views the section, Then no organize prompts appear for those objects

**2.6** As a player, I can maintain light fixtures using context-sensitive interaction so that I can preserve safe zones
- **AC1:** Given a light fixture has a burnt-out bulb, When the player highlights it, Then the prompt shows "Replace Bulb" (if the player has a spare) or "Needs Bulb" (if they don't)
- **AC2:** Given the player has a spare bulb and initiates replacement, When the action completes, Then the fixture lights up, the spare bulb is consumed from inventory, and the zone lighting state updates
- **AC3:** Given a fixture is functioning, When the player highlights it, Then no maintenance prompt appears

**2.7** *(NEW — FR30)* As a player, I can carry and manage a limited inventory of supplies so that resource decisions feel meaningful
- **AC1:** Given the player picks up a spare bulb or tool, When the item is added, Then it appears in the inventory with the current count visible
- **AC2:** Given the player's inventory is at capacity, When they attempt to pick up another item, Then they receive feedback that inventory is full and must choose what to drop or leave behind
- **AC3:** Given the player opens the inventory view, When they review contents, Then all carried items (spare bulbs, repair tools) are listed with quantities
- **AC4:** Given the player uses an item (e.g., replacing a bulb), When the action completes, Then the item count decreases by one in inventory

---

## Epic 3: Notebook & Knowledge System

### Goal
Establish the knowledge loop that defines survival progression.

### Scope
**Includes:** Hybrid notebook system (auto-log + manual notes), record organization and categorization, cross-reference functionality, persistent knowledge tracking across sessions, notebook UI readability pass, office PC integration for record review and preparation.

**Excludes:** Puzzle cross-referencing, narrative fragment system, ending condition tracking.

### Dependencies
Epic 2 (observation and logging systems).

### Deliverable
Player can open the notebook, review auto-logged discoveries, write manual notes, cross-reference entries, and use the office PC to review records and prepare for nights. Knowledge persists across sessions.

### Stories

**3.1** As a player, I can open my notebook with Tab and review auto-logged entries so that I can track what I've discovered
- **AC1:** Given the player has auto-logged entries, When they press Tab, Then the notebook opens displaying entries organized by category
- **AC2:** Given the notebook is open, When the player scrolls through entries, Then all auto-logged discoveries are readable with timestamps and source context
- **AC3:** Given the notebook is open, When the player presses Tab or ESC, Then the notebook closes and gameplay resumes

**3.2** As a player, I can add manual notes to the notebook so that I can record personal observations and suspicions
- **AC1:** Given the notebook is open, When the player selects "Add Note," Then a text input area appears for manual entry
- **AC2:** Given the player types a note and confirms, When the note is saved, Then it appears in the notebook under a "Personal Notes" category with a timestamp
- **AC3:** Given the player has written manual notes, When they reopen the notebook later, Then all manual notes persist and are readable

**3.3** As a player, I can organize and categorize notebook entries so that information remains accessible
- **AC1:** Given the notebook contains multiple entries of different types, When the player views the notebook, Then entries are grouped by category (Discoveries, Observations, Personal Notes, etc.)
- **AC2:** Given a category contains more than 5 entries, When the player views that category, Then entries are listed chronologically with clear date/cycle labels
- **AC3:** Given the player navigates between categories, When they switch, Then the transition is immediate with no loading delay

**3.4** As a player, I can cross-reference entries to identify patterns so that knowledge becomes a survival tool
- **AC1:** Given two or more entries reference the same object, location, or event, When the player views one entry, Then related entries are linked and navigable
- **AC2:** Given the player follows a cross-reference link, When they select it, Then the notebook navigates to the linked entry
- **AC3:** Given new entries create cross-reference relationships with existing entries, When the new entry is logged, Then cross-reference links are generated automatically

**3.5** As a player, I can see my notebook persist across play sessions so that accumulated knowledge is never lost
- **AC1:** Given the player has notebook entries and saves the game, When they load the save later, Then all auto-logged and manual entries are restored exactly as saved
- **AC2:** Given entries were created across multiple day/night cycles, When the player loads a save, Then entry timestamps and cycle associations are preserved
- **AC3:** Given the player's notebook contains cross-reference links, When the save is loaded, Then all links remain functional

**3.6** *(NEW — FR23)* As a player, I can use the office PC to review records, monitor stability, and prepare for nights so that knowledge becomes actionable
- **AC1:** Given the player interacts with the office PC, When the PC interface opens, Then the player can view logged observations organized by area and day
- **AC2:** Given environmental instability data exists, When the player checks the stability monitor, Then current stability levels per zone are displayed
- **AC3:** Given the player has inventory items, When they access the PC's resource view, Then current supplies (spare bulbs, tools) and their condition are summarized
- **AC4:** Given the player closes the PC interface, When they press ESC or the close action, Then the interface closes and gameplay resumes with no state loss

---

## Phase 2 — Core Loop

---

## Epic 4: Day/Night Cycle Framework

### Goal
Create the structural loop that drives tension and pacing.

### Scope
**Includes:** Day phase logic (routine tasks available, low tension), night phase transition (lighting shift, audio change), lighting state transitions (warm amber → cold blue-green), zone stability tracking foundation, time progression logic.

**Excludes:** Entity behavior, environmental persistence across days, full audio system, NPC scheduling.

### Dependencies
Epic 3 (knowledge system for recording observations across phases).

### Deliverable
Player experiences a full day-to-night transition with visible lighting and atmospheric changes.

### Stories

**4.1** As a player, I can experience the day phase with warm lighting and routine tasks so that I build familiarity
- **AC1:** Given the day phase begins, When the player enters the bookstore, Then warm amber lighting is active across all accessible areas
- **AC2:** Given the day phase is active, When the player interacts with routine objects (shelves, fixtures, desk), Then context-sensitive day tasks are available (organize, maintain, inspect)
- **AC3:** Given the day phase is active, When the player performs routine tasks, Then task completion is tracked and contributes to environmental order

**4.2** As a player, I can see the environment transition from day to night so that I feel the shift in tone
- **AC1:** Given the day phase is ending, When the transition triggers, Then lighting shifts gradually from warm amber to cold blue-green over a visible transition period
- **AC2:** Given the transition is occurring, When the player observes the environment, Then shadow depth increases and ambient brightness decreases progressively
- **AC3:** Given the transition completes, When the night phase begins, Then the environment has fully shifted to night lighting and audio states

**4.3** As a player, I can experience night phase with cold lighting and reduced visibility so that tension increases
- **AC1:** Given the night phase is active, When the player navigates the bookstore, Then visibility range is noticeably reduced compared to day phase
- **AC2:** Given the night phase is active, When the player enters areas without maintained lighting, Then those areas are significantly darker and harder to navigate
- **AC3:** Given the flashlight is available during night, When the player toggles it on, Then it provides a focused light cone that partially compensates for reduced visibility

**4.4** As a player, I can see zone stability indicators change across phases so that I understand environmental state
- **AC1:** Given a zone has been well-maintained during the day, When the night phase begins, Then that zone's stability remains higher than neglected zones
- **AC2:** Given a zone's stability decreases, When the player checks the office PC or observes environmental cues, Then the lower stability is reflected through visual or data indicators
- **AC3:** Given multiple zones exist, When the player compares them, Then stability differences between maintained and neglected zones are clearly distinguishable

**4.5** As a player, I can experience the recovery phase after surviving a night so that I get relief and preparation time
- **AC1:** Given the player survives the night phase, When the recovery phase begins, Then lighting returns to warm daytime levels and immediate threat pressure is removed
- **AC2:** Given the recovery phase is active, When the player reviews their notebook or office PC, Then they can assess what changed overnight and plan accordingly
- **AC3:** Given the recovery phase ends, When the next day begins, Then environmental state carries forward including any unresolved damage or disorder from the previous night

---

## Epic 5: Environmental Persistence System

### Goal
Make the world feel reactive, changing, and persistent across days.

### Scope
**Includes:** Environmental state tracking (object positions, lighting states), shelf displacement tracking, lighting failure persistence, object repositioning logic (entity-driven and natural entropy), multi-day state continuity, resource degradation over time, dynamic safe zone mechanics.

**Excludes:** Entity AI behavior, puzzle state tracking, narrative triggers, area unlocking.

### Dependencies
Epic 4 (day/night cycle for multi-day structure).

### Deliverable
Changes from previous days persist — misplaced books remain misplaced, failed lights stay failed until repaired, environmental disorder accumulates, resources degrade with use, and safe zones require active maintenance.

### Stories

**5.1** As a player, I can see environmental changes persist across day/night cycles so that the world feels reactive
- **AC1:** Given an object was displaced during the night phase, When the next day begins, Then the object remains in its displaced position
- **AC2:** Given the player organized a section during the day, When the night ends without entity interference in that section, Then the organization persists into the next day
- **AC3:** Given environmental changes accumulate across 3+ days, When the player returns to a neglected area, Then multiple accumulated changes are visible simultaneously

**5.2** As a player, I can notice natural entropy (minor disorder accumulating) so that maintenance feels necessary
- **AC1:** Given the player does not organize a section for one full day/night cycle, When they return, Then minor disorder has appeared (1-2 slightly misplaced items)
- **AC2:** Given entropy has affected a zone, When the player organizes the affected items, Then the zone returns to ordered state
- **AC3:** Given entropy is occurring across multiple zones, When the player checks stability, Then neglected zones show measurably lower stability than maintained zones

**5.3** As a player, I can see lighting failures persist until I repair them so that light feels like a fragile resource
- **AC1:** Given a light fixture fails during the night, When the next day begins, Then the fixture remains off and the area stays dark
- **AC2:** Given a failed fixture exists, When the player repairs it with a spare bulb, Then the light returns and the zone lighting state updates
- **AC3:** Given no spare bulbs are available, When the player encounters a failed fixture, Then they cannot repair it and must work around the dark zone

**5.4** As a player, I can observe objects displaced across days so that pattern recognition becomes meaningful
- **AC1:** Given the entity displaced specific objects during the night, When the player inspects those objects the next day, Then the displacement is detectable through observation and inspection
- **AC2:** Given the player has notebook entries about previous object positions, When they compare current state to recorded state, Then the change is confirmable through cross-reference
- **AC3:** Given displacement follows entity behavioral rules, When the player tracks patterns across multiple days, Then displacement patterns become recognizable and partially predictable

**5.5** As a developer, I can track environmental state through a flag-based system so that persistence is reliable without full scene serialization
- **AC1:** Given the game saves at the start of a new day, When the save file is written, Then all object positions, lighting states, and zone stability values are stored as discrete flags
- **AC2:** Given a save file is loaded, When the environment rebuilds, Then all tracked states match the saved values with no visual discrepancies
- **AC3:** Given the save file size is measured, When 7 days of accumulated state are saved, Then the file remains under 1 MB and saves complete in under 1 second

**5.6** *(NEW — FR29)* As a player, I can see resources degrade with repeated use so that long-term planning matters
- **AC1:** Given a light bulb has been replaced and is functioning, When it has been active for a defined duration, Then its brightness gradually decreases compared to a fresh bulb
- **AC2:** Given a fixture has been repaired multiple times, When repaired again, Then the fixture's reliability decreases (shorter time before next failure)
- **AC3:** Given resource condition is tracked, When the player inspects a fixture or checks the office PC, Then the current condition level of maintained resources is visible

**5.7** *(NEW — FR24)* As a player, I can create and maintain temporary safe zones through lighting and order so that survival depends on preparation
- **AC1:** Given an area has all lights functioning and objects properly organized, When the night phase begins, Then that area is flagged as a temporary safe zone with reduced entity activity
- **AC2:** Given a temporary safe zone exists, When the player neglects maintenance for one full cycle, Then the safe zone degrades and loses its protective status
- **AC3:** Given the player maintains a safe zone during the night, When the entity approaches, Then the entity avoids or is repelled from that zone as long as stability remains above threshold
- **AC4:** Given multiple zones exist, When the player checks stability, Then the difference between a maintained safe zone and an unprotected area is clearly visible through lighting quality and environmental order

---

## Epic 6: First Horror Loop (Vertical Slice)

### Goal
Prove the core gameplay loop with the first controlled Night Borrower encounter.

### Scope
**Includes:** Basic entity presence system, detection logic (line of sight, proximity to instability), simple pursuit behavior, environmental interference (entity alters nearby objects), early sound triggers (telegraphing), line-of-sight reactions, soft failure (night ends prematurely, consequences persist).

**Excludes:** Full entity escalation across 7 days, multiple entity behaviors, advanced pursuit AI, area expansion, puzzle integration, NPC systems.

### Dependencies
Epics 1–5 (all foundation and core loop systems).

### Deliverable
A complete Day 1 → Night 1 cycle. Player opens the bookstore, performs routine tasks, experiences unease, survives a controlled Night Borrower encounter, and reaches recovery. This proves: Routine → Unease → Survival → Relief.

### Stories

**6.1** As a player, I can sense the entity's presence through lighting flickers and audio cues so that I feel anticipation
- **AC1:** Given the entity is approaching the player's zone, When it enters proximity range, Then nearby lights flicker subtly before any visual manifestation
- **AC2:** Given the entity is in proximity, When ambient audio is playing, Then the audio shifts to include subtle environmental tension cues (low hum, quiet scraping)
- **AC3:** Given the entity moves away from the player's zone, When it exits proximity range, Then lighting stabilizes and audio cues fade over 3-5 seconds

**6.2** As a player, I can see the entity moving at distance or through gaps so that threat feels real but not fully visible
- **AC1:** Given the entity is active in a nearby area, When the player looks through shelf gaps or down corridors, Then the entity is partially visible as a moving silhouette or shadow
- **AC2:** Given the entity passes through the player's peripheral view, When the player turns to look directly, Then the entity has moved or is no longer fully visible
- **AC3:** Given the entity is in the same room, When the player observes it, Then the entity is never fully clear — obscured by shelves, darkness, or visual distortion

**6.3** As a player, I can avoid the entity by staying in lit areas and breaking line of sight so that survival depends on awareness
- **AC1:** Given the player is in a well-lit zone, When the entity approaches, Then the entity slows, diverts, or retreats from the lit area
- **AC2:** Given the entity has line of sight to the player in a dark zone, When the player moves behind cover or a shelf, Then line of sight breaks and the entity loses direct tracking
- **AC3:** Given the player is hiding in a stable zone, When the entity patrols nearby, Then the entity does not enter the zone if lighting and order are above threshold

**6.4** As a player, I can experience environmental interference during the entity's presence so that instability feels connected to the threat
- **AC1:** Given the entity is active in a zone, When it passes through, Then nearby objects are displaced (books shifted, items moved)
- **AC2:** Given the entity interferes with an organized area, When the player returns to that area, Then the displacement is visible and measurable as a stability decrease
- **AC3:** Given the entity is near a functioning light, When it passes close enough, Then the light may flicker, dim, or fail entirely

**6.5** As a player, I can fail (entity catches me) and experience soft consequences so that failure teaches rather than punishes
- **AC1:** Given the entity catches the player, When the catch event triggers, Then the current night ends prematurely and a brief transition occurs
- **AC2:** Given the player was caught, When the next day begins, Then certain notebook entries may be corrupted or altered, and environmental disorder is worse than it would have been
- **AC3:** Given the player was caught, When they review their state, Then the game has not reset — all prior progress and consequences carry forward

**6.6** As a player, I can complete a full Day 1 → Night 1 → Recovery cycle so that the core loop is proven
- **AC1:** Given the player starts Day 1, When they perform routine tasks (organize, maintain, inspect), Then the game tracks task completion and environmental state
- **AC2:** Given Day 1 ends, When Night 1 begins, Then the full transition occurs and the entity becomes active with controlled Night 1 behavior
- **AC3:** Given the player survives Night 1, When the recovery phase triggers, Then the player returns to daytime with all accumulated state intact and ready for Day 2

---

## Phase 3 — Feature Expansion

---

## Epic 7: Puzzle System Framework

### Goal
Build the intellectual challenge layer using core mechanics.

### Scope
**Includes:** Book arrangement puzzles, code/combination systems using discovered knowledge, cross-reference puzzles, environmental logic puzzles, hint system foundation (notebook entries, environmental cues, PC reference).

**Excludes:** All late-game puzzles, puzzle-entity interaction, narrative-driven puzzle content.

### Dependencies
Epic 6 (vertical slice proving core mechanics that puzzles build on).

### Deliverable
Player can solve simple integrated puzzles using observe, inspect, record, and organize mechanics.

### Stories

**7.1** As a player, I can solve book arrangement puzzles by identifying and correcting misplaced books so that organization becomes a challenge
- **AC1:** Given a shelf has books in incorrect positions, When the player identifies the correct order through clues (labels, catalog, notebook), Then they can rearrange books using the organize mechanic
- **AC2:** Given all books are placed correctly, When the puzzle completes, Then a reward triggers (area unlocks, record reveals, stability bonus)
- **AC3:** Given the player places books incorrectly, When they attempt to complete the arrangement, Then the puzzle does not resolve but no penalty is applied

**7.2** As a player, I can solve code/combination puzzles using information from records so that knowledge has mechanical value
- **AC1:** Given a locked container or system requires a code, When the player has discovered the code through records or inspection, Then entering the correct code unlocks the target
- **AC2:** Given the player enters an incorrect code, When they submit it, Then the lock does not open and the player receives feedback that the code is wrong
- **AC3:** Given the code was discovered across multiple records, When the player cross-references entries, Then the combined information produces the correct code

**7.3** As a player, I can cross-reference notebook entries to solve multi-step puzzles so that the knowledge system drives puzzle solving
- **AC1:** Given a puzzle requires information from two or more notebook entries, When the player views related entries, Then cross-reference links point toward the needed information
- **AC2:** Given the player has all required entries, When they apply the combined knowledge to the puzzle, Then the puzzle accepts the solution
- **AC3:** Given the player is missing one or more required entries, When they attempt the puzzle, Then the puzzle cannot be completed but contextual hints guide them toward what's missing

**7.4** As a player, I can receive in-world hints through notebook entries and environmental cues so that I'm guided without being told
- **AC1:** Given a puzzle remains unsolved after a defined time threshold, When the player views related notebook entries, Then a contextual hint is subtly added to the entry
- **AC2:** Given environmental cues exist near a puzzle, When the player observes the area carefully, Then visual alignment cues or repeated motifs indicate the solution direction
- **AC3:** Given the player uses the office PC reference tools, When they look up relevant information, Then historical data or cross-references provide indirect guidance

**7.5** As a player, I can fail a puzzle without permanently blocking progress so that experimentation is encouraged
- **AC1:** Given the player fails a puzzle attempt, When the failure occurs, Then the puzzle resets to a solvable state (possibly with minor environmental consequence)
- **AC2:** Given a puzzle failure has environmental consequences, When the consequences apply, Then the impact is limited to increased local instability — not permanent progress loss
- **AC3:** Given the player leaves a puzzle unsolved, When they return later, Then the puzzle remains available in its current state

---

## Epic 8: Area Expansion System

### Goal
Unlock new spaces across the 7-day progression.

### Scope
**Includes:** Storage room unlock (Day 3), basement archives unlock (Day 4–5), upstairs apartment unlock (Day 6), area-specific environmental behaviors, navigation expansion and route complexity, exterior zones (street front, alley, forest edge, closed rail line).

**Excludes:** Area-specific puzzles, narrative content for new areas.

### Dependencies
Epic 6 (vertical slice with core loop proven).

### Deliverable
New areas open progressively across days — both interior and exterior — each introducing new instability patterns, responsibilities, and environmental risk.

### Stories

**8.1** As a player, I can unlock the storage room on Day 3 so that multi-zone management begins
- **AC1:** Given Day 3 has started, When the player approaches the storage room door, Then access is granted and the storage room is explorable
- **AC2:** Given the storage room is accessible, When the player enters, Then maintenance supplies and replacement components are available inside
- **AC3:** Given the storage room is now part of the managed environment, When the player neglects it, Then entropy and instability accumulate there independently

**8.2** As a player, I can unlock the basement archives on Day 4–5 so that high-risk exploration becomes available
- **AC1:** Given the unlock trigger for the basement fires (Day 4 or 5 depending on progression), When the player descends, Then the basement archives are explorable with poor baseline lighting
- **AC2:** Given the basement has poor lighting coverage, When the player navigates it, Then they must rely on flashlight and any maintained fixtures more heavily than in upper areas
- **AC3:** Given the basement has high-risk environmental behavior, When instability events occur, Then they are more frequent and severe than on the main floor

**8.3** As a player, I can unlock the upstairs apartment on Day 6 so that narrative discovery and emotional escalation occurs
- **AC1:** Given Day 6 has started, When the player accesses the stairway, Then the upstairs apartment is explorable
- **AC2:** Given the apartment contains personal records and narrative objects, When the player inspects them, Then narrative discoveries are triggered and auto-logged
- **AC3:** Given the apartment has late-game risk levels, When the player is there during night, Then instability and entity presence are elevated

**8.4** As a player, I can experience unique environmental behaviors in each new area so that expansion feels meaningful
- **AC1:** Given the storage room is unlocked, When instability occurs there, Then it manifests differently than on the main floor (e.g., supply displacement, equipment failures)
- **AC2:** Given the basement is unlocked, When the player observes it over multiple days, Then its instability patterns are distinct (e.g., deeper shadows, sound echoes, record corruption)
- **AC3:** Given the apartment is unlocked, When instability occurs there, Then it carries emotional and narrative weight distinct from mechanical zones

**8.5** As a player, I can navigate increasingly complex routes between areas so that movement becomes a strategic consideration
- **AC1:** Given multiple areas are unlocked, When the player plans movement between them, Then route options exist with varying distance, lighting coverage, and risk levels
- **AC2:** Given a preferred route becomes unstable, When the player encounters degraded lighting or blocked paths, Then alternate routes are available
- **AC3:** Given the player is in the basement during night, When they need to return to the office, Then the return route feels tense due to distance and lighting conditions

**8.6** *(NEW — FR34)* As a player, I can access exterior zones so that the world extends beyond the bookstore walls
- **AC1:** Given the street front is accessible during day phases, When the player exits the front door, Then they can receive deliveries and observe the town environment
- **AC2:** Given the alley behind the store is accessible after early progression, When the player enters it, Then poor lighting and medium-risk instability patterns are present
- **AC3:** Given the forest edge is accessible in late game, When the player explores it, Then high environmental uncertainty and rare resource discovery opportunities exist
- **AC4:** Given the closed rail line is accessible in late game, When the player visits, Then narrative-heavy content is available with minimal mechanical use but high atmospheric importance

---

## Epic 9: Atmosphere & Tension Systems

### Goal
Transform mechanics into emotional experience through dynamic atmosphere.

### Scope
**Includes:** Dynamic audio layers (zone stability, lighting state, entity proximity), zone instability visual effects, lighting degradation across days, sound event triggers, ambient environmental systems, visual degradation across cycles, dynamic safe zone feedback, adaptive difficulty elements.

**Excludes:** Final audio mixing, entity visual polish, music composition, voice acting.

### Dependencies
Epic 6 (vertical slice with day/night and entity systems).

### Deliverable
Environment produces consistent, escalating tension through coordinated visual and audio systems, with adaptive feedback that maintains challenge without punishing struggling players.

### Stories

**9.1** As a player, I can hear ambient audio shift based on zone stability so that sound communicates environmental state
- **AC1:** Given a zone is stable (maintained, well-lit), When the player enters it, Then ambient audio is calm and predictable (soft bookstore sounds, gentle hum)
- **AC2:** Given a zone is unstable (neglected, poor lighting), When the player enters it, Then ambient audio introduces subtle irregular sounds (unexpected creaks, tonal shifts)
- **AC3:** Given a zone's stability changes while the player is present, When stability decreases, Then the audio transition occurs gradually over several seconds

**9.2** As a player, I can hear audio cues before visual detection of the entity so that sound creates anticipation
- **AC1:** Given the entity is approaching, When it enters audio detection range, Then the player hears low-frequency tonal changes before any visual sign
- **AC2:** Given the entity is in the same zone, When audio cues are active, Then the cues intensify as the entity gets closer
- **AC3:** Given the entity departs the zone, When it leaves audio range, Then the cues fade to normal ambient levels over 3-5 seconds

**9.3** As a player, I can see lighting degrade across days so that visual comfort decreases over time
- **AC1:** Given Day 1 lighting is fully stable, When the player reaches Day 4+, Then baseline lighting brightness is noticeably lower and shadow depth has increased
- **AC2:** Given lighting degrades across days, When the player compares the same area on Day 1 vs Day 6, Then the difference in visual comfort is clearly perceptible
- **AC3:** Given lighting has degraded, When the player repairs and maintains fixtures, Then local lighting quality improves but never fully returns to Day 1 levels

**9.4** As a player, I can experience silence used intentionally before major events so that absence of sound creates dread
- **AC1:** Given a major instability event is about to trigger, When the event approaches, Then ambient sound fades to near-silence for 2-4 seconds before the event
- **AC2:** Given silence occurs, When the player notices the absence, Then it feels unnatural and unsettling compared to normal ambient sound
- **AC3:** Given the silence ends, When the event triggers, Then the return of sound (or a subtle sound) marks the moment of change

**9.5** As a player, I can feel coordinated visual and audio tension so that atmosphere functions as gameplay
- **AC1:** Given a zone is unstable, When the player is present, Then both visual effects (flickering, shadow depth) and audio effects (tonal tension, irregular sounds) are active simultaneously
- **AC2:** Given the player moves from an unstable zone to a stable zone, When the transition occurs, Then both visual and audio atmospheres shift together — not independently
- **AC3:** Given the late game (Day 6-7) is reached, When the player navigates any area, Then visual and audio tension are elevated across all zones compared to early-game levels

**9.6** *(NEW — FR38)* As a player, I can experience subtle adaptive difficulty so that the game remains challenging but fair after repeated failures
- **AC1:** Given the player has failed (caught by entity) multiple times in a row, When the next night begins, Then instability growth is slightly slower than it would otherwise be
- **AC2:** Given a puzzle has remained unsolved past a defined threshold, When the player next views related hints, Then hint visibility is increased (more prominent notebook entries, clearer environmental cues)
- **AC3:** Given the player has failed and restarted a cycle, When they receive adaptive assistance, Then the assistance is invisible — no UI indicator, no difficulty label, no player notification
- **AC4:** Given the player succeeds after receiving adaptive assistance, When the next cycle begins, Then the adaptive adjustments taper off and standard difficulty resumes

---

## Epic 11: NPC & Customer System

### Goal
Bring daytime interactions to life and ground the routine.

### Scope
**Includes:** Customer behavior loops (enter, browse, request, purchase), dialogue system (text-based with optional short voice lines), interaction prompts (serve, assist), routine-based NPC movement, customer influence on supply availability.

**Excludes:** Deep NPC branching dialogue, NPC narrative arcs, NPC progression across days.

### Dependencies
Epic 6 (vertical slice with day phase structure).

### Deliverable
Daytime feels active and grounded with believable customer interactions that influence resource flow.

### Stories

**11.1** As a player, I can serve customers during day phases so that routine feels grounded and purposeful
- **AC1:** Given a customer approaches the service desk, When the player interacts with them, Then a context-sensitive service task begins (find book, brew coffee, process transaction)
- **AC2:** Given the player completes a service task, When the task finishes, Then the customer reacts positively and leaves or continues browsing
- **AC3:** Given the player ignores a waiting customer, When enough time passes, Then the customer leaves on their own with no service completed

**11.2** As a player, I can interact with customers through text-based dialogue so that social interactions support world-building
- **AC1:** Given a customer initiates dialogue, When the text appears, Then dialogue is displayed in a readable text box with character identification
- **AC2:** Given dialogue requires player response, When response options appear, Then the player can select from available options using mouse or keyboard
- **AC3:** Given dialogue completes, When the conversation ends, Then the UI closes cleanly and gameplay resumes

**11.3** As a player, I can see customers move naturally through the bookstore so that the space feels alive
- **AC1:** Given the day phase is active, When customers are present, Then they follow believable paths — entering, browsing shelves, sitting in the cafe area, and leaving
- **AC2:** Given a customer is browsing, When the player observes them, Then their movement feels natural (pausing at shelves, picking up books, looking around)
- **AC3:** Given multiple customers are present, When they move simultaneously, Then they do not clip through each other or walk through furniture

**11.4** As a player, I can complete service tasks (brew coffee, find books, process transactions) so that routine becomes ritual
- **AC1:** Given a customer requests a specific book, When the player locates and provides it, Then the transaction completes and the task is tracked
- **AC2:** Given a customer requests coffee, When the player interacts with the cafe equipment, Then the brewing task completes through a simple interaction sequence
- **AC3:** Given the player completes multiple service tasks in one day, When the day ends, Then task completion count is tracked and available for review

**11.5** *(NEW — FR28)* As a player, I can see customer satisfaction influence supply availability so that service has survival consequences
- **AC1:** Given the player served customers well during the day, When the next day's supplies arrive, Then supply quantity or quality is slightly improved (extra bulb, better tool condition)
- **AC2:** Given the player neglected or failed customer interactions, When the next day's supplies arrive, Then supply availability is reduced compared to baseline
- **AC3:** Given customer satisfaction is high, When a special delivery event triggers, Then rare materials or bonus resources become available that would not appear otherwise

---

## Phase 4 — Narrative Integration

---

## Epic 10: Narrative & Ending System

### Goal
Deliver story progression and consequence-driven endings.

### Scope
**Includes:** Narrative fragment system (found documents, environmental storytelling, dialogue reveals), event triggers (scripted narrative moments), ending conditions tracking (variable-based), multi-ending logic (up to 10 variable-tracked outcomes).

**Excludes:** Voice acting implementation, cutscene production, NPC narrative arc expansion beyond base system.

### Dependencies
Phase 3 systems (puzzles, areas, atmosphere, NPCs) must be in place for narrative to layer onto.

### Deliverable
Player reaches narrative resolution through one of multiple endings determined by accumulated knowledge, pattern recognition, and survival decisions.

### Stories

**10.1** As a player, I can discover narrative fragments through found documents and environmental changes so that story unfolds through gameplay
- **AC1:** Given a narrative document exists in the environment, When the player inspects it, Then the document content is displayed and an auto-entry is logged
- **AC2:** Given an environmental change carries narrative significance, When the player observes and records it, Then the narrative implication is reflected in notebook entries
- **AC3:** Given narrative fragments are scattered across areas and days, When the player collects multiple fragments, Then cross-references between them reveal deeper story connections

**10.2** As a player, I can experience scripted narrative moments at key progression points so that the story has anchor beats
- **AC1:** Given the player reaches a progression trigger (specific day, area unlock, discovery threshold), When the trigger fires, Then a scripted narrative event occurs (environmental change, new document appearance, NPC dialogue shift)
- **AC2:** Given a scripted event occurs, When the player is present, Then the event is noticeable but does not forcibly interrupt gameplay (no mandatory cutscene)
- **AC3:** Given the player misses a scripted trigger (not in the right area), When they later visit the area, Then the event's result is still present and discoverable

**10.3** As a player, I can see my decisions and knowledge accumulate toward ending conditions so that choices feel consequential
- **AC1:** Given the player maintains accurate records throughout, When the ending evaluation runs, Then knowledge preservation score is high
- **AC2:** Given the player identified and correctly tracked environmental patterns, When the evaluation runs, Then pattern recognition score reflects their accuracy
- **AC3:** Given the player's scores across multiple tracked variables are calculated, When the final cycle ends, Then the appropriate ending is selected based on combined variable thresholds

**10.4** As a player, I can reach one of multiple endings based on what I preserved and discovered so that replays reveal new outcomes
- **AC1:** Given the player completes the final cycle, When the ending triggers, Then the ending presented corresponds to their tracked variable scores
- **AC2:** Given different variable combinations produce different endings, When a player replays with different choices, Then they can reach a different ending
- **AC3:** Given the game has up to 10 endings, When any ending is reached, Then it provides narrative closure appropriate to the player's journey

**10.5** As a player, I can experience meaningful "bad" endings that feel like consequences rather than punishment so that every conclusion has narrative value
- **AC1:** Given the player reaches a low-scoring ending, When the ending plays, Then it presents a coherent narrative conclusion — not a generic "game over" screen
- **AC2:** Given a bad ending reflects specific failures, When the player reviews what happened, Then they can identify what went wrong (lost records, missed patterns, failed maintenance)
- **AC3:** Given any ending is reached, When the credits or end screen appears, Then the player is returned to a state where they can start a new playthrough

---

## Phase 5 — Finalization

---

## Epic 12: UI & UX Completion

### Goal
Make the game readable, intuitive, and accessible.

### Scope
**Includes:** Final notebook UI polish, HUD refinement, interaction prompt clarity, accessibility settings (text size, brightness, reduced flicker, audio subtitles), readability improvements across all UI.

**Excludes:** New feature development, gameplay changes, content additions.

### Dependencies
Phase 4 (all gameplay systems finalized).

### Deliverable
All systems feel clear, usable, and accessible.

### Stories

**12.1** As a player, I can read notebook entries clearly at all supported resolutions so that knowledge is always accessible
- **AC1:** Given the notebook is open at 1080p, When the player reads any entry, Then text is sharp, legible, and appropriately sized without squinting
- **AC2:** Given the notebook is open at 1440p, When the player reads any entry, Then text scales correctly and remains readable
- **AC3:** Given the player has adjusted text size in settings, When they open the notebook, Then entries reflect the chosen text size

**12.2** As a player, I can adjust text size, brightness, and visual settings so that the game is comfortable to play
- **AC1:** Given the player opens the settings menu, When they adjust text size, Then all in-game text (notebook, prompts, dialogue) updates to the selected size
- **AC2:** Given the player adjusts brightness or gamma, When they return to gameplay, Then the visual environment reflects the new settings
- **AC3:** Given settings are changed, When the player saves and reloads, Then all settings persist

**12.3** As a player, I can enable reduced flicker lighting mode so that visual effects don't cause discomfort
- **AC1:** Given the reduced flicker option is enabled, When lights flicker in-game, Then the flicker effect is replaced with a smooth dimming transition
- **AC2:** Given the option is enabled, When entity proximity causes lighting effects, Then all rapid light changes are softened
- **AC3:** Given the option is toggled during gameplay, When the change applies, Then it takes effect immediately without requiring a restart

**12.4** As a player, I can see audio subtitles for key sound cues so that important audio information is accessible
- **AC1:** Given audio subtitles are enabled, When a key sound cue plays (entity approach, environmental failure, scripted event), Then a text description appears on screen
- **AC2:** Given the subtitle is displayed, When the sound cue ends, Then the subtitle fades after a brief delay
- **AC3:** Given multiple sound cues overlap, When subtitles are active, Then they stack or cycle without obscuring gameplay

**12.5** As a player, I can understand all interaction prompts and HUD elements intuitively so that UI never blocks gameplay
- **AC1:** Given an interactable object is highlighted, When the prompt appears, Then the action label is clear and concise (e.g., "Inspect," "Organize," "Replace Bulb")
- **AC2:** Given the player is in a time-sensitive situation, When HUD elements are visible, Then they do not obscure the player's view of critical environmental information
- **AC3:** Given the player encounters a new interaction type for the first time, When the prompt appears, Then the action is self-explanatory without requiring a separate tutorial

---

## Epic 13: Audio & Visual Finalization

### Goal
Refine atmosphere into the game's final identity.

### Scope
**Includes:** Final lighting passes (per-area polish), entity visual polish, sound balancing across all areas, music layering and transitions, visual transition polish (day/night, area changes).

**Excludes:** New system development, feature additions, gameplay rebalancing.

### Dependencies
Phase 4 (all content and systems in place).

### Deliverable
Game achieves its intended atmospheric mood across all areas and phases.

### Stories

**13.1** As a developer, I can finalize lighting across all areas so that visual atmosphere is consistent and intentional
- **AC1:** Given every playable area is lit, When reviewed sequentially, Then each area has a distinct lighting identity consistent with its risk level and emotional tone
- **AC2:** Given day and night phases exist, When each area is viewed in both phases, Then lighting transitions feel cohesive and intentional
- **AC3:** Given the lighting budget is defined, When profiled, Then dynamic light count per zone stays within performance budget

**13.2** As a developer, I can polish entity visuals so that The Night Borrower feels physically present but visually unstable
- **AC1:** Given the entity is visible, When the player observes it, Then it appears as an elongated silhouette with irregular proportions and subtle environmental distortion
- **AC2:** Given the entity moves, When observed, Then its motion feels slightly delayed or unnatural compared to normal environmental movement
- **AC3:** Given the entity is in partial view, When seen through shelf gaps or at distance, Then it remains visually ambiguous — the player never sees its complete form clearly

**13.3** As a developer, I can balance audio levels across all zones and phases so that sound design feels cohesive
- **AC1:** Given the player moves between zones, When audio transitions occur, Then volume levels and ambient layers feel balanced with no jarring volume jumps
- **AC2:** Given day and night phases have different audio profiles, When transitions occur, Then the audio shift is smooth and gradual
- **AC3:** Given entity presence audio, environmental sounds, and music all play simultaneously, When layered, Then no single layer dominates or becomes inaudible

**13.4** As a developer, I can implement final music layers and transitions so that audio escalation across days works seamlessly
- **AC1:** Given Day 1 has soft warm tonal patterns, When compared to Day 7, Then music layers have progressively increased in dissonance and tonal pressure
- **AC2:** Given music transitions between phases, When day shifts to night, Then the musical transition supports the tonal shift without abrupt changes
- **AC3:** Given late-game music layers are active, When the player enters a stable zone, Then music subtly reflects the relative safety without fully relaxing

**13.5** As a developer, I can verify visual degradation across the 7-day cycle so that the aesthetic journey is complete
- **AC1:** Given Day 1 visuals are warm and stable, When Day 7 is reached, Then the visual environment has measurably shifted toward colder tones, deeper shadows, and reduced comfort
- **AC2:** Given each day has a target visual baseline, When each day is reviewed, Then degradation progresses consistently without abrupt visual jumps between days
- **AC3:** Given the player has maintained areas well, When those areas are viewed in late game, Then they still show some degradation but remain noticeably better than neglected areas

---

## Epic 14: Save System & Stability Pass

### Goal
Ensure reliable persistence and crash-free operation.

### Scope
**Includes:** Full flag-based save system implementation, load testing across all progression states, edge-case handling (interrupted saves, corrupted state), crash prevention, save recovery safeguards.

**Excludes:** Cloud save integration, new features, performance optimization.

### Dependencies
Epic 12 (UI complete for save/load interface).

### Deliverable
Game runs reliably across all sessions with no save corruption or progression-blocking issues.

### Stories

**14.1** As a player, I can save and load reliably at all autosave points so that progress is never lost unexpectedly
- **AC1:** Given an autosave triggers at day start, When the save completes, Then a valid save file exists and a confirmation indicator appears briefly
- **AC2:** Given an autosave triggers at night end, When the save completes, Then environmental state, notebook entries, inventory, and progression flags are all persisted
- **AC3:** Given a save file is loaded, When the game restores, Then the player's position, environmental state, notebook, inventory, and cycle progress match the saved state exactly

**14.2** As a player, I can recover from interrupted saves without corruption so that edge cases don't destroy progress
- **AC1:** Given a save is in progress, When the process is interrupted (crash, power loss, force quit), Then the previous valid save remains intact and loadable
- **AC2:** Given the game detects a corrupted save file, When load is attempted, Then the game falls back to the most recent valid save and notifies the player
- **AC3:** Given backup saves are maintained, When the primary save is corrupted, Then the backup save is available as a recovery option

**14.3** As a developer, I can test save/load across all 7 days and ending paths so that persistence is validated end-to-end
- **AC1:** Given a save is created on each of the 7 days, When each save is loaded, Then the game state for that day is accurate and playable
- **AC2:** Given different ending paths are reached, When saves along each path are tested, Then variable tracking and ending conditions load correctly
- **AC3:** Given edge-case saves (caught by entity, mid-puzzle, during transition), When loaded, Then the game recovers to a valid playable state

**14.4** As a developer, I can handle all known edge cases in the save system so that stability is production-ready
- **AC1:** Given the save file is tampered with or manually edited, When loaded, Then the game detects invalid data and falls back to a safe state rather than crashing
- **AC2:** Given the disk is full, When a save is attempted, Then the game notifies the player of insufficient storage without losing the current session state
- **AC3:** Given rapid save/load cycles are triggered, When executed, Then no race conditions or data corruption occur

---

## Epic 15: Polish & Performance Optimization

### Goal
Meet performance targets and eliminate rough edges.

### Scope
**Includes:** Performance profiling, asset optimization (textures, models, audio), lighting optimization (baked vs dynamic balance), memory usage reduction, bug fixing pass.

**Excludes:** New content, feature additions, save system changes.

### Dependencies
Epic 14 (save system stable before optimization pass).

### Deliverable
Game meets 60 FPS target on mid-range hardware at 1080p with no critical bugs.

### Stories

**15.1** As a developer, I can profile performance across all areas and phases so that bottlenecks are identified
- **AC1:** Given the profiler is running, When each area is tested during day and night phases, Then frame time, draw calls, and memory usage are logged per area per phase
- **AC2:** Given profiling data is collected, When bottlenecks are identified, Then each bottleneck is documented with its cause (lighting, geometry, audio, scripts)
- **AC3:** Given profiling covers the full 7-day cycle, When late-game areas are tested, Then performance data reflects maximum-complexity scenarios

**15.2** As a developer, I can optimize assets to meet memory and performance budgets so that mid-range hardware runs smoothly
- **AC1:** Given texture budgets are defined, When all textures are reviewed, Then oversized textures are downscaled or compressed to meet budget
- **AC2:** Given model complexity is reviewed, When high-poly models are identified, Then LODs or simplified versions are implemented for distant viewing
- **AC3:** Given audio files are reviewed, When uncompressed or oversized audio is found, Then it is compressed to appropriate quality levels

**15.3** As a developer, I can optimize lighting to balance atmosphere and performance so that visual quality doesn't sacrifice frame rate
- **AC1:** Given baked lighting is the primary system, When dynamic lights are counted per zone, Then no zone exceeds the defined dynamic light budget
- **AC2:** Given lighting optimization is applied, When profiled on target hardware, Then lighting-related frame time stays within budget
- **AC3:** Given lighting is optimized, When visually reviewed, Then atmospheric quality remains consistent with the art direction

**15.4** As a developer, I can fix all critical and high-priority bugs so that the game is release-stable
- **AC1:** Given a bug tracking list exists, When all critical bugs are reviewed, Then each is resolved with no regressions introduced
- **AC2:** Given high-priority bugs are fixed, When the game is played end-to-end, Then no progression blockers or crash-causing issues remain
- **AC3:** Given bug fixes are complete, When a full regression pass is run, Then previously fixed bugs remain fixed

**15.5** As a developer, I can validate 60 FPS at 1080p on target hardware so that performance targets are met
- **AC1:** Given the game runs on GTX 1060 / i5 / 8 GB RAM at 1080p, When profiled across all areas and phases, Then average frame rate is 60 FPS or above
- **AC2:** Given worst-case scenarios (late-game, entity active, multiple unstable zones), When profiled, Then frame rate does not drop below 30 FPS
- **AC3:** Given frame pacing is measured, When analyzed, Then frame time variance stays within acceptable bounds (no persistent stutter or hitching)

---

## Epic 16: Steam Integration & Release Preparation

### Goal
Prepare for distribution and public release.

### Scope
**Includes:** Steam build integration, save compatibility testing across builds, achievement implementation (optional), store asset preparation (screenshots, description, tags).

**Excludes:** Post-launch content, DLC planning, marketing strategy.

### Dependencies
Epic 15 (optimization complete, game release-stable).

### Deliverable
Game is ready for Steam submission and public release.

### Stories

**16.1** As a developer, I can build and upload to Steam so that the game is distributable
- **AC1:** Given the final build is compiled, When uploaded to Steamworks, Then the build passes Steam's validation checks
- **AC2:** Given the build is uploaded, When a tester downloads via Steam, Then the game installs and launches correctly
- **AC3:** Given the Steam client manages the install, When the game is run, Then save paths, working directory, and Steam overlay function correctly

**16.2** As a developer, I can test saves across build versions so that updates don't break player progress
- **AC1:** Given a save file from the previous build exists, When the new build loads it, Then the save loads successfully with no data loss or corruption
- **AC2:** Given a save file format has changed between versions, When the new build detects an old format, Then migration logic converts it and preserves player progress
- **AC3:** Given save compatibility is validated, When multiple build versions are tested, Then forward-compatibility is confirmed for planned update scenarios

**16.3** As a developer, I can prepare store assets (screenshots, description, tags) so that the Steam page is ready
- **AC1:** Given the game is in final visual state, When screenshots are captured, Then they represent actual gameplay across day/night phases and multiple areas
- **AC2:** Given the store description is written, When reviewed, Then it accurately describes the game's genre, mechanics, and unique features
- **AC3:** Given tags are selected, When the store page is reviewed, Then tags match the game's genre positioning (psychological horror, atmospheric, indie, single-player)

**16.4** As a developer, I can submit the final build for release so that The Night Borrower ships
- **AC1:** Given all previous epics are complete and validated, When the release build is compiled, Then it includes all content and passes a final smoke test
- **AC2:** Given the build is submitted to Steam, When the release date arrives, Then the game becomes purchasable and downloadable
- **AC3:** Given the game is live, When a player purchases and downloads it, Then the full experience is playable from start to any ending without blockers
