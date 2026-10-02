# Data-use & Privacy Statement – Whopper (v0.2.0, versionCode 4)

## Data collected
Whopper itself collects **no personal data**. There are no accounts, no sign-in and no in-game tracking.

However, the **Unity engine** the game is built with includes Unity's own analytics/diagnostics services.
When the phone is online, Unity may send anonymous technical data (such as device model,
OS version and crash/diagnostic information) to Unity Technologies. Whopper does not add to
or read this data. Evidence: the device log shows connection attempts to Unity servers
(see Network activity).

## Data stored on the device
Settings are saved locally with PlayerPrefs and never leave the phone:
- `haptics` – haptics on/off
- `textScale` – text size (Small / Normal / Large)
- `shake` – screen shake on/off

Any future save files will be stored in `Application.persistentDataPath`, and the Week 6
telemetry log will be **local only**. To remove all stored data: uninstall the game.

## Network activity
The game has no online features and plays fully offline. The only network activity is from
the Unity engine, which attempts to contact:
- `cdp.cloud.unity3d.com`
- `engine-data.unity3d.com`
- `config.uca.cloud.unity3d.com`

These are Unity analytics/diagnostics endpoints. If the phone is offline, the attempts fail
and the game is unaffected.

## Third-party SDKs
None added. Only the Unity engine's built-in services (see above).

## Permissions requested
From `adb shell dumpsys package com.archil.whopper`:
- `android.permission.INTERNET` – used by the Unity engine's analytics/diagnostics services, not by game features.
- `android.permission.VIBRATE` – haptic buzz on jump (can be turned off in Settings).
- `com.archil.whopper.DYNAMIC_RECEIVER_NOT_EXPORTED_PERMISSION` – added automatically by Android/Unity to keep the app's internal messages private. Not user data.

## How this would be declared on Google Play
- **Data collection & security:** declare app diagnostics / device info collected by the Unity engine;
  sent over an encrypted connection; no user-entered data collected.
- **Data sharing:** data goes to Unity Technologies as the engine provider; the game itself shares nothing.
- **Privacy policy URL:** required for release; this statement would be published as that page.
- The Data safety form is required before release on the closed, open or production tracks.
  The internal testing track is exempt.

## Planned change
Before any real store release, Unity analytics/diagnostics will be turned off and Internet Access
set to Auto, so the game makes no network requests at all.