---
title: 'Narrative Design Document'
project: 'The Night Borrower'
date: '2026-03-28'
author: 'Dakota'
version: '1.0'
stepsCompleted: [1]
status: 'in-progress'
narrativeComplexity: 'moderate'
gdd: 'gdd.md'
---

# Narrative Design Document

## The Night Borrower

### Document Status

This narrative document is being created through the GDS Narrative Workflow.

**Narrative Complexity:** Moderate
**Steps Completed:** 1 of 11 (Initialize)

---

## Narrative Content Budget

### Design Constraints

- **Solo development** — every line of text must be written, implemented, and tested by one person
- **No branching dialogue trees** — NPCs use linear, day-gated dialogue pools
- **No voice acting** — all narrative is text-delivered (notebook entries, documents, NPC text boxes, environmental text)
- **Moderate complexity** — narrative enhances gameplay systems, doesn't replace them
- **7-day structure** — all content must fit within and be paced across 7 day/night cycles

---

### 1. Total Content Inventory

#### Lore Fragments — 35 total

Found objects: notes, letters, receipts, book marginalia, newspaper clippings, photographs, recordings. Each is 2-4 sentences. Player reads them in the world and they auto-log to the notebook.

| Category | Count | Purpose |
|---|---|---|
| **Bookstore History** | 8 | How the store was founded, previous owners, its role in Black Pines |
| **Entity Origin** | 8 | What The Night Borrower is, where it came from, what it wants |
| **Black Pines Town** | 7 | Town history, logging industry, isolation, local folklore |
| **Previous Incidents** | 6 | Past encounters with the entity, what happened to others |
| **Personal Records** | 6 | The player character's predecessor, the Elder's history, family connections |

**Writing estimate:** 35 fragments x ~75 words avg = **~2,625 words**

#### NPC Dialogue — 6 NPCs, 90 lines total

No dialogue trees. Each NPC has a **daily dialogue pool**: 1-4 lines available per day they appear. Player talks to them, gets the next unread line. Simple queue, not branching.

**Core NPCs (required for progression):**

| NPC | Role | Days Present | Lines | Narrative Function |
|---|---|---|---|---|
| **The Elder** | Bookstore founder/retired owner. Primary lore source. | Days 1-7 | 24 | Gates R1, R5, R8. Required for Endings 1, 2. Carries the critical narrative spine. |
| **Regular Customer** | Daily visitor. Grounds the routine. Notices changes. | Days 1-7 | 20 | Reinforces routine pillar. Emotional anchor. Reacts to environmental changes the player may have missed. |

**Elder daily breakdown (24 lines):**

| Day | Lines | Content Focus |
|---|---|---|
| 1 | 4 | Welcome, store orientation, hints at long history (gates R1) |
| 2 | 3 | Routine advice, mentions past employees casually |
| 3 | 3 | Reacts to Night 2 events, first hint of concern |
| 4 | 4 | Reveals personal connection to the store's past (gates R5 if elderConversations >= 3) |
| 5 | 3 | Discusses containment, references "the old ways" |
| 6 | 3 | Reveals knowledge of player's hiring (supports R7) |
| 7 | 4 | Final truth, supports R8, emotional conclusion |

**Regular Customer daily breakdown (20 lines):**

| Day | Lines | Content Focus |
|---|---|---|
| 1 | 3 | Friendly introduction, establishes routine ("same order as always") |
| 2 | 3 | Comments on a book they're reading, mentions something felt "off" last night |
| 3 | 3 | Asks if anything changed in the store, reacts to displacement |
| 4 | 3 | Growing unease, mentions other townsfolk acting strange |
| 5 | 3 | Shorter visit, notices lighting issues, asks if player is okay |
| 6 | 3 | Concerned, considers not coming back |
| 7 | 2 | Final visit — either supportive or frightened depending on store stability |

**Optional NPCs (expand lore, never gate gameplay):**

| NPC | Role | Days Present | Lines | Priority |
|---|---|---|---|---|
| **Mail Carrier** | Delivers packages. Brings outside-world context. | Days 1, 3, 5, 7 | 12 | High — easiest to implement, 4 appearances, 3 lines each |
| **Librarian** | Neighboring town library. Cross-references records. | Days 2, 4, 6 | 12 | Medium — enriches archive/cross-reference themes |
| **Local Historian** | Researching Black Pines. Provides town context. | Days 3, 5, 7 | 12 | Medium — deepens Black Pines setting |
| **Repair Technician** | Fixes electrical. Comments on building oddities. | Days 2, 4 | 10 | Low — building flavor, reinforces lighting system |

**Line format:** Each line is 1-3 sentences of spoken text + optional notebook auto-log trigger.

**Writing estimate:** 90 lines x ~40 words avg = **~3,600 words**

**Why only 2 required NPCs:** The Elder carries the critical narrative path. The Regular Customer is the emotional barometer. All 4 optional NPCs enrich the world but are never checked by ending variables or puzzle prerequisites. Build and test the 2 core NPCs first, layer in optionals during polish.

#### Key Narrative Reveals — 8 total

Major story beats that shift the player's understanding. Each is unlocked by a specific trigger (puzzle solved, lore threshold reached, Elder conversation, etc.). These map directly to the `criticalReveals` ending variable.

| # | Reveal | Trigger | Day Available |
|---|---|---|---|
| R1 | The bookstore has had unexplained inventory losses for decades | Elder conversation (Day 1-2) | 1 |
| R2 | A previous employee disappeared under similar circumstances | Lore fragment discovery (5+ found) | 2 |
| R3 | The entity feeds on written records, not people directly | Surviving Night 2 + notebook corruption observed | 3 |
| R4 | The store was built on a site with indigenous warnings about "memory eaters" | Archive investigation puzzle (P06) | 4 |
| R5 | The Elder knows more than they've admitted — they survived a previous cycle | Elder conversation (Day 4-5, requires elderConversations >= 3) | 4 |
| R6 | The entity can be contained but not destroyed — previous owners managed it through ritual maintenance | Lore fragment discovery (20+ found) | 5 |
| R7 | The player's hiring was not accidental — the Elder chose them specifically | Apartment safe puzzle (P10) | 6 |
| R8 | The full history: the entity is bound to the written word itself, the store is both its prison and food source | Final record assembly puzzle (P12) + Elder conversation | 7 |

**Writing estimate:** 8 reveals x ~150 words (reveal text + notebook entry) = **~1,200 words**

#### Environmental Storytelling Moments — 15 total (hard cap)

Wordless or near-wordless narrative beats delivered through the environment. Each moment is classified by its role in supporting gameplay systems vs. providing atmosphere.

**[SYSTEM-CRITICAL] — 6 moments. These teach or reinforce core mechanics. Never cut.**

| # | Moment | Zone | Day Introduced | System Supported |
|---|---|---|---|---|
| E01 | Books displaced overnight to wrong shelves — player notices during Day Phase | Main Floor | Day 2 | Teaches observation of displacement. Directly supports Book Arrangement puzzles. |
| E02 | A notebook entry subtly changes after Night Phase — different wording, altered detail | Office | Day 3 | Teaches record corruption. Player learns to distrust corrupted entries. |
| E03 | Light fixture flickers in a pattern that matches entity proximity | Main Floor | Day 2 | Teaches lighting-as-warning. Reinforces Light Creates Safety pillar. |
| E04 | A shelf section the player organized reverts to disorder after night | Main Floor | Day 3 | Teaches entropy system. Player learns maintenance is ongoing, not one-time. |
| E05 | A document in the office contradicts a notebook entry the player read earlier | Office | Day 4 | Encourages cross-referencing and manual note-taking. Supports notebook system. |
| E06 | A zone's lighting degrades visibly across days — functional on Day 1, dim by Day 3, failed by Day 5 if not repaired | Any zone | Progressive | Teaches degradation as a system, not random events. Motivates preventive maintenance. |

**[CORE NARRATIVE] — 5 moments. These deliver story without words. Cut only as last resort.**

| # | Moment | Zone | Day Introduced | Narrative Function |
|---|---|---|---|---|
| E07 | A photograph on the office wall shows the store staff — one face is scratched out more each day | Office | Day 1 (changes daily) | Establishes memory erasure theme. Foreshadows R2 (disappeared employee). |
| E08 | A locked cabinet in the basement contains photographs of previous employees, each with a resignation letter dated the same day of the year | Basement | Day 4 (unlock) | Supports R2, R5. Establishes cyclical pattern. |
| E09 | Scratch marks on the office doorframe — tally marks, accumulated over decades, in different handwriting | Office | Day 1 (static) | Establishes that others survived this before. Supports R6 (containment). |
| E10 | The apartment upstairs contains personal belongings left mid-task — a half-packed suitcase, an unfinished letter | Apartment | Day 6 (unlock) | Supports R7 (player was chosen). Previous person left in a hurry. |
| E11 | A handprint on the basement window glass — elongated fingers, proportions not quite human | Basement | Day 4 | Entity evidence. Only direct physical trace the player finds. |

**[OPTIONAL ATMOSPHERIC] — 4 moments. World-building flavor. First candidates for removal if scope tightens.**

| # | Moment | Zone | Day Introduced | Atmosphere Function |
|---|---|---|---|---|
| E12 | A mug of coffee on the office desk — still warm on Day 1, untouched and cold by Day 7 | Office | Day 1 (changes daily) | Passage of time. Small comfort detail that degrades. |
| E13 | A clock in the main floor that runs backwards during Night Phase | Main Floor | Night 1 | Surreal unease. Time distortion flavor. |
| E14 | Rain sounds outside intensify across the week — drizzle Day 1, storm Day 7 | Exterior (audio) | Progressive | Pathetic fallacy. Escalating weather mirrors escalating threat. |
| E15 | A "HELP WANTED" sign in the window — the player's own job listing, still posted | Street Front | Day 1 (static) | Irony. Subtle unease. Implies replaceability. |

**Writing estimate:** ~300 words total (inscriptions, letter fragments, short notes in E08, E10)

**If further cuts are needed:** Remove E15 first (pure flavor), then E14 (audio-only, no level design), then E13 (surreal tone may not fit grounded horror), then E12 (nice detail but not load-bearing). Never remove E01-E11.

#### Ending Narratives — 10 endings

Each ending is a short text sequence (no cutscenes). Displayed as a series of notebook-style pages summarizing what happened, what was preserved, and what was lost.

| Ending | Tone | Length |
|---|---|---|
| 1. Full Preservation | Best | ~200 words |
| 2. Understanding | Good | ~200 words |
| 3. Survival Through Order | Good | ~175 words |
| 4. The Archivist | Bittersweet | ~200 words |
| 5. Imperfect Memory | Neutral (default) | ~175 words |
| 6. Pattern Breaker | Neutral | ~150 words |
| 7. The Borrower Remains | Dark | ~175 words |
| 8. Lost Records | Dark | ~175 words |
| 9. Collapse | Worst | ~150 words |
| 10. Forgotten | Worst | ~125 words |

**Writing estimate:** 10 endings x ~175 words avg = **~1,750 words**

---

### 2. Narrative Distribution Across the 7-Day Cycle

#### Content Pacing Table

| Day | Lore | Core NPC Lines | Optional NPC Lines | Reveals | Puzzles | Env Moments Active | Tone |
|---|---|---|---|---|---|---|---|
| **1** | 4 | 7 (Elder 4, Regular 3) | 3 (Mail 3) | R1 | P01, P02 | E03, E06, E07, E09, E12, E13, E15 | Comfort, curiosity |
| **2** | 5 (9) | 6 (Elder 3, Regular 3) | 7 (Repair 5, Lib 0, Hist 0) | R2 | P03 | E01, E03, E06, E07 | Routine, first unease |
| **3** | 5 (14) | 6 (Elder 3, Regular 3) | 6 (Mail 3, Hist 3) | R3 | P04, P05 | E02, E04, E06, E07 | Disruption confirmed |
| **4** | 6 (20) | 7 (Elder 4, Regular 3) | 9 (Repair 5, Lib 4) | R4, R5 | P06, P07 | E05, E06, E07, E08, E11 | Deeper truth |
| **5** | 5 (25) | 6 (Elder 3, Regular 3) | 6 (Mail 3, Hist 3) | R6 | P08, P09 | E06, E07 | Understanding |
| **6** | 5 (30) | 6 (Elder 3, Regular 3) | 7 (Lib 4, Hist 3) | R7 | P10, P11 | E06, E07, E10 | Personal stakes |
| **7** | 5 (35) | 6 (Elder 4, Regular 2) | 6 (Mail 3, Hist 3) | R8 | P12 | E06, E07 | Resolution |

#### Pacing Principles

**Front-load comfort, back-load truth.** Days 1-2 establish normalcy and build the player's emotional investment in the routine. Days 3-4 introduce cracks. Days 5-7 deliver reveals at increasing pace.

**No day has zero narrative.** Even the lightest day (Day 1) has 4 findable lore fragments, 12 NPC lines, and 1 possible reveal. The player always has something to discover.

**Reveals accelerate.** Days 1-3 have 1 reveal each. Day 4 has 2. Days 5-7 have 1 each but they're the heaviest (R6, R7, R8 are the "everything clicks" beats).

**NPC availability creates natural rhythm.** The Elder and Regular Customer are daily anchors. Other NPCs rotate in, making each day feel slightly different without requiring unique schedules for every character.

---

### 3. Maximum Recommended Limits

These are hard ceilings. If you hit a limit, cut or merge — don't expand.

| Element | Budget | Hard Ceiling | Rationale |
|---|---|---|---|
| Lore fragments | 35 | 40 | Ending 4 threshold adjusted to `>= 28` to match (see systems-spec.md) |
| NPC dialogue lines | 90 | 100 | ~13 lines/day. Core NPCs carry the load, optionals add texture. |
| Key reveals | 8 | 8 | Ending 1 requires `criticalReveals >= 7` — 8 is tight by design |
| Environmental moments | 15 | 15 | Hard cap. 6 system-critical, 5 core narrative, 4 optional atmospheric. |
| Ending narratives | 10 | 10 | Locked to ending count — never add more endings |
| NPCs | 6 | 6 | 2 core + 4 optional. Each needs schedule, dialogue pool, and visual. |
| Total written words | ~9,475 | 10,000 | Roughly equivalent to a 30-35 page short story |

