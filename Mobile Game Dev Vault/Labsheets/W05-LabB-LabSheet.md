# Week 5, Lab B (2h): Greybox lane and async loading that never hangs

**Module:** Mobile Game Development (A12581) · **Engine:** Unity 6.6 · **Platform:** Android (sideload) · **Date:** Wed 7 Oct 2026
**Feeds:** CA2 Playable Vertical Slice (due Sun 15 Nov 2026, 23:59) · **LOs:** LO1, LO3

> **Goal:** Greybox one complete content lane that plays start to finish, load it through an `Awaitable` scene loader with a loading screen that shows real progress and never hangs, and put your first load-time numbers in the Baseline Sheet.

## What you will complete today
1. One lane in greybox: start, fail and win reachable in 2 to 3 minutes, feedback hooks on the core verb.
2. A persistent `SceneLoader` with a Loading canvas and a Slider that tracks `AsyncOperation.progress`.
3. Proof that you understand the `allowSceneActivation` trap (you will make it hang, then fix it).
4. Three cold-start timings, time to interactive and a scene-load time in `docs/CA2/baseline/Performance_Baseline.md`.
5. A two-minute neighbour playtest with two confusions noted; tag `v0.3.1-lane`.

## Pre-flight
- Lab A checkpoints: scene flow, `WaveTimer`, `EnemyPool`, tag `v0.3.0-skeleton`.
- ProBuilder installed if you want it (**Window > Package Manager > Unity Registry > ProBuilder**); primitives are fine.
- Phone plugged in, release pipeline working.
- `docs/CA2/baseline/Performance_Baseline.md` open.

## Part A: Greybox the lane (~30 min)
Greybox means real dimensions, real timings, real camera, flat colours, no art. Every verb works; nothing is faked.

1. Decide what "one lane" means for your project and write it at the top of the journal entry:
   - Arena-Survivor: one arena, one wave set (3 to 5 waves), one boss or timer end.
   - Endless Runner: one biome with 4 to 6 chunk types and one difficulty ramp.
   - Turn-Based Tactics: one map, two unit types each side, one win condition.
   - Idle/Crafting: one crafting chain of three steps with one timer.
   - Action Platformer: one segment set (5 to 8 segments) around the single verb.
2. Block it out with primitives or ProBuilder in `Game`. Use a single flat material per gameplay role (floor, hazard, pickup, enemy) so the roles read at a glance.
3. Hook the states: reaching the end sets `GameManager` to `Won`; dying or timing out sets `Lost`. Both lead to `Result`.
4. Feedback hooks on the core verb: score change, sound stub (a single click is enough), hit flash or camera shake. These are the "feedback loop" half of core loop to feedback loop.
5. Play it once on the phone. Time it: a full attempt should be 2 to 3 minutes.

*Expected result: a stranger can start, understand the verb, fail or win, and see the Result screen, without you explaining anything.*

## Part B: Loading scene and SceneLoader (~25 min)
1. In `Boot`, add a `LoadingCanvas` (Screen Space Overlay, sort order high) with a full-screen dark `Image` and a **Slider** (interactable off, handle removed). Keep it as a child of the persistent `App` object so it survives scene changes.
2. Add `SceneLoader.cs` to `App`:
   ```csharp
   using System.Threading; using UnityEngine;
   using UnityEngine.SceneManagement; using UnityEngine.UI;
   public class SceneLoader : MonoBehaviour   // persistent, see Awake
   {
       [SerializeField] Slider bar;
       [SerializeField] GameObject loadingScreen;
       void Awake() => DontDestroyOnLoad(gameObject);
       public async Awaitable Load(string scene, CancellationToken ct)
       {
           loadingScreen.SetActive(true);
           var op = SceneManager.LoadSceneAsync(scene);
           op.allowSceneActivation = false;  // progress now stops at 0.9
           while (op.progress < 0.9f)        // NOT while (!op.isDone)
           {
               bar.value = op.progress / 0.9f;
               await Awaitable.NextFrameAsync(ct);
           }
           op.allowSceneActivation = true;
           await op;                         // completes on activation
           loadingScreen.SetActive(false);
       }
   }
   ```
3. Find the loader from anywhere without `Find`: a static `public static SceneLoader Instance` set in `Awake` is acceptable for this module (one persistent object), or pass the reference from `Bootstrap`.
4. Menu Play button and Result Retry button call:
   ```csharp
   _ = SceneLoader.Instance.Load("Game", Application.exitCancellationToken);
   ```
   `Bootstrap.Start` should also go through `Load("Menu", ...)` so the first load uses the same path.
5. Optional but recommended: while the loader holds at 0.9, prewarm your pools (Lab A) so the first frame of `Game` does no `Instantiate`. One way is to expose an `Awaitable Prewarm()` on the pool and await it before `allowSceneActivation = true`.

*Expected result: the bar fills, the scene appears, the loading canvas disappears. No frozen frame.*

