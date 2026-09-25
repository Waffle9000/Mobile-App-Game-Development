# Week 3, Lab B (2h): Frame pacing and your performance baseline

**Module:** Mobile Game Development (A12581) · **Engine:** Unity 6.6 · **Platform:** Android (sideload) · **Date:** Wed 23 Sep 2026
**Feeds:** CA2 Playable Vertical Slice (due Sun 15 Nov 2026), baseline profiling evidence; CA3 before-and-after table · **LOs:** LO1

> **Goal:** Set and verify a 60 fps target with Optimized Frame Pacing, measure average and 99th-percentile frame time in the release build with a sampler that allocates nothing, collect GC, memory, rendering, cold-start and APK-size numbers, and commit row one of your Performance Baseline Sheet.

## What you will complete today
1. `Application.targetFrameRate = 60` and Optimized Frame Pacing verified on the phone.
2. `FrameTimeSampler.cs` in the release build, with average and p99 recorded for menu, steady gameplay and worst case.
3. GC allocated per frame in steady gameplay, with every allocating marker named.
4. Peak Total Reserved, TOTAL PSS, SetPass calls, batches and triangles for the worst case.
5. Cold start (median of three) and APK size, and `/docs/CA2/baseline/baseline-sheet.md` with its first row committed.

## Pre-flight
- Lab A checkpoints committed: `bottleneck-01.md`, the Profiler capture, `RenderScaleProbe.cs`.
- Both Build Profiles working; keystore passwords to hand for the release build.
- The phone cool and at least 50% charged; plug it in but do not measure while it is fast-charging (the SoC may throttle).
- `adb` on your PATH; `adb devices` shows the phone.

## Part A: Set and verify the frame target (~10 min)
1. Confirm `MobileBootstrap.cs` sets `Application.targetFrameRate = 60` and `QualitySettings.vSyncCount = 0`. Turn-based and idle options may choose 30; write the reason in the MDA budget if so.
2. **Player Settings > Resolution and Presentation > Optimized Frame Pacing** on. Unity then uses Android's frame pacing library to align presents with vsync.
3. Add `FrameTimeSampler.cs` (Part B) to `Bootstrap`, build with **Android Dev**, run, and read the Profiler's CPU module: the frame time should sit near 16.7 ms with a flat line when nothing is happening. *Expected: a flat 16 to 17 ms trace in the menu.*

## Part B: Frame time in three states (~25 min)
1. Create `Assets/Scripts/FrameTimeSampler.cs`:

```csharp
using System;
using UnityEngine;

// Logs average and 99th percentile frame time every 600 frames.
public class FrameTimeSampler : MonoBehaviour
{
    const int N = 600;                 // 10 s at 60 fps
    readonly float[] _ms = new float[N];
    readonly float[] _sorted = new float[N];
    int _i;

    void Update()
    {
        _ms[_i++] = Time.unscaledDeltaTime * 1000f;
        if (_i < N) return;
        _i = 0;
        Array.Copy(_ms, _sorted, N);
        Array.Sort(_sorted);           // reused buffer, no garbage
        float sum = 0f;
        for (int k = 0; k < N; k++) sum += _sorted[k];
        float p99 = _sorted[Mathf.FloorToInt(0.99f * (N - 1))];
        Debug.Log($"[Baseline] avg {sum / N:F2} ms  p99 {p99:F2} ms");
    }
}
```

2. Raise **Bundle Version Code**, activate **Android Release**, build `Builds/MyGame-0.1.1.apk`, and sideload it with `adb install -r`. Measurements come from the **release** build; the Dev build carries profiler overhead.
3. Run `adb logcat -s Unity` on the PC. For each state, hold the game there for at least 20 s so you get two `[Baseline]` lines, and record the second one:
   - **Menu**: sitting on the main menu.
   - **Steady gameplay**: normal play, nothing special happening.
   - **Worst case**: the same situation you profiled in Lab A.
4. Note the display refresh rate of your phone (Settings > Display) in the sheet. A 120 Hz phone still shows 60 fps here because Android defaults games to 60 Hz unless the app asks for more; the High FPS toggle is Week 7.

## Part C: GC allocations per frame (~15 min)
1. Switch to the **Android Dev** profile, Build And Run, attach the Profiler.
2. CPU Usage module, **Hierarchy** view, play steady gameplay for 20 s, then select a typical frame and sort by the **GC Alloc** column.
3. Every row with a non-zero GC Alloc is a per-frame allocation. Typical culprits: string building for HUD text, `GetComponent` in `Update`, LINQ, `foreach` over a `List` in older code paths, boxing in `Debug.Log` calls left in.
4. Record the steady-state **GC Allocated in Frame** (bytes) and the names of every allocating marker in the sheet and the journal. Target is 0 B; you fix them in Week 5 with pooling.
5. Also note how many seconds pass between `GC.Collect` markers, if any appear.

## Part D: Peak memory and rendering counts (~15 min)
1. Memory module, Simple view, play the worst case for 60 s and note the highest **Total Reserved** you see.
2. Rendering module on the worst-case frame: **SetPass Calls**, **Batches**, **Triangles**.
3. Cross-check memory as Android sees it, after two minutes of play in the **release** build:

```bash
adb shell dumpsys meminfo com.setu.mygame
# ... TOTAL PSS:   312456   (kB)
```

4. Record Total Reserved (MB) and TOTAL PSS (MB) side by side. They differ; the sheet needs both, and the trend over weeks matters more than either number.

## Part E: Cold start and APK size (~15 min)
1. Device fields, then three cold starts, reading the `Displayed` line each time (older Android versions log it under `ActivityManager`):

```bash
adb shell getprop ro.product.model
adb shell getprop ro.build.version.release

adb shell am force-stop com.setu.mygame
adb logcat -c
adb shell monkey -p com.setu.mygame 1
adb logcat -d -s ActivityTaskManager
# Displayed com.setu.mygame/...UnityPlayerActivity: +1s412ms
```

2. Record the **median** of the three `Displayed` times. If a loading screen follows the first frame, add the seconds until the first interactive frame (stopwatch is fine) and record both.
3. APK size from the file you sideload: `ls -l Builds/MyGame-0.1.1.apk` in Git Bash, or `(Get-Item Builds\MyGame-0.1.1.apk).Length` in PowerShell. Record it in MB.

## Part F: Commit row one of the Baseline Sheet (~10 min)
1. Create `/docs/CA2/baseline/baseline-sheet.md` with this table (one row per build; add a row whenever something meaningful changes):

```markdown
| Field | Row 1 |
|-------|-------|
| Date, commit, versionName / versionCode, Release or Dev, Unity version | |
| Device model, Android version, SoC / GPU, graphics API, refresh rate | |
| Target fps | |
| Menu: avg ms / p99 ms | |
| Steady gameplay: avg ms / p99 ms | |
| Worst case: avg ms / p99 ms | |
| GC allocated per frame (bytes), allocating markers | |
| Peak Total Reserved (MB) / TOTAL PSS (MB) | |
| Worst case: SetPass / batches / triangles | |
| Cold start ms (median of 3), first interactive s | |
| APK size (MB) | |
| Thermal delta, throttling (Week 7) | |
| Load time (Week 5) | |
```

2. Fill every field you measured today; leave the Week 5 and Week 7 rows empty rather than guessing.
3. Attach a screenshot of the `[Baseline]` log lines as `/docs/CA2/baseline/w03-sampler-log.png`.
4. Commit. Compare the worst-case p99 with the budget in your MDA; if it is already over, that is your first Week 5 fix.

## Troubleshooting
- **Frame time sits at 33 ms in the menu** -> `targetFrameRate` is still -1 (30 fps on Android) or the phone's Battery Saver is on.
- **`[Baseline]` lines never appear** -> the sampler is on an object that is not in the first scene, or the release build is older than you think; check versionCode in the About line and reinstall.
- **`Displayed` line is missing** -> filter `ActivityManager` instead, or run `adb logcat -d | findstr Displayed` on Windows.
- **TOTAL PSS is much bigger than Total Reserved** -> normal; PSS includes native, graphics driver and shared memory that Unity does not count.
- **GC Alloc column is empty** -> you are in Timeline view; switch to Hierarchy and enable the column.
- **Numbers drift upward during the lab** -> thermal throttling has started; rest the phone, and write it down: that is Week 7's topic arriving early.

## Checkpoints to commit
1. `Assets/Scripts/FrameTimeSampler.cs`.
2. `/docs/CA2/baseline/baseline-sheet.md` with row one complete.
3. `/docs/CA2/baseline/w03-sampler-log.png`.
4. `/docs/journal.md` with the allocating marker list and any throttling observed.

## Exit ticket
Commit the first row of your Baseline Sheet and tell me your worst-case p99 and whether it is inside the budget you wrote in the MDA.

## Reference links
- Application.targetFrameRate: https://docs.unity3d.com/6000.6/Documentation/ScriptReference/Application-targetFrameRate.html
- Time.unscaledDeltaTime: https://docs.unity3d.com/6000.6/Documentation/ScriptReference/Time-unscaledDeltaTime.html
- Android Player settings, Optimized Frame Pacing (6000.6): https://docs.unity3d.com/6000.6/Documentation/Manual/class-PlayerSettingsAndroid.html
- CPU Usage Profiler module (GC Alloc column): https://docs.unity3d.com/6000.6/Documentation/Manual/ProfilerCPU.html
- Memory Profiler module: https://docs.unity3d.com/6000.6/Documentation/Manual/ProfilerMemory.html
- Rendering Profiler module: https://docs.unity3d.com/6000.6/Documentation/Manual/ProfilerRendering.html
- Android frame pacing library (Swappy): https://developer.android.com/games/sdk/frame-pacing
- Android Frame Rate API: https://developer.android.com/media/optimize/performance/frame-rate
- App startup time (cold, warm, hot, and the Displayed line): https://developer.android.com/topic/performance/vitals/launch-time
- dumpsys (meminfo): https://developer.android.com/tools/dumpsys
- Android Debug Bridge (adb): https://developer.android.com/tools/adb