#### Ending Threshold Adjustments (already applied to systems-spec.md)

| Ending | Old Threshold | Current Threshold |
|---|---|---|
| 4 — The Archivist | `loreFragmentsFound > 40` | `loreFragmentsFound >= 28` |
| 8 — Lost Records | `loreFragmentsFound < 15` | `loreFragmentsFound < 10` |

Relative difficulty preserved: The Archivist requires 80% of lore (28/35), Lost Records means finding less than ~29% (10/35).

---

### 4. Required vs Optional Content

#### Required for Progression (Must Ship)

| Element | Count | Words | Why Required |
|---|---|---|---|
| The Elder (NPC) | 24 lines | ~960 | Gates R1, R5, R8. Required for Endings 1, 2. |
| Regular Customer (NPC) | 20 lines | ~800 | Grounds routine pillar. Required for emotional arc. |
| Entity Origin lore | 8 fragments | ~600 | Required for player to understand the threat. |
| Previous Incidents lore | 6 fragments | ~450 | Required for R2, R6. Provides stakes. |
| All 8 reveals | 8 triggers | ~1,200 | Map to `criticalReveals` — directly drive ending selection. |
| All 10 ending narratives | 10 texts | ~1,750 | Every reachable ending needs a payoff. |
| System-critical env moments | 6 moments | ~0 | Level design only. No writing, but require placement + testing. |
| Core narrative env moments | 5 moments | ~200 | Minimal writing (inscriptions, short notes in E08, E10). |

**Required total: ~5,960 words** — 63% of total budget

#### Optional but Recommended (Ship if Time Allows)

| Element | Count | Words | Priority | Impl. Order |
|---|---|---|---|---|
| Mail Carrier (NPC) | 12 lines | ~480 | High | First optional NPC — 4 appearances, 3 lines each |
| Librarian (NPC) | 12 lines | ~480 | Medium | Second — enriches cross-reference themes |
| Local Historian (NPC) | 12 lines | ~480 | Medium | Third — deepens Black Pines setting |
| Repair Technician (NPC) | 10 lines | ~400 | Low | Last NPC — building flavor |
| Bookstore History lore | 8 fragments | ~600 | Medium | World depth. Not required for any ending. |
| Black Pines Town lore | 7 fragments | ~525 | Low | Setting color. Not required for any ending. |
| Personal Records lore | 6 fragments | ~450 | Medium | Emotional depth. Supports R7. |
| Optional atmospheric env | 4 moments | ~100 | Low | Add last. Cut first if scope tightens. |

**Optional total: ~3,515 words** — 37% of total budget

#### Implementation Order

```
Phase 1 — Vertical Slice (Epic 6):
  ✓ Elder Day 1 lines (4 lines)
  ✓ Regular Customer Day 1 lines (3 lines)
  ✓ Day 1 lore fragments (4 fragments)
  ✓ R1 reveal trigger
  ✓ P01, P02 narrative rewards
  ✓ Env moments present on Day 1: E03, E06, E07, E09
  Writing needed: ~700 words

Phase 2 — Core Loop (Epics 7-9):
  ✓ Elder full dialogue (remaining 20 lines)
  ✓ Regular Customer full dialogue (remaining 17 lines)
  ✓ All Entity Origin + Previous Incidents lore (14 fragments)
  ✓ Reveals R2-R7
  ✓ All puzzle narrative rewards
  ✓ System-critical env moments: E01, E02, E04, E05
  Writing needed: ~3,100 words

Phase 3 — Narrative Integration (Epic 10):
  ✓ All 10 ending narratives
  ✓ R8 final reveal
  ✓ Remaining required lore
  ✓ Core narrative env moments: E08, E10, E11
  Writing needed: ~2,160 words

Phase 4 — Polish (Epics 12-16):
  ✓ Optional NPCs: Mail Carrier → Librarian → Historian → Technician
  ✓ Optional lore: Personal Records → Bookstore History → Black Pines Town
  ✓ Optional atmospheric env: E12 → E13 → E14 → E15
  Writing needed: ~3,515 words (add as time allows)
```

---

### Content Budget Summary

| Metric | Previous | Adjusted | Change |
|---|---|---|---|
| **NPC dialogue lines** | 120 | 90 | -30 lines (-25%) |
| **Environmental moments** | 20 | 15 | -5 moments (-25%) |
| **Lore fragments** | 35 | 35 | No change |
| **Key reveals** | 8 | 8 | No change |
| **Endings** | 10 | 10 | No change |
| **NPCs** | 6 | 6 | No change (2 core + 4 optional) |
| **Total written words** | ~10,875 | ~9,475 | -1,400 words (-13%) |
| **Required words** | ~6,500 (60%) | ~5,960 (63%) | Tighter required core |
| **Optional words** | ~4,375 (40%) | ~3,515 (37%) | More cuttable buffer |
| **Required NPCs** | 2 of 6 | 2 of 6 | No change |
| **Required lore** | 14 of 35 | 14 of 35 | No change |
| **Writing pace** | ~1,500/epic | ~1,200/epic | Easier sustained pace |

**Vertical slice (Epic 6) requires ~700 words.** That's 4 Elder lines, 3 Regular Customer lines, 4 lore fragments, 1 reveal trigger, 2 puzzle narrative rewards, and 4 environmental moments placed in the scene. Achievable in a single focused writing session.

**Total budget (~9,475 words) is roughly a 30-page short story** spread across 5 development epics. At ~1,200 words per epic, this is approximately one evening of writing per epic — sustainable for solo development alongside implementation work.

---

## Narrative Trigger Map

Every narrative element that fires at runtime needs a defined trigger condition, a system dependency, and a safety net for missed content. This map ensures no required narrative depends on rare player behavior and that the implementation is deterministic.

---

### SYSTEM-CRITICAL Trigger Table

These moments teach core mechanics. Every player must encounter them. Triggers are designed to be **unavoidable through normal play**.

| ID | Moment | Trigger Condition | Timing | System Dependency | Failure Safety | Replay Behavior |
|---|---|---|---|---|---|---|
| **E01** | Books displaced overnight | `OnDayStarted(day >= 2)` — entropy system displaces objects during Day→Unease transition | Day 2+, Day Phase, Main Floor | Environment (natural entropy) | **Cannot miss.** Entropy runs automatically every day. If player doesn't notice, displaced books remain visible until interacted with. | Recurs every day. New displacements each morning. |
| **E02** | Notebook entry corrupted | `OnEntryCorrupted(entryId)` — first corruption event fires after Night 2 if entity entered HUNTING state | Day 3+, Recovery Phase, Office (notebook) | Notebook + Entity | **Guaranteed by Day 3.** If entity didn't hunt on Night 2 (very unlikely at threshold 0.7), corruption occurs on first night where entity hunts. If player somehow avoids all hunting through Night 4, force-corrupt 1 entry at Night 4 Recovery as a failsafe. | First corruption is flagged specially in UI. Subsequent corruptions are normal system behavior. |
| **E03** | Light flickers matching entity proximity | `OnEntityEnteredZone(zoneId)` where zone has fixtures with `degradationLevel > 0.3` | Night Phase, any zone with degraded lights | Entity + Environment | **Cannot miss during Night Phase.** Any night where entity moves through a zone with degraded lights triggers flicker. If all lights are perfect (player over-maintained), entity presence still causes subtle flicker on nearest fixture. | Every night. Intensity scales with entity state (subtle in OBSERVING, rapid in HUNTING). |
| **E04** | Organized shelf reverts to disorder | `OnDayStarted(day >= 3)` — entropy re-displaces objects in zones player organized the previous day | Day 3+, Day Phase, Main Floor | Environment (entropy targets HIGH orderScore zones) | **Cannot miss if player has organized anything.** Entropy prioritizes zones with high orderScore. If player never organized (unlikely — P01 requires it), this moment still occurs because entropy displaces objects regardless. | Recurs every day. The specific objects change. |
| **E05** | Document contradicts notebook entry | `OnRecordCorrupted(objectId)` where the corrupted record is a document the player previously inspected AND has a notebook entry for | Day 4+, Day Phase, Office | Environment + Notebook | **Depends on player having inspected + had an entry corrupted for the same source.** See Risk Flags below — this is the most fragile system-critical trigger. Mitigation: if no natural contradiction exists by Day 4, place a pre-authored contradicting document in the office that references a guaranteed auto-entry (e.g., the P01 puzzle completion entry). | First contradiction is flagged. Subsequent contradictions are normal corruption behavior. |
| **E06** | Zone lighting degrades across days | `OnLightFailed(fixtureId, zoneId)` accumulated across days — visible as progressive dimming | Progressive (Day 1-7), any zone | Environment (natural degradation) | **Cannot miss.** Degradation is automatic and cumulative. Even if player repairs aggressively, they'll see fixtures degrade between repairs. Degradation rate increases each day. | Continuous. This is the lighting system itself, not a discrete event. |

### SYSTEM-CRITICAL Reveal Triggers

| ID | Reveal | Trigger Condition | Timing | System Dependency | Failure Safety | Replay Behavior |
|---|---|---|---|---|---|---|
| **R1** | Unexplained inventory losses for decades | `elderConversations >= 1` — Elder's first conversation on Day 1 contains this information | Day 1, Day Phase, Main Floor (Elder location) | NPC (Elder dialogue queue) | **Cannot miss unless player never talks to Elder on Day 1.** Failsafe: if `elderConversations == 0` at start of Day 2, Elder approaches player with opening line ("Before you get started today..."). Player must press E to dismiss, which counts as the conversation. | One-time. Logged to notebook. |
| **R3** | Entity feeds on records, not people | `nightsSurvived >= 2 AND corruptedEntries >= 1` — auto-triggers during Day 3 Recovery Phase as a notebook auto-entry titled "What It Wants" | Day 3+, Recovery Phase | Notebook + Entity (corruption must have occurred) | **Depends on E02 having fired.** E02's failsafe guarantees corruption by Night 4 at latest, so R3 fires by Day 4 Recovery at latest. If corruption somehow never occurs (impossible given failsafe), R3 fires anyway at Day 5 start as a forced insight. | One-time. Permanent notebook entry. |
| **R4** | Indigenous warnings about "memory eaters" | `OnPuzzleSolved("P06")` — Archive Investigation puzzle completion | Day 4+, Day Phase, Basement | Puzzle (P06 is required) | **Cannot miss.** P06 is a required puzzle. Player must solve it to progress. Reveal text is the puzzle's narrative reward. | One-time. Logged on puzzle completion. |
| **R5** | Elder survived a previous cycle | `elderConversations >= 3 AND currentDay >= 4` — Elder's Day 4 dialogue contains this if threshold met | Day 4-5, Day Phase | NPC (Elder conversation count) | **Requires 3 prior Elder conversations.** At 4 lines/day on Days 1-3, player has 10 opportunities before Day 4. Talking to Elder once on 3 of 4 days is sufficient. If `elderConversations < 3` by Day 5, Elder initiates: "I need to tell you something." This forces the conversation. | One-time. Logged to notebook. |
| **R6** | Entity can be contained through ritual maintenance | `loreFragmentsFound >= 20 AND currentDay >= 5` — auto-triggers as notebook insight | Day 5+, Day Phase | Notebook (lore count) | **Requires 20 of 35 lore fragments (57%).** With 4-6 fragments available per day and 25 cumulative by Day 5, this requires finding 80% of available fragments. See Risk Flags — this is gated behind significant exploration. Failsafe: if `loreFragmentsFound < 20` by Day 6, lower threshold to 15. | One-time. Permanent notebook entry. |
| **R7** | Player's hiring was not accidental | `OnPuzzleSolved("P10")` — Apartment Safe puzzle completion | Day 6+, Day Phase, Apartment | Puzzle (P10 is required) | **Cannot miss.** P10 is required. Reveal is the puzzle's narrative reward — the safe contains the Elder's letter explaining the hire. | One-time. Logged on puzzle completion. |
| **R8** | Full history — entity bound to written word | `OnPuzzleSolved("P12") AND elderConversations >= 5` — Final Record Assembly + Elder has shared enough context | Day 7, Day Phase, Office | Puzzle (P12) + NPC (Elder) | **P12 requires P10 OR P11** (OR-dependency). Elder conversation count of 5 requires talking to Elder on 5 of 7 days. See Risk Flags for the Elder dependency. Failsafe: if `elderConversations < 5` when P12 is solved, deliver R8 with reduced context (notebook entry says "pieces are missing, but the pattern is clear"). | One-time. The capstone reveal. |

---

### CORE NARRATIVE Trigger Table

These deliver story through the environment. They don't teach mechanics but build emotional context for the reveals.

| ID | Moment | Trigger Condition | Timing | System Dependency | Failure Safety | Replay Behavior |
|---|---|---|---|---|---|---|
| **E07** | Photograph with face scratched out | `OnDayStarted(day)` — photograph state updates each morning. `hasBeenInspected` tracks whether player has looked at it. | Day 1+ (present from start, changes daily), Office | Environment (daily state update on a single prop) | **Player may never inspect the photo.** That's acceptable — it's environmental, not required. However, if player inspects it on any day, the current scratch state is visible. No auto-log unless inspected. If inspected on Day 4+, the drastic change is immediately obvious. | Updates daily. Each inspection shows current state. Player can inspect multiple times. |
| **E08** | Cabinet with employee photos and resignation letters | `OnAreaUnlocked("Basement") AND OnObjectInspected("basement_cabinet")` — cabinet is interactable after basement unlocks on Day 4 | Day 4+, Day Phase, Basement | Environment (area unlock + object inspection) | **Player may never open the cabinet.** Mitigation: cabinet is positioned on the direct path to the archive shelf where P06 takes place. Visual highlight (slightly ajar, light visible through crack) draws attention. If inspected, auto-logs 2 lore fragments. | One-time inspection. Contents persist after first viewing. |
| **E09** | Tally marks on office doorframe | `OnObjectInspected("office_doorframe_marks")` — static prop, available from Day 1 | Day 1+, any phase, Office | None (static prop) | **Player may never notice.** Acceptable — it's ambient detail. No auto-log unless inspected. Positioned near the office door the player walks through every day, increasing odds of eventual inspection. | Always available. One-time notebook entry on first inspection. |
| **E10** | Apartment with abandoned belongings | `OnAreaUnlocked("Apartment") AND OnPlayerEnteredZone("Apartment")` — visible immediately upon entering apartment | Day 6+, Day Phase, Apartment | Environment (area unlock) | **Cannot miss if player enters apartment.** Since P10 (required puzzle) is in the apartment, player must enter this zone. The suitcase and letter are visible in the room where the safe is. | Static scene. Always visible after unlock. Letter auto-logs on inspection. |
| **E11** | Entity handprint on basement glass | `OnAreaUnlocked("Basement") AND OnObjectInspected("basement_window")` — visible after basement unlock | Day 4+, any phase, Basement | None (static prop) | **Player may never inspect the window.** Mitigation: handprint is near a light fixture the player likely needs to repair. Night Phase entity sounds draw attention to the basement window area. If inspected, auto-logs 1 lore fragment (Entity Origin category). | Always visible after Day 4. One-time notebook entry. |

