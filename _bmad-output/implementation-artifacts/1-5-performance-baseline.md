# Story 1.5: Performance Baseline

Status: done

## Story

As a developer,
I want to load the bookstore scene with acceptable performance,
so that the technical baseline is validated.

## Acceptance Criteria

1. Given the bookstore scene with all base assets loaded, When profiled on target hardware (GTX 1060, i5, 8 GB RAM), Then frame rate is stable at 60 FPS or above
2. Given the scene loads from the main menu, When the load completes, Then time from trigger to player control is under 5 seconds
3. Given the player moves through all accessible areas, When profiled, Then no frame drops below 30 FPS occur

## Tasks / Subtasks

- [x] Task 1: Create DevLog.cs static logging class (AC: 1, 2, 3)
  - [x] 1.1 Create `Assets/Scripts/Core/DevLog.cs` per architecture spec — static class with `Log(LogCategory, string)` and `Warn(LogCategory, string)` methods
  - [x] 1.2 Both methods use `[System.Diagnostics.Conditional("UNITY_EDITOR"), System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]` attributes — zero cost in release builds
  - [x] 1.3 Add `ActiveCategories` static field (default `LogCategory.All`) with bitwise category filtering: `if ((ActiveCategories & cat) == 0) return;`
  - [x] 1.4 Format: `UnityEngine.Debug.Log($"[{cat}] {message}")` for Log, `UnityEngine.Debug.LogWarning(...)` for Warn
  - [x] 1.5 Create `Assets/Scripts/Core/GameEnums.cs` update — add `LogCategory` flags enum if not already present (check existing file first): `None=0, Cycle=1<<0, Environment=1<<1, Entity=1<<2, Notebook=1<<3, Puzzle=1<<4, Save=1<<5, Ending=1<<6, Narrative=1<<7, Performance=1<<8, All=~0`
- [x] Task 2: Create lightweight FPS display for dev builds (AC: 1, 3)
  - [x] 2.1 Create `Assets/Scripts/Debug/FPSDisplay.cs` MonoBehaviour — uses `[Conditional]` or `#if UNITY_EDITOR || DEVELOPMENT_BUILD` to strip from release
  - [x] 2.2 Calculate FPS using `1.0f / Time.unscaledDeltaTime` with a smoothing factor (rolling average over ~0.5s)
  - [x] 2.3 Track `minFPS` and `maxFPS` since scene load (resettable)
  - [x] 2.4 Display on-screen via `OnGUI()`: current FPS, min, max — small text in top-left corner, color-coded (green >=60, yellow >=30, red <30)
  - [x] 2.5 Toggle visibility with F2 key (default: visible in editor, hidden in builds)
  - [x] 2.6 Log FPS summary via DevLog every 10 seconds: `DevLog.Log(LogCategory.Performance, $"FPS: avg={avg:F0} min={min:F0} max={max:F0}")`
- [x] Task 3: Add scene load time measurement (AC: 2)
  - [x] 3.1 In `FPSDisplay.cs` (or a separate lightweight component), measure time from `Awake()` to first `Update()` frame using `Time.realtimeSinceStartup`
  - [x] 3.2 Log load time via DevLog: `DevLog.Log(LogCategory.Performance, $"Scene ready: {elapsed:F2}s")`
  - [x] 3.3 Display load time on-screen for 5 seconds after scene load, then fade/hide
- [x] Task 4: Add performance baseline constants to GameConstants.cs (AC: 1, 2, 3)
  - [x] 4.1 Add `TargetFPS = 60` (int) — preferred frame rate target
  - [x] 4.2 Add `MinimumFPS = 30` (int) — absolute minimum acceptable
  - [x] 4.3 Add `MaxLoadTimeSeconds = 5.0f` — maximum scene load time
  - [x] 4.4 Add `FPSLogIntervalSeconds = 10.0f` — interval for periodic FPS logging
