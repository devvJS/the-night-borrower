# Implementation Readiness Assessment Report

**Date:** 2026-03-28
**Project:** the-night-borrower
**Run:** 2 (re-assessment after epic updates)

---

## Step 1: Document Discovery

**stepsCompleted:** [step-01-document-discovery]

### Documents Included

| Document | Location | Size | Modified |
|---|---|---|---|
| GDD | `_bmad-output/gdd.md` | 75 KB | 2026-03-28 10:08 |
| Architecture | `_bmad-output/system-architecture.md` | 52 KB | 2026-03-28 11:01 |
| Epics & Stories | `_bmad-output/epics.md` | 66 KB | 2026-03-28 12:36 |
| Narrative Design | `_bmad-output/narrative-design.md` | 117 KB | 2026-03-28 11:56 |
| Systems Spec | `_bmad-output/systems-spec.md` | 37 KB | 2026-03-28 10:39 |
| Game Brief | `_bmad-output/game-brief.md` | 24 KB | 2026-03-28 08:06 |

### Notes

- No duplicates found
- No UX Design document present (optional — not a blocker)
- Epics updated since last assessment: AC added to all stories, 7 new FR stories inserted

---

## Step 2: GDD Analysis

**stepsCompleted:** [step-01-document-discovery, step-02-gdd-analysis]

*GDD unchanged from previous assessment (md5: a3aa12b1efd004e7bcf630342302a0f7). Full extraction carried forward.*

### Functional Requirements

| ID | Requirement | GDD Section |
|---|---|---|
| FR1 | Observation system (camera-based detection of inconsistencies) | Primary Mechanics (Observe) |
| FR2 | Inspection system (focused view with rotation/zoom) | Primary Mechanics (Inspect) |
| FR3 | Persistent hybrid notebook (auto-log + manual notes, persists across cycles) | Primary Mechanics (Record) |
| FR4 | Environmental organization (sorting books, fixing objects) | Primary Mechanics (Organize) |
| FR5 | Lighting maintenance system (replace bulbs, repair fixtures) | Primary Mechanics (Maintain) |
| FR6 | Navigation system (first-person movement, expanding spaces) | Primary Mechanics (Navigate) |
| FR7 | Entity avoidance system (hiding, repositioning, staying in light) | Primary Mechanics (Avoid) |
| FR8 | Customer service interactions (assist, serve, maintain store) | Primary Mechanics (Serve) |
| FR9 | Day/Night cycle (7-8 cycles, 4 phases, 30-60 min per cycle) | Core Gameplay Loop |
| FR10 | Environmental persistence (state-driven changes across days) | Environmental State Tracking |
| FR11 | Entity behavior system (escalating manifestation Night 1-7) | Enemy/Threat Design |
| FR12 | Entity telegraphing (lighting, audio, environmental signals) | Enemy/Threat Design |
| FR13 | Light economy (fixtures, bulbs, power distribution, limited supplies) | Resource Scarcity |
| FR14 | Knowledge loss/recovery (records altered/erased, partial recovery) | Resource Scarcity |
| FR15 | Environmental entropy (order decays, entity accelerates decay) | Resource Scarcity |
| FR16 | Autosave system (save at day start / night end, soft restart on failure) | Saving System |
| FR17 | Book arrangement puzzles | Puzzle Integration |
| FR18 | Record cross-referencing puzzles | Puzzle Integration |
| FR19 | Pattern recognition puzzles across days | Puzzle Integration |
| FR20 | Code and combination systems | Puzzle Integration |
| FR21 | Hint system (notebook, environmental cues, PC reference) | Puzzle Integration |
| FR22 | Area unlock progression (Day 1-7 schedule) | Level Design |
| FR23 | Office PC systems (review records, monitor stability, manage inventory) | Safe Zones |
| FR24 | Dynamic safe zones (created through lighting + order, require upkeep) | Safe Zones |
| FR25 | 10 variable-tracked endings | Win/Loss Conditions |
| FR26 | Night-level soft failure (caught → night ends, consequences persist) | Failure Conditions |
| FR27 | Cumulative soft failure (no hard game-over, forward with consequences) | Failure Conditions |
| FR28 | Customer interaction economy (satisfaction → supply availability) | Economy |
| FR29 | Resource degradation (bulbs weaken, fixtures degrade, materials less effective) | Economy |
| FR30 | Inventory system (spare bulbs, tools, notebook; limited space) | Economy |
| FR31 | Tutorial integration via Night 1 (no separate tutorial) | Tutorial Integration |
| FR32 | Dynamic audio system (layers respond to stability, lighting, proximity) | Audio Systems |
| FR33 | NPC dialogue system (text-based, sparse voice lines) | Voice/Dialogue |
| FR34 | Exterior zones (Street Front, Alley, Forest Edge, Closed Rail Line) | Level Design |
| FR35 | Narrative progression (story unfolds across 7 days, fragments, discoveries) | Progression |
| FR36 | Entity interference with puzzles (alter records, move objects, mislead) | Puzzle Integration |
| FR37 | Difficulty scaling (exponential instability growth, sawtooth micro-rhythms) | Difficulty Curve |
| FR38 | Adaptive difficulty (slowdown after failures, increased hints, stability boosts) | Difficulty Options |

