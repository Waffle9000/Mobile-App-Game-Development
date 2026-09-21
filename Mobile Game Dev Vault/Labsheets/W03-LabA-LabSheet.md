# Week 3, Lab A (2h): Rendering budgets and profiling on device

**Module:** Mobile Game Development (A12581) · **Engine:** Unity 6.6 · **Platform:** Android (sideload) · **Date:** Mon 21 Sep 2026
**Feeds:** CA2 Playable Vertical Slice (due Sun 15 Nov 2026), baseline profiling evidence; CA3 dossier before picture · **LOs:** LO1

> **Goal:** Attach the Unity Profiler to your phone, read the CPU, Rendering and Memory modules on a worst-case capture, use a render-scale probe to decide whether you are CPU-bound or GPU-bound, take an AGI (or Frame Debugger) capture, and record one named bottleneck with numbers.

## What you will complete today
1. A Development Build with the Profiler attached, capturing 30 to 60 s of your heaviest gameplay.
2. Headline numbers from three modules: main-thread ms and tallest marker, SetPass calls and batches, GC allocated per frame and Total Reserved.
3. A CPU-bound or GPU-bound verdict from the `RenderScaleProbe` test, with both frame times written down.
4. One AGI system trace or Frame Debugger capture with a screenshot of the most expensive pass.
5. `/docs/CA2/baseline/bottleneck-01.md`: what, where, numbers, verdict, and one fix to try.

## Pre-flight
- Week 2 checkpoints committed; the **Android Dev** build profile exists with Development Build and Autoconnect Profiler on.
- Something heavy to profile: the worst case your option has today (a spawn wave, a biome change, a full grid of units, many UI elements). If you have nothing yet, a test scene with 200 pooled cubes moving is fine for today.
- Android GPU Inspector installed on the laptop (link below) and the supported-devices page checked for your phone's GPU.
- Folder `/docs/CA2/baseline/` created in the repo.

## Part A: Android Dev build with the Profiler attached (~10 min)
1. Activate **Android Dev** in **File > Build Profiles**. Development Build and Autoconnect Profiler on; **Deep Profiling Support** off.
2. **Build And Run** with the phone selected as Run Device.
3. **Window > Analysis > Profiler**. Pick `AndroidPlayer (...)` in the target drop-down. If it is missing, run `adb forward tcp:34999 localabstract:Unity-com.yourname.mygame`, choose `<Enter IP>` and type `127.0.0.1`.
4. Reduce the module list to **CPU Usage**, **Rendering** and **Memory**. *Expected: three live graphs scrolling while the game runs on the phone.*

## Part B: CPU capture of your worst case (~20 min)
1. Press **Record**, play into the heaviest situation you have, keep going for 30 to 60 s, stop.
2. In the CPU Usage module switch to **Timeline** view. Click a tall frame. Read the **main thread** time in ms at the top and expand `PlayerLoop` until you find the tallest child marker.
3. Switch to **Hierarchy** view for the same frame, sort by **Time ms**, and note the top three entries with their self time.
4. Look for `GC.Collect` anywhere in the capture. If it appears, note the frame it landed in and its duration; that is a stutter the player felt.
5. Write in `/docs/journal.md`: worst-frame main-thread ms, the tallest marker name, top three hierarchy entries, GC.Collect yes or no.

## Part C: Rendering and Memory modules (~20 min)
1. With the same frame selected, open the **Rendering** module: **SetPass Calls**, **Batches**, **Triangles**, **Vertices**. Every SetPass is a material or shader switch on the render thread.
2. In the CPU module search for `Gfx.WaitForPresentOnGfxThread`. A big value means the CPU is waiting for the GPU; a small one means the GPU has spare time.
3. **Memory** module, **Simple** view: note **Total Reserved**, **GC Allocated in Frame** and the **Textures**, **Meshes** and **Audio** rows.
4. Click **Take Sample** (Memory module, Detailed view). Note the three largest categories and the single largest asset. Textures with mipmaps off or uncompressed formats usually top the list.
5. Save the whole capture: **Save** to `/docs/CA2/baseline/w03-profile.data`. Screenshot the bad frame with the Timeline and the Rendering numbers visible; save as `/docs/CA2/baseline/w03-bad-frame.png`.

## Part D: RenderScaleProbe test (~15 min)
1. Create `Assets/Scripts/RenderScaleProbe.cs`:

```csharp
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

// Halve the render scale. Frame time drops a lot: GPU-bound.
// Frame time barely moves: CPU-bound. Now you know where to look.
public class RenderScaleProbe : MonoBehaviour
{
    [Range(0.3f, 1f)] public float lowScale = 0.5f;
    UniversalRenderPipelineAsset _urp;
    float _full;

    void Start()
    {
        _urp = GraphicsSettings.currentRenderPipeline
               as UniversalRenderPipelineAsset;
        _full = _urp.renderScale;
    }

    public void Toggle()                  // bind to a debug button
    {
        bool atFull = Mathf.Approximately(_urp.renderScale, _full);
        _urp.renderScale = atFull ? lowScale : _full;
        Debug.Log($"[Probe] renderScale = {_urp.renderScale}");
    }
}
```