## Part C: Break it on purpose (~15 min)
1. Change the loop condition to `while (!op.isDone)`. Build to the phone (or test in the editor). Press Play in the menu.
   *Expected: the bar stops at 1.0 and the game never appears. With `allowSceneActivation = false`, `progress` stops at 0.9 and `isDone` never becomes true. This is the classic infinite loading screen.*
2. Revert to `op.progress < 0.9f`.
3. Resilience test: start a load, press Home mid-load, wait five seconds, come back. The load must complete and the canvas must disappear. If it does not, check that nothing in `OnApplicationPause` cancels the token you passed.
4. Press the Android back button on the loading screen: nothing should happen (no navigation mid-load).

## Part D: Measure (~15 min)
1. Find the launch component without guessing the activity class name:
   ```bash
   adb shell cmd package resolve-activity --brief com.yourname.mygame
   ```
   The last line is `com.yourname.mygame/<activity>`.
2. Cold start, three times, with a force-stop before each:
   ```bash
   adb shell am force-stop com.yourname.mygame
   adb shell am start -W -n com.yourname.mygame/<activity>
   ```
   Note `TotalTime` (ms) each run. It stops at the first frame Android draws, which is before Unity is interactive.
3. Time to interactive: in the first `Update` of a `MenuReady` component in `Menu`, log once:
   ```csharp
   Debug.Log($"[perf] interactive {Time.realtimeSinceStartup:F2}s");
   ```
   Read it with `adb logcat -s Unity | grep perf`.
4. Scene load: wrap the body of `Load` in a `System.Diagnostics.Stopwatch` and log `[perf] scene {scene} loaded in {sw.ElapsedMilliseconds} ms`.
5. Baseline Sheet rows: cold start (median of three), time to interactive, scene load ms, APK size (`ls -l releases/`). Android vitals flags cold starts of 5 s or more as slow; write your own budget next to the number.

## Part E: Playtest and journal (~10 min)
1. Hand your phone to a neighbour. Say nothing. Two minutes. Write down two things that confused them.
2. Journal: lane definition, the two confusions, the perf numbers, and what you will change first.
3. Commit and tag `v0.3.1-lane`.

## Troubleshooting
- **Loading canvas appears in every scene forever** -> `loadingScreen.SetActive(false)` is not reached because `await op` never completed; check that `allowSceneActivation` was set to true.
- **`Slider` not found** -> `using UnityEngine.UI;` missing, or the uGUI package was removed; re-add **com.unity.ugui** in Package Manager.
- **The loader gets destroyed on scene change** -> it is not on the `DontDestroyOnLoad` object, or a second `App` exists in `Menu`; keep exactly one, in `Boot`.
- **`am start` says "Activity class does not exist"** -> use the component from `resolve-activity`, not a guessed class name.
- **Cold start numbers jump around** -> you are getting warm starts; `force-stop` before each run and keep the phone unplugged from the charger.
- **Back button quits the app on the loading screen** -> handle `InputSystem` back (Escape) in the loader and ignore it while loading.

## Checkpoints to commit
1. Greybox lane in `Assets/Scenes/Game.unity` with win and lose reachable.
2. `SceneLoader.cs`, `LoadingCanvas` in `Boot`, `MenuReady.cs` with the perf marker.
3. `docs/CA2/baseline/Performance_Baseline.md`: cold start x3, time to interactive, scene load, APK size.
4. `docs/dev-journal.md`: lane definition, two playtest confusions, numbers.
5. Tag `v0.3.1-lane`.

## Exit ticket
Show the loading bar progressing on your phone and your three cold-start timings in the Baseline Sheet.

## Reference links
- Unity 6.6: `SceneManager.LoadSceneAsync`: https://docs.unity3d.com/6000.6/Documentation/ScriptReference/SceneManagement.SceneManager.LoadSceneAsync.html
- Unity 6.6: `AsyncOperation.allowSceneActivation` (the 0.9 behaviour): https://docs.unity3d.com/6000.6/Documentation/ScriptReference/AsyncOperation-allowSceneActivation.html
- Unity 6.6: Awaitable code examples (awaiting `AsyncOperation`): https://docs.unity3d.com/6000.6/Documentation/Manual/async-awaitable-examples.html
- Unity 6.6: `Awaitable.NextFrameAsync`: https://docs.unity3d.com/6000.6/Documentation/ScriptReference/Awaitable.NextFrameAsync.html
- Unity 6.6: `Object.DontDestroyOnLoad`: https://docs.unity3d.com/6000.6/Documentation/ScriptReference/Object.DontDestroyOnLoad.html
- Unity Addressables package (awareness): https://docs.unity3d.com/Packages/com.unity.addressables@latest
- Unity ProBuilder package: https://docs.unity3d.com/Packages/com.unity.probuilder@latest
- Android vitals: app startup time: https://developer.android.com/topic/performance/vitals/launch-time
- Android: `am` and `pm` commands in the adb shell: https://developer.android.com/tools/adb
