# Week 5, Lab A (2h): Vertical-slice skeleton: Awaitable and pooling

**Module:** Mobile Game Development (A12581) · **Engine:** Unity 6.6 · **Platform:** Android (sideload) · **Date:** Mon 5 Oct 2026
**Feeds:** CA2 Playable Vertical Slice (due Sun 15 Nov 2026, 23:59) · **LOs:** LO1, LO3

> **Goal:** Build the scene-flow skeleton of your vertical slice, remove every coroutine in favour of `Awaitable` with cancellation, and spawn from a pool so that steady-state play allocates nothing per frame.

## What you will complete today
1. Boot > Menu > Game > Result scene flow with a `GameManager` state machine and one piece of feedback on your core verb.
2. Every `StartCoroutine` in the project listed in your journal and at least one converted to `Awaitable` with a cancellation token.
3. `EnemyPool` and `Enemy` (or the equivalent for your project: chunks, tiles, units, particles) with prewarm, spawn and release.
4. A Profiler capture on your phone showing 0 B GC Alloc per frame during spawning, recorded in the Baseline Sheet.
5. Tag `v0.3.0-skeleton`.

## Pre-flight
- CA1 submitted (Sun 4 Oct). If it is not, tell the lecturer now.
- Project builds to your phone (Week 4 pipeline). Development Build and Autoconnect Profiler are known to you from Week 3.
- Read the Awaitable handout on Moodle (10 minutes) before this lab.
- `docs/CA2/baseline/Performance_Baseline.md` exists from Week 3 with your first numbers.

## Part A: Scene flow skeleton (~20 min)
1. Create four scenes in `Assets/Scenes/`: `Boot`, `Menu`, `Game`, `Result`. Add all four to the Build Profile scene list, `Boot` first.
2. In `Boot`, one empty object `App` with a `Bootstrap` component:
   ```csharp
   using UnityEngine;
   using UnityEngine.SceneManagement;
   
   public class Bootstrap : MonoBehaviour
   {
       void Awake()
       {
           Application.targetFrameRate = 60;
           DontDestroyOnLoad(gameObject);
       }
       async Awaitable Start() => await SceneManager.LoadSceneAsync("Menu");
   }
   ```
3. In `Game`, a `GameManager` with a simple state enum (`Playing`, `Paused`, `Won`, `Lost`) and a `public event System.Action<State> StateChanged`. Pause and resume from Week 2 (`OnApplicationPause`) should set `Paused`.
4. Wire one piece of feedback on your core verb (a coin sound, a score popup, a hit flash). The point is that the loop *reports back* to the player from day one.
5. `Result` shows the score and a Retry button that loads `Game` again. No Quit button: mobile apps do not have one.

*Expected result: Boot shows nothing, Menu appears, Play leads to Game, Win or Lose leads to Result, Retry loops back.*

## Part B: Hunt the coroutines (~15 min)
1. From the repo root:
   ```bash
   grep -rn "IEnumerator\|StartCoroutine\|WaitForSeconds" Assets/ --include=*.cs
   ```
   List every hit in `docs/dev-journal.md` under today's date.
2. Rules for new code from now on: no `IEnumerator` methods, no `StartCoroutine`. Use `Awaitable`:
   - `yield return null` becomes `await Awaitable.NextFrameAsync(ct);`
   - `yield return new WaitForSeconds(s)` becomes `await Awaitable.WaitForSecondsAsync(s, ct);`
   - `yield return asyncOp` becomes `await asyncOp;`
   - `StopCoroutine` becomes `cts.Cancel()`
3. Convert the first one using the deck's `WaveTimer` as the template:
   ```csharp
   using System; using System.Threading; using UnityEngine;
   
   public class WaveTimer : MonoBehaviour
   {
       [SerializeField] float interval = 2f;
       [SerializeField] EnemyPool pool;
       CancellationTokenSource cts;
       void OnEnable()
       {
           cts = CancellationTokenSource.CreateLinkedTokenSource(
               Application.exitCancellationToken); // cancels on quit
           _ = RunAsync(cts.Token);   // was StartCoroutine(Loop())
       }
       void OnDisable() { cts.Cancel(); cts.Dispose(); }
   
       async Awaitable RunAsync(CancellationToken ct)
       {
           try { while (true) { pool.Spawn(transform.position);
                 await Awaitable.WaitForSecondsAsync(interval, ct); } }
           catch (OperationCanceledException) { }   // normal on disable
       }
   }
   ```
4. Why the token is linked to `Application.exitCancellationToken`: without it, the loop keeps running after you stop Play Mode in the editor, and after the app quits on device. Why `OperationCanceledException` is caught: cancellation is the normal way out, not an error.
5. The one rule: **never await the same `Awaitable` instance twice**. They are pooled. If you need `WhenAll`, wrap in `Task` with the `AsTask()` extension from the handout.

*Expected result: Play, then stop Play Mode: no "spawn" logs appear after the stop.*