2. Add a small **Probe** button in a corner of the `SafeArea` panel, active only when `Debug.isDebugBuild` is true, and bind its OnClick to `Toggle`.
3. Build And Run. With the Profiler recording, play the worst case for 20 s at full scale, press Probe, play 20 s at half scale.
4. Compare the two windows: main-thread ms and `Gfx.WaitForPresentOnGfxThread`. Frame time dropped by a third or more: **GPU-bound**. Barely moved: **CPU-bound**. Write both numbers and the verdict in the journal.
5. Leave the probe in for Week 7; it becomes the basis of the performance-mode toggle.
6. If the verdict is GPU-bound, your first candidate fix is a lower Render Scale or fewer transparent layers; if CPU-bound, it is the tallest marker from Part B. Write the candidate down now, before you forget which one it was.

## Part E: AGI or Frame Debugger capture (~15 min)
1. **If your phone is on the AGI supported list**: open Android GPU Inspector, **Capture System Profile**, select the device and your app, 10 to 15 s of the worst case. Read GPU utilisation next to the frame time track, then open one frame in the Frame Profiler and find the most expensive draw or pass.
2. **If it is not supported**: with the Android Dev build running, **Window > Analysis > Frame Debugger > Enable** with the AndroidPlayer target. Step through the draw calls; the render-target thumbnails show what each pass paints. There are no GPU timings here, but the order and count are the same.
3. Screenshot the most expensive pass (or the longest list of draws in one pass) to `/docs/CA2/baseline/w03-gpu-pass.png`.
4. AGI traces are large. Commit them only if under 50 MB; otherwise record the file name and keep it out of Git.

## Part F: Record the bottleneck (~10 min)
1. Create `/docs/CA2/baseline/bottleneck-01.md` with exactly these headings: **What** (one sentence), **Where** (marker or pass name), **Numbers** (main-thread ms, SetPass, GC alloc, frame time at full and half scale), **Verdict** (CPU-bound or GPU-bound and why), **Fix to try** (one change, one sentence).
2. Link the two screenshots and the `.data` file from the note.
3. Commit. This note is the before picture for CA3: every fix you claim later must point back to a note like this.

## Troubleshooting
- **Profiler shows the Editor, not the phone** -> the target drop-down is still on Play Mode; select AndroidPlayer.
- **Numbers jump around wildly** -> Deep Profile is on, or the phone is already throttling; let it cool for two minutes and disable Deep Profile.
- **`_urp` is null in RenderScaleProbe** -> the active pipeline asset is not URP, or a Quality level overrides it with a different asset; check **Project Settings > Quality > Render Pipeline Asset** for the Android quality level.
- **AGI does not list the device** -> USB debugging is on but the device is not supported; use the Frame Debugger route.
- **Frame Debugger says no player connected** -> it needs a Development Build and the same `adb forward` line as the Profiler.
- **GC.Collect every few seconds** -> something allocates per frame; Week 3 Lab B finds it with the GC Alloc column.

## Checkpoints to commit
1. `/docs/CA2/baseline/w03-profile.data` and `/docs/CA2/baseline/w03-bad-frame.png`.
2. `/docs/CA2/baseline/w03-gpu-pass.png` (AGI or Frame Debugger).
3. `Assets/Scripts/RenderScaleProbe.cs` and the debug button in the scene.
4. `/docs/CA2/baseline/bottleneck-01.md`.
5. `/docs/journal.md` with the Part B and Part D numbers.

## Exit ticket
Commit bottleneck-01.md with the Profiler screenshot and tell me, in one sentence, whether you are CPU-bound or GPU-bound and how you know.

## Reference links
- Profiler window overview (6000.6): https://docs.unity3d.com/6000.6/Documentation/Manual/ProfilerWindow.html
- Unity Profiler (6000.6): https://docs.unity3d.com/6000.6/Documentation/Manual/Profiler.html
- CPU Usage Profiler module: https://docs.unity3d.com/6000.6/Documentation/Manual/ProfilerCPU.html
- Rendering Profiler module: https://docs.unity3d.com/6000.6/Documentation/Manual/ProfilerRendering.html
- Memory Profiler module: https://docs.unity3d.com/6000.6/Documentation/Manual/ProfilerMemory.html
- Profiling on an Android device (6000.6): https://docs.unity3d.com/6000.6/Documentation/Manual/android-profile-on-an-android-device.html
- Frame Debugger (6000.6): https://docs.unity3d.com/6000.6/Documentation/Manual/FrameDebugger.html
- URP Asset reference, Render Scale and MSAA: https://docs.unity3d.com/6000.6/Documentation/Manual/urp/universalrp-asset.html
- SRP Batcher (6000.6): https://docs.unity3d.com/6000.6/Documentation/Manual/SRPBatcher.html
- Texture compression formats per platform (6000.6): https://docs.unity3d.com/6000.6/Documentation/Manual/class-TextureImporterOverride.html
- Compute shaders (6000.6): https://docs.unity3d.com/6000.6/Documentation/Manual/class-ComputeShader.html
- Android GPU Inspector: https://developer.android.com/agi
- Unity: best practices for profiling game performance: https://unity.com/how-to/best-practices-for-profiling-game-performance
