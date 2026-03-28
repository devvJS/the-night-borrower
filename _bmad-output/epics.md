# The Night Borrower - Development Epics

**Generated from GDD:** 2026-03-28
**Total Epics:** 16
**Phases:** 5

---

## Epic Overview

| # | Epic Name | Phase | Dependencies | Est. Stories |
|---|---|---|---|---|
| 1 | Core Technical Foundation | 1 - Foundation | None | 8 |
| 2 | Observation & Interaction Systems | 1 - Foundation | Epic 1 | 6 |
| 3 | Notebook & Knowledge System | 1 - Foundation | Epic 2 | 5 |
| 4 | Day/Night Cycle Framework | 2 - Core Loop | Epic 3 | 5 |
| 5 | Environmental Persistence System | 2 - Core Loop | Epic 4 | 5 |
| 6 | First Horror Loop (Vertical Slice) | 2 - Core Loop | Epics 1–5 | 6 |
| 7 | Puzzle System Framework | 3 - Expansion | Epic 6 | 5 |
| 8 | Area Expansion System | 3 - Expansion | Epic 6 | 5 |
| 9 | Atmosphere & Tension Systems | 3 - Expansion | Epic 6 | 5 |
| 10 | Narrative & Ending System | 4 - Narrative | Phase 3 | 5 |
| 11 | NPC & Customer System | 3 - Expansion | Epic 6 | 4 |
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
1. As a player, I can move through the bookstore using WASD and mouse look so that I can explore the space
2. As a player, I can see objects highlight when I look at them so that I know what is interactable
3. As a player, I can press E to interact with highlighted objects so that I can engage with the environment
4. As a player, I can see basic lighting in the bookstore so that the space feels grounded
5. As a developer, I can load the bookstore scene with acceptable performance so that the technical baseline is validated
6. As a player, I can see a basic UI framework (crosshair, interaction prompts) so that I understand available actions
7. As a developer, I can save and load basic game state so that persistence infrastructure exists
8. As a player, I can toggle the flashlight with F so that I have a portable light source

---

## Epic 2: Observation & Interaction Systems

### Goal
Build the core verbs that define moment-to-moment gameplay.

### Scope
**Includes:** Observation system (passive camera-based detection of important objects), inspection system (focused view with rotation/zoom), object tagging and metadata system (tracks object state, position, importance), early notebook UI framework, basic record logging system (auto-log discoveries), environmental interaction triggers (context-sensitive E for inspect, organize, maintain).

**Excludes:** Full notebook manual entry system, cross-referencing, puzzle integration, NPC interactions.

### Dependencies
Epic 1 (player controller, interaction system, scene structure).

### Deliverable
Player can observe environmental details, inspect objects in focused view, and see discoveries logged automatically.

### Stories
1. As a player, I can notice objects of interest through subtle visual highlighting so that observation feels natural
2. As a player, I can inspect objects in focused view with rotation so that I can gather detailed information
3. As a player, I can see inspection results reveal clues or confirm object state so that inspection feels meaningful
4. As a player, I can see important discoveries automatically logged so that I don't miss critical information
5. As a player, I can organize misplaced objects using context-sensitive interaction so that I can restore environmental order
6. As a player, I can maintain light fixtures using context-sensitive interaction so that I can preserve safe zones

---

## Epic 3: Notebook & Knowledge System

### Goal
Establish the knowledge loop that defines survival progression.

### Scope
**Includes:** Hybrid notebook system (auto-log + manual notes), record organization and categorization, cross-reference functionality, persistent knowledge tracking across sessions, notebook UI readability pass.

**Excludes:** Office PC integration, puzzle cross-referencing, narrative fragment system, ending condition tracking.

### Dependencies
Epic 2 (observation and logging systems).

### Deliverable
Player can open the notebook, review auto-logged discoveries, write manual notes, and cross-reference entries. Knowledge persists across sessions.

