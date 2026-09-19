# Week 2, Lab A (2h): Release signing, touch input and safe areas

**Module:** Mobile Game Development (A12581) · **Engine:** Unity 6.6 · **Platform:** Android (sideload) · **Date:** Mon 14 Sep 2026
**Feeds:** CA1 Platform Readiness & Publication Awareness (due Sun 4 Oct 2026) · **LOs:** LO1, LO2

> **Goal:** First half: create your release keystore, set the Android Player Settings, build a release-signed APK, sideload it with `adb install -r`, and save a Profiler capture and Logcat snippet as CA1 evidence. Second half: make your core verb respond to touch with Enhanced Touch, and keep the HUD inside `Screen.safeArea`.

## What you will complete today
1. A release keystore stored outside the project and documented (path, alias, validity) in the README, with no secrets in Git.
2. A release-signed APK (IL2CPP, ARM64, App Bundle off) installed with `adb install -r`, with the console output and an on-device screenshot under `/docs/CA1/`.
3. A 30 to 60 s Unity Profiler capture and an Android Logcat launch snippet saved under `/docs/CA1/`.
4. `TapSwipeInput.cs` driving your core verb from tap and swipe, with thresholds in dp.
5. A `SafeArea` panel under the Canvas, verified on your phone's notch in both orientations.

## Pre-flight
- Week 1 checkpoints committed: project builds to the phone, `MobileBootstrap.cs` prints the `[Boot]` line, `.gitignore` already lists `*.keystore` and `*.jks`.
- A password manager (or at least an encrypted note) ready to hold the keystore passwords.
- Phone charged, USB debugging on, `adb devices` shows it as `device`.
- Two empty folders in the repo: `/docs/CA1/profiler/` and `/docs/CA1/logcat/`.

## Part A: Keystore and Player Settings (~15 min)
1. **Edit > Project Settings > Player > Android tab > Publishing Settings > Keystore Manager**.
2. **Keystore > Create New > Anywhere**. Save it **outside** the project folder, for example `~/keystores/mygame-release.keystore`. Enter a keystore password, an alias (for example `mygame`), an alias password, and at least your name for the certificate. Validity 50 years is fine.
3. Back in Publishing Settings confirm **Custom Keystore** is ticked and the alias is selected. Unity does not save the passwords; you re-enter them each Editor session.
4. Record in `README.md`: keystore file name and location (not the password), alias, validity. Put both passwords in your password manager now, and copy the keystore to a second encrypted location before you leave.
5. **Other Settings**: Package Name `com.yourname.gametitle` (unchanged from Week 1), **Version** `0.1.0`, **Bundle Version Code** `1`, **Minimum API Level** at Unity's default (or the oldest phone you must support), **Target API Level** Automatic (highest installed), **Scripting Backend** IL2CPP, **Target Architectures** ARM64 only.
6. **Publishing Settings > Build**: **Build App Bundle (Google Play)** off. **Resolution and Presentation**: **Optimized Frame Pacing** on, **Render outside safe area** on.
7. **File > Build Profiles**: rename the existing Android profile **Android Dev** (Development Build on, **Autoconnect Profiler** on). Duplicate it as **Android Release** with Development Build off. *Expected: two Android profiles in the list; the active one is highlighted.*

## Part B: Release APK, sideload, proof (~20 min)
1. Activate **Android Release** and click **Build**. Save as `Builds/MyGame-0.1.0.apk`. Unity asks for the keystore and alias passwords if it has not got them this session.
2. If the Week 1 development build is still on the phone, uninstall it first: the release keystore differs from the debug key and Android refuses to replace one with the other.

```bash
adb uninstall com.yourname.mygame
adb install -r Builds/MyGame-0.1.0.apk > docs/CA1/install-proof.txt
adb shell monkey -p com.yourname.mygame 1
```

