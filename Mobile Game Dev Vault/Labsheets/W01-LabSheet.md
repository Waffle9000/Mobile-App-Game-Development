# Week 1 (2h): Kick-off, constraints and project selection

**Module:** Mobile Game Development (A12581) · **Engine:** Unity 6.6 · **Platform:** Android (sideload) · **Date:** Wed 9 Sep 2026
**Feeds:** CA1 Platform Readiness & Publication Awareness (due Sun 4 Oct 2026) · **LOs:** LO1

> **Goal:** Get Unity 6.6 building to your own Android phone with `adb install -r`, create the Git repository that will hold every CA, and shortlist one of the five project options. Week 1 has a single lab this year; from Week 2, Lab A is Monday and Lab B is Wednesday.

## What you will complete today
1. Unity 6.6 installed with Android Build Support, and your phone visible to `adb devices`.
2. A new URP project switched to Android with `MobileBootstrap.cs` in the first scene.
3. A Development Build (IL2CPP, ARM64) installed on your phone with `adb install -r`, and its `[Boot]` line read from `adb logcat`.
4. A Git repository with a Unity `.gitignore` and a README that names your device, Android version and graphics API.
5. One project option shortlisted in `/docs/journal.md`, and the MDA one-pager template copied into `/docs/mda-onepager.md`.

## Pre-flight
- Laptop with at least 25 GB free (Unity 6.6 plus the Android modules is large) and a working USB port.
- Your own Android phone with about 1 GB free, and a **data** USB cable (many charging cables have no data lines).
- A GitHub, GitLab or SETU Git account you can push to.
- Unity Hub installed and signed in (Personal licence is fine).
- Windows only: if the phone does not show up later, you may need the Google USB driver or your phone maker's OEM driver (links at the end).

## Part A: Install Unity 6.6 with Android Build Support (~15 min)
1. Open **Unity Hub > Installs > Install Editor** and pick the current **Unity 6.6** release. Never pin a patch version in your notes; the module tracks the current 6.x release.
2. Tick **Android Build Support**, and under it **Android SDK & NDK Tools** and **OpenJDK**. Unity manages these; do not install a separate Android Studio SDK for this module.
3. Let it download while you do Part B. *Expected: the install shows an Android icon under Installs when finished.*
4. Optional but recommended: **Edit > Preferences > External Tools** should list the SDK, NDK and JDK paths as "Installed with Unity" once the editor is open.

## Part B: Developer options and USB debugging on the phone (~10 min)
1. **Settings > About phone** (on some phones **Software information**) and tap **Build number** seven times until it says you are a developer.
2. **Settings > System > Developer options** (Samsung: **Settings > Developer options**) and turn on **USB debugging**.
3. Plug in the phone. On the prompt **Allow USB debugging?** tick **Always allow from this computer** and tap **Allow**.
4. On the phone, set the USB mode to **File transfer** if it defaults to charging only; some makes hide ADB behind that setting.
5. Write your phone model and Android version on paper now; you need them for the exit ticket and the README.

## Part C: New URP project, Android platform, bootstrap script (~20 min)
1. **Unity Hub > Projects > New project**, template **Universal 3D** (or **Universal 2D** if your option is 2D), Unity 6.6, project name without spaces, for example `ArenaSurvivor`.
2. **File > Build Profiles**, select **Android**, click **Switch Platform**. *Expected: the Android profile becomes active and the Editor re-imports assets.*
3. In the same window open **Player Settings** and set, under **Other Settings**: **Scripting Backend** IL2CPP, **Target Architectures** ARM64 only (untick ARMv7). ARM64 is greyed out until IL2CPP is selected.
4. Set the **Package Name** to `com.yourname.gametitle` (lower case, no hyphens). It must never change again this semester.
5. Create `Assets/Scripts/MobileBootstrap.cs`, attach it to an empty GameObject called `Bootstrap` in your first scene:

```csharp
using UnityEngine;

public class MobileBootstrap : MonoBehaviour
{
    void Awake()
    {
        // Android defaults to 30 fps when targetFrameRate is -1.
        Application.targetFrameRate = 60;
        // vSyncCount is ignored on Android; keep it at 0.
        QualitySettings.vSyncCount = 0;
        // Keep the screen on while the game is in the foreground.
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        Debug.Log($"[Boot] {SystemInfo.deviceModel} | " +
                  $"{SystemInfo.operatingSystem} | " +
                  $"{SystemInfo.graphicsDeviceType} | " +
                  $"{Screen.width}x{Screen.height} @ {Screen.dpi} dpi");
    }
}
```

6. Add the scene to the build profile's **Scene List** if it is not already there. *Expected: pressing Play in the Editor prints a `[Boot]` line with your PC's details; on the phone it will print the phone's.*