### Stories
1. As a player, I can open my notebook with Tab and review auto-logged entries so that I can track what I've discovered
2. As a player, I can add manual notes to the notebook so that I can record personal observations and suspicions
3. As a player, I can organize and categorize notebook entries so that information remains accessible
4. As a player, I can cross-reference entries to identify patterns so that knowledge becomes a survival tool
5. As a player, I can see my notebook persist across play sessions so that accumulated knowledge is never lost

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
1. As a player, I can experience the day phase with warm lighting and routine tasks so that I build familiarity
2. As a player, I can see the environment transition from day to night so that I feel the shift in tone
3. As a player, I can experience night phase with cold lighting and reduced visibility so that tension increases
4. As a player, I can see zone stability indicators change across phases so that I understand environmental state
5. As a player, I can experience the recovery phase after surviving a night so that I get relief and preparation time

---

## Epic 5: Environmental Persistence System

### Goal
Make the world feel reactive, changing, and persistent across days.

### Scope
**Includes:** Environmental state tracking (object positions, lighting states), shelf displacement tracking, lighting failure persistence, object repositioning logic (entity-driven and natural entropy), multi-day state continuity.

**Excludes:** Entity AI behavior, puzzle state tracking, narrative triggers, area unlocking.

### Dependencies
Epic 4 (day/night cycle for multi-day structure).

### Deliverable
Changes from previous days persist — misplaced books remain misplaced, failed lights stay failed until repaired, environmental disorder accumulates.

### Stories
1. As a player, I can see environmental changes persist across day/night cycles so that the world feels reactive
2. As a player, I can notice natural entropy (minor disorder accumulating) so that maintenance feels necessary
3. As a player, I can see lighting failures persist until I repair them so that light feels like a fragile resource
4. As a player, I can observe objects displaced across days so that pattern recognition becomes meaningful
5. As a developer, I can track environmental state through a flag-based system so that persistence is reliable without full scene serialization

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
1. As a player, I can sense the entity's presence through lighting flickers and audio cues so that I feel anticipation
2. As a player, I can see the entity moving at distance or through gaps so that threat feels real but not fully visible
3. As a player, I can avoid the entity by staying in lit areas and breaking line of sight so that survival depends on awareness
4. As a player, I can experience environmental interference during the entity's presence so that instability feels connected to the threat
5. As a player, I can fail (entity catches me) and experience soft consequences so that failure teaches rather than punishes
6. As a player, I can complete a full Day 1 → Night 1 → Recovery cycle so that the core loop is proven

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
1. As a player, I can solve book arrangement puzzles by identifying and correcting misplaced books so that organization becomes a challenge
2. As a player, I can solve code/combination puzzles using information from records so that knowledge has mechanical value
3. As a player, I can cross-reference notebook entries to solve multi-step puzzles so that the knowledge system drives puzzle solving
4. As a player, I can receive in-world hints through notebook entries and environmental cues so that I'm guided without being told
5. As a player, I can fail a puzzle without permanently blocking progress so that experimentation is encouraged

---

## Epic 8: Area Expansion System

### Goal
Unlock new spaces across the 7-day progression.

### Scope
**Includes:** Storage room unlock (Day 3), basement archives unlock (Day 4–5), upstairs apartment unlock (Day 6), area-specific environmental behaviors, navigation expansion and route complexity.

**Excludes:** Exterior areas (street, alley, forest, rail line), area-specific puzzles, narrative content for new areas.

### Dependencies
Epic 6 (vertical slice with core loop proven).

### Deliverable
New areas open progressively across days, each introducing new instability patterns and responsibilities.

### Stories
1. As a player, I can unlock the storage room on Day 3 so that multi-zone management begins
2. As a player, I can unlock the basement archives on Day 4–5 so that high-risk exploration becomes available
3. As a player, I can unlock the upstairs apartment on Day 6 so that narrative discovery and emotional escalation occurs
4. As a player, I can experience unique environmental behaviors in each new area so that expansion feels meaningful
5. As a player, I can navigate increasingly complex routes between areas so that movement becomes a strategic consideration

---

## Epic 9: Atmosphere & Tension Systems

### Goal
Transform mechanics into emotional experience through dynamic atmosphere.

### Scope
**Includes:** Dynamic audio layers (zone stability, lighting state, entity proximity), zone instability visual effects, lighting degradation across days, sound event triggers, ambient environmental systems, visual degradation across cycles.

