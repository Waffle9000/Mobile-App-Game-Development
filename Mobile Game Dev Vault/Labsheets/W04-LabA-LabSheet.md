# Week 4, Lab A (2h): Harden the release pipeline: signing and versioning

**Module:** Mobile Game Development (A12581) · **Engine:** Unity 6.6 · **Platform:** Android (sideload) · **Date:** Mon 28 Sep 2026
**Feeds:** CA1 Platform Readiness & Publication Awareness (due Sun 4 Oct 2026, 23:59) · **LOs:** LO2

> **Goal:** Turn the build you made in Week 2 into a repeatable release pipeline: stable package name, bumped versionCode, a keystore that is backed up and documented without secrets, and an APK that installs cleanly on a phone that is not yours.

## What you will complete today
1. Identification settings locked: package name, Version `0.2.0`, Bundle Version Code `2`, Target API Level Automatic.
2. Release keystore backed up to two places and documented (path, alias, validity) in the README, with no passwords anywhere in the repo.
3. A release-signed APK (IL2CPP, ARM64) installed on your phone with `dumpsys` proof of versionCode 2.
4. The same APK installed on a neighbour's phone, both results recorded in `docs/CA1/device-matrix.md`.
5. README build steps from clone to phone, tagged `v0.2.0`.

## Pre-flight
- Unity 6.6 with Android Build Support; project switched to Android in **File > Build Profiles**.
- Your release keystore from Week 2 (you know where the file is and you know its two passwords).
- Phone with USB debugging on, cable, and `adb devices` showing it as `device`.
- Git Bash available (for `grep` and `keytool`), or be ready to use the PowerShell equivalents in the Troubleshooting section.
- `docs/CA1/` folder exists in the repo (create it now if not).

## Part A: Set Identification (~15 min)
1. Open **Edit > Project Settings > Player**, Android tab, **Other Settings > Identification**.
2. **Package Name**: `com.yourname.gametitle`, all lower case, no spaces. This never changes again: a new name is a different app to Android.
3. **Version**: `0.2.0` (versionName; humans read it).
4. **Bundle Version Code**: `2` (versionCode; Android compares it as an integer and refuses downgrades).
5. **Minimum API Level**: leave Unity's default unless a feature you use needs more. Write the value down for the README device targets.
6. **Target API Level**: **Automatic (highest installed)**. Play requires new apps and updates to target a recent API level and the number moves every year; link the current policy page in your README rather than copying a number: https://support.google.com/googleplay/android-developer/answer/11926878
7. **Configuration**: Scripting Backend **IL2CPP**, Target Architectures **ARM64** ticked (ARMv7 optional). Play requires 64-bit native code; this is how you satisfy it.

*Expected result: the Identification block shows your package name, 0.2.0 and 2; ARM64 is ticked and not greyed out.*

## Part B: Back up and document the keystore (~15 min)
1. Locate the keystore file you created in **Publishing Settings > Keystore Manager** in Week 2. If it sits inside the project folder, move it out now (for example `~/keys/mygame.keystore`) and re-select it in **Publishing Settings > Project Keystore**.
2. Copy it to two safe places: an attachment in your password manager, and an encrypted drive or cloud folder with 2FA. Two copies, two locations.
3. Store both passwords (keystore and key) in the password manager, never in a text file in the repo.
4. Confirm the repo ignores keystores. Add to `.gitignore`:
   ```
   *.keystore
   *.jks
   ```
   Then run `git status` and make sure no keystore is tracked (`git rm --cached` it if one is).
5. Document the keystore without secrets. `keytool` ships inside Unity's OpenJDK, for example `C:\Program Files\Unity\Hub\Editor\<version>\Editor\Data\PlaybackEngines\AndroidPlayer\OpenJDK\bin`. Run:
   ```bash
   keytool -list -v -keystore ~/keys/mygame.keystore -alias mygame
   ```
   It asks for the keystore password; the output shows `Alias name`, `Valid from ... until`, and the certificate fingerprints.
6. Add a **Signing** section to the README: keystore location (outside the repo), alias, validity dates, SHA-256 fingerprint. **No passwords.**

*Expected result: `git status` shows no keystore; README has a Signing section with alias and validity only.*

## Part C: Release build and install proof (~15 min)
1. **File > Build Profiles > Android**. Development Build **off**, Build App Bundle **off** (CA1 submits an APK). Build to `releases/MyGame-0.2.0-arm64.apk` (create `releases/` at the repo root; it is tracked).
2. Install and verify:
   ```bash
   adb install -r releases/MyGame-0.2.0-arm64.apk
   adb shell dumpsys package com.yourname.mygame | grep version
   ```
   *Expected: `Success`, then a line with `versionCode=2` and one with `versionName=0.2.0`.*
