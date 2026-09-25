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