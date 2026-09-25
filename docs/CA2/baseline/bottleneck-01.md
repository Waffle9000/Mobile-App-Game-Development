# Bottleneck 01 – 25 Sep 2026

## What
The 200 stress-test cubes are drawn one at a time instead of batched, which costs CPU time every frame.

## Where
PostLateUpdate.FinishFrameRendering (~5.6 ms), in the (RP 1:0) Renderer2D Pass with 199 separate RenderLoop.Draw calls.

## Numbers
- Main thread: 17.09 ms
- SetPass calls: 6
- GC alloc: 59 B per frame
- Frame time at full scale: 16.85 ms
- Frame time at half scale: 17.14 ms

## Verdict
CPU-bound. Halving the render scale barely changed the frame time, and Gfx.WaitForPresentOnGfxThread was 0.00 ms, so the GPU is not the limit.

## Fix to try
Give the cubes a URP Lit material so the SRP Batcher can group them instead of drawing each one separately.

## Evidence
- Profiler screenshot: [w03-bad-frame.png](w03-bad-frame.png)
- Frame Debugger screenshot: [w03-gpu-pass.png](w03-gpu-pass.png)
- Profiler capture: [w03-profile.zip](w03-profile.zip) (zipped .data file)