## Part C: EnemyPool and Enemy (~25 min)
1. `Enemy.cs` (rename to whatever your project spawns):
   ```csharp
   using UnityEngine;
   
   public class Enemy : MonoBehaviour
   {
       EnemyPool pool;
       [SerializeField] int maxHealth = 3;
       int health;
       Rigidbody body;
   
       public Enemy Init(EnemyPool p) { pool = p; body = GetComponent<Rigidbody>(); return this; }
       void OnEnable() { health = maxHealth; if (body) body.linearVelocity = Vector3.zero; }
       public void Hit() { if (--health <= 0) Despawn(); }
       public void Despawn() => pool.Release(this);
   }
   ```
   `OnEnable` is where state resets: health, velocity, timers, trail renderers (`trail.Clear()`), particle systems.
2. `EnemyPool.cs` exactly as on the deck (prewarm in `Awake`, `Spawn` pops from a `Stack<Enemy>`, `Release` deactivates and pushes back). Set `size` to the worst wave you expect plus a margin.
3. Put an `EnemyPool` object in `Game`, assign the prefab, add `WaveTimer` pointing at it.
4. Make enemies die: a tap, a collision, a timer, whatever your core verb is. Confirm they return to the pool (the object stays in the Hierarchy, greyed out) rather than being destroyed.
5. If you prefer the built-in, `UnityEngine.Pool.ObjectPool<T>` does the same job with callbacks for create, get, release and destroy. The pattern and the checks are identical.

*Expected result: after the first wave, the Hierarchy count under EnemyPool never grows; enemies toggle active and inactive.*

## Part D: Profile on the phone (~20 min)
1. **File > Build Profiles > Android**: Development Build **on**, Autoconnect Profiler **on**. Build and Run.
2. **Window > Analysis > Profiler**, target your device. Enable the CPU and Memory modules.
3. Play through two waves. In the CPU module, select a frame during spawning and open **Hierarchy**; search `Instantiate` and `GC.Alloc`. Check the **GC Alloc** column of your spawner and enemies: it should be 0 B (excluding the cancellation tokens).
4. If it is not 0 B: common culprits are `string` concatenation in a score label (cache and update only on change), `new Vector3[]` per frame, `GetComponent` in `Update`, `foreach` over a non-generic collection, LINQ.
5. Record in `docs/CA2/baseline/Performance_Baseline.md`: frame time average and 99th percentile during spawning, GC Alloc per frame, and one sentence on what you changed. Save the Profiler capture as `docs/CA2/baseline/w05-spawner.data` (Profiler window > Save).
6. Rebuild with Development Build **off** before you leave; the release build is what CA2 marks.

## Part E: Commit and journal (~10 min)
1. Journal entry: coroutines found, coroutine converted, GC Alloc before and after, anything that surprised you.
2. Commit and tag `v0.3.0-skeleton`.

## Troubleshooting
- **`Awaitable` not found** -> `using UnityEngine;` is missing, or the file is inside an assembly definition that targets an old API compatibility level.
- **`linearVelocity` does not exist** -> you are on an older Unity; in Unity 6 `Rigidbody.velocity` was renamed `linearVelocity`. The module uses Unity 6.6.
- **Enemies spawn but never die** -> `Hit()` is never called, or `Despawn` returns to a different pool instance; check the `Init` call in `Create`.
- **`OperationCanceledException` shows as a red error** -> you awaited without the `try/catch`, or you caught `Exception` in an outer scope and logged it; catch `OperationCanceledException` specifically.
- **Profiler does not connect** -> same Wi-Fi is not required with USB; run `adb forward tcp:34999 localabstract:Unity-com.yourname.mygame` if Autoconnect fails, or pick the device in the Profiler target dropdown.
- **GC Alloc shows 0 B in the editor but not on device** -> profile on the device only; the editor allocates differently and lies about steady state.

## Checkpoints to commit
1. `Assets/Scenes/Boot.unity`, `Menu.unity`, `Game.unity`, `Result.unity` and `Bootstrap.cs`, `GameManager.cs`.
2. `WaveTimer.cs` (or your converted equivalent), `EnemyPool.cs`, `Enemy.cs`.
3. `docs/CA2/baseline/Performance_Baseline.md` updated; `docs/CA2/baseline/w05-spawner.data`.
4. `docs/dev-journal.md` entry for today.
5. Tag `v0.3.0-skeleton`.

## Exit ticket
Show a Profiler capture from your phone with 0 B GC allocation per frame during spawning (excluding cancellation tokens), and name the coroutine you replaced.

## Reference links
- Unity 6.6: `Awaitable` script reference: https://docs.unity3d.com/6000.6/Documentation/ScriptReference/Awaitable.html
- Unity 6.6: introduction to async with Awaitable: https://docs.unity3d.com/6000.6/Documentation/Manual/async-awaitable-introduction.html
- Unity 6.6: Awaitable code examples (coroutine conversions, `AsTask`): https://docs.unity3d.com/6000.6/Documentation/Manual/async-awaitable-examples.html
- Unity 6.6: `Application.exitCancellationToken`: https://docs.unity3d.com/6000.6/Documentation/ScriptReference/Application-exitCancellationToken.html
- Unity 6.6: `ObjectPool<T>`: https://docs.unity3d.com/6000.6/Documentation/ScriptReference/Pool.ObjectPool_1.html
- Unity 6.6: Profiler overview: https://docs.unity3d.com/6000.6/Documentation/Manual/Profiler.html
- Unity 6.6: profiling on Android: https://docs.unity3d.com/6000.6/Documentation/Manual/android-profiling.html
- Microsoft: async return types (why not `async void`): https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/async-return-types