**Total FRs: 38**

### Non-Functional Requirements

| ID | Requirement | GDD Section |
|---|---|---|
| NFR1 | 60 FPS preferred, 30 FPS minimum on mid-range hardware | Performance |
| NFR2 | Stable frame pacing (priority over max frame rate) | Performance |
| NFR3 | Target hardware: i5 CPU, GTX 1060 GPU, 8-16 GB RAM | Performance |
| NFR4 | Primary: 1080p optimized; 1440p supported | Resolution |
| NFR5 | Load times: seamless in core bookstore, under 5s for secondary areas | Load Times |
| NFR6 | Save/load reliability: no corruption, no progression blockers | Technical KPIs |
| NFR7 | No progression-blocking bugs, minimal soft-locks | Technical KPIs |
| NFR8 | Subtitles required | Accessibility |
| NFR9 | Adjustable text size | Accessibility |
| NFR10 | Brightness and gamma controls | Accessibility |
| NFR11 | Optional reduced flicker lighting mode | Accessibility |
| NFR12 | Audio subtitles for key sound cues | Accessibility |
| NFR13 | Rebindable keys | Accessibility |
| NFR14 | Controller support (later milestone, not MVP) | Input |
| NFR15 | Baked lighting with limited dynamic lights | Technical Constraints |
| NFR16 | Flag/state-based save system | Technical Constraints |
| NFR17 | English-only at launch | Localization |
| NFR18 | Steam platform integration | Platform |
| NFR19 | Unity Engine, C# | Technical |
| NFR20 | Session length: 30-60 min per cycle | Gameplay |

**Total NFRs: 20**

### Additional Requirements & Constraints

| ID | Requirement |
|---|---|
| AR1 | Solo developer — scope prioritizes completion over expansion |
| AR2 | Hybrid asset model — buy generic, build core identity assets |
| AR3 | Custom-built: bookstore layout, puzzle props, books, notebook UI, entity, narrative objects |
| AR4 | 5 audio categories: lighting, bookstore, spatial movement, environmental failure, entity |
| AR5 | Demo after Epic 6; 15-30 min; validates core loop, drives wishlists |
| AR6 | 2,000-5,000 Steam wishlists before launch |
| AR7 | No multiplayer, level editor, mod support, procedural gen, VR, console at launch |
| AR8 | Narrative design document completed |

### GDD Completeness Assessment

GDD is thorough and well-structured. No gaps that would block implementation readiness.

---

## Step 3: Epic Coverage Validation

