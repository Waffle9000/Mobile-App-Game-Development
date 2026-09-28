journal.md

Date:15/09/26

The first core verb for my game would be to swipe(dodge/turn) and the first thing I would cut if time ran out would be movement form side to side, I would restrict it only sliding and jumping


---------------------------------

CPU main thread time = 16.57 ms

setpass calls = 3

GC Alloc: 59 B


------------------------------------------------


| Test                                         | Should happen                                  |
| -------------------------------------------- | ---------------------------------------------- |
| Press Home, wait 10 s, come back             | Paused, panel showing, no sound ✅              |
| Pull the notification shade down and up      | Paused ✅                                       |
| Get a neighbour to call you, then hang up    | Paused, and only resumes when you tap Resume ✅ |
| Power button off, then on                    | Paused ✅                                       |
| Force stop in Settings > Apps, then relaunch | Progress is still there                        |




Date: 25/09/2026
## Week 3 Lab A – Part B (CPU capture)
- Test: 200 spinning cubes (StressTest), Samsung SM-A025F
- Worst-frame main thread: 17.09 ms
- Tallest marker: PostLateUpdate.FinishFrameRendering (5.58 ms)
- Top 3 (Time ms / Self ms):
  1. PostLateUpdate.FinishFrameRendering – 5.58 / 0.16
  2. TimeUpdate.WaitForLastPresentationAndUpdateTime – 4.53 / 0.01
  3. FixedUpdate.PhysicsFixedUpdate – 2.84 / 0.01
- GC.Collect: No

## Week 3 Lab A – Part C (Rendering & Memory) – 25 Sep 2026
- SetPass calls: 6 | Batches/Draw calls: 0 (not counted with URP) | Triangles: 3.4k | Vertices: 5.5k
- Gfx.WaitForPresentOnGfxThread: 0.00 ms (CPU not waiting for GPU)
- Total Reserved: 321.7 MB (153.9 MB in use)
- GC allocated in frame: 59 B
- Textures: 28.6 MB | Meshes: 107.2 KB | Audio: 0.8 MB
- Note: Reserved is just over the 300 MB budget, but 36.4 MB of that is the Profiler itself, so the real game should be under budget.
- Detailed sample (phone): 248.8 MB used
- 3 biggest categories: Native 284.9 MB, Graphics 31.6 MB, Managed 5.2 MB
- Biggest object groups: Textures 28.6 MB, Render Textures 23.8 MB
- Single biggest asset: _CameraTargetAttachment – 4.4 MB (camera render target; ignoring the Profiler's own screenshot)

## Week 3 Lab A – Part D (RenderScaleProbe) – 25 Sep 2026
- Full scale: 16.85 ms | Half scale: 17.14 ms
- Verdict: CPU-bound. Frame time barely moved at half scale, and Gfx.WaitForPresentOnGfxThread was 0.00 ms.
- Fix to try: tallest marker is PostLateUpdate.FinishFrameRendering (~5.6 ms). The cubes use a default (pink) material that isn't made for URP, so they can't be batched. Give them a URP Lit material so the SRP Batcher can draw them more cheaply.

## Week 3 Lab B – Part B (Frame time, release build 0.1.1) – 28 Sep 2026
- Menu (paused): avg 16.71 ms / p99 16.88 ms
- Steady gameplay: avg 16.71 ms / p99 16.90 ms
- Worst case: avg 16.71 ms / p99 16.90 ms
- First [Baseline] line after launch (avg 22.58 ms) ignored, as it includes startup.
- Note: all three states are almost the same. The 200 stress cubes are always drawn,
  even when paused (pausing only stops them rotating), and the game is capped at 60 fps,
  so frame time stays at ~16.7 ms.
- Worst-case p99 16.90 ms is inside the MDA budget (under 25 ms).
- - Display refresh rate: 60 Hz (no Motion smoothness option on this phone)
- - GC allocated per frame (steady gameplay): 59 B - Allocating markers: - PostLateUpdate.FinishFrameRendering – 32 B - FixedUpdate.NewInputFixedUpdate – 9 B - PostLateUpdate.PlayerSendFrameStarted – 9 B - PreUpdate.NewInputUpdate – 9 B - All from Unity's rendering, Input System and Profiler, none from my own scripts. - GC.Collect: none seen in the capture. - Target is 0 B, to revisit in Week 5.
- - Memory and Rendering - Peak Total Reserved: 317.0 MB (Dev build, includes Profiler overhead)
- - Worst case rendering: SetPass 11 / Batches 0 (not counted with URP) / Triangles 3.6k
- - TOTAL PSS (Release build, after 2 min play): 194.3 MB (198,954 KB)
- Peak Total Reserved (Dev) 317.0 MB vs TOTAL PSS (Release) 194.3 MB. They measure different things; the trend matters more.



- ## Week 3 Lab B – Part E (Cold start & APK size) – 28 Sep 2026 - Device: SM-A025F, Android 12 - Cold start (median of 3): Displayed 716 ms (731 / 716 / 713) - First interactive (Fully drawn, median): 2.46 s (2.506 / 2.464 / 2.459)
- - APK size: 71.0 MB (MyGame-0.1.1.apk, ARMv7 + ARM64)