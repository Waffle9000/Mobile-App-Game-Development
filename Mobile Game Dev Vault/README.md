# Game Name: Whopper
# Option Number:
2 - Endless Runner

# Phone Model:
 Will be using Emulator

# Android Version:
Will be using Emulator, will update later,

# [Boot] line info
20L8S76701 (LENOVO) | Windows 11  (10.0.26200) | Direct3D11 | 1223x510 @ 120 dpi
UnityEngine.Debug:Log (object)
MobileBootstrap:Awake () (at Assets/Scripts/MobileBootstrap.cs:14)



# Build steps:

Check the phone is connected:

```
adb devices
# List of devices attached
# R58M12ABCDE   device
```

Install the APK
# 

```
adb install -r Builds/MyGame-dev.apk
# Performing Streamed Install
# Success
```

Launch the Game


```
adb shell monkey -p com.yourname.mygame 1
adb logcat -s Unity
# [Boot] SM-S911B | Android OS 15 / API-35 | Vulkan | 1080x2340 @ 425 dpi
```

Keystore file location: C:\Users\achik\Desktop\Keystores

Keystore file name: mygame-release.keystore

Keystore Alias: mygame

Validity: 50 years