**Excludes:** Final audio mixing, entity visual polish, music composition, voice acting.

### Dependencies
Epic 6 (vertical slice with day/night and entity systems).

### Deliverable
Environment produces consistent, escalating tension through coordinated visual and audio systems.

### Stories
1. As a player, I can hear ambient audio shift based on zone stability so that sound communicates environmental state
2. As a player, I can hear audio cues before visual detection of the entity so that sound creates anticipation
3. As a player, I can see lighting degrade across days so that visual comfort decreases over time
4. As a player, I can experience silence used intentionally before major events so that absence of sound creates dread
5. As a player, I can feel coordinated visual and audio tension so that atmosphere functions as gameplay

---

## Epic 11: NPC & Customer System

### Goal
Bring daytime interactions to life and ground the routine.

### Scope
**Includes:** Customer behavior loops (enter, browse, request, purchase), dialogue system (text-based with optional short voice lines), interaction prompts (serve, assist), routine-based NPC movement.

**Excludes:** Deep NPC branching dialogue, NPC narrative arcs, customer economy balancing, NPC progression across days.

### Dependencies
Epic 6 (vertical slice with day phase structure).

### Deliverable
Daytime feels active and grounded with believable customer interactions.

### Stories
1. As a player, I can serve customers during day phases so that routine feels grounded and purposeful
2. As a player, I can interact with customers through text-based dialogue so that social interactions support world-building
3. As a player, I can see customers move naturally through the bookstore so that the space feels alive
4. As a player, I can complete service tasks (brew coffee, find books, process transactions) so that routine becomes ritual

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
1. As a player, I can discover narrative fragments through found documents and environmental changes so that story unfolds through gameplay
2. As a player, I can experience scripted narrative moments at key progression points so that the story has anchor beats
3. As a player, I can see my decisions and knowledge accumulate toward ending conditions so that choices feel consequential
4. As a player, I can reach one of multiple endings based on what I preserved and discovered so that replays reveal new outcomes
5. As a player, I can experience meaningful "bad" endings that feel like consequences rather than punishment so that every conclusion has narrative value

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
1. As a player, I can read notebook entries clearly at all supported resolutions so that knowledge is always accessible
2. As a player, I can adjust text size, brightness, and visual settings so that the game is comfortable to play
3. As a player, I can enable reduced flicker lighting mode so that visual effects don't cause discomfort
4. As a player, I can see audio subtitles for key sound cues so that important audio information is accessible
5. As a player, I can understand all interaction prompts and HUD elements intuitively so that UI never blocks gameplay

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
1. As a developer, I can finalize lighting across all areas so that visual atmosphere is consistent and intentional
2. As a developer, I can polish entity visuals so that The Night Borrower feels physically present but visually unstable
3. As a developer, I can balance audio levels across all zones and phases so that sound design feels cohesive
4. As a developer, I can implement final music layers and transitions so that audio escalation across days works seamlessly
5. As a developer, I can verify visual degradation across the 7-day cycle so that the aesthetic journey is complete

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
1. As a player, I can save and load reliably at all autosave points so that progress is never lost unexpectedly
2. As a player, I can recover from interrupted saves without corruption so that edge cases don't destroy progress
3. As a developer, I can test save/load across all 7 days and ending paths so that persistence is validated end-to-end
4. As a developer, I can handle all known edge cases in the save system so that stability is production-ready

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
1. As a developer, I can profile performance across all areas and phases so that bottlenecks are identified
2. As a developer, I can optimize assets to meet memory and performance budgets so that mid-range hardware runs smoothly
3. As a developer, I can optimize lighting to balance atmosphere and performance so that visual quality doesn't sacrifice frame rate
4. As a developer, I can fix all critical and high-priority bugs so that the game is release-stable
5. As a developer, I can validate 60 FPS at 1080p on target hardware so that performance targets are met

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
1. As a developer, I can build and upload to Steam so that the game is distributable
2. As a developer, I can test saves across build versions so that updates don't break player progress
3. As a developer, I can prepare store assets (screenshots, description, tags) so that the Steam page is ready
4. As a developer, I can submit the final build for release so that The Night Borrower ships