**stepsCompleted:** [step-01-document-discovery, step-02-gdd-analysis, step-03-epic-coverage-validation]

### Coverage Matrix

| FR | Requirement | Epic Coverage | Status |
|---|---|---|---|
| FR1 | Observation system | Epic 2, Story 2.1 | ✓ Covered |
| FR2 | Inspection system | Epic 2, Stories 2.2 & 2.3 | ✓ Covered |
| FR3 | Persistent hybrid notebook | Epic 2 Story 2.4, Epic 3 Stories 3.1-3.5 | ✓ Covered |
| FR4 | Environmental organization | Epic 2, Story 2.5 | ✓ Covered |
| FR5 | Lighting maintenance | Epic 2, Story 2.6 | ✓ Covered |
| FR6 | Navigation system | Epic 1 Story 1.1, Epic 8 Story 8.5 | ✓ Covered |
| FR7 | Entity avoidance | Epic 6, Story 6.3 | ✓ Covered |
| FR8 | Customer service interactions | Epic 11, Stories 11.1 & 11.4 | ✓ Covered |
| FR9 | Day/Night cycle | Epic 4, Stories 4.1-4.5 | ✓ Covered |
| FR10 | Environmental persistence | Epic 5, Stories 5.1-5.5 | ✓ Covered |
| FR11 | Entity behavior system | Epic 6, Stories 6.1-6.4 | ✓ Covered |
| FR12 | Entity telegraphing | Epic 6 Story 6.1, Epic 9 Story 9.2 | ✓ Covered |
| FR13 | Light economy | Epic 2 Story 2.6, Epic 5 Story 5.3 | ✓ Covered |
| FR14 | Knowledge loss/recovery | Epic 5 Story 5.4, Epic 6 Story 6.5 | ✓ Covered |
| FR15 | Environmental entropy | Epic 5, Story 5.2 | ✓ Covered |
| FR16 | Autosave system | Epic 1 Story 1.7, Epic 14 Stories 14.1-14.4 | ✓ Covered |
| FR17 | Book arrangement puzzles | Epic 7, Story 7.1 | ✓ Covered |
| FR18 | Record cross-referencing puzzles | Epic 7, Story 7.3 | ✓ Covered |
| FR19 | Pattern recognition puzzles | Epic 3 Story 3.4, Epic 7 Stories 7.1-7.3 | ✓ Covered |
| FR20 | Code/combination systems | Epic 7, Story 7.2 | ✓ Covered |
| FR21 | Hint system | Epic 7, Story 7.4 | ✓ Covered |
| FR22 | Area unlock progression | Epic 8, Stories 8.1-8.3 | ✓ Covered |
| FR23 | Office PC systems | Epic 3, Story 3.6 *(NEW)* | ✓ Covered |
| FR24 | Dynamic safe zones | Epic 5, Story 5.7 *(NEW)* | ✓ Covered |
| FR25 | 10 variable-tracked endings | Epic 10, Stories 10.3-10.5 | ✓ Covered |
| FR26 | Night-level soft failure | Epic 6, Story 6.5 | ✓ Covered |
| FR27 | Cumulative soft failure | Epic 5 Stories 5.1-5.4, Epic 6 Story 6.5 | ✓ Covered |
| FR28 | Customer interaction economy | Epic 11, Story 11.5 *(NEW)* | ✓ Covered |
| FR29 | Resource degradation | Epic 5, Story 5.6 *(NEW)* | ✓ Covered |
| FR30 | Inventory system | Epic 2, Story 2.7 *(NEW)* | ✓ Covered |
| FR31 | Tutorial via Night 1 | Epic 6, Story 6.6 | ✓ Covered |
| FR32 | Dynamic audio system | Epic 9, Stories 9.1-9.4 | ✓ Covered |
| FR33 | NPC dialogue system | Epic 11, Story 11.2 | ✓ Covered |
| FR34 | Exterior zones | Epic 8, Story 8.6 *(NEW)* | ✓ Covered |
| FR35 | Narrative progression | Epic 10, Stories 10.1-10.2 | ✓ Covered |
| FR36 | Entity interference with puzzles | Epic 6 Story 6.4 (general interference) | ⚠ Partial |
| FR37 | Difficulty scaling | Epic 5 Stories 5.1-5.2 (implicit), Epic 9 Story 9.3 | ⚠ Partial |
| FR38 | Adaptive difficulty | Epic 9, Story 9.6 *(NEW)* | ✓ Covered |