## Part D: First device build with adb install -r (~25 min)
1. In **Build Profiles > Android** tick **Development Build**. Leave **Autoconnect Profiler** off for today.
2. Click **Build**, create a folder `Builds` next to `Assets` (not inside it), and name the file `MyGame-dev.apk`. The first IL2CPP build takes several minutes. *Expected: an `.apk` in `Builds/`.*
3. Open a terminal in the project folder. Unity's ADB lives in the editor install; add it to your PATH or call it by full path, for example on Windows `C:\Program Files\Unity\Hub\Editor\<version>\Editor\Data\PlaybackEngines\AndroidPlayer\SDK\platform-tools\adb.exe`.
4. Check the phone is visible, install, launch and read the log:

```bash
adb devices
# List of devices attached
# R58M12ABCDE   device

adb install -r Builds/MyGame-dev.apk
# Performing Streamed Install
# Success

adb shell monkey -p com.yourname.mygame 1
adb logcat -s Unity
# [Boot] SM-S911B | Android OS 15 / API-35 | Vulkan | 1080x2340 @ 425 dpi
```

5. Copy the `[Boot]` line into your README. `Ctrl+C` stops `logcat`.
6. Now try **Build And Run** from the same profile with the phone selected under **Run Device**. It does the install and launch for you; you have just seen what it does underneath.

## Part E: Create your Git repository (~10 min)
1. `git init` in the project folder, then add the standard Unity `.gitignore` (link below). It excludes `Library/`, `Temp/`, `Logs/`, `Builds/` and user settings.
2. Add these lines to `.gitignore` now, before Week 2 creates the files: `*.keystore`, `*.jks`.
3. Create `README.md` with: game title and option number, your phone model, Android version, graphics API from the `[Boot]` line, and the three build steps above.
4. Create `/docs/journal.md` with today's date and three lines on what worked and what did not.
5. Commit and push. Branch names from now on: `feat/...` and `fix/...`.

## Part F: Shortlist one option and start the MDA (~10 min)
1. In `/docs/journal.md`, write one paragraph: the option you are shortlisting, its **core verb** (the one action the player repeats), and the first thing you would cut if time ran out.
2. Copy the MDA one-pager template from Moodle into `/docs/mda-onepager.md`. Fill in the title and one-line pitch only; the rest is homework for **Wed 16 Sep 2026** (Week 2 Lab B), when scope is locked.
3. Copy your option's Week 6 vertical-slice target from the Student Project Brief to the bottom of the page, verbatim.

## Troubleshooting
- **`adb devices` shows nothing** -> try another cable or port; confirm USB debugging is on and the phone prompt was accepted; on Windows install the Google USB driver or the OEM driver.
- **`adb devices` shows `unauthorized`** -> look at the phone and tap Allow; if the prompt never appears, revoke USB debugging authorisations in Developer options and replug.
- **ARM64 is greyed out** -> set Scripting Backend to IL2CPP first.
- **Build fails with an SDK, NDK or JDK error** -> Edit > Preferences > External Tools: all three must say installed with Unity; if not, add the modules from Unity Hub.
- **`INSTALL_FAILED_VERSION_DOWNGRADE`** -> raise Bundle Version Code in Player Settings, or add `-d` to `adb install` once.
- **App installs but shows a black screen** -> check `adb logcat -s Unity` for exceptions; most often the first scene is missing from the Scene List.

## Checkpoints to commit
1. `README.md` with device model, Android version, graphics API and build steps.
2. `.gitignore` (Unity template plus `*.keystore` and `*.jks`).
3. `Assets/Scripts/MobileBootstrap.cs`.
4. `/docs/journal.md` with the shortlisted option, core verb and first cut.
5. `/docs/mda-onepager.md` skeleton with title, pitch and the Week 6 slice target.

## Exit ticket
Post your phone model, Android version and shortlisted project option in the Moodle forum before you leave.

## Reference links
- Installing Unity with Unity Hub (6000.6): https://docs.unity3d.com/6000.6/Documentation/Manual/GettingStartedInstallingUnity.html
- Android environment setup (6000.6): https://docs.unity3d.com/6000.6/Documentation/Manual/android-sdksetup.html
- Build Profiles (6000.6): https://docs.unity3d.com/6000.6/Documentation/Manual/build-profiles.html
- Android Player settings (6000.6): https://docs.unity3d.com/6000.6/Documentation/Manual/class-PlayerSettingsAndroid.html
- IL2CPP overview (6000.6): https://docs.unity3d.com/6000.6/Documentation/Manual/scripting-backends-il2cpp.html
- Application.targetFrameRate: https://docs.unity3d.com/6000.6/Documentation/ScriptReference/Application-targetFrameRate.html
- Enable Developer options and USB debugging: https://developer.android.com/studio/debug/dev-options
- Android Debug Bridge (adb): https://developer.android.com/tools/adb
- Google USB Driver (Windows): https://developer.android.com/studio/run/win-usb
- OEM USB drivers: https://developer.android.com/studio/run/oem-usb
- Unity .gitignore template: https://github.com/github/gitignore/blob/main/Unity.gitignore
- MDA: A Formal Approach to Game Design and Game Research (Hunicke, LeBlanc, Zubek, 2004): https://users.cs.northwestern.edu/~hunicke/MDA.pdf