### CORE NARRATIVE NPC Triggers

| ID | Element | Trigger Condition | Timing | System Dependency | Failure Safety | Replay Behavior |
|---|---|---|---|---|---|---|
| **Elder** | Daily dialogue (24 lines) | `OnPlayerInteract("elder") AND elderHasUnreadLines == true` — player presses E near Elder during Day Phase | Days 1-7, Day Phase | NPC (dialogue queue) | **Elder is present every Day Phase.** Lines queue — if player skips Day 2 Elder, Day 2 lines are still available on Day 3 (queue doesn't advance by day, only by interaction). This means no lines are permanently missed. R1 failsafe at Day 2 (see above). R5 failsafe at Day 5. | Queue-based. Each line is delivered once in order. Undelivered lines carry forward. |
| **Regular** | Daily dialogue (20 lines) | `OnPlayerInteract("regular_customer") AND regularHasUnreadLines == true` | Days 1-7, Day Phase | NPC (dialogue queue) | **Same queue behavior as Elder.** Lines carry forward. Regular Customer is never required for reveals or ending variables — missing them only costs emotional context. No failsafe needed. | Queue-based. Lines carry forward. |
| **R2** | Previous employee disappeared | `loreFragmentsFound >= 5` — auto-triggers as notebook insight entry | Day 2+ (earliest realistic), any phase | Notebook (lore count) | **5 fragments is achievable by Day 2** (4 available Day 1, 5 more Day 2). If player finds fewer, R2 simply triggers later. With 35 total and only 5 needed, this is extremely safe. | One-time. Triggers whenever threshold is first crossed. |

---

### Risk Flags

Three triggers have elevated fragility. Each has a defined mitigation.

#### FLAG 1: E05 (Document Contradiction) — MODERATE RISK

**Risk:** Requires a specific intersection: player must have (a) inspected a document, (b) have a notebook entry for it, AND (c) that same document must later be corrupted. This is a three-condition coincidence.

**Why it matters:** E05 teaches cross-referencing and manual note-taking — a system-critical mechanic.

**Mitigation:** Don't rely on organic corruption to produce this moment. Instead:
```
On Night 3 → Recovery transition:
  If player has NOT experienced E05:
    Force-corrupt the notebook entry for P01 (Shelf Restoration).
    Every player solves P01 (required, Day 1).
    Every player has the P01 completion entry.
    Place a pre-authored "original shelf manifest" document in the office
    that contradicts the now-corrupted P01 entry.
    This guarantees E05 fires by Day 4 morning.
```
**Post-mitigation risk:** LOW. P01 completion is guaranteed. The manufactured contradiction is indistinguishable from organic corruption.

#### FLAG 2: R6 (Containment Reveal) — MODERATE RISK

**Risk:** Requires `loreFragmentsFound >= 20` by Day 5. With 25 cumulative fragments available by Day 5, this demands finding 80% — a high bar for players who aren't thorough explorers.

**Why it matters:** R6 is required for Endings 1 and 2 (best endings) via the `criticalReveals` variable, but not for progression.

**Mitigation:**
```
If loreFragmentsFound < 20 at start of Day 6:
  Lower R6 threshold to 15.
  15 of 30 (Day 6 cumulative) = 50%, much more achievable.

If loreFragmentsFound < 15 at start of Day 7:
  R6 fires automatically as a forced notebook insight:
  "Something about the patterns you've seen... the maintenance,
   the routine — it's not just upkeep. It's containment."
```
**Post-mitigation risk:** LOW. R6 will always fire by Day 7 at latest. Players who explore thoroughly get it earlier (Day 5) as a reward.

#### FLAG 3: R8 (Final Reveal) Elder Dependency — LOW-MODERATE RISK

**Risk:** Requires `elderConversations >= 5` when P12 is solved. Player must talk to Elder on 5 of 7 days. Missing 3+ days of Elder conversation makes R8 incomplete.

**Why it matters:** R8 is the capstone reveal. Without full Elder context, the Final Record Assembly puzzle's narrative payoff is weaker.

**Mitigation:**
```
R8 has two quality tiers:

FULL (elderConversations >= 5):
  Complete reveal with Elder's full context woven in.
  Notebook entry: ~150 words, references Elder's testimony.

PARTIAL (elderConversations < 5, P12 solved):
  Reduced reveal — records tell the story but gaps remain.
  Notebook entry: ~100 words, notes "the Elder could have
  filled in more" — encourages replay.

Both count as criticalReveals += 1 for ending purposes.
```
**Post-mitigation risk:** LOW. R8 always fires when P12 is solved. Quality varies, but the ending variable increments regardless.

---

### Trigger Reliability Improvements

Four structural recommendations to prevent fragile triggers system-wide.

#### 1. NPC Dialogue Queue (Not Day-Locked)

**Current design risk:** If NPCs deliver lines only on their scheduled day, missed days = missed lines permanently.

**Recommendation:** NPCs use a **queue**, not a daily reset. Lines are ordered but not day-bound. If a player skips the Elder on Day 2, the Day 2 lines are still next in queue on Day 3. This means:
- No content is permanently missable through inaction
- Players who talk to NPCs every day get content at the authored pace
- Players who skip days get compressed delivery (multiple days' lines available at once)
- `elderConversations` counts total interactions, not unique days

**Implementation:** `NpcDialogueData.nextLineIndex` — increments on each interaction. Day-specific content references (`"Reacts to Night 2 events"`) should be written to work regardless of which day they're actually delivered.

#### 2. Lore Fragment Placement Density

**Current design risk:** If fragments are hidden in hard-to-find locations, thresholds become unreliable.

**Recommendation:** Lore fragments use a **visibility tier** system:
```
Tier 1 — OBVIOUS (40%, ~14 fragments):
  On desks, counters, and shelves at eye level.
  Player walks past them during normal activity.
  These alone satisfy R2 (needs 5) and approach R6 threshold.

Tier 2 — DISCOVERABLE (35%, ~12 fragments):
  In drawers, on lower shelves, behind objects.
  Found through active inspection of the environment.
  Required for R6 at the Day 5 threshold.

Tier 3 — HIDDEN (25%, ~9 fragments):
  Behind puzzles, in locked spaces, requires prerequisites.
  Rewards thorough play. Needed for Ending 4 (The Archivist).
```
This means a casual player finds 14+ fragments (sufficient for R2, R3). An engaged player finds 26+ (sufficient for R6). Only completionists find all 35.

#### 3. Reveal Cascade Protection

**Design rule:** No reveal should depend on another reveal having fired first. Each reveal has an independent trigger. This prevents cascade failures where missing R2 somehow blocks R5.

**Current status:** All 8 reveals have independent triggers:
- R1: Elder conversation count
- R2: Lore fragment count
- R3: Night survived + corruption observed
- R4: Puzzle P06 solved
- R5: Elder conversation count + day threshold
- R6: Lore fragment count
- R7: Puzzle P10 solved
- R8: Puzzle P12 solved + Elder conversation count

**No reveal requires a previous reveal.** R8 has the richest context when all 7 prior reveals have fired, but it triggers and increments `criticalReveals` regardless.

#### 4. Autolog Triggers for Environmental Moments

**Design rule:** System-critical environmental moments (E01-E06) should fire notebook auto-entries **without requiring player inspection**. The moment IS the system behavior — the notebook records that it happened.

| Moment | Auto-Entry Trigger | Entry Title |
|---|---|---|
| E01 | `OnObjectDisplaced` during Day→Unease (first time) | "Something moved overnight" |
| E02 | `OnEntryCorrupted` (first time) | "My notes don't match what I remember" |
| E03 | `OnEntityEnteredZone` where player is present (first time) | "The lights flickered — something passed by" |
| E04 | `OnObjectDisplaced` targeting a previously restored object (first time) | "I organized this shelf yesterday. It's wrong again." |
| E05 | Manufactured contradiction placed (see Flag 1 mitigation) | "This document says something different than my notes" |
| E06 | `OnLightFailed` (first time) | "Another light out. They're getting worse." |

Each auto-entry fires once (first occurrence only) and is flagged `isRead: false` so it appears in the Unread notebook view. Subsequent occurrences of the same system behavior are normal gameplay — no additional auto-entries.

---

## Narrative Echo System

### Purpose

After a major reveal fires, the game world subtly reinforces what the player learned — not by repeating the information, but by making the player **notice evidence they'd already walked past**. Echoes recontextualize existing details. They reward attention and make the world feel like it was always telling the truth.

### Design Rules

1. **Echoes never restate the reveal.** No notebook entry says "as I learned earlier..." — the echo is a separate detail that rhymes with the reveal.
2. **Echoes use existing systems.** No new mechanics. Echoes are delivered through environment state, NPC lines, lore fragments, or existing props. Zero new UI.
3. **Echoes are optional.** Missing an echo has no gameplay or ending consequence. They exist purely to deepen the experience for attentive players.
4. **1-2 echoes per reveal.** Hard cap. This is a solo dev project — 8-16 total echo moments, not 40.
5. **Echoes fire on the day AFTER the reveal.** This gives the information time to settle before reinforcement. Exception: R8 (final day, echoes are in the ending narrative itself).

### Implementation

Echoes use a lightweight flag system, not a new manager:

```csharp
// In GameEvents — 1 new event
public static event Action<int> OnRevealFired;    // (revealNumber 1-8)

// In EnvironmentManager or relevant system — check flag before placing echo
if (revealFired[1] && currentDay >= revealDay[1] + 1)
    ActivateEcho("R1_echo_a");
```

Echoes are pre-placed in the scene but **inactive by default**. They activate when their parent reveal has fired and the day threshold is met. No dynamic generation, no procedural text.

---

### Echo Map

#### R1 — "Inventory losses for decades"
*Reveal: The bookstore has had unexplained inventory losses for decades.*

| Echo | Method | Trigger Timing | Description |
|---|---|---|---|
| **R1-A** | Environmental detail | Day after R1 fires, Day Phase, Main Floor | A shelf label reads "SECTION C — 340 TITLES." Player can count the shelf — only 28 books. The gap was always visible, but now it means something. Inspecting the label auto-logs: *"Three hundred and forty. I count twenty-eight."* |
| **R1-B** | Regular Customer line | Next Regular Customer interaction after R1 | Customer says: *"Funny, I could've sworn you had a whole row of Hemingway last week. Maybe I'm imagining things."* No auto-log — just spoken. The customer noticed what the player now understands. |

#### R2 — "A previous employee disappeared"
*Reveal: A previous employee disappeared under similar circumstances.*

| Echo | Method | Trigger Timing | Description |
|---|---|---|---|
| **R2-A** | Environmental detail | Day after R2 fires, Day Phase, Office | A name tag in the desk drawer — first name only, no last name. Was always there, now the player knows whose it was. Inspecting auto-logs: *"Someone left their name tag behind. Just the first name. No one to return it to."* |
| **R2-B** | Lore fragment recontextualization | Day after R2 fires, notebook | If player has previously found any "Bookstore History" lore fragment mentioning staff, that entry gains a `relatedEntries` link to the R2 notebook entry. The "Related" indicator appears next time the player opens the notebook. No new text — just a connection the player can now draw. |

#### R3 — "It feeds on records, not people"
*Reveal: The entity feeds on written records, not people directly.*

| Echo | Method | Trigger Timing | Description |
|---|---|---|---|
| **R3-A** | Environmental detail | Day after R3 fires, Day Phase, Main Floor | A book the player has inspected before now has blank pages where text used to be. Not corrupted — *consumed*. The pages are physically empty, smooth, as if never printed. Inspecting auto-logs: *"The pages are blank. Not torn out. Just... empty. Like the words were never there."* |
| **R3-B** | Entity behavior shift (subtle) | Next Night Phase after R3 fires | During OBSERVING state, the entity pauses at bookshelves longer than before. Player may notice it isn't hunting them — it's browsing. This uses existing entity movement: patrol target lingering time increases from 10s to 20s at shelf-adjacent slots. No new code path, just a tuning change activated by a flag. |

#### R4 — "Indigenous warnings about memory eaters"
*Reveal: The store was built on a site with indigenous warnings about "memory eaters."*

| Echo | Method | Trigger Timing | Description |
|---|---|---|---|
| **R4-A** | Environmental detail | Day after R4 fires, Day Phase, Basement | A symbol carved into the building's foundation stone, previously hidden by a storage crate that entropy displaced. The symbol matches illustrations from the archive documents. Inspecting auto-logs: *"This mark is old. Older than the building. Someone tried to warn us before the foundation was poured."* |

*Single echo. R4 is already dense (puzzle reward + archive context). One environmental beat is sufficient.*

#### R5 — "The Elder survived a previous cycle"
*Reveal: The Elder knows more than they've admitted — they survived a previous cycle.*

| Echo | Method | Trigger Timing | Description |
|---|---|---|---|
| **R5-A** | Environmental detail | Day after R5 fires, Day Phase, Office | The tally marks on the doorframe (E09) — one set is in the Elder's handwriting. Player may have inspected E09 on Day 1 and thought nothing of it. Now, if they re-inspect, a new notebook entry appears: *"One of these tallies... I recognize the handwriting. It's the Elder's."* This only fires if E09 was previously inspected. If not, no echo — the player hasn't built the connection yet. |
| **R5-B** | Elder dialogue shift | Next Elder interaction after R5 | Elder's next queued line opens differently. Instead of the normal tone, it begins: *"You look at me different now. That's fair."* Then continues with the scheduled content. This is a **prefix line** — it doesn't consume a queue slot. It's prepended to whatever line is next. |

#### R6 — "Containment through ritual maintenance"
*Reveal: The entity can be contained but not destroyed — previous owners managed it through ritual maintenance.*

| Echo | Method | Trigger Timing | Description |
|---|---|---|---|
| **R6-A** | Environmental detail | Day after R6 fires, Day Phase, any zone | The next time the player organizes a displaced book (calls `RestoreObject`), the auto-entry text shifts from the standard *"Returned [book] to its shelf"* to a one-time variant: *"Returned [book] to its shelf. Not just organizing. Maintaining the boundary."* This is a single replacement of the standard text, not a permanent change. Every subsequent restore uses the normal text. |
| **R6-B** | Lore fragment recontextualization | Day after R6 fires, notebook | All previously found "Previous Incidents" lore fragments gain `relatedEntries` links to the R6 notebook entry. The stories of previous owners now read as containment logs, not just history. No new text — just new connections appearing in the Related indicator. |

#### R7 — "The player's hiring was not accidental"
*Reveal: The player's hiring was not accidental — the Elder chose them specifically.*

| Echo | Method | Trigger Timing | Description |
|---|---|---|---|
| **R7-A** | Environmental detail | Day after R7 fires (Day 7), Day Phase, Street Front | If E15 (HELP WANTED sign) is implemented: the sign in the window is now gone. Removed overnight. The Elder took it down — there was never going to be anyone else. If E15 is not implemented (cut as optional), this echo is also cut. |
| **R7-B** | Regular Customer line | Day 7 Regular Customer interaction (if R7 has fired) | Customer says: *"The old man asked about you, you know. Before you even applied. Asked if I thought you were the observant type."* This is a **conditional replacement** for one of the Regular Customer's Day 7 lines — only fires if R7 has triggered. Otherwise the standard Day 7 line plays. |

#### R8 — "Entity bound to the written word"
*Reveal: The entity is bound to the written word itself. The store is both its prison and its food source.*

| Echo | Method | Trigger Timing | Description |
|---|---|---|---|
| **R8-A** | Ending narrative integration | End of game, ending sequence | R8 is the final reveal, delivered on Day 7. Its echo lives in the ending text itself. Endings 1-4 (positive/neutral) reference R8 directly: the player's understanding of the entity shapes the concluding narrative. Endings that fire without R8 (player solved P12 but got partial R8) use vaguer language — "something bound to this place" instead of the specific truth. This is not a separate echo moment — it's a quality modifier on existing ending text. |

*Single echo embedded in endings. No environmental echo needed — Day 7 Night Phase begins shortly after R8, leaving no time for the player to wander and notice environmental details.*

---

### Echo Suppression Rule

#### The Problem

Environmental echoes assume the player re-encounters a prop *after* the reveal fires. But players explore freely. A player may have already inspected the echo prop — shelf label (R1-A), name tag (R2-A), blank book (R3-A), foundation stone (R4-A), tally marks (R5-A) — before the corresponding reveal. When that happens, the standard echo ("inspect this prop and get a new notebook entry") produces nothing, because the player has no reason to re-inspect an object they've already seen.

#### The Rule

**If the echo prop was inspected before the reveal fired, deliver the echo as a notebook auto-entry instead of an environmental re-inspection.**

The player already saw the object. They have a memory of it. The echo recontextualizes that memory through the notebook — the player character *realizes the significance* of something they saw earlier.

#### Fallback Activation Conditions

```
For each environmental echo (R1-A, R2-A, R3-A, R4-A, R5-A):

  On day after reveal fires:
    If echo prop HAS been inspected before reveal:
      → FALLBACK: Generate notebook auto-entry (recontextualization)
      → Skip prop activation (player won't re-inspect)

    If echo prop has NOT been inspected before reveal:
      → STANDARD: Activate prop for first-time inspection
      → Player discovers the detail fresh, post-reveal
```

#### Fallback Notebook Entries

Each fallback is a short auto-entry that references the object by name. It reads as the player character connecting a dot — not as a system message.

| Echo | Standard (prop not yet seen) | Fallback (prop already seen) |
|---|---|---|
| **R1-A** | Player inspects shelf label, reads count discrepancy. Logs: *"Three hundred and forty. I count twenty-eight."* | Auto-entry on next Day Phase start: *"That shelf label in Section C — three hundred and forty titles. I counted. There were twenty-eight. Where did three hundred books go?"* |
| **R2-A** | Player inspects name tag in drawer. Logs: *"Someone left their name tag behind. Just the first name. No one to return it to."* | Auto-entry on next Day Phase start: *"That name tag in the desk. I didn't think anything of it before. Now I know whose it was."* |
| **R3-A** | Player inspects blank book. Logs: *"The pages are blank. Not torn out. Just... empty. Like the words were never there."* | Auto-entry on next Day Phase start: *"I keep thinking about that book with blank pages. Not damaged. Not torn. The words just... aren't there anymore. It ate them."* |
| **R4-A** | Player inspects foundation symbol. Logs: *"This mark is old. Older than the building. Someone tried to warn us before the foundation was poured."* | Auto-entry on next Day Phase start: *"That carving in the basement foundation. I thought it was just old graffiti. It matches the warnings in the archive. They knew what this place was."* |
| **R5-A** | Player re-inspects tally marks, recognizes Elder's handwriting. Logs: *"One of these tallies... I recognize the handwriting. It's the Elder's."* | Auto-entry on next Day Phase start: *"The tally marks on the doorframe. One set looked familiar but I couldn't place it. Now I know — it's the Elder's handwriting. They were counting their own nights."* |

#### Echoes That Don't Need Fallbacks

Not every echo has this problem. These are immune:

| Echo | Why No Fallback Needed |
|---|---|
| **R1-B** (customer line) | NPC dialogue — delivered through conversation, not prop inspection. Always available. |
| **R2-B** (notebook link) | Adds `relatedEntries` links. Works regardless of inspection state. |
| **R3-B** (entity linger) | Entity behavior change. Player observes it during Night Phase, not through inspection. |
| **R5-B** (Elder prefix) | NPC dialogue. Always delivered on next interaction. |
| **R6-A** (restore text) | Triggered by player action (organizing a book), not inspection. Works regardless. |
| **R6-B** (notebook link) | Adds `relatedEntries` links. Works regardless. |
| **R7-A** (sign removed) | Sign absence is noticed visually, not through inspection. |
| **R7-B** (customer line) | NPC dialogue. Always delivered. |
| **R8-A** (ending quality) | Embedded in ending text. No prop involved. |

**Only the 5 environmental prop echoes (R1-A through R5-A) need the fallback.** All other echo types are delivery-method-immune to prior interaction.

#### Implementation

```csharp
// In NarrativeTriggerTracker.CheckEchoActivations():

private void ActivateEnvironmentalEcho(int echoId, string propId, string fallbackEntryTitle, string fallbackEntryBody)
{
    var obj = EnvironmentManager.Instance.GetObject(propId);

    if (obj != null && obj.hasBeenInspected)
    {
        // FALLBACK: Player already saw this prop. Deliver as notebook entry.
        NotebookManager.Instance.CreateAutoEntry(
            EntryType.AutoEvent,
            EntryCategory.Observation,
            fallbackEntryTitle,
            fallbackEntryBody,
            obj.zoneId.ToString(),
            propId
        );

        DevLog.Log(LogCategory.Narrative,
            $"Echo fallback: {GetName(echoId)} delivered as notebook entry (prop {propId} already inspected)");
    }
    else
    {
        // STANDARD: Activate prop for first-time discovery.
        EnvironmentManager.Instance.ActivateEchoProp(propId);

        DevLog.Log(LogCategory.Narrative,
            $"Echo standard: {GetName(echoId)} prop activated (prop {propId} not yet inspected)");
    }

    FireEcho(echoId, GetParentReveal(echoId));
}
```

#### Example Walkthrough

**Scenario:** Player inspects the tally marks (E09) on Day 1 and gets the standard entry: *"Dozens of marks. Different pens. Different hands. Someone's been counting."* On Day 4, R5 fires (Elder survived a previous cycle). Day 5 morning, the echo system checks R5-A.

**Without suppression rule:** R5-A activates the tally prop for re-inspection. But the player already inspected it on Day 1. They walk past it every day. They have no reason to press E on it again. The echo is effectively lost.

**With suppression rule:** R5-A detects `hasBeenInspected == true` on the tally prop. Instead of activating the prop, it creates a notebook auto-entry: *"The tally marks on the doorframe. One set looked familiar but I couldn't place it. Now I know — it's the Elder's handwriting. They were counting their own nights."* The entry appears in the Unread view. The player reads it whenever they next open the notebook. The echo lands.

**Debug log output:**
```
[Narrative] Echo fallback: R5-A delivered as notebook entry (prop office_doorframe_marks already inspected)
```

---

### Echo Budget Summary

| Reveal | Echoes | Methods Used | New Writing Required |
|---|---|---|---|
| R1 | 2 | Environment + NPC line | ~40 words (label inspect + customer line) |
| R2 | 2 | Environment + Notebook link | ~30 words (name tag inspect) |
| R3 | 2 | Environment + Entity behavior | ~40 words (blank pages inspect) |
| R4 | 1 | Environment | ~35 words (foundation symbol inspect) |
| R5 | 2 | Environment + Elder prefix line | ~40 words (tally recontextualization + prefix) |
| R6 | 2 | Environment (one-time text swap) + Notebook link | ~15 words (restore text variant) |
| R7 | 2 | Environment + NPC line | ~25 words (customer line; sign removal is wordless) |
| R8 | 1 | Ending text modifier | ~0 words (quality variation in already-budgeted endings) |
| **Total** | **14** | | **~225 words** |

**Total writing cost: ~225 words.** This is negligible against the 9,475-word budget.

**Total implementation cost:** 14 echo moments, all using existing systems:
- 6 pre-placed inactive props (activate on flag)
- 3 conditional NPC line variants (prefix or replacement)
- 3 notebook `relatedEntries` link additions (computed, no storage)
- 1 one-time auto-entry text swap
- 1 entity tuning parameter change

No new managers, no new events, no new UI. Echoes are flags that activate pre-authored content.

---

### Trigger Timing Summary

```
Day 1:  R1 fires ──────────────────────────────────────────────────
Day 2:  R1 echoes (shelf count, customer line) | R2 fires ────────
Day 3:  R2 echoes (name tag, notebook link) | R3 fires ───────────
Day 4:  R3 echoes (blank pages, entity lingers) | R4, R5 fire ────
Day 5:  R4 echo (foundation symbol) | R5 echoes (tally, Elder) | R6 fires
Day 6:  R6 echoes (restore text, notebook links) | R7 fires ──────
Day 7:  R7 echoes (sign gone, customer line) | R8 fires → echo in ending
```

Every echo fires the day after its parent reveal. No two reveals share the same echo day. The player encounters a steady drip of "wait, I recognize that" moments across the full 7-day cycle.

---

---

## Narrative Writing Order

### Purpose

A sequenced task list for writing all narrative content. Ordered by implementation dependency — each task produces text that later tasks may reference. Paced for solo development across 4 implementation phases.

---

### Writing Order — Full Task List

Each task is one sit-down writing session. Tasks within a batch have no dependencies on each other and can be written in any internal order.

#### BATCH 1 — System-Critical Auto-Entries (E01–E06)

These are the notebook auto-log texts that fire when system events occur for the first time. Write these first because they establish the player's voice — every subsequent piece of writing must sound like the same person.

| # | Task | Content | Words | Dependencies | Phase |
|---|---|---|---|---|---|
| W01 | E01 auto-entry | "Something moved overnight" — first displacement noticed | ~25 | None | Epic 6 |
| W02 | E06 auto-entry | "Another light out. They're getting worse." — first light failure | ~25 | None | Epic 6 |
| W03 | E03 auto-entry | "The lights flickered — something passed by" — first entity proximity | ~25 | None | Epic 6 |
| W04 | E04 auto-entry | "I organized this shelf yesterday. It's wrong again." — entropy revert | ~30 | W01 (must match tone) | Epic 7 |
| W05 | E02 auto-entry | "My notes don't match what I remember" — first corruption | ~30 | W01 (must match tone) | Epic 7 |
| W06 | E05 auto-entry + manufactured document | "This document says something different than my notes" + the shelf manifest document that contradicts P01 entry | ~60 | W05 (corruption voice established) | Epic 7 |

**Batch 1 total: ~195 words. 6 tasks. Establishes player voice for all subsequent writing.**

---

#### BATCH 2 — Core Narrative Environmental Text (E07–E11)

Inspection text for the 5 core narrative props. Write these before reveals because the reveals reference these moments ("the photograph," "the tally marks"). The writer needs to know exactly what these props say.

| # | Task | Content | Words | Dependencies | Phase |
|---|---|---|---|---|---|
| W07 | E09 inspection text | Tally marks on doorframe — "Dozens of marks. Different pens. Different hands. Someone's been counting." | ~25 | None | Epic 6 |
| W08 | E07 inspection text (x7 variants) | Photograph on wall — Day 1: "The whole staff, smiling. Someone in the back row." Day 4: "The face in the back is almost gone now." Day 7: "Just a smudge where someone used to be." Needs 3-4 variants (not all 7 — reuse between similar days). | ~80 | None | Epic 6 |
| W09 | E11 inspection text | Handprint on basement glass — "The fingers are too long. The palm is too narrow. This isn't human." | ~25 | None | Epic 10 |
| W10 | E08 inspection text | Employee cabinet — photographs + resignation letters. "Six letters. Six different names. All dated October 14th. Different years." | ~60 | W08 (photo references same employees) | Epic 10 |
| W11 | E10 inspection text | Apartment belongings — suitcase + unfinished letter. "The letter stops mid-sentence. 'I can't keep pretending that—'" | ~50 | None | Epic 10 |

**Batch 2 total: ~240 words. 5 tasks. Establishes environmental storytelling voice.**

---

#### BATCH 3 — Core NPC Dialogue: Elder Day 1 + Regular Customer Day 1

Vertical slice requirement. Write these before other NPC dialogue because they establish both characters' voices. All subsequent Elder/Regular lines must be consistent with these.

| # | Task | Content | Words | Dependencies | Phase |
|---|---|---|---|---|---|
| W12 | Elder Day 1 (4 lines) | Welcome, store orientation, history hints. Line 4 contains R1 content. | ~160 | W01-W06 (player voice established; Elder must sound different) | Epic 6 |
| W13 | Regular Customer Day 1 (3 lines) | Friendly introduction, routine establishment, warmth. | ~120 | W12 (characters must contrast) | Epic 6 |

**Batch 3 total: ~280 words. 2 tasks. Character voices locked.**

---

#### BATCH 4 — Day 1 Lore Fragments

The 4 lore fragments available on Day 1. Write after NPC voice is established so found-document tone contrasts with spoken dialogue.

| # | Task | Content | Words | Dependencies | Phase |
|---|---|---|---|---|---|
| W14 | Entity Origin fragment x1 | First hint of the entity — oblique, not direct. "...reports of missing inventory pre-date the current filing system." | ~75 | None | Epic 6 |
| W15 | Bookstore History fragment x1 | Store founding, original owner. Establishes normal history before the strangeness. | ~75 | None | Epic 6 |
| W16 | Previous Incidents fragment x1 | A note from a previous employee, mundane but slightly off. "Moved the Steinbeck section again. I don't remember moving it the first time." | ~75 | None | Epic 6 |
| W17 | Black Pines Town fragment x1 | Town context — logging town, isolation, "folks here keep to themselves." | ~75 | None | Epic 6 |

**Batch 4 total: ~300 words. 4 tasks. Lore voice established across all 4 required categories (+1 optional).**

---

#### BATCH 5 — Reveals R1–R3

The first three reveals. Write in order — R1 is the gentlest, R3 is the first real shock. Each sets the escalation bar for the next.

| # | Task | Content | Words | Dependencies | Phase |
|---|---|---|---|---|---|
| W18 | R1 reveal text + notebook entry | "Inventory losses for decades" — embedded in Elder Day 1 line 4 (already drafted in W12) + standalone notebook auto-entry summarizing the reveal. | ~75 | W12 (Elder voice) | Epic 6 |
| W19 | R2 reveal text + notebook entry | "Previous employee disappeared" — notebook insight triggered by lore count. Must reference tone of W16 (previous employee fragment). | ~100 | W16 (employee fragment voice) | Epic 7 |
| W20 | R3 reveal text + notebook entry | "It feeds on records, not people" — notebook insight after surviving Night 2 + seeing corruption. This is the tonal pivot: player's inner voice shifts from curious to alarmed. | ~120 | W05 (corruption auto-entry), W18-W19 (escalation curve) | Epic 7 |

**Batch 5 total: ~295 words. 3 tasks. Escalation voice established: curious → concerned → alarmed.**

---

#### BATCH 6 — Core NPC Dialogue: Elder Days 2–7 + Regular Customer Days 2–7

Write all remaining core NPC lines. The Elder's arc goes from evasive to confessional. The Regular Customer's arc goes from friendly to frightened.

| # | Task | Content | Words | Dependencies | Phase |
|---|---|---|---|---|---|
| W21 | Elder Days 2-3 (6 lines) | Routine advice, casual mentions, first hint of concern. | ~240 | W12 (voice locked) | Epic 7 |
| W22 | Regular Customer Days 2-3 (6 lines) | Book talk, "something felt off," asks about changes. | ~240 | W13 (voice locked) | Epic 7 |
| W23 | Elder Days 4-5 (7 lines) | Personal connection revealed, containment discussion. Contains R5 content. | ~280 | W21 (arc progression) | Epic 9 |
| W24 | Regular Customer Days 4-5 (6 lines) | Growing unease, notices lighting, townsfolk acting strange. | ~240 | W22 (arc progression) | Epic 9 |
| W25 | Elder Days 6-7 (7 lines) | Hiring truth, final testimony. Contains R8 support content. | ~280 | W23 (arc progression) | Epic 10 |
| W26 | Regular Customer Days 6-7 (5 lines) | Concerned, considers leaving, final visit. | ~200 | W24 (arc progression) | Epic 10 |

**Batch 6 total: ~1,480 words. 6 tasks. Both character arcs complete.**

---

#### BATCH 7 — Reveals R4–R8

The back half of reveals. Write after Elder dialogue because R5, R7, R8 are tightly coupled to Elder content.

| # | Task | Content | Words | Dependencies | Phase |
|---|---|---|---|---|---|
| W27 | R4 reveal text + notebook entry | "Indigenous warnings" — puzzle P06 reward text. Must reference archive documents (write the specific document text here too). | ~150 | W10 (E08 cabinet text — same archive tone) | Epic 9 |
| W28 | R5 reveal text + notebook entry | "Elder survived a previous cycle" — embedded in Elder Day 4 line (W23). Standalone notebook summary. | ~100 | W23 (Elder Day 4 dialogue) | Epic 9 |
| W29 | R6 reveal text + notebook entry | "Containment through ritual maintenance" — notebook insight at lore threshold. Reframes everything the player has been doing. | ~120 | W20 (R3 tone), W23 (Elder containment hints) | Epic 9 |
| W30 | R7 reveal text + notebook entry | "Hiring was deliberate" — puzzle P10 reward. The letter in the safe. | ~150 | W11 (E10 apartment text), W25 (Elder Day 6) | Epic 10 |
| W31 | R8 reveal text + notebook entry (full + partial variants) | "Entity bound to written word" — capstone. Full version (~150 words) references Elder testimony. Partial version (~100 words) stands alone. | ~250 | W25 (Elder Day 7), W27-W29 (all prior reveals) | Epic 10 |

**Batch 7 total: ~770 words. 5 tasks. All reveals complete.**

---

#### BATCH 8 — Remaining Lore Fragments (Days 2–7)

31 remaining fragments. Write in category clusters so voice stays consistent within each category. Write after all reveals so fragments can plant seeds that reveals later pay off.

| # | Task | Content | Words | Dependencies | Phase |
|---|---|---|---|---|---|
| W32 | Entity Origin x7 | Remaining entity lore. Escalates from oblique to direct across the 7 fragments. | ~525 | W20 (R3 — defines what entity does), W31 (R8 — defines what entity is) | Epic 9 |
| W33 | Previous Incidents x5 | Remaining incident reports. Each is a different person's account from a different decade. | ~375 | W19 (R2 — establishes disappearance pattern) | Epic 9 |
| W34 | Bookstore History x7 | Remaining store history. Mundane-to-strange gradient. | ~525 | W14 (voice established) | Epic 10 |
| W35 | Black Pines Town x6 | Remaining town fragments. Logging industry, isolation, local legends. | ~450 | W17 (voice established) | Epic 12+ |
| W36 | Personal Records x6 | Predecessor's notes, Elder's history, family connections. Most emotionally heavy category — write last. | ~450 | W28 (R5), W30 (R7) — these reveals define whose records these are | Epic 12+ |

**Batch 8 total: ~2,325 words. 5 tasks. All 35 lore fragments complete.**

---

#### BATCH 9 — Ending Narratives

Write after everything else. Endings reference reveals, echo the player's journey, and must account for what the player did and didn't discover. Each ending needs to work whether the player found 10 or 35 lore fragments.

| # | Task | Content | Words | Dependencies | Phase |
|---|---|---|---|---|---|
| W37 | Endings 1-2 (Best/Good — truth-focused) | Full Preservation + Understanding. Reference R8, Elder relationship, knowledge preserved. | ~400 | W31 (R8), W25 (Elder arc) | Epic 10 |
| W38 | Endings 3-4 (Good/Bittersweet — action-focused) | Survival Through Order + The Archivist. Reference maintenance, lore, notebook usage. | ~375 | W29 (R6 — containment), W36 (personal records) | Epic 10 |
| W39 | Ending 5 (Default — neutral) | Imperfect Memory. Must work for the broadest range of player states. The "most players get this" ending. | ~175 | W20 (R3 — minimum understanding) | Epic 10 |
| W40 | Ending 6 (Neutral — mechanical) | Pattern Breaker. Puzzle mastery, pattern recognition. Less emotional, more analytical. | ~150 | None (mechanical ending, references systems not story) | Epic 10 |
| W41 | Endings 7-8 (Dark) | The Borrower Remains + Lost Records. Entity wins, knowledge lost. Must be satisfying losses, not frustrating ones. | ~350 | W31 (R8 — player may have partial understanding) | Epic 10 |
| W42 | Endings 9-10 (Worst) | Collapse + Forgotten. Total failure states. Short, bleak, but not punishing. Should motivate replay, not quit. | ~275 | None (these endings assume player missed most content) | Epic 10 |

**Batch 9 total: ~1,725 words. 6 tasks. All 10 endings complete.**

---

#### BATCH 10 — Echo Writing

Write after reveals and endings, since echoes reference both. Minimal word count — mostly activation logic, not prose.

| # | Task | Content | Words | Dependencies | Phase |
|---|---|---|---|---|---|
| W43 | R1 echoes (R1-A, R1-B) | Shelf label text + fallback notebook variant + customer Hemingway line | ~70 | W18 (R1 text) | Epic 10 |
| W44 | R2 echoes (R2-A) | Name tag inspection text + fallback notebook variant (R2-B is a notebook link, no writing) | ~55 | W19 (R2 text) | Epic 10 |
| W45 | R3 echo (R3-A) | Blank pages inspection text + fallback notebook variant (R3-B is entity tuning, no writing) | ~70 | W20 (R3 text) | Epic 10 |
| W46 | R4 echo (R4-A) | Foundation symbol inspection text + fallback notebook variant | ~65 | W27 (R4 text) | Epic 10 |
| W47 | R5 echoes (R5-A, R5-B) | Tally re-inspection text + fallback notebook variant + Elder prefix line | ~70 | W28 (R5 text), W07 (E09 tally text) | Epic 10 |
| W48 | R6 echo (R6-A) | One-time restore text variant (R6-B is notebook link, no writing) | ~15 | W29 (R6 text) | Epic 10 |
| W49 | R7 echoes (R7-B) | Customer "old man asked about you" line (R7-A is prop removal, no writing) | ~25 | W30 (R7 text) | Epic 10 |

**Batch 10 total: ~370 words. 7 tasks. All echoes + fallback variants complete.**

---

#### BATCH 11 — Optional NPC Dialogue

Write only after all required content is done. Each NPC is a self-contained batch — can stop after any one.

| # | Task | Content | Words | Dependencies | Phase |
|---|---|---|---|---|---|
| W50 | Mail Carrier (12 lines, Days 1/3/5/7) | Outside-world context. Packages, weather, town gossip. 3 lines per appearance. | ~480 | W13 (Regular Customer voice — Mail Carrier should contrast) | Epic 12 |
| W51 | Librarian (12 lines, Days 2/4/6) | Archive connections, cross-reference hints, academic tone. 4 lines per appearance. | ~480 | W27 (R4 — archive tone established) | Epic 12 |
| W52 | Local Historian (12 lines, Days 3/5/7) | Town history, local legends, Black Pines context. 4 lines per appearance. | ~480 | W35 (town lore fragments — same subject matter) | Epic 13 |
| W53 | Repair Technician (10 lines, Days 2/4) | Electrical oddities, building complaints, blue-collar voice. 5 lines per appearance. | ~400 | None (standalone voice) | Epic 13 |

**Batch 11 total: ~1,840 words. 4 tasks. All optional NPCs complete.**

---

#### BATCH 12 — Optional Atmospheric Environmental Text

Last priority. Pure flavor. Write if and only if all required content is shipped.

| # | Task | Content | Words | Dependencies | Phase |
|---|---|---|---|---|---|
| W54 | E12 coffee mug (daily state descriptions) | Warm → lukewarm → cold → untouched. 2-3 inspection variants. | ~40 | None | Epic 14 |
| W55 | E15 HELP WANTED sign | Inspection text: "My job listing. Still in the window." | ~15 | None | Epic 14 |
| W56 | E13 backwards clock | No writing needed — visual/audio only. | 0 | None | Epic 14 |
| W57 | E14 rain intensification | No writing needed — audio only. | 0 | None | Epic 14 |

**Batch 12 total: ~55 words. 4 tasks (2 require writing, 2 are asset-only).**

---

### Dependency Graph

```
BATCH 1 (System auto-entries)
  W01 W02 W03 ─── independent, write in any order
       │
       ▼
  W04 W05 ──────── must match W01 tone
       │
       ▼
  W06 ──────────── needs W05 corruption voice
       │
       ▼
BATCH 2 (Environmental props)
  W07 W08 ──────── independent
  W09 W10 W11 ──── W10 needs W08 (same employees)
       │
       ▼
BATCH 3 (Day 1 NPC voices)
  W12 ──────────── needs Batch 1 (player voice contrast)
  W13 ──────────── needs W12 (character contrast)
       │
       ▼
BATCH 4 (Day 1 lore)
  W14 W15 W16 W17 ─ independent, write in any order
       │
       ▼
BATCH 5 (Reveals R1–R3)
  W18 ──────────── needs W12 (Elder Day 1)
  W19 ──────────── needs W16 (employee fragment)
  W20 ──────────── needs W05 + W18 + W19
       │
       ▼
BATCH 6 (NPC Days 2–7)          BATCH 8a (Lore: Entity + Incidents)
  W21-W26 ────────────────────     W32 W33 ──── need R2, R3 reveals
       │                                │
       ▼                                ▼
BATCH 7 (Reveals R4–R8)         BATCH 8b (Lore: History + Town + Personal)
  W27-W31 ────────────────────     W34 W35 W36 ── need R5, R7 reveals
       │
       ▼
BATCH 9 (Endings)
  W37-W42 ────────────────────── need all reveals + NPC arcs
       │
       ▼
BATCH 10 (Echoes)
  W43-W49 ────────────────────── need parent reveals
       │
       ▼
BATCH 11 (Optional NPCs)
  W50-W53 ────────────────────── independent of each other
       │
       ▼
BATCH 12 (Optional Atmospheric)
  W54-W57 ────────────────────── no dependencies
```

---

### Solo Development Pacing

#### Phase-by-Phase Schedule

| Phase | Epic | Batches | Tasks | Words | Sessions |
|---|---|---|---|---|---|
| **Vertical Slice** | Epic 6 | 1 (partial), 2 (partial), 3, 4, 5 (partial) | W01-W03, W07-W08, W12-W18 | ~730 | 1-2 sessions |
| **Core Loop** | Epics 7-9 | 1 (rest), 5 (rest), 6 (partial), 7 (partial), 8a | W04-W06, W19-W24, W27-W29, W32-W33 | ~3,155 | 4-5 sessions |
| **Narrative Integration** | Epic 10 | 2 (rest), 6 (rest), 7 (rest), 8b (partial), 9, 10 | W09-W11, W25-W26, W30-W31, W34, W37-W49 | ~3,895 | 5-6 sessions |
| **Polish** | Epics 12-16 | 8b (rest), 11, 12 | W35-W36, W50-W57 | ~2,375 | 3-4 sessions |
| | | | **Total: 57 tasks** | **~10,155 words** | **13-17 sessions** |

#### Session Definition

One **writing session** = 45-90 minutes of focused writing producing 400-800 words of final text. This accounts for:
- Drafting (~60% of session)
- Self-editing for voice consistency (~25% of session)
- Placeholder implementation notes (~15% of session)

#### Pacing Rules

1. **Never write more than 2 sessions in one day.** Writing fatigue produces inconsistent voice.
2. **Re-read the last 3 tasks before starting a new session.** Maintains voice continuity.
3. **Write all of a character's lines in one sitting where possible.** Elder Days 2-3 (W21) should be one session, not split across days.
4. **Lore fragments within a category should be written together.** All Entity Origin fragments (W32) in one session keeps the category voice tight.
5. **Endings should be written in one concentrated push.** W37-W42 across 2-3 consecutive sessions. The endings must feel like they belong to the same game.
6. **Stop after Batch 9.** Everything past Batch 9 is optional content. If you're behind schedule, Batches 10-12 can be cut entirely without affecting the game's ability to ship.

---

## Narrative Tone Guide

### Purpose

A reference sheet for maintaining consistent voice across all 57 writing tasks and ~10,000 words of narrative content. Print this. Tape it to the wall. Re-read it before every writing session.

---

### 1. Player Inner Voice

The player character writes notebook entries, reacts to inspected objects, and thinks in auto-logged observations. This voice carries 60%+ of all written text. It must be the most consistent voice in the game.

#### Character Profile

The player is a young adult (mid-20s) who took a bookstore job in a small town. Educated but not academic. Observant by nature — the kind of person who notices when a picture frame is crooked. Calm under uncertainty, not prone to panic. Processes fear through **noticing and recording**, not screaming or freezing. They write the way someone writes in a journal: direct, honest, slightly detached when scared.

#### Sentence Style

- **Short declarative sentences.** Subject-verb-object. State what is, not what might be.
- **Fragments are allowed** when the character is processing something. A fragment after a full sentence creates a beat: *"The shelf was wrong. All of it."*
- **One idea per sentence.** Never stack clauses. If a sentence has a semicolon, split it.
- **Present observations, past reflections.** Inspections are present tense ("The pages are blank"). Notebook entries reflecting on events may use past ("I organized this shelf yesterday").

#### Vocabulary Level

- **Everyday words.** The player says "wrong" not "anomalous." "Gone" not "absent." "Old" not "antiquated."
- **Specific concrete nouns.** Name the object. "The Steinbeck" not "a book." "Section C" not "one of the shelves." Specificity creates the feeling that the player knows this store.
- **No jargon, no literary flourish.** This person works in a bookstore. They read, but they write plainly.

#### Emotional Baseline

- **Days 1-2: Curious.** Noticing things with mild interest. Tone: a new employee figuring out a space. *"Dozens of marks. Different pens. Different hands. Someone's been counting."*
- **Days 3-4: Unsettled.** The same observation style, but sentences get shorter. Pauses appear. *"My notes don't match what I remember."*
- **Days 5-6: Controlled alarm.** Still recording, but the entries feel tighter. The player is holding it together by writing things down. *"Not just organizing. Maintaining the boundary."*
- **Day 7: Resolve.** Fear has become understanding. Entries are clear-eyed. *"The words just... aren't there anymore. It ate them."*

The player **never panics in text.** Fear shows through precision — shorter sentences, more fragments, fewer adjectives. The scariest entries are the simplest.

#### Pacing Style

- **Observation → Beat → Implication.** State what you see. Pause (period or em dash). State what it means.
- *"Six letters. Six different names. All dated October 14th. Different years."*
- *"Three hundred and forty. I count twenty-eight."*
- The pattern is: fact, fact, fact — then the quiet implication the reader fills in.

#### Punctuation Constraints

- **Periods and fragments for processing.** This is the player's default rhythm.
- **Exception: One ellipsis is permitted in player voice at peak dread or realization moments, maximum once per day.** It should signal mental interruption, not hesitation.

#### Reference Lines

1. *"Something moved overnight."* — E01 (flat observation, no emotion, lets the player feel their own unease)
2. *"I organized this shelf yesterday. It's wrong again."* — E04 (simple past + simple present, the contradiction carries the weight)
3. *"The pages are blank. Not torn out. Just... empty. Like the words were never there."* — R3-A echo (observation → correction → quiet horror, ellipsis marks the moment understanding arrives)
4. *"That name tag in the desk. I didn't think anything of it before. Now I know whose it was."* — R2-A fallback (past indifference → present knowledge, no stated emotion)
5. *"The fingers are too long. The palm is too narrow. This isn't human."* — E11 (three observations, ascending certainty, the conclusion is the shortest sentence)

---

### 2. Elder Voice

The Elder is the bookstore's retired founder. Old, measured, carrying a secret they've held for decades. They speak like someone choosing every word carefully — not because they're hiding something (though they are), but because they've learned that words matter in this place.

#### Character Profile

70s or 80s. Has owned this store for 40+ years. Survived a previous entity cycle. Hired the player deliberately. Carries guilt about not warning them directly, but believes the player must discover the truth through observation (the same way the Elder did). Speaks with the cadence of someone who has told parts of this story before — to themselves, late at night, rehearsing what to say and never quite saying it.

#### Sentence Style

- **Measured, deliberate.** Sentences are medium-length — longer than the player's, but never run-on.
- **Indirect when concealing, direct when revealing.** Early days: talks around the subject. Late days: drops the pretense.
- **Uses rhetorical questions sparingly.** Not to be coy — to genuinely ask the player to think. *"You ever notice how the shelves look different in the morning?"*
- **Trails off occasionally.** Not confusion — reluctance. The sentence starts to say too much, and the Elder catches it. Marked with an em dash, not ellipsis. *"The last person who worked here — well, they were observant too."*

#### Vocabulary Level

- **Slightly elevated but not formal.** The Elder reads. They use words like "peculiar" and "arrangement" naturally, not performatively.
- **Bookstore-specific language.** "Shelving," "inventory," "the stacks." The store is their language.
- **Avoids modern slang.** This person's speech patterns froze in the 1970s. No "like" as filler, no hedging.

#### Emotional Baseline

- **Days 1-2: Warm, welcoming, evasive.** Playing the role of a friendly retired owner. Information is offered as advice, not warning.
- **Days 3-4: Slipping.** Concern breaks through the warmth. The Elder starts saying things they hadn't planned to say.
- **Days 5-6: Confessional.** The pretense drops. Sentences are shorter. The Elder speaks to be understood, not to be pleasant.
- **Day 7: Unburdened.** Everything on the table. Speaks with the relief of someone who's finally told the truth.

#### Reference Lines

1. *"It's a good store. Been here longer than most things in this town. You'll learn its rhythms."* — Day 1 (warm, possessive of the store, "rhythms" foreshadows the routine-as-survival theme)
2. *"You ever notice how the shelves look different in the morning? I used to think it was the cleaning crew. We never had a cleaning crew."* — Day 2 (rhetorical question, anecdote that reveals too much, catches himself)
3. *"The last person who worked here — they were careful. Kept good notes. That matters more than you'd think."* — Day 3 (trails off, redirects to advice, "that matters" is the warning disguised as encouragement)
4. *"You look at me different now. That's fair."* — R5-B echo prefix (shortest Elder line, no deflection, meets the player's eyes for the first time)
5. *"I should have told you sooner. I tell myself I was protecting you, but I was protecting me. I didn't want to say it out loud because then it's real again."* — Day 7 (confessional, three sentences of escalating honesty, the last clause breaks his composure)

---

### 3. Regular Customer Voice

A daily visitor who comes for coffee and browsing. The emotional barometer of the game — their shifting comfort level mirrors what the player should be feeling. Friendly, chatty, grounded. The one normal person in an increasingly abnormal situation.

#### Character Profile

30s-40s. Lives in Black Pines. Works locally (unspecified — doesn't matter). Comes to the bookstore every day out of habit. Not perceptive the way the player is, but notices big changes. Processes discomfort by talking around it. Leaves when things get too strange. Their arc: comfortable → mildly uneasy → openly rattled → considering not coming back → final visit.

#### Sentence Style

- **Conversational and loose.** The longest average sentence length of the three voices. Uses "and" to chain thoughts. Speaks the way people actually talk.
- **Hedges and qualifiers.** "Maybe," "sort of," "I don't know." This person is not certain about anything — which contrasts with the player's precision and the Elder's deliberateness.
- **Trails off with ellipsis (not em dash).** Unlike the Elder's deliberate cut-off, the Customer just loses their train of thought. *"I just thought... never mind."*
- **Asks questions they don't want answered.** *"Is everything okay in here? I mean, it's probably fine."*

#### Vocabulary Level

- **Casual everyday speech.** "Weird," "off," "freaked me out a little." No elevated language.
- **Vague when describing unease.** Can't articulate what's wrong. Uses "something" and "it" without antecedents. *"It just feels different in here."*
- **References their own routine.** "My usual," "same time tomorrow," "the chair by the window." Their language is built around habit.

#### Emotional Baseline

- **Days 1-2: Warm, chatty, at home.** This is their happy place. They talk about books, the weather, small town life.
- **Days 3-4: Fidgety.** Still comes in, but pauses more. Starts mentioning that things feel "off" without being able to say why.
- **Days 5-6: Nervous.** Visits are shorter. Looks around more than they read. Considering leaving.
- **Day 7: Final visit.** Either supportive (high stability) or frightened (low stability). The only NPC line in the game that branches based on game state.

#### Reference Lines

1. *"Same order as always. You know, I think I come here more for the quiet than the coffee."* — Day 1 (establishes routine, warmth, the bookstore as comfort)
2. *"Funny, I could've sworn you had a whole row of Hemingway last week. Maybe I'm imagining things."* — R1-B echo (notices displacement but dismisses it, hedges with "maybe," trusts normalcy over evidence)
3. *"Did something happen in here last night? I don't know, it just feels... different. Like the air is heavier or something."* — Day 3 (question they don't want answered, vague sensory language, trails off)
4. *"I almost didn't come in today. That sounds silly, right? I just — I don't know."* — Day 6 (confession of avoidance, self-doubt, unfinished thought)
5. *"The old man asked about you, you know. Before you even applied. Asked if I thought you were the observant type."* — R7-B echo (delivers information plainly, no awareness of its weight, casual tone makes the revelation land harder)

---

### 4. Writing Constraints

Rules that apply across all three voices and all narrative content types (auto-entries, NPC dialogue, lore fragments, ending text).

#### Sentence Length

| Voice | Max Sentence Length | Typical Range |
|---|---|---|
| Player | **18 words** | 4-12 words |
| Elder | **25 words** | 8-18 words |
| Regular Customer | **22 words** | 6-16 words |
| Lore Fragments | **20 words** | 6-15 words |
| Ending Narratives | **22 words** | 8-18 words |

**If a sentence exceeds the max, split it.** No exceptions. Long sentences diffuse tension. This game runs on precision.

#### Language Style

**Prefer:**
- Concrete nouns over abstract ones ("the shelf" not "the environment")
- Active voice over passive ("I counted twenty-eight" not "twenty-eight were counted")
- Simple past and present tense — avoid progressive ("was running") and conditional ("would have been")
- Silence over explanation — if the object tells the story, the text doesn't need to
- Em dashes for interrupted or redirected thoughts (Elder)
- Ellipsis for trailing, unfinished thoughts (Regular Customer primarily; see Player Punctuation Constraints for peak-dread exception)
- Periods and fragments for processing (Player)

**Avoid:**
- Adverbs. If the verb needs modification, pick a better verb. Not "walked quickly" — "rushed." Not "spoke quietly" — "murmured." But prefer the simplest verb when possible: "said" over "murmured."
- Rhetorical escalation. No "but that wasn't the worst part." No "little did I know." No narrative throat-clearing.
- Exclamation marks. One per 1,000 words maximum. Fear is quiet in this game.
- Metaphor and simile. One per character per day, maximum. The player says "like the words were never there" — that's their one simile. The rest is literal.
- Second-person address to the player. No NPC says "you should be careful." They say "I'd be careful" or just "be careful."

#### Forbidden Tone Elements

These break the game's atmosphere. Never use them in any voice:

1. **Humor or quips.** No levity, no wisecracks, no dark comedy. Warmth is allowed (Regular Customer Days 1-2). Humor is not.
2. **Explicit horror language.** No "terrifying," "horrifying," "nightmarish," "blood-curdling." Fear is shown through observation, never named.
3. **Exposition dumps.** No character explains the plot. Information comes in fragments. If a line contains more than 2 new facts, split it across multiple lines or days.
4. **Self-aware genre references.** No character says "this is like a horror movie." No winking at the audience.
5. **Profanity.** The game's tension comes from restraint. Even mild profanity breaks the register.
6. **Emotional stage directions.** No "[said nervously]" or "voice trembling." The words carry the emotion. If they don't, rewrite the words.
7. **Purple prose.** No "the shadows seemed to breathe" or "darkness crept like a living thing." State what is. Let the player's imagination do the rest.

---

### 5. Sensory Anchor Guidelines

Each voice processes the world through a different sensory filter. The player *sees*. The Elder *remembers*. The Customer *feels*. These anchors prevent all three characters from sounding like the same person describing the same room.

#### Player — Visual Observation Bias

The player trusts their eyes above all else. They report what they see, count, and measure. They do not interpret feelings, guess causes, or project emotion onto objects.

**Allowed sensory anchors:**

| Sense | Usage | Example Words |
|---|---|---|
| **Sight** (primary — 70%+ of references) | Direct visual reporting. What's there, what's missing, what changed. | see, look, count, notice, read, blank, dark, dim, empty, crooked, displaced, wrong |
| **Touch** (secondary — 15%) | Only when the player physically interacts. Inspection moments. | cold, smooth, rough, heavy, light, damp, worn, brittle |
| **Sound** (limited — 10%) | Only reactive — the player hears something, then looks. Sound prompts vision, never stands alone. | heard, click, creak, hum, silence, quiet |
| **Smell** (rare — 5%) | Grounding details only. Old paper, dust, coffee. Never used for mood. | dust, paper, ink, wood, coffee |

**Restricted sensory patterns:**

- **No emotional projection onto objects.** Not "the shelf felt wrong." Instead: "The shelf was wrong." Objects don't feel things.
- **No temperature-as-mood.** Not "the room felt colder." The player says "the light was out" — the player explains *why* it's cold, or doesn't mention temperature at all.
- **No synesthesia.** Not "the silence tasted like metal." Senses stay in their lanes.
- **No ambient atmosphere descriptions.** Not "the air was thick with dread." The player describes what they see. The reader supplies the dread.

**Example line adjustments:**

| Before (wrong) | After (correct) | Why |
|---|---|---|
| *"The basement felt oppressive and heavy."* | *"The basement light was out. Water stains on the ceiling. Low clearance."* | Replaces emotional atmosphere with three visual observations. The reader feels the oppression. |
| *"A cold chill ran down my spine when I saw the handprint."* | *"The fingers are too long. The palm is too narrow. This isn't human."* | Removes bodily sensation cliche. Three visual measurements create more dread than a chill ever could. |
| *"I could hear something breathing in the dark."* | *"The shelf at the far end moved. I saw it."* | Sound → vision redirect. Player heard something but reports what they can confirm: visual evidence. |

---

#### Elder — Memory-Oriented Phrasing

The Elder lives in two time periods at once. Everything they see reminds them of something they saw before. Their sensory anchors are temporal — they filter the present through decades of accumulated past.

**Allowed sensory anchors:**

| Anchor Type | Usage | Example Phrases |
|---|---|---|
| **Temporal markers** (primary — 50%+) | Anchoring present observations to past experience. When things happened. How long things have been a certain way. | "used to," "back when," "for years," "longer than," "I remember when," "that was before," "the first time I" |
| **Memory-sense overlap** (secondary — 25%) | Describing current things through the lens of past versions. What something *was* versus what it *is*. | "it wasn't always like this," "there was a time when," "you wouldn't know it now, but," "this used to be" |
| **Material knowledge** (tertiary — 15%) | Knowing objects by their history, not their appearance. The Elder identifies a book by who donated it, a shelf by who built it. | "that shelf was the original," "those came from the estate sale in '72," "I installed that light myself" |
| **Sound memory** (rare — 10%) | Sounds from the store's past overlaid on the present. Only used when deeply reflective. | "the store used to be quiet in a different way," "I still listen for the bell" |

**Restricted sensory patterns:**

- **No present-tense visual reporting.** That's the player's job. The Elder doesn't say "I see the light is out." They say "That light's been trouble for years."
- **No bodily sensation.** The Elder doesn't describe their own physical state. No "my hands shook" or "I felt cold." Their body is not the instrument — their memory is.
- **No nostalgia-as-warmth.** Memory is not comforting for the Elder. When they reference the past, there's weight, not fondness. "Back when" doesn't mean "the good old days."

**Example line adjustments:**

| Before (wrong) | After (correct) | Why |
|---|---|---|
| *"I can see the lights are failing again."* | *"Those lights have been failing since before you were born. I just stopped replacing them for a while."* | Present observation → temporal framing + confession. The Elder adds history, not description. |
| *"This place feels dangerous now."* | *"There was a time I thought keeping this place open was the brave thing to do. Now I'm not sure it wasn't the selfish thing."* | Removes vague feeling, replaces with self-reflective memory. Danger is implied through the Elder's moral reckoning, not stated. |
| *"I remember the old days fondly."* | *"I remember the old days. That's not the same as missing them."* | Kills nostalgia-as-warmth. Memory is precise, not sentimental. |

---

#### Regular Customer — Uncertainty Signals

The Customer processes the world through feeling — not emotion exactly, but *vibes*. Something is "off." The air is "heavier." They can't point to what changed. Their sensory anchors are the opposite of the player's precision: vague, body-centered, and always hedged.

**Allowed sensory anchors:**

| Anchor Type | Usage | Example Phrases |
|---|---|---|
| **Vague body-sense** (primary — 40%) | Physical discomfort they can't locate or explain. Gut feelings. Restlessness. | "feels different," "feels off," "something about," "I can't put my finger on it," "it's just a feeling," "my stomach" |
| **Conversational hedges** (primary — 30%) | Qualifiers that soften every claim. The Customer never commits to an observation. | "maybe," "sort of," "I guess," "I don't know," "probably," "I could be wrong," "it's silly but," "I think," "right?" |
| **Routine disruption** (secondary — 20%) | Noticing that their habits don't feel right. The chair is in the wrong spot. The coffee tastes different. The store doesn't *sound* like itself. | "usually," "always," "my usual," "normally," "every day," "same time," "I always sit" |
| **Comparative unease** (rare — 10%) | Comparing today to yesterday, this visit to last visit. Never absolute, always relative. | "more than yesterday," "worse than last time," "not like it was," "it didn't used to be" |

**Restricted sensory patterns:**

- **No precise visual observation.** The Customer doesn't count books or notice specific displacements. That's the player's skill. The Customer says "didn't you have more of those?" — not "Section C is missing 312 titles."
- **No naming what's wrong.** The Customer never identifies the entity, the corruption, or the pattern. They feel the effects without understanding the cause.
- **No confident statements about the environment.** Every observation is hedged. If the Customer says something definitive, it must be immediately followed by self-doubt. *"Something is wrong in here. I mean — probably not. I'm being weird."*

**Example line adjustments:**

| Before (wrong) | After (correct) | Why |
|---|---|---|
| *"The books on that shelf have been moved."* | *"Didn't you have more of those? On that shelf over there? Maybe I'm thinking of a different shelf."* | Removes precision. The Customer notices something but immediately doubts their own observation. |
| *"I'm scared to come in here."* | *"I almost didn't come in today. That sounds silly, right?"* | Removes direct fear language. The Customer admits avoidance behavior but frames it as irrational. Asks for permission to feel uneasy. |
| *"The entity is affecting the store."* | *"It just feels different in here. Like the air is heavier or something."* | Removes all causal knowledge. Replaces with body-sense vagueness. "Or something" is the quintessential Customer hedge. |

---

### Sensory Quick Reference

```
PLAYER SENSES
  Eyes first. Always.
  "I count twenty-eight." "The light was out." "The pages are blank."
  Allowed: see, count, notice, read, dark, empty, cold (touch), quiet (sound)
  Banned: felt [emotion], seemed, as if, oppressive, eerie, dread

ELDER SENSES
  Memory first. Always.
  "Those lights have been trouble for years." "I remember when."
  Allowed: used to, back when, before, longer than, the first time
  Banned: I see, I feel, fondly, the good old days, I sense

CUSTOMER SENSES
  Gut first. Always.
  "It just feels... different." "Maybe I'm imagining things."
  Allowed: feels, off, something, maybe, I don't know, usually, right?
  Banned: specific counts, object names, confident assertions, causal knowledge
```

---

### Quick Reference Card

Tear this out. Keep it visible while writing.

```
PLAYER VOICE
  Short. Flat. Precise. Observes, doesn't interpret.
  "Something moved overnight."
  Fear = shorter sentences, more fragments, fewer words.
  Senses: EYES first. Count, measure, report. Touch on inspect only.
  NEVER: panics, explains, uses adjectives to create mood.
  NEVER: "felt [emotion]," "seemed," "as if," "eerie," "dread."

ELDER VOICE
  Measured. Deliberate. Carries weight under control.
  "You'll learn its rhythms."
  Evasive early, confessional late. Em dashes when catching himself.
  Senses: MEMORY first. "Used to," "back when," "before you were born."
  NEVER: casual, modern, vague. Always chooses words carefully.
  NEVER: present-tense visual reporting, nostalgia-as-warmth.

REGULAR CUSTOMER VOICE
  Chatty. Hedging. Normal person in abnormal place.
  "Maybe I'm imagining things."
  Comfortable early, rattled late. Ellipsis when losing thread.
  Senses: GUT first. "Feels off," "something about," "I don't know."
  NEVER: precise, analytical, brave. Always slightly behind the truth.
  NEVER: specific counts, object names, confident assertions.

ALL VOICES
  Max sentence: 18 (player) / 25 (Elder) / 22 (customer)
  No adverbs. No exclamation marks. No horror adjectives.
  No humor. No exposition dumps. No profanity.
  Concrete nouns. Active voice. State what is.
```

---

### Narrative Silence Rules — Silence Reference Guide

Silence is the primary tool. Dialogue is the exception. If a moment works without words, it already works.

#### 1. When NOT to Write Dialogue

These situations must have **zero dialogue**. No inner monologue, no NPC lines, no notebook entries.

| Situation | Why Silence Works | What Replaces Words |
|---|---|---|
| **Entity presence** | Explaining the entity diminishes it. The player's imagination is worse than anything you write. | Sound design, lighting shift, object displacement |
| **Discovering a displaced object** | The player already knows something is wrong. Narrating it makes it smaller. | Object in wrong slot. Player connects the dots. |
| **Returning to the bookstore after hours** | Tension comes from what might be different, not from being told it is. | Environmental state changes only |
| **First 30 seconds of any day** | Let the player look around before feeding them information. | Ambient audio, natural light shift |
| **Puzzle solving (mid-solve)** | Interrupting flow breaks concentration and cheapens the challenge. | UI feedback only (click, snap, glow) |
| **Any moment the player is already scared** | Narration during fear reads as hand-holding. Let them sit in it. | Nothing. Let them breathe. |
| **NPC departures** | "See you tomorrow" is filler. The door closing says it. | Door sound, bell chime, empty room |
| **Repeated inspections** | If the player has already read a lore fragment or notebook entry, don't re-trigger it. | "Already recorded" UI indicator, no new text |

**The test:** Before writing any line, ask: *Does removing this make the moment worse?* If the answer is no or you're unsure, don't write it.

#### 2. When Silence Replaces Explanation

These are moments where the design might tempt you to explain. Resist.

| The Temptation | The Silence Instead |
|---|---|
| Explain why books moved overnight | Show the books in new positions. Period. |
| Explain the entity's motivation | Never. Not even in endings. |
| Explain why the Elder is evasive | Let the player notice the pattern across days |
| Tell the player a puzzle is solvable | Place the pieces. Let them try. |
| Confirm the player's theory about the store | Provide evidence. Never confirm. |
| Explain what a lore fragment means | The fragment is the explanation. If it needs a gloss, rewrite the fragment. |
| Narrate the passage of time | Lighting change. Clock position. Done. |
| React to the ending the player earned | State what happened. Don't editorialize. The tone comes from what was preserved or lost. |

**Core principle:** The player is an employee doing a job. They observe, record, and decide. They do not receive explanations from the universe.

#### 3. Maximum Narrative Density Per Day

Hard limits on how much text the player can encounter in a single in-game day, regardless of what's technically available.

```
NARRATIVE DENSITY CAPS (per in-game day)

  Player inner voice lines:     max 6 unique triggers
  NPC dialogue exchanges:       max 3 conversations (any length)
  Lore fragments findable:      max 6 (see Content Pacing Table)
  Notebook auto-entries:         max 2 (echoes, failsafe entries)
  Environmental text props:      max 2 readable (signs, notes, labels)
  Reveal-weight moments:         max 2 per day (Day 4 is the ceiling)

TOTAL TEXT EXPOSURE PER DAY:    ~800 words maximum
  Days 1-2:                     ~400-500 words (comfort phase)
  Days 3-4:                     ~600-800 words (disruption, peak density)
  Days 5-6:                     ~500-700 words (understanding, personal)
  Day 7:                        ~400-600 words (resolution — pull back)

COOLDOWN RULE:
  After any reveal fires (R1-R8), suppress all non-essential
  narrative triggers for the remainder of that in-game period
  (morning/afternoon/evening). Let the reveal breathe.

STACKING PROHIBITION:
  No more than 2 narrative triggers within 60 seconds of
  real-time gameplay. If a third would fire, queue it for
  the next period or next interaction.
```

**Day 7 pull-back:** The final day should feel quieter than Days 3-4 even though R8 fires. The store is winding down. The player already has the pieces. Let them assemble meaning from silence.

#### 4. When Ambiguity Is Preferred Over Clarity

These elements must **never** be fully explained. Partial understanding is the design intent.

| Element | What the Player Can Know | What Must Stay Ambiguous |
|---|---|---|
| **The entity** | It exists. It moves things. It responds to the store's state. | What it is. Why it's here. Whether it's malevolent. |
| **The Elder's past** | He's been here a long time. He knows more than he says. Something happened. | What exactly happened. Whether he caused it. Whether he's protecting the player or himself. |
| **The store's nature** | It has patterns. Objects have preferred positions. The collection matters. | Whether the store is alive. Whether the patterns are natural. What "preservation" actually preserves. |
| **The borrowing** | Objects leave their slots. They return changed or don't return. | Where they go. What uses them. Whether "borrowing" is the right word. |
| **The endings** | What the player preserved or lost. The state of the store. | Whether it was the "right" choice. Whether the entity is satisfied. What happens after. |
| **Lore fragments** | Individual facts about the store's history, past employees, incidents. | The complete picture. No single playthrough reveals everything. 25% of lore is hidden-tier. |
| **NPC knowledge** | Each NPC has a piece. The Elder has the most. | No NPC has the full truth. The player cannot assemble certainty even with all lines heard. |

**The ambiguity hierarchy:**

1. **Entity** — most ambiguous. Zero explanation. Ever.
2. **Store's nature** — highly ambiguous. Lore fragments suggest, never state.
3. **Elder's past** — moderately ambiguous. R7 and R8 reveal the most, but gaps remain.
4. **Endings** — mildly ambiguous. The outcome is clear; the meaning is the player's.

**Writing test for ambiguity:** If a line answers a question the player hasn't asked yet, delete it. If a line closes a door the player should walk through themselves, delete it. If a line makes the player feel smart for understanding, it's probably too clear — make it 20% less direct.

#### Silence Quick Reference

```
WHEN IN DOUBT: DON'T WRITE IT.

  Entity on screen?           → Zero words.
  Object displaced?           → Zero words. The slot is empty. That's enough.
  Player already scared?      → Zero words. You'll make it worse (in the bad way).
  NPC leaving?                → Zero words. Door closes.
  Mid-puzzle?                 → Zero words. UI only.
  First 30 sec of day?        → Zero words. Let them look.
  After a reveal?             → Cooldown. Suppress non-essential triggers.
  Could the player figure
    it out without this line? → Delete the line.

  MAX PER DAY:  ~800 words. Days 1-2 and 7 are lighter.
  MAX STACKING: 2 triggers per 60 seconds real-time.
  AMBIGUITY:    Entity > Store > Elder > Endings (most to least)

  The scariest thing in the store is what the player doesn't know.
  Don't take that away from them.
```

---

### Player Character Identity Anchor

*Internal reference only. Not player-facing content. Used for writing consistency across all narrative elements.*

#### Relationship to Black Pines: New Arrival

The player has never been to Black Pines before. They arrived three days before Day 1. They know no one in town. The bookstore job was found through a handwritten index card on a community board at a Portland used bookshop — no online listing, no agency, no referral. They applied by mail. The Elder responded within two days.

#### Why They Took the Job

They needed distance from something they won't name. Not fleeing danger — fleeing stagnation. A quiet job in a small town felt like permission to stop performing competence for a while. The bookstore paid enough, included the upstairs apartment, and asked for no references.

#### How Long in Town

Three days before gameplay begins. Long enough to unpack, walk the main street twice, and buy groceries. Not long enough to learn anyone's name besides the Elder's.

#### What They Expected

A slow, boring job. Shelving books, running a register, sweeping floors. A place where nothing happens. They were correct about the job description and wrong about everything else.

#### Private Internal Trait (Never Spoken)

They are precise because they are afraid of forgetting. The notebook isn't just a game mechanic — it is how they have always processed the world. They write things down because they do not trust their own memory. This is never stated, never referenced in dialogue, never revealed. It simply shapes *how* they observe: carefully, specifically, as if recording evidence against future doubt.

#### Player–Elder Tension

The Elder gives instructions but withholds reasons. The player follows directions but resents the gaps. This tension is never confrontational — the player needs the job, the Elder needs the help — but it surfaces in what the Elder *doesn't* say and what the player *doesn't* ask. The player notices the evasions. They catalog them. They do not push. Yet.

#### Why the Elder Trusts Them

The Elder chose someone with no ties to Black Pines, no prior knowledge of the store's history, and no reason to dig. Someone who would follow the routine because the routine is all they have. The Elder saw the precision in the handwritten application letter — the careful penmanship, the specific inventory experience, the absence of personal detail — and recognized someone who would maintain order without asking why it mattered. What the Elder underestimated is that precision cuts both ways: the same trait that makes them a good caretaker makes them a good investigator.

```
IDENTITY QUICK REFERENCE

  Relation to town:     New arrival. 3 days. Knows nobody.
  Job reason:           Needed distance and quiet. No deeper motive.
  Expectation:          Boring shelving job. Wrong.
  Hidden trait:         Writes things down because they don't trust memory.
  Tension with Elder:   Follows instructions, catalogs the evasions.
  Elder's trust basis:  Precise, rootless, asks no questions. (Miscalculation.)

  Voice consequence:    Short observations. Flat tone. Records, doesn't interpret.
  Silence consequence:  Doesn't ask when they should. Writes instead of speaks.
  Ambiguity consequence: Assembles evidence but never reaches certainty.
```

---

### Elder Identity Record

*Internal reference only. Not player-facing content. Used for record writing, dialogue tone, and lore fragment consistency.*

#### Legal Name

**Harold Edward Calloway**

Goes by "Mr. Calloway" to customers. The player never learns his first name through dialogue — only through lore fragments (Personal Records category) and the letter found in the apartment safe (P10/R7). Regulars in town call him "Calloway" or "old Calloway." He has never invited the player to use his first name.

#### Biographical Timeline

| Detail | Value | Notes |
|---|---|---|
| Born | 1917 | Makes him 79-80 during gameplay (mid-to-late 1990s). Consistent with "70s or 80s" character profile. |
| Arrived in Black Pines | 1951 | Age 34. Came from Portland after the war. 45+ years in town by gameplay. |
| Opened Black Pines Books & Cafe | 1953 | Two years settling in before opening. The building existed before him — formerly a general store. |
| First entity cycle survived | Winter 1961-62 | Age 44. Eight years into ownership. This is the cycle referenced in R5. |
| Second known cycle | Winter 1978-79 | Age 61. Managed it alone. Lost his assistant (Margaret "Maggie" Hewitt) (the "predecessor" referenced in Personal Records lore). |
| Retired from daily operations | 1991 | Age 74. Hired a series of short-term caretakers. None lasted more than a season. |
| Hired the player | 1996 or 1997 | Placed the index card in Portland deliberately. Chose the player from the application letter. |

**Key constraint:** The Elder has survived **two** previous entity cycles. R5 reveals he survived "a previous cycle" (singular). The second cycle and the lost assistant (Margaret "Maggie" Hewitt) are discoverable only through hidden-tier lore fragments. This supports the ambiguity hierarchy — the player can learn the surface truth (R5) without ever learning the full scope.

#### Hidden Regret

He did not warn his assistant (Margaret "Maggie" Hewitt) in 1978. He believed — as he believes now — that the truth must be discovered through observation, not told. His assistant did not discover it in time. The regret is not that he kept the secret. The regret is that his method failed and someone paid for it. He has not changed his method. He has only chosen more carefully.

This regret is **never stated directly**. It surfaces in:
- The way he pauses before answering questions about previous employees (Elder Days 4-5 lines)
- The em dash interruptions in his late-game dialogue — moments where he starts to say something and redirects
- R7's letter, which explains the hiring logic but not the guilt behind it
- One hidden-tier Personal Records fragment: a 1979 insurance claim listing an employee as "no longer reachable"

#### Secret He Intended to Reveal

He planned to tell the player about the entity on **Day 4**. Not the full history — just enough: that the store has a pattern, that the disruptions are not random, that the night shift matters more than it seems. A controlled disclosure. Enough to prepare them without overwhelming them.

He rehearsed it. He had the words. He has had the words for thirty years.

#### Why He Delayed

Three reasons, layered:

1. **Practical:** Each previous time he told someone directly, they either didn't believe him or they panicked. Observation works better than instruction. He knows this from experience. He is wrong about this, but he has evidence to support it.

2. **Selfish:** If the player discovers the truth through the store itself, they're more likely to stay. Someone told the truth walks out. Someone who finds it feels ownership over it. He needs them to stay.

3. **Guilty:** Saying it aloud makes it real in a way that silence doesn't. Every day he delays is another day he can pretend this time might be different — that maybe the cycle won't come, the entity won't wake, the pattern has finally broken. He knows better. He delays anyway.

The delay is **never explained to the player**. R5 reveals he knew. R7 reveals he chose them. R8 reveals the full history. But the *why* of the silence is the Elder's private territory — the player can infer it, but the game never confirms it. This aligns with the ambiguity hierarchy: Elder's past is "moderately ambiguous."

---

#### Historical Record Signatures

These are the formats Harold Calloway uses in store records, ledgers, and correspondence. Use these as templates when writing lore fragments in the Personal Records and Bookstore History categories.

**Signature 1 — Formal store correspondence:**
```
H. E. Calloway
Proprietor, Black Pines Books & Cafe
Black Pines, WA
```

**Signature 2 — Internal ledger notation (pre-1991):**
```
— H.E.C., 14 Nov 1978
```

**Signature 3 — Late personal notes (post-retirement, shaky hand implied):**
```
— Harold, March '96
```

*Writing note:* Signature 1 appears on anything a customer or supplier might see. Signature 2 appears on inventory records, incident logs, and maintenance notes — the working documents of someone who ran the store alone. Signature 3 appears only in private writings found in the apartment safe or hidden in the office. The shift from initials to first name signals documents written for himself, not the store.

#### Ledger-Style References

Use these formats for Bookstore History and Previous Incidents lore fragments that take the form of store records.

**Ledger Reference 1 — Inventory discrepancy log:**
```
INVENTORY — SECTION C (REGIONAL HISTORY), SHELF 3
Date: 6 Feb 1979
Discrepancy: 3 volumes unaccounted. Titles: "Logging Roads of
  the Olympic Peninsula" (1st ed.), "Pacific Northwest Folk
  Traditions" (Harmon, 1964), "The Mill Towns" (unsigned copy).
Status: Not found in returns, storage, or misfiled sections.
Note: Third incident this month. Pattern consistent.
— H.E.C., 6 Feb 1979
```

**Ledger Reference 2 — Maintenance record:**
```
MAINTENANCE LOG — BLACK PINES BOOKS & CAFE
Date: 22 Dec 1961
Issue: Basement light fixture (NE corner) failed. Bulb intact.
  Wiring undamaged. No visible cause. Replaced fixture assembly.
  Operational as of 14:00. Failed again by closing.
Action: Installed secondary fixture. Both operational at lockup.
Follow-up: Check morning.
— H.E.C., 22 Dec 1961
```

*Writing note:* Calloway's ledger entries are precise, factual, and devoid of speculation — the same voice the player uses, filtered through forty years of practice. The entry says what happened and what was done. It never says what it means. The reader connects the dots. This is where the player should recognize a version of themselves in the Elder's younger handwriting.

#### Archived Note Format

For private documents — the kind found in the apartment safe (P10), hidden desk compartments, or tucked into book margins. These are not store records. These are the Elder writing to himself.

**Archived note — unsent letter format:**
```
March 14, 1996

I have placed the card in Portland. The Hawthorne shop,
community board, left side. Handwritten so it looks like
every other card up there. Someone precise will notice
the specifics. Someone careless won't read past the
first line.

I will know by the application.

I am running out of seasons to be wrong about this.

— Harold
```

*Writing note:* Private notes use full sentences but short paragraphs. No headers, no dates in the margin — just the date at top and the name at bottom. The tone is confessional but controlled. He writes the way he speaks: deliberately, with weight underneath. Em dashes appear here too, but less often than in speech — on paper he has time to finish his thoughts. The final line of any private note should land like a held breath.

```
ELDER IDENTITY QUICK REFERENCE

  Full name:            Harold Edward Calloway
  Born:                 1917. Age 79-80 during gameplay.
  In Black Pines since: 1951 (45+ years).
  Store opened:         1953.
  Cycles survived:      2 (1961-62, 1978-79). R5 reveals one. Second is hidden lore.
  Retired:              1991. Hired short-term caretakers. None lasted.
  Hidden regret:        Didn't warn the '78 assistant (Margaret "Maggie" Hewitt). Same method. Better selection.
  Intended reveal:      Day 4, controlled disclosure. Never delivered it.
  Delay reasons:        Observation > instruction, needs them to stay, guilt.

  Signature evolution:  H. E. Calloway → H.E.C. → Harold
  Formal = store business. Initials = working records. First name = private.

  Voice consequence:    Chooses words like someone who knows words matter here.
  Silence consequence:  Rehearses what to say. Doesn't say it. Repeats.
  Ambiguity consequence: Player learns what he did. Never fully learns why.
```

---

## Private Historical Character References

> **Writer reference only.** These characters never appear as NPCs, have no dialogue trees, and are not directly encountered by the player. They exist to ensure consistency and emotional authenticity when their names, habits, or traces surface in historical records and lore fragments.

### 1978 Store Assistant

```
  Full name:            Margaret "Maggie" Hewitt
  Role:                 Store Assistant, Black Pines Books & Cafe
  Active years:         1976–1979
  Age during employment: 22–25
  Relationship to Elder: Hired through a Portland classifieds ad. Harold saw
                         the same precision in her application that he later
                         recognized in the player's. She was the first person
                         he trusted with closing shifts.
  Defining work habit:  Annotated the margins of inventory logs with small
                         pencil sketches — a book spine here, a shelf bracket
                         there. Her ledger pages are identifiable at a glance.
  Personal trait:       Kept a postcard collection pinned above the staff desk.
                         All Pacific Northwest — lighthouses, coastal roads,
                         small-town main streets. Never sent any of them.
  Quiet fear:           That she was not observant enough. She once told Harold
                         she worried she missed things other people noticed.
                         He told her that was unlikely. He was wrong.
  Historical detail:    A 1979 insurance claim lists her as "no longer
                         reachable at last known address." Harold filed it
                         three months after the cycle ended. The claim was
                         denied for insufficient documentation.
```

*Writing note:* Maggie's traces appear only in Personal Records fragments (W36) — pencil sketches in old ledgers, a postcard still pinned to the office corkboard, her name on a faded schedule. Never describe what happened to her directly. The records stop. That is the story.