### Missing Requirements

**No FRs are missing from epics.** All 7 previously missing FRs (FR23, FR24, FR28, FR29, FR30, FR34, FR38) are now covered by new stories.

### Partially Covered FRs

**FR36: Entity interference with puzzles** — Epic 6 Story 6.4 covers general entity environmental interference, but the GDD specifies entity-specific *puzzle* interference (altering recorded information used by puzzles, moving key puzzle objects, creating misleading patterns). This is a subset behavior that would naturally emerge during Epic 7 implementation but has no dedicated story.
- **Recommendation:** Add a story to Epic 7 covering entity-puzzle interaction specifically. Low risk — the mechanic foundation exists in Epic 6.

**FR37: Difficulty scaling curve** — The exponential instability growth and sawtooth micro-rhythms are supported by persistence (Epic 5) and atmosphere (Epic 9) systems, but no story explicitly implements the tuning of the difficulty curve across the 7-day arc.
- **Recommendation:** Add a tuning/balancing story to Epic 15 (Polish). Low risk — this is configuration of existing systems, not new architecture.

### Coverage Statistics

- **Total GDD FRs:** 38
- **FRs fully covered:** 36 (95%)
- **FRs partially covered:** 2 (5%)
- **FRs missing:** 0 (0%)
- **Coverage percentage:** 95% full, 100% with partials

---

## Step 4: UX Alignment Assessment

**stepsCompleted:** [step-01-document-discovery, step-02-gdd-analysis, step-03-epic-coverage-validation, step-04-ux-alignment]

### UX Document Status

**Not Found.** No dedicated UX design document exists.

### UX Implied in GDD

The GDD implies significant UI/UX requirements:
- Notebook UI (Tab to open, auto-log + manual, cross-referencing, categories, adjustable text size)
- HUD (crosshair, interaction prompts, context-sensitive labels)
- Office PC interface (record review, stability monitoring, inventory, preparation)
- Accessibility settings (text size, brightness, gamma, reduced flicker, audio subtitles, rebindable keys)
- Pause menu (ESC-triggered)

### Architecture Support for UI

- ✓ `NotebookManager` with entries, manual notes, cross-referencing fully architected
- ✓ `NotebookUI.cs` in file structure
- ✓ Event-driven architecture supports UI updates
- ✓ Story 3.6 (NEW) now covers Office PC interface
- ✗ No HUD/interaction prompt system in architecture
- ✗ No accessibility settings architecture
- ✗ No pause menu system defined

### Alignment Improvement Since Last Assessment

The addition of Story 3.6 (Office PC) addresses the most significant UI gap. The PC interface now has a story with acceptance criteria. HUD, accessibility, and pause menu are covered by Epic 12 (UI & UX Completion) stories but lack architecture-level definitions.

### Warnings

⚠ **UX document recommended but not blocking.** The GDD and epics now provide sufficient coverage for all major UI systems. However, wireframes for the notebook and PC interface would reduce implementation ambiguity.

⚠ **Architecture gaps for HUD, accessibility, and pause systems.** These are addressed in Epic 12 stories but have no architecture-level component definitions. Manageable — these are standard Unity UI systems that don't require novel architecture.

---

## Step 5: Epic Quality Review

