# Week 4, Lab B (2h): Store-awareness assets and the CA1 submission

**Module:** Mobile Game Development (A12581) · **Engine:** Unity 6.6 · **Platform:** Android (sideload) · **Date:** Wed 30 Sep 2026
**Feeds:** CA1 Platform Readiness & Publication Awareness (due Sun 4 Oct 2026, 23:59) · **LOs:** LO2

> **Goal:** Finish the publication-awareness half of CA1: a store-asset checklist with the real Play sizes, listing copy that describes the game you can show, a one-page data-use statement written from evidence, and a packaged, peer-checked submission.

## What you will complete today
1. `docs/CA1/store-assets-checklist.md` (icon 512x512, feature graphic 1024x500, 4 to 6 screenshot plan) and `docs/CA1/descriptions.md` (short and long copy).
2. `docs/CA1/privacy-statement.md`: one page that matches the permissions and behaviour of your actual APK.
3. Install proof and on-device screenshot captured with `adb`.
4. The CA1 folder tree complete, README updated, tag `v0.2.0-ca1`, ZIP or release link ready.
5. A peer check against the rubric, with gaps fixed.

## Pre-flight
- Lab A checkpoints done: `releases/MyGame-0.2.0-arm64.apk`, `docs/CA1/install-proof.txt`, `docs/CA1/device-matrix.md`, README Signing and Build sections.
- MDA one-pager (Week 2) and dev journal in the repo.
- Phone plugged in, `adb devices` shows `device`; the release build installed.
- Git Bash for the `grep` commands.

## Part A: Store-asset checklist (~25 min)
You are not uploading anything this semester. You are proving you know the specs and can plan the assets. Sizes below come from the Play Console help page **Add preview assets**; open it and confirm them on the day you submit.

1. Create `docs/CA1/store-assets-checklist.md` with a table:

   | Asset | Spec | Status | Notes |
   |-------|------|--------|-------|
   | App icon | 512x512, 32-bit PNG with alpha, up to 1024 KB | planned | no rounded corners, Play masks it |
   | Feature graphic | 1024x500, JPEG or 24-bit PNG, no alpha | planned | shown above a listing video |
   | Phone screenshots | 2 to 8; each side 1080 to 7680 px; JPEG or 24-bit PNG; 16:9 or 9:16 | planned | gameplay first |