3. Open `docs/CA1/install-proof.txt` and check it says `Success`. Add one line at the top with the date, device model and the APK name.
4. Take a screenshot on the phone with the game running (volume down + power on most phones). Copy it to `docs/CA1/device-screenshot.png`. The device model must be readable somewhere: either the About screen you will add in Week 12, or a note in the README for now.
5. Raise **Bundle Version Code** to `2` now, so the next install does not fail with a downgrade error.

## Part C: First Profiler capture and Logcat snippet (~20 min)
1. Activate **Android Dev** and click **Build And Run** with the phone selected under **Run Device**.
2. **Window > Analysis > Profiler**. The target drop-down (top left) should list `AndroidPlayer (...)`. Select it. Turn **Deep Profile** off.
3. Keep only **CPU Usage**, **Rendering** and **Memory** modules; remove the rest with the Profiler Modules drop-down.
4. Press **Record**, play the game for 30 to 60 s (real play, not the menu), press **Record** again.
5. **Save** (disk icon) to `docs/CA1/profiler/w02-first-capture.data`. Write the three headline numbers in `/docs/journal.md`: CPU main thread ms in a typical frame, SetPass calls, GC allocated in frame.
6. **Window > Package Manager > Unity Registry**, install **Android Logcat**. Open **Window > Analysis > Android Logcat**, pick your device and filter by your package name.
7. Force stop the app on the phone, clear the log, launch the app, wait for the first scene, then **Save** the log to `docs/CA1/logcat/launch.txt`.

If the Profiler cannot see the phone: `adb forward tcp:34999 localabstract:Unity-com.yourname.mygame`, then choose `<Enter IP>` in the target drop-down and type `127.0.0.1`.

## Part D: Drive your core verb from touch (~25 min)
1. **Window > Package Manager > Unity Registry > Input System**, install, and accept the prompt to restart with the new backend. Confirm **Player Settings > Other Settings > Active Input Handling** is **Input System Package (New)**.
2. Create `Assets/Scripts/TapSwipeInput.cs` and attach it to `Bootstrap`:

```csharp
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class TapSwipeInput : MonoBehaviour
{
    public float swipeDp = 50f;     // distance in dp, not pixels
    public float tapMax = 0.3f;     // seconds

    void OnEnable() => EnhancedTouchSupport.Enable();
    void OnDisable() => EnhancedTouchSupport.Disable();

    void Update()
    {
        foreach (var t in Touch.activeTouches)
        {
            if (t.phase != TouchPhase.Ended) continue;
            float px = swipeDp * Mathf.Max(Screen.dpi, 160f) / 160f;
            Vector2 d = t.screenPosition - t.startScreenPosition;
            if (d.magnitude >= px) Debug.Log("Swipe " + d.normalized);
            else if (t.time - t.startTime < tapMax) Debug.Log("Tap");
        }
    }
}
```

3. Build And Run (Android Dev) and watch `adb logcat -s Unity` while you tap and swipe. *Expected: `Tap` and `Swipe (x, y)` lines with the direction normalised.*
4. Replace the two `Debug.Log` calls with your verb: jump or dash for the platformer, lane change for the runner, select and move for tactics, tap-to-collect for idle, aim and fire for the arena. Keep the recogniser and the verb in separate scripts if the verb needs state.
5. If your verb needs a drag (joystick, aiming), handle `TouchPhase.Moved` in the same loop and use `t.delta` for the per-frame movement. Do not let a drag also fire as a swipe on release: set a flag once the finger has moved past the slop.
6. Tune `swipeDp` and `tapMax` on the phone, not in the Editor. Note the values you settle on in the journal.

## Part E: SafeArea panel, verified on the phone (~15 min)
1. In your UI Canvas (Canvas Scaler: **Scale With Screen Size**, reference 1080x1920 or 1920x1080 to match your orientation) add an empty child `SafeArea`, anchor preset **stretch/stretch**, all offsets 0.
2. Move every HUD element under `SafeArea`. Backgrounds and world-space content stay outside it.
3. Create `Assets/Scripts/SafeArea.cs` and attach it to the panel:

```csharp
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class SafeArea : MonoBehaviour
{
    RectTransform _rt;
    Rect _applied;

    void Awake() { _rt = GetComponent<RectTransform>(); Apply(); }
    void Update() { if (Screen.safeArea != _applied) Apply(); }

    void Apply()
    {
        _applied = Screen.safeArea;
        Vector2 min = _applied.position;
        Vector2 max = _applied.position + _applied.size;
        min.x /= Screen.width; min.y /= Screen.height;
        max.x /= Screen.width; max.y /= Screen.height;
        _rt.anchorMin = min; _rt.anchorMax = max;
        _rt.offsetMin = Vector2.zero; _rt.offsetMax = Vector2.zero;
    }
}
```

4. **Window > General > Device Simulator**, pick a notched device, rotate it. *Expected: the HUD moves in from the notch and the gesture bar; the background does not.*
5. Build And Run on the phone. Rotate (if your game allows it), cover the notch with a finger to see nothing sits under it, and swipe up from the bottom edge to confirm no button lives in the gesture zone.

## Troubleshooting
- **`INSTALL_FAILED_UPDATE_INCOMPATIBLE`** -> the installed build was signed with a different key; `adb uninstall` it first.
- **Unity asks for keystore passwords on every build** -> normal; they are not stored. Keep the Editor open between builds.
- **Profiler target list is empty** -> Development Build and Autoconnect Profiler must both be on in the active profile; otherwise use the `adb forward` line above.
- **No `Tap` or `Swipe` in the log** -> Active Input Handling is still Input Manager (Old); switch to Input System and rebuild.
- **`Touch` is ambiguous** -> you are missing the `using Touch = ...` alias at the top of the script.
- **HUD ignores the notch** -> `SafeArea` is not directly under the Canvas, or its anchors were not stretch/stretch before the script ran.

## Checkpoints to commit
1. `README.md`: keystore location (not the file), alias, validity; the two Build Profiles and what each is for.
2. `docs/CA1/install-proof.txt` and `docs/CA1/device-screenshot.png`.
3. `docs/CA1/profiler/w02-first-capture.data` and `docs/CA1/logcat/launch.txt`.
4. `Assets/Scripts/TapSwipeInput.cs` and `Assets/Scripts/SafeArea.cs`, plus your verb script.
5. `/docs/journal.md`: headline Profiler numbers and the touch thresholds you settled on.

## Exit ticket
Commit install-proof.txt, the on-device screenshot and the Profiler capture, then show me your verb responding to a swipe inside the safe area on your phone.

## Reference links
- Android Keystore Manager (6000.6): https://docs.unity3d.com/6000.6/Documentation/Manual/android-keystore-manager.html
- Android Player settings (6000.6): https://docs.unity3d.com/6000.6/Documentation/Manual/class-PlayerSettingsAndroid.html
- Build Profiles (6000.6): https://docs.unity3d.com/6000.6/Documentation/Manual/build-profiles.html
- Profiling on an Android device (6000.6): https://docs.unity3d.com/6000.6/Documentation/Manual/android-profile-on-an-android-device.html
- Unity Profiler (6000.6): https://docs.unity3d.com/6000.6/Documentation/Manual/Profiler.html
- Android Logcat package: https://docs.unity3d.com/Packages/com.unity.mobile.android-logcat@latest
- Input System package (Touch support is under "Devices > Touch"): https://docs.unity3d.com/Packages/com.unity.inputsystem@latest
- Screen.safeArea: https://docs.unity3d.com/6000.6/Documentation/ScriptReference/Screen-safeArea.html
- Screen.cutouts: https://docs.unity3d.com/6000.6/Documentation/ScriptReference/Screen-cutouts.html
- Device Simulator (6000.6): https://docs.unity3d.com/6000.6/Documentation/Manual/device-simulator.html
- Android Debug Bridge (adb): https://developer.android.com/tools/adb
- Android touch target size (Accessibility Scanner guidance): https://support.google.com/accessibility/android/answer/7101858