**stepsCompleted:** [step-01-document-discovery, step-02-gdd-analysis, step-03-epic-coverage-validation, step-04-ux-alignment, step-05-epic-quality-review]

### Epic Player/User Value Assessment

| Epic | Title | Player Value? | Assessment |
|---|---|---|---|
| 1 | Core Technical Foundation | ⚠ Borderline | Technical title but player-facing deliverable and player-framed stories. Acceptable for greenfield foundation. |
| 2 | Observation & Interaction Systems | ✓ Yes | Core player verbs — observe, inspect, organize, maintain, carry. |
| 3 | Notebook & Knowledge System | ✓ Yes | Player records, reviews, cross-references, uses PC. |
| 4 | Day/Night Cycle Framework | ✓ Yes | Player experiences atmospheric transitions and phase structure. |
| 5 | Environmental Persistence System | ✓ Yes | Player sees world react and persist, manages degradation and safe zones. |
| 6 | First Horror Loop (Vertical Slice) | ✓ Yes | Player experiences full core loop with entity encounter. |
| 7 | Puzzle System Framework | ✓ Yes | Player solves integrated puzzles. |
| 8 | Area Expansion System | ✓ Yes | Player unlocks and explores interior and exterior areas. |
| 9 | Atmosphere & Tension Systems | ✓ Yes | Player feels escalating tension and adaptive feedback. |
| 10 | Narrative & Ending System | ✓ Yes | Player discovers story and reaches variable endings. |
| 11 | NPC & Customer System | ✓ Yes | Player interacts with customers, service affects supply flow. |
| 12 | UI & UX Completion | ✓ Yes | Player experiences clear, readable, accessible interface. |
| 13 | Audio & Visual Finalization | ⚠ Borderline | Developer-framed stories but player-facing deliverable. |
| 15 | Polish & Performance Optimization | ⚠ Borderline | Developer-framed stories; performance is player-valuable. |
| 16 | Steam Integration & Release Prep | ⚠ Borderline | Developer-framed; distribution is the player outcome. |
| 14 | Save System & Stability Pass | ⚠ Borderline | Mix of player/developer stories; save reliability is directly player-valuable. |

### Epic Independence Validation

- ✓ No circular dependencies
- ✓ No forward references (no epic requires a later epic to function)
- ✓ Phase 1 → 2 → 3 → 4 → 5 chain is logical and sequential
- ✓ Phase 3 epics (7, 8, 9, 11) are correctly parallel after Epic 6
- ✓ Phase 5 epics (12 → 14 → 15 → 16) are correctly sequential

**Independence validated. No violations.**

### Story Quality Assessment

#### Acceptance Criteria — RESOLVED

All 90 stories now have formal acceptance criteria in Given/When/Then format. Each story has 3-4 testable criteria covering:
- Happy path behavior
- Edge cases and boundary conditions
- Failure or empty-state handling

**This was the #1 critical issue from the previous assessment. It is now resolved.**

#### Story Sizing

All stories are appropriately scoped — single capabilities, not epic-sized. New stories (2.7, 3.6, 5.6, 5.7, 8.6, 9.6, 11.5) are consistent with existing story sizing.

#### Data Creation Timing

No violations. Systems and data structures are introduced when first needed.

### Remaining Quality Findings

#### 🟠 Major Issues (1)

1. **Epics 13, 15, 16 use developer-framed stories.** These epics deliver player-valuable outcomes (atmosphere, performance, distribution) but all stories are written as "As a developer, I can..." This is a framing issue, not a structural one. The acceptance criteria are testable and the deliverables are clear.
   - **Recommendation:** Reframe stories to player outcomes where natural (e.g., "As a player, the game runs at 60 FPS" instead of "As a developer, I can validate 60 FPS"). For genuinely technical stories (profiling, build uploads), developer framing is acceptable.

#### 🟡 Minor Concerns (2)