2. Below the table, write the **screenshot plan**: 4 to 6 rows, each with a working caption and what must be visible (for example "Shot 1: mid-run, biome 2, score visible, no UI overlap with the notch"). Match your game's orientation.
3. Note where each asset will come from in Week 10 (in-game capture at 1080 px or more; icon from your MDA's key art).
4. Create `docs/CA1/descriptions.md`:
   - **Short description**: up to 80 characters. Genre, hook, feel. Count the characters.
   - **Long description**: up to 4000 characters. Features in order of importance, only what the vertical slice will contain by CA2. State offline play, no ads, no purchases if that is true.

*Expected result: two files; the short description is 80 characters or fewer; every feature in the long description exists or is on the scope-locked plan.*

## Part B: Data-use statement (~25 min)
The statement must describe what the APK actually does, not what a future version might do. Start from evidence.

1. List the permissions the installed build requests:
   ```bash
   adb shell dumpsys package com.yourname.mygame | grep -A4 requested
   ```
   *Expected: `requested permissions:` followed by zero or more `android.permission.*` lines. Unity adds `INTERNET` only when **Internet Access** is set to Require or a package needs it; `VIBRATE` appears if you used `Handheld.Vibrate` in Week 2.*
2. Search your code for anything that talks to the network or writes files: `UnityWebRequest`, `PlayerPrefs`, `File.`, `persistentDataPath`, `Analytics`, `Ads`.
3. Write `docs/CA1/privacy-statement.md` (one page) with these headings:
   - **Data collected**: for most of you, "none". Say it explicitly.
   - **Data stored on the device**: PlayerPrefs keys, save files (path: `Application.persistentDataPath`), and note that the Week 6 telemetry log will be local only. How a player removes it: uninstall.
   - **Network activity**: none, or each endpoint and why.
   - **Third-party SDKs**: none this semester.
   - **Permissions requested**: paste the `dumpsys` list; one line of justification each.
   - **How this would be declared on Play**: map each heading to the Data safety form sections (data collection and security practices, data sharing, and the privacy policy URL that would be required). Note that the form is required before release on closed, open or production tracks, and that the internal testing track is exempt.
4. Read it back against the build: if the statement says "no network" and `INTERNET` is in the permission list, fix one or the other.

## Part C: Capture the proofs (~15 min)
1. Install proof (redo it so the file is clean):
   ```bash
   adb install -r releases/MyGame-0.2.0-arm64.apk \
     > docs/CA1/install-proof.txt
   adb shell dumpsys package com.yourname.mygame | grep version \
     >> docs/CA1/install-proof.txt
   ```
2. Device model and Android version for the caption and the device matrix:
   ```bash
   adb shell getprop ro.product.model
   adb shell getprop ro.build.version.release
   ```
3. On-device screenshot with the game running:
   ```bash
   adb shell screencap -p /sdcard/ca1.png
   adb pull /sdcard/ca1.png docs/CA1/device-screenshot.png
   ```
4. If the model is not visible in the shot, add a caption line to `install-proof.txt`: "Screenshot taken on <model>, Android <version>".

*Expected result: `install-proof.txt` contains `Success` and `versionCode=2`; `device-screenshot.png` shows your game.*

## Part D: Package and tag (~15 min)
1. Build the folder tree exactly as the checklist expects:
   ```
   releases/MyGame-0.2.0-arm64.apk
   docs/CA1/install-proof.txt
   docs/CA1/device-screenshot.png
   docs/CA1/device-matrix.md
   docs/CA1/store-assets-checklist.md
   docs/CA1/descriptions.md
   docs/CA1/privacy-statement.md
   docs/CA1/mda-onepager.md
   docs/dev-journal.md
   README.md
   ```
2. Move or copy your MDA one-pager into `docs/CA1/mda-onepager.md` if it lives elsewhere; confirm it contains the scope lock, the cuts list and a performance budget.
3. README: confirm Signing (no secrets), Build, Device targets, and an AI-assistance note if applicable.
4. Commit and tag:
   ```bash
   git add -A && git commit -m "CA1 package"
   git tag v0.2.0-ca1
   ```
5. Prepare the submission: either a ZIP of the repo at that tag (including `releases/`), or a repo release link pointing at `v0.2.0-ca1` with the APK attached.

## Part E: Peer check (~10 min)
1. Swap laptops with a neighbour. Open their repo at the tag.
2. Tick each rubric line against the actual files:
   - Signed build on device (40): IL2CPP + ARM64 config; keystore documented, no secrets; install proof and screenshot; package name stable, versionCode bumped.
   - Publication awareness pack (30): asset checklist with the sizes above; privacy statement matches `dumpsys`; short and long descriptions present and honest.
   - MDA and scope (20): MDA, risks, cuts list, performance budget; realistic scope lock.
   - Repo hygiene (10): README build steps, release artefacts in `releases/`, tag present.
3. Write the gaps on a sticky note, swap back, fix them before you leave.

## Troubleshooting
- **`grep -A4` prints nothing** -> the package name is wrong; `adb shell pm list packages | grep yourname` to find it.
- **`adb pull` says "remote object does not exist"** -> the screenshot went to a different storage root; try `/storage/emulated/0/ca1.png`.
- **`install-proof.txt` is empty in PowerShell** -> `>` captured nothing because `adb` wrote to stderr; use Git Bash, or `adb install -r ... 2>&1 | Tee-Object docs/CA1/install-proof.txt`.
- **Long description over 4000 characters** -> cut features you cannot show; the CA2 slice is one lane.
- **Statement says no network but `INTERNET` is listed** -> **Player Settings > Other Settings > Internet Access** is Require; set it to Auto and rebuild, or document the reason.

## Checkpoints to commit
1. `docs/CA1/store-assets-checklist.md` and `docs/CA1/descriptions.md`.
2. `docs/CA1/privacy-statement.md`.
3. `docs/CA1/install-proof.txt` and `docs/CA1/device-screenshot.png`.
4. `docs/CA1/mda-onepager.md`, `docs/dev-journal.md`, updated `README.md`.
5. Tag `v0.2.0-ca1`; ZIP or release link submitted on Moodle by **Sun 4 Oct 2026, 23:59**.

## Exit ticket
Show your neighbour-checked CA1 folder tree and the first line of your data-use statement before you leave; submit on Moodle by Sun 4 Oct 2026, 23:59.

## Reference links
- Play: add preview assets (icon, feature graphic, screenshot specs): https://support.google.com/googleplay/android-developer/answer/9866151
- Play: Data safety form: https://support.google.com/googleplay/android-developer/answer/10787469
- Play: target API level requirements: https://support.google.com/googleplay/android-developer/answer/11926878
- Android permissions overview: https://developer.android.com/guide/topics/permissions/overview
- Unity 6.6: Android permissions: https://docs.unity3d.com/6000.6/Documentation/Manual/android-permissions-in-unity.html
- Unity 6.6: `Application.persistentDataPath`: https://docs.unity3d.com/6000.6/Documentation/ScriptReference/Application-persistentDataPath.html
- Unity 6.6 Android Player settings (Internet Access, Identification): https://docs.unity3d.com/6000.6/Documentation/Manual/class-PlayerSettingsAndroid.html
- Android Debug Bridge reference (`screencap`, `pull`, `getprop`): https://developer.android.com/tools/adb