3. If you see `INSTALL_FAILED_UPDATE_INCOMPATIBLE`, a debug-signed build is still installed: `adb uninstall com.yourname.mygame`, then install again.
4. If you see `INSTALL_FAILED_VERSION_DOWNGRADE`, your phone has a higher versionCode than the build: bump Bundle Version Code and rebuild. (`adb install -r -d` forces a downgrade for a quick test; never rely on it for a hand-in.)
5. Paste the install output and the `dumpsys` lines into `docs/CA1/install-proof.txt`.

## Part D: Sideload rehearsal (~25 min)
1. Swap phones with a neighbour. Plug theirs in, accept the USB debugging prompt on their screen, run `adb devices -l` and note the serial.
2. Install with the serial explicitly (two phones plugged in is where `adb` gets confused):
   ```bash
   adb -s <serial> install -r releases/MyGame-0.2.0-arm64.apk
   adb -s <serial> shell dumpsys package com.yourname.mygame | grep version
   ```
3. Launch the game on their phone and play for one minute. Watch for: safe-area problems on a different notch, aspect-ratio clipping, a different refresh rate, an install-permission prompt on some brands (Xiaomi and others require **Install via USB** in Developer options).
4. Create `docs/CA1/device-matrix.md` with one row per phone:

   | Device | Android | Serial (last 4) | Install result | Notes |
   |--------|---------|-----------------|----------------|-------|
   | Your phone | ... | ... | Success, versionCode 2 | ... |
   | Neighbour's phone | ... | ... | ... | anything odd |

5. Uninstall your game from their phone when you are done (`adb -s <serial> uninstall com.yourname.mygame`), then install their build on yours and fill in their matrix.

*Expected result: two rows in the matrix, both with versionCode 2, and at least one observation about the other device.*

## Part E: README build steps (~20 min)
1. Add a **Build** section to the README with numbered steps a stranger could follow: clone, open in Unity 6.6, switch platform, Player Settings checklist (IL2CPP, ARM64, package name, versionCode), Build Profiles, output path, `adb install -r`.
2. Add a **Device targets** section: minimum API, tested devices (from the matrix).
3. Add an **AI assistance** note if you used AI for anything substantial.
4. Commit everything and tag:
   ```bash
   git add -A && git commit -m "Release pipeline hardened: v0.2.0, versionCode 2"
   git tag v0.2.0
   ```

## Troubleshooting
- **ARM64 greyed out** -> Scripting Backend is Mono. Set it to IL2CPP, then tick ARM64.
- **`grep` not recognised in PowerShell** -> `adb shell dumpsys package com.yourname.mygame | Select-String version`.
- **`keytool` not found** -> use the full path to Unity's OpenJDK `bin` folder, or add it to PATH for this session.
- **`INSTALL_FAILED_UPDATE_INCOMPATIBLE` on the neighbour's phone** -> they had your older or debug build; uninstall first.
- **Phone shows as `unauthorized`** -> unlock it and accept the USB debugging dialog; `adb kill-server` then `adb devices` if the dialog does not appear.
- **Build fails with a Gradle or SDK error after changing Target API** -> set it back to Automatic; Unity installs what it needs with the Android module.

## Checkpoints to commit
1. `releases/MyGame-0.2.0-arm64.apk` (release-signed, IL2CPP, ARM64).
2. `docs/CA1/install-proof.txt` with the `adb install -r` output and the `dumpsys` version lines.
3. `docs/CA1/device-matrix.md` with two rows.
4. `README.md` with Signing, Build, Device targets sections; `.gitignore` excluding keystores.
5. Git tag `v0.2.0`.

## Exit ticket
Show dumpsys output with versionCode 2 on your phone and on a neighbour's, and your README build section, before you leave.

## Reference links
- Unity 6.6 Android Player settings: https://docs.unity3d.com/6000.6/Documentation/Manual/class-PlayerSettingsAndroid.html
- Unity 6.6 Android Keystore Manager: https://docs.unity3d.com/6000.6/Documentation/Manual/android-keystore-manager.html
- Unity 6.6 Build Profiles: https://docs.unity3d.com/6000.6/Documentation/Manual/build-profiles.html
- Android: version your app (versionCode and versionName): https://developer.android.com/studio/publish/versioning
- Android: sign your app (keystore concepts): https://developer.android.com/studio/publish/app-signing
- Android Debug Bridge reference: https://developer.android.com/tools/adb
- Play: target API level requirements (check the current page): https://support.google.com/googleplay/android-developer/answer/11926878
- Play: 64-bit requirement: https://developer.android.com/google/play/requirements/64-bit
- `keytool` reference: https://docs.oracle.com/en/java/javase/17/docs/specs/man/keytool.html
