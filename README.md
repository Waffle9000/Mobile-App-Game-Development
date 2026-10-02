# Whopper

**Option:** 2 – Endless Runner
**Engine:** Unity 6.6 (6000.6.0f1) · **Platform:** Android (sideload)

## Test device
- Phone: Samsung Galaxy A02s (SM-A025F)
- Android: 12 (API 31)
- Graphics API: OpenGL ES 3
- Boot line:
  `[Boot] samsung SM-A025F | Android OS 12 / API-31 (SP1A.210812.016/A025FXXS8CXH1) | OpenGLES3 | 720x1600 @ 280 dpi`

## Build profiles
- **Android Dev**: Development Build on, Autoconnect Profiler on. Used for profiling and testing.
- **Android Release**: Development Build off, App Bundle off. Used for release APKs.

## Build
1. Clone the repo and open the project folder in **Unity 6.6** (6000.6.0f1).
2. **File > Build Profiles**, select **Android**, then **Switch Platform**.
3. **Edit > Project Settings > Player > Android > Other Settings**, and check:
   - Package Name: `com.archil.whopper`
   - Version: `0.2.0`, Bundle Version Code: `4`
   - Scripting Backend: **IL2CPP**
   - Target Architectures: **ARM64** + **ARMv7**
4. **Publishing Settings**: select the release keystore (see Signing) and enter its passwords.
5. **File > Build Profiles > Android Release**, then **Build**.
6. Save to `releases/MyGame-0.2.0-arm64.apk`.
7. Install on a phone with USB debugging on:
```
adb devices
adb install -r releases/MyGame-0.2.0-arm64.apk
adb shell monkey -p com.archil.whopper 1
adb logcat -s Unity
```

## Device targets
- Minimum API: 26 (Android 8.0)
- Target API: Automatic (highest installed); see Play's current policy:
  https://support.google.com/googleplay/android-developer/answer/11926878
- Tested devices: see `docs/CA1/device-matrix.md`

## Signing
- Keystore file: `mygame-release.keystore`, stored **outside the repo**
  (local Desktop/Keystores folder, backup on OneDrive)
- Keystore type: PKCS12
- Alias: `mygame`
- Valid: 19 Sep 2026 to 06 Sep 2076
- SHA-256 fingerprint: C3:35:79:4F:94:0C:C1:B2:94:10:23:23:EC:F1:CB:6C:42:FC:91:52:8A:E7:68:40:BF:20:5A:66:86:76:2C:A8
- Passwords are kept in a password manager, never in the repo.
- Note: keytool warns the certificate uses SHA1withRSA (weak). Fine for sideloading;
  a new SHA-256 key would be needed before a real store release.

## AI assistance
I used Claude (Anthropic) as a step-by-step guide while working through the lab sheets, mainly to explain settings and error logs. I reviewed and edited everything it suggested. All builds, installs and measurements were carried out by me on my own device.