- [x] Task 5: Add FPSDisplay to Bookstore scene (AC: 1, 2, 3)
  - [x] 5.1 Add an empty GameObject "PerformanceMonitor" to Bookstore.unity at root level
  - [x] 5.2 Attach FPSDisplay component to it — **User must do this in Unity Editor** (script GUID not available until Unity imports the new .cs file)
  - [x] 5.3 Ensure it does NOT have static flags (it's a runtime debug tool)
- [x] Task 6: Manual testing verification (AC: 1, 2, 3) — **Requires Unity Editor; to be verified by user**
  - [x] 6.1 Enter Play mode — verify FPS counter appears in top-left corner
  - [x] 6.2 Check Console for scene load time log — verify under 5 seconds (AC2)
  - [x] 6.3 Walk through all areas — verify FPS stays green (>=60) or at minimum yellow (>=30) (AC1, AC3)
  - [x] 6.4 Check Console for periodic FPS logs every 10 seconds
  - [x] 6.5 Press F2 — verify FPS display toggles on/off
  - [x] 6.6 Alternatively: use Unity's Game view **Stats** button (top-right) to cross-check FPS
  - [x] 6.7 Alternatively: use **Window > Analysis > Profiler** for detailed frame analysis

## Dev Notes
- User manual testing concluded average of 680-800 FPS on target hardware with current scene setup (5 Mixed lights, baked lightmaps, ~20 objects). Load time from main menu to player control averaged .18 seconds. No frame drops below 30 FPS observed during walkthrough. PerformanceMonitor and FPSDisplay functioned as expected.
### Architecture Compliance

- **Architecture explicitly states: "No performance profiling. Unity Profiler already exists. Don't reimplement it."** — This story creates a LIGHTWEIGHT FPS display, NOT a profiler. The display is a convenience overlay; actual profiling uses Unity's built-in Profiler.
- **File:** `Assets/Scripts/Core/DevLog.cs` — Architecture-mandated static logging class with `[Conditional]` attributes for zero-cost release builds. Required by all future stories.
- **File:** `Assets/Scripts/Debug/FPSDisplay.cs` — Lightweight MonoBehaviour in the Debug/ folder (architecture: "Entire folder stripped from release builds"). Uses `#if` preprocessor directives.
- **File:** `Assets/Scripts/Core/GameConstants.cs` — Add performance baseline constants.
- **File:** `Assets/Scenes/Bookstore.unity` — Add PerformanceMonitor GameObject.
- **No new packages or dependencies** — uses only UnityEngine built-ins.

### Previous Story (1.4) Learnings

From Stories 1.1-1.4:
- **All tunable values go in GameConstants** — performance thresholds follow this pattern
- **Manual testing tasks stay unchecked** — dev agent cannot run Unity Editor; user verifies FPS
- **Scene YAML edits work** — adding a GameObject to the scene via YAML is proven reliable
- **User noted in 1.4 they didn't know how to verify FPS** — this story provides both an in-game counter AND documents Unity's built-in Stats/Profiler options
- **`[Conditional]` compilation pattern** is architecture-approved for zero-cost debug code

### Existing Scene State

**Bookstore.unity** (after Story 1.4):
- 1 Directional Light (Mixed, 0.4 intensity), 4 Ceiling Point Lights (Mixed, 1.2 intensity, range 7), 1 Desk Lamp (Mixed, 0.6 intensity)
- Baked lightmaps enabled (40 texels/unit), warm amber ambient fill
- Ground (20x15m), 4 Walls, 3 Shelves, Counter — all with ContributeGI static flags
- Player, Books, Vase, Crate_Decor — dynamic objects
- Scene root order 0-19 (next available: 20)
- Current light budget: 5 Mixed lights + baked indirect — well within performance target

### Existing Code State

- `GameConstants.cs` — Player movement, object highlighting, and lighting constants
- `GameEvents.cs` — Full static event bus with invoke helpers
- `GameEnums.cs` — Game enums (GamePhase, EntityState, ObjectType, etc.) — check if LogCategory already exists before adding
- `PlayerController.cs` — WASD + mouse look, raycasting, E key interaction
- `InteractableObject.cs` — Highlight/unhighlight with emission-based glow
- **DevLog.cs does NOT exist yet** — must be created per architecture spec
- **Debug/ folder does NOT exist yet** — must be created

### GDD Context

Per GDD (Performance Requirements): "60 FPS preferred, 30 FPS minimum acceptable. Performance priority: stable frame rate over visual fidelity."

Per GDD (Technical KPIs): "Frame pacing: Stable pacing more important than maximum frame rate."

Per GDD (Load Times): "Core bookstore spaces should feel seamless... under 5 seconds where possible."

Per GDD (Target Hardware): GTX 1060, i5, 8GB RAM at 1080p.

### What This Story Does NOT Include

- No full Debug Overlay system (F1 key, multi-panel — that's future work)
- No DebugEventLogger.cs (subscribes to GameEvents — not needed yet, no events firing)
- No profiling tools beyond FPS counter — architecture says use Unity Profiler
- No optimization work — this story MEASURES, doesn't fix (optimization is Epic 15)
- No build pipeline or release configuration
- No automated performance testing framework

### Cross-Story Context

- **Story 1.6 (Basic UI)** will add HUD elements — FPS display uses `OnGUI()` which won't conflict with future UI Canvas
- **Story 1.8 (Flashlight)** will add another dynamic light — FPS counter will help validate it stays within budget
- **Epic 15 (Polish & Performance)** will use the baseline established here for optimization targets
- **DevLog.cs** created here will be used by ALL future stories for debug logging

### Technical Notes

- **`[Conditional]` attribute**: Applied to methods, causes the compiler to remove ALL call sites in builds where the symbol is not defined. Different from `#if` which only strips the method body. Use `[Conditional]` on DevLog methods; use `#if` for the FPSDisplay class body.
- **`OnGUI()` for FPS display**: Simplest approach. No Canvas/UI overhead. Adequate for a debug display. The architecture's full DebugOverlay will use this same approach.
- **`Time.unscaledDeltaTime`**: Not affected by `Time.timeScale`, gives accurate frame time measurement even if game uses slow-motion.
- **Rolling average**: Smooth FPS display by accumulating frames over 0.5s intervals rather than showing per-frame jitter.
- **Scene load measurement**: `Time.realtimeSinceStartup` in `Awake()` vs first `Update()` gives wall-clock time for scene initialization. This includes all `Awake()`/`Start()` calls and first render.

### Performance Expectations

Current scene with 5 Mixed lights, ~20 objects, and baked lightmaps should easily hit 60 FPS on target hardware. This story establishes the measurement infrastructure and baseline numbers — any performance issues discovered will be noted but NOT fixed here (that's Epic 15's job).

### Project Structure Notes

- New directory: `Assets/Scripts/Debug/` — all contents stripped from release builds
- New files: `DevLog.cs` (Core/), `FPSDisplay.cs` (Debug/)
- Modified: `GameConstants.cs` (new constants), `GameEnums.cs` (LogCategory if missing), `Bookstore.unity` (new GameObject)

### References

- [Source: system-architecture.md#Debug Logging Framework] — DevLog.cs spec, LogCategory enum, format standard
- [Source: system-architecture.md#Conditional Compilation Pattern] — `[Conditional]` attribute usage
- [Source: system-architecture.md#Performance Philosophy] — "No performance profiling. Unity Profiler already exists."
- [Source: system-architecture.md#Debug Overlay] — Future F1 overlay design (not this story)
- [Source: gdd.md#Performance Requirements] — 60 FPS preferred, 30 FPS minimum
- [Source: gdd.md#Target Hardware Specification] — GTX 1060, i5, 8GB RAM
- [Source: gdd.md#Technical KPIs] — Frame pacing priority
- [Source: gdd.md#Load Times] — Under 5 seconds for core spaces
- [Source: epics.md#Epic 1, Story 1.5] — AC definitions
- [Source: epics.md#Epic 15] — Future optimization work depends on this baseline

## Dev Agent Record

### Agent Model Used

Claude Opus 4.6

### Debug Log References

None — no runtime errors; all changes are new file creation, code edits, and scene YAML edits.

### Completion Notes List

- Task 1: Created `DevLog.cs` static logging class with `[Conditional]` attributes for zero-cost release builds. Added `Performance = 1 << 8` to existing `LogCategory` enum in `GameEnums.cs`.
- Task 2: Created `FPSDisplay.cs` in new `Debug/` folder. Wrapped entirely in `#if UNITY_EDITOR || DEVELOPMENT_BUILD`. Rolling average FPS over ~0.5s window, min/max tracking, color-coded OnGUI display, F2 toggle (visible in editor by default, hidden in builds).
- Task 3: Integrated load time measurement into `FPSDisplay.cs` — measures `Time.realtimeSinceStartup` from `Awake()` to first `Update()`, logs via DevLog, displays for 5 seconds then hides.
- Task 4: Added 4 performance baseline constants to `GameConstants.cs`: `TargetFPS=60`, `MinimumFPS=30`, `MaxLoadTimeSeconds=5.0f`, `FPSLogIntervalSeconds=10.0f`.
- Task 5: Added `PerformanceMonitor` empty GameObject to `Bookstore.unity` at root order 20 with no static flags. **Note:** FPSDisplay component must be attached by user in Unity Editor — cannot reference script GUID via YAML until Unity imports the new .cs file and generates its .meta.
- Task 6: Manual testing — requires user to: refresh assets (Ctrl+R), attach FPSDisplay to PerformanceMonitor, enter Play mode, and verify FPS counter/logs.

### Change Log

- 2026-03-28: Completed all dev-automatable tasks (1-5). Created DevLog.cs, FPSDisplay.cs, added Performance to LogCategory, added performance constants, added PerformanceMonitor to scene.
- 2026-03-28: Code review — 0 High, 1 Medium (FPS rolling average inflated during buffer warmup, maxFPS locked to ~1800), 1 Low (GUIStyle allocated every OnGUI call). Both fixed: added frameSamples counter to defer min/max until buffer fills; cached GUIStyle. All ACs validated, all tasks confirmed. Approved.

### File List

- Assets/Scripts/Core/DevLog.cs (created — static logging class with [Conditional] attributes)
- Assets/Scripts/Debug/FPSDisplay.cs (created — lightweight FPS counter, load timer, OnGUI display)
- Assets/Scripts/Core/GameEnums.cs (modified — added Performance = 1 << 8 to LogCategory)
- Assets/Scripts/Core/GameConstants.cs (modified — added Performance Baseline constants section)
- Assets/Scenes/Bookstore.unity (modified — added PerformanceMonitor GameObject at root order 20)
