# Week 2, Lab B (2h): Lifecycle, back, accessibility and scope lock

**Module:** Mobile Game Development (A12581) · **Engine:** Unity 6.6 · **Platform:** Android (sideload) · **Date:** Wed 16 Sep 2026
**Feeds:** CA1 Platform Readiness & Publication Awareness (due Sun 4 Oct 2026) · **LOs:** LO1, LO2

> **Goal:** Make the game survive Home, calls, notifications and the back gesture with `OnApplicationPause` and `OnApplicationFocus`; add haptics and a text-size setting; run a 10-minute accessibility pass; then lock your scope and submit the MDA one-pager.

## What you will complete today
1. `LifecycleGuard.cs` and a pause menu that opens on Android back, tested against a five-row matrix on the phone.
2. One meaningful haptic event behind a Settings toggle stored in `PlayerPrefs`.
3. A Small / Normal / Large text-size setting applied to every TextMeshPro text.
4. `/docs/CA1/accessibility-pass.md` with findings and fixes from the 10-minute pass.
5. `/docs/mda-onepager.md` complete, with an ordered cuts list and a numeric performance budget, submitted on Moodle.

## Pre-flight
- Lab A checkpoints committed: release APK installed, `TapSwipeInput.cs` and `SafeArea.cs` in the project.
- A neighbour with a phone who can call you (or a second phone) for the incoming-call test.
- The MDA one-pager draft from Week 1 with at least the title, pitch and Week 6 slice target filled in.
- TextMeshPro essentials imported (**Window > TextMeshPro > Import TMP Essential Resources**).

## Part A: LifecycleGuard, pause menu and Android back (~25 min)
1. Create `Assets/Scripts/LifecycleGuard.cs` and attach it to `Bootstrap`. This is the deck version plus one event so the UI can react:

```csharp
using UnityEngine;

public class LifecycleGuard : MonoBehaviour
{
    public static bool IsPaused { get; private set; }
    public static event System.Action<bool> PausedChanged;

    // Home, app switch, incoming call, screen off: save HERE.
    void OnApplicationPause(bool paused)
    {
        if (!paused) return;           // resuming is a player choice
        PlayerPrefs.Save();            // swap for your save system
        SetPaused(true);
    }

    // Notification shade, permission dialog, on-screen keyboard.
    void OnApplicationFocus(bool f) { if (!f) SetPaused(true); }

    public void SetPaused(bool value)  // Resume button passes false
    {
        IsPaused = value;
        Time.timeScale = value ? 0f : 1f;
        AudioListener.pause = value;
        PausedChanged?.Invoke(value);
    }
}
```

2. Build a pause panel under `SafeArea` with a title, a **Resume** button and a **Settings** button (Settings is filled in Parts B and C). No Quit button: mobile games do not have one.
3. Create `Assets/Scripts/PauseMenu.cs` on the Canvas. Android back arrives as `Escape` in the Input System:

```csharp
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject panel;
    [SerializeField] LifecycleGuard guard;

    void OnEnable()  { LifecycleGuard.PausedChanged += Show; Show(LifecycleGuard.IsPaused); }
    void OnDisable() { LifecycleGuard.PausedChanged -= Show; }
    void Show(bool paused) => panel.SetActive(paused);

    void Update()
    {
        var kb = Keyboard.current;
        if (kb == null || !kb.escapeKey.wasPressedThisFrame) return;
        guard.SetPaused(!LifecycleGuard.IsPaused);
    }

    public void OnResumePressed() => guard.SetPaused(false);
}
```

4. Wire the Resume button's OnClick to `PauseMenu.OnResumePressed`. In menus outside play, back should go up one level; a `MenuStack` with `Push` and `Pop` is enough, and it is on your cuts list if you run out of time.
5. Gate your verb: in `TapSwipeInput.Update` add `if (LifecycleGuard.IsPaused) return;` at the top. `timeScale = 0` does not stop `Update`.
6. Build And Run and run the matrix on the phone, ticking each row in `/docs/journal.md`:

| Test | Expected |
|------|----------|
| Press Home, wait 10 s, return | Paused, panel visible, audio silent, progress saved |
| Pull the notification shade down and up | Paused |
| Neighbour calls you, you hang up | Paused, game resumes only on Resume |
| Screen off with the power button, back on | Paused |
| Force stop from Settings, relaunch | Progress restored from the save |

## Part B: One haptic event behind a toggle (~15 min)
1. Create `Assets/Scripts/Haptics.cs`:

```csharp
using UnityEngine;

public static class Haptics
{
    const string Key = "haptics";

    public static bool Enabled
    {
        get => PlayerPrefs.GetInt(Key, 1) == 1;
        set { PlayerPrefs.SetInt(Key, value ? 1 : 0); PlayerPrefs.Save(); }
    }

    public static void Pulse()
    {
        if (!Enabled) return;
        Handheld.Vibrate();   // one fixed pulse; no duration or intensity control
    }
}
```

2. Call `Haptics.Pulse()` from exactly one meaningful event (a hit landed, a level-up, a piece placed). Never from `Update`.
3. Add a **Haptics** toggle to the Settings panel and bind it to `Haptics.Enabled`. Unity adds the `VIBRATE` permission to the manifest automatically because `Handheld.Vibrate` is referenced.
4. Verify on the phone with the toggle on and off. *Expected: one short buzz per event, none when the toggle is off.*

## Part C: Text size setting (~20 min)
1. Create `Assets/Scripts/TextScale.cs` and add it to every TMP text that players read (HUD numbers, menu labels, dialogue). Disable **Auto Size** on those texts, or the component's size is overridden:

```csharp
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class TextScale : MonoBehaviour
{
    const string Key = "textScale";
    public static event System.Action Changed;

    public static float Factor
    {
        get => PlayerPrefs.GetFloat(Key, 1f);
        set { PlayerPrefs.SetFloat(Key, value); PlayerPrefs.Save(); Changed?.Invoke(); }
    }

    TMP_Text _text;
    float _baseSize;

    void Awake() { _text = GetComponent<TMP_Text>(); _baseSize = _text.fontSize; }
    void OnEnable() { Changed += Apply; Apply(); }
    void OnDisable() { Changed -= Apply; }
    void Apply() { _text.fontSize = _baseSize * Factor; }
}
```

2. Add three buttons (or a dropdown) to Settings: Small `0.85`, Normal `1.0`, Large `1.25`, each setting `TextScale.Factor`.
3. Build And Run, set Large, and check the HUD still fits inside the `SafeArea` panel in your worst-case screen. Fix overflow with TMP **Overflow: Ellipsis** on labels and wrapping on paragraphs, not by shrinking the text again.
4. Body text at Normal should never render below about 14 sp; measure with the Device Simulator at a 1080x2340 phone.

## Part D: Accessibility pass with a neighbour (~15 min)
1. Create `/docs/CA1/accessibility-pass.md` with a two-column table: **Check** and **Finding / Fix**.
2. Run the ten checks on the phone, with a neighbour who has not seen the game for the last one:
   - One-handed reach: every control reachable with the thumb.
   - Smallest button at least 48 dp including padding (measure against a 48 dp reference square in the Device Simulator).
   - Android **Settings > Display > Display size and text** at maximum: HUD still inside the safe area.
   - **Developer options > Simulate colour space > Monochromacy**: every game state still readable.
   - Volume at zero: the game still tells you what happened.
   - Text contrast 4.5:1 (use any online contrast checker with your colours).
   - Motion or screen shake behind a toggle (add the toggle now even if it only stores a value).
   - No timed tap without an alternative or a slower mode.
   - Haptics off: nothing is lost.
   - 60 seconds of silent observation: write down the first thing your neighbour got wrong.
3. For every finding, write the fix and whether it is done today or on the cuts list.

## Part E: Finalise scope lock and submit the MDA (~15 min)
1. Complete `/docs/mda-onepager.md`: title and one-line pitch; aesthetics; 3 to 5 core mechanics; dynamics; progression and content; platform features (touch model, safe area, haptics, text size); performance budget; monetisation and ethics notes; risks; cuts list.
2. The **performance budget** is numbers, not adjectives: target fps (60, or 30 with a reason), frame time 16.7 ms, 99th percentile under 25 ms, peak memory ceiling in MB, cold start under 3 s. Week 3 measures them.
3. The **cuts list** is ordered, at least five items, most expendable first. The core verb, pause and resume, and the device build are never on it.
4. Paste your option's Week 6 vertical-slice target verbatim at the bottom, and mark anything you are adding on top as a stretch goal.
5. Submit on Moodle before you leave. From now on you cut; you do not add.

## Troubleshooting
- **`OnApplicationPause(false)` fires at launch** -> normal on Android; the code above only acts on `true`.
- **Panel does not appear when Home is pressed** -> `PausedChanged` is invoked, but the panel is under an inactive parent; keep the pause panel's parent active and toggle the panel itself.
- **Back closes the app instead of pausing** -> Active Input Handling is not Input System, or `Keyboard.current` is null on this device; test with the legacy `Input.GetKeyDown(KeyCode.Escape)` to confirm and report it.
- **No vibration** -> the phone's system haptics are off, or the toggle default is 0; check `PlayerPrefs` with a `Debug.Log`.
- **Text size change does nothing** -> Auto Size is still on for that text.
- **Audio keeps playing while paused** -> a source has `ignoreListenerPause` on, or plays through a separate listener.

## Checkpoints to commit
1. `Assets/Scripts/LifecycleGuard.cs`, `PauseMenu.cs`, `Haptics.cs`, `TextScale.cs`.
2. `/docs/journal.md` with the completed five-row lifecycle matrix.
3. `/docs/CA1/accessibility-pass.md`.
4. `/docs/mda-onepager.md` (final, matching the Moodle submission).

## Exit ticket
Submit your MDA one-pager with scope lock and cuts list on Moodle before you leave; show me pause and resume surviving an incoming call.

## Reference links
- MonoBehaviour.OnApplicationPause: https://docs.unity3d.com/6000.6/Documentation/ScriptReference/MonoBehaviour.OnApplicationPause.html
- MonoBehaviour.OnApplicationFocus: https://docs.unity3d.com/6000.6/Documentation/ScriptReference/MonoBehaviour.OnApplicationFocus.html
- Handheld.Vibrate: https://docs.unity3d.com/6000.6/Documentation/ScriptReference/Handheld.Vibrate.html
- PlayerPrefs: https://docs.unity3d.com/6000.6/Documentation/ScriptReference/PlayerPrefs.html
- Input System package (Keyboard, Android back as Escape): https://docs.unity3d.com/Packages/com.unity.inputsystem@latest
- TextMeshPro (part of the Unity UI package in Unity 6): https://docs.unity3d.com/Packages/com.unity.ugui@latest
- Android predictive back gesture: https://developer.android.com/guide/navigation/custom-back/predictive-back-gesture
- Android touch target size (Accessibility Scanner guidance): https://support.google.com/accessibility/android/answer/7101858
- Android accessibility principles: https://developer.android.com/guide/topics/ui/accessibility/principles
- MDA: A Formal Approach to Game Design and Game Research (Hunicke, LeBlanc, Zubek, 2004): https://users.cs.northwestern.edu/~hunicke/MDA.pdf