2. **Missing project scaffolding story in Epic 1.** No explicit "Set up Unity project from architecture spec" story. The first story assumes the project exists.
   - **Recommendation:** Add Story 1.0 — project setup (Unity project, folder structure, initial scene).

3. **Epic 1 title "Core Technical Foundation" is technical.** Deliverable is player-facing; could be reframed as "Bookstore Exploration Foundation."
   - **Recommendation:** Optional cosmetic improvement. Not blocking.

#### Previously Critical — Now Resolved

- ~~83 stories lack acceptance criteria~~ → **All 90 stories have AC** ✓
- ~~7 FRs missing from epics~~ → **All 7 FRs now covered** ✓
- ~~Epics 15 & 16 are pure technical milestones~~ → **Downgraded to framing issue** (AC now provides testable player-relevant outcomes even with developer framing)

---

## Summary and Recommendations

### Overall Readiness Status

**READY** — with minor recommendations

The project is ready to proceed to sprint planning and implementation. All critical blockers from the previous assessment have been resolved. Remaining issues are minor framing concerns and two low-risk partial FR coverage items.

### Assessment Comparison (Run 1 → Run 2)

| Metric | Run 1 | Run 2 | Change |
|---|---|---|---|
| FR coverage (full) | 79% (30/38) | 95% (36/38) | +16% |
| FR coverage (with partials) | 84% | 100% | +16% |
| Stories with AC | 0/83 (0%) | 90/90 (100%) | Resolved |
| Missing FR stories | 7 | 0 | Resolved |
| Critical issues | 3 | 0 | Resolved |
| Major issues | 2 | 1 | -1 |
| Minor concerns | 2 | 2 | Unchanged |
| Total stories | 83 | 90 | +7 |

### Remaining Issues

#### 🟠 Major (1)

1. **Epics 13, 15, 16 developer-framed stories.** Deliverables are player-valuable but story language uses "As a developer." AC is testable regardless. Reframe where natural; not blocking.

#### 🟡 Minor (4)

2. **FR36 (entity-puzzle interference) partially covered.** Epic 6 covers general interference; a dedicated Epic 7 story for puzzle-specific interference would close the gap.
3. **FR37 (difficulty curve tuning) partially covered.** An explicit tuning story in Epic 15 would close the gap.
4. **Missing project scaffolding story.** Epic 1 needs a Story 1.0 for Unity project setup.
5. **No UX design document.** Notebook and PC wireframes would reduce ambiguity but are not blocking.

### Recommended Next Steps

1. **Proceed to Sprint Planning** (`/gds-sprint-planning`). The project is ready for implementation.
2. **Optionally:** Add Story 1.0 (project scaffolding) to Epic 1 before sprint planning.
3. **During Epic 7 development:** Add a story for entity-puzzle interference (FR36).
4. **During Epic 15 development:** Add a story for difficulty curve tuning (FR37).
5. **When approaching Epic 3:** Consider a UX pass on notebook and PC interface wireframes.

### Strengths

- **GDD:** Comprehensive, well-structured, 38 FRs + 20 NFRs clearly defined
- **Architecture:** Practical, solo-dev-appropriate, event-driven singleton design
- **Narrative Design:** Production-ready — tone guides, character identities, trigger maps, writing batches
- **Epics:** Clean dependency chain, no circular references, logical phase progression, full AC coverage
- **Vertical Slice (Epic 6):** Well-defined first playable milestone proving the core loop
- **FR Coverage:** 95% full coverage, 100% with partials — all core gameplay systems have implementation paths

### Final Note

This re-assessment confirms that the 3 critical blockers from the initial run — missing acceptance criteria, 7 uncovered FRs, and technical-only epics — have been resolved. The project is in strong shape to enter sprint planning. The remaining issues are low-risk and can be addressed incrementally during implementation.

**Assessment Date:** 2026-03-28
**Assessor:** Implementation Readiness Workflow (GDS Module)
**Assessment Run:** 2
