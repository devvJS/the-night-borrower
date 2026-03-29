<p align="center">
  <img src="https://img.shields.io/badge/Unity-2022.3%20LTS-000000?style=for-the-badge&logo=unity&logoColor=white" alt="Unity 2022.3 LTS" />
  <img src="https://img.shields.io/badge/C%23-.NET%20Standard%202.1-239120?style=for-the-badge&logo=csharp&logoColor=white" alt="C#" />
  <img src="https://img.shields.io/badge/Platform-Windows-0078D6?style=for-the-badge&logo=windows&logoColor=white" alt="Windows" />
  <img src="https://img.shields.io/badge/Target-Steam-1b2838?style=for-the-badge&logo=steam&logoColor=white" alt="Steam" />
</p>

# The Night Borrower

**A first-person psychological survival horror game where routine becomes ritual and memory is your only weapon.**

You run a quiet small-town bookstore by day. By night, something moves through the shelves — rewriting records, displacing objects, feeding on memory itself. Survival depends not on combat or reflexes, but on observation, pattern recognition, and environmental awareness.

Set in **Black Pines, Washington** — a fictional, isolated Pacific Northwest logging town in the mid-to-late 1990s — the game builds genuine comfort through daily routines before unraveling that comfort through subtle, personal disruption.

---

## About the Project

The Night Borrower is a solo-developed indie horror game built in **Unity 2022.3 LTS** with **C#**. It explores a different kind of fear — not jump scares or combat, but the creeping realization that something familiar has changed and you can't explain why.

### Core Design Pillars

| Pillar | Philosophy |
|--------|------------|
| **Knowledge Is Survival** | Observation, recording, and pattern recognition determine whether you endure |
| **Routine Becomes Ritual** | Daily bookstore tasks transform into protective rituals with real mechanical weight |
| **Small Changes Create Fear** | Horror emerges from disrupted familiarity, not monsters in the dark |
| **Light Creates Safety** | Maintained lighting defines safe zones — darkness introduces uncertainty and threat |

### Core Gameplay Loop

```
Day Phase  -->  Stock shelves, serve customers, maintain lighting, record observations
                              |
Transition -->  Subtle environmental changes signal the approaching night
                              |
Night Phase --> Observe, avoid, preserve — the entity manifests and feeds on memory
                              |
Recovery   -->  Assess damage, restore order, prepare for the next cycle
```

---

## Tech Stack

| Component | Technology |
|-----------|-----------|
| **Engine** | Unity 2022.3.0f1 LTS |
| **Language** | C# (.NET Standard 2.1) |
| **Input** | Unity New Input System (inline InputAction definitions) |
| **Rendering** | Built-in Render Pipeline with programmatic lighting |
| **Serialization** | JsonUtility for save/load persistence |
| **Target Platform** | Windows (Steam) |

---

## Development Approach

### AI-Assisted Development with Claude

This project is developed using **Claude Code** (Anthropic's CLI agent) as a collaborative development partner. Claude assists with:

- **Implementation** — writing C# scripts following established architectural patterns
- **Code Review** — adversarial review process catching resource leaks, architectural violations, and missed acceptance criteria
- **Story Planning** — creating comprehensive developer story files with guardrails and context

The human developer handles all Unity Editor work (scene composition, component attachment, visual tuning, playtesting) and makes all design decisions. Claude validates structure; human eyes validate experience.

### BMad Method

Project planning and workflow management uses the **[BMad Method](https://github.com/bmadcode/BMAD-METHOD)** — a structured agile framework designed for AI-assisted solo development. It provides:

- **Epic and story tracking** with sprint status management
- **Workflow automation** for story creation, implementation, code review, and retrospectives
- **Quality gates** ensuring acceptance criteria are met before stories are marked complete
- **Iterative improvement** through retrospectives that feed learnings into subsequent work

The BMad artifacts live in `_bmad/` (configuration) and `_bmad-output/` (generated planning and implementation documents).

---

## Getting Started

### Prerequisites

- **Unity Hub** with **Unity 2022.3.0f1 LTS** installed
- **Git**

### Clone and Open

```bash
git clone git@github.com:devvJS/the-night-borrower.git
cd the-night-borrower
```

1. Open **Unity Hub**
2. Click **Open** and select the cloned `the-night-borrower` directory
3. Unity will import assets and compile scripts
4. Open `Assets/Scenes/Bookstore.unity` to load the main scene
5. Press **Play** to enter the bookstore

### Controls

| Key | Action |
|-----|--------|
| **WASD** | Move |
| **Mouse** | Look |
| **Shift** | Sprint |
| **E** | Interact with highlighted objects |
| **F** | Toggle flashlight |
| **F5** | Quick save (dev) |
| **F9** | Quick load (dev) |

---

## Project Structure

```
Assets/
  Scripts/
    Core/           # GameConstants, GameEvents, GameEnums, DevLog
    Data/           # Serializable data classes (SaveData)
    Environment/    # InteractableObject, future LightFixture/EnvironmentManager
    Managers/       # SaveManager, future game state managers
    Player/         # PlayerController, PlayerHUD, PlayerFlashlight, ObservationSystem
  Scenes/
    Bookstore.unity # Main development scene
```

---

## Current Status

**Phase 1 — Foundation** | Epic 2 of 3 in progress

The project follows a phased development plan across 16 epics:

| Phase | Focus | Status |
|-------|-------|--------|
| **Phase 1** | Foundation (player, interaction, observation, notebook) | In Progress |
| **Phase 2** | Core Loop (day/night cycle, persistence, entity) | Backlog |
| **Phase 3** | Feature Expansion (puzzles, areas, atmosphere, NPCs) | Backlog |
| **Phase 4** | Narrative Integration (story, endings) | Backlog |
| **Phase 5** | Finalization (UI, polish, optimization, release) | Backlog |

---

## License

All rights reserved. This repository is public for portfolio and educational purposes. The source code, design documents, and game assets are not licensed for reuse, modification, or distribution without explicit permission.

---

<p align="center">
  <i>Fear begins not when something attacks — but when something familiar changes and you can't explain why.</i>
</p>
