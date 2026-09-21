# Game Name: Whopper
# Option Number:
2 - Endless Runner

# Phone Model:
 Galaxy A02s

# Android Version:
12

# [Boot] line info

[Boot] samsung SM-A025F | Android OS 12 / API-31 (SP1A.210812.016/A025FXXS8CXH1) | OpenGLES3 | 720x1600 @ 280 dpi


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