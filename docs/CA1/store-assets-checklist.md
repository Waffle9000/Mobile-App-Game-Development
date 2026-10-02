# Store-asset checklist – Whopper

Specs from Play Console help, "Add preview assets"
(https://support.google.com/googleplay/android-developer/answer/9866151), checked on submission day.

| Asset | Spec | Status | Notes |
|-------|------|--------|-------|
| App icon | 512x512, 32-bit PNG with alpha, up to 1024 KB | planned | No rounded corners, Play masks it. Burger character on a plain background. |
| Feature graphic | 1024x500, JPEG or 24-bit PNG, no alpha | planned | Burger mid-jump in the kitchen, game title. |
| Phone screenshots | 2 to 8; each side 1080 to 7680 px; JPEG or 24-bit PNG; 16:9 or 9:16 | planned | Portrait (9:16). Gameplay first. |

## Screenshot plan (portrait, 9:16)
| Shot | Caption | Must be visible |
|------|---------|-----------------|
| 1 | "Run, dodge, survive!" | Mid-run in the kitchen, score visible, nothing under the camera hole |
| 2 | "Swipe up to jump" | Burger mid-jump over an obstacle |
| 3 | "Swipe down to squish" | Burger squished, sliding under an obstacle |
| 4 | "Switch lanes to dodge" | Burger moving between lanes, hazards ahead |
| 5 | "Complete missions" | Mission prompt or completion on screen |
| 6 | "Play your way" | Settings panel: haptics, text size, screen shake toggles |

## Where assets will come from (Week 10)
- Screenshots: in-game captures on device with `adb shell screencap`, at 1080 px wide or more.
- Icon and feature graphic: made from the burger character / key art in the MDA one-pager.  