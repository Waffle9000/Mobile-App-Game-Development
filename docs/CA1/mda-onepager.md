mda-onepager.md


# Title & one-line pitch
Whopper,

A mobile game where you squish and drag a burger in order to dodge the hazardous environment you are running away from.

# Aesthetics (target player feelings/experiences)

Fun, silly, tense and quick reflex-based fun with a comedic vibe.

# Core mechanics (3–5 verbs/systems)
1. Swipe left/right: change lane
2. Swipe up: jump over obstacles
3. Swipe down: squish (slide under obstacles)
4. Dodge: avoid hazards (the goal of every move)

# Dynamics (how play unfolds)
The burger runs forward automatically. The player swipes left/right to change lanes, up to jump and down to squish, dodging obstacles that get faster and more frequent over time.
# Progression & content (levels/biomes/sessions; run length)
Endless runner style, one continuous run per session while the difficulty increases the longer you survive. There will be a kitchen level/biome which would transition into a farm level. There will be distance-based difficulty scaling.

# Platform features (haptics, safe-area handling; store/testing tracks = awareness only)
- Portrait orientation, swipe-based touch controls
- Safe area: UI stays clear of notches and rounded corners
- Pauses on Home, calls, notifications and Android back
- Haptic buzz on jump, with an on/off toggle in Settings
- Text size setting: Small / Normal / Large
- Screen shake toggle in Settings 
- Sideloaded APK for testing; Play Store tracks = awareness only

# Performance budget (target frame-time, memory, load time)
- Target: 60 fps
- Frame time: 16.7 ms average
- 99th percentile frame time: under 25 ms
- Peak memory: under 300 MB
- Cold start: under 3 s

# Monetisation (if any) & ethics notes
None planned for this assignment.
No ads or in-app purchases. 
No data collection and no dark patterns.
Accessibility options included (text size, haptics toggle, screen shake toggle).


# Risks & cuts list

Risks
- Squish mechanic may be hard to make feel right; could simplify if time runs short.
- Older test phone (ARMv7) may struggle to hit the performance budget.
- Playtest: swipes only trigger on finger release, which feels laggy. Fix: trigger once the swipe is far enough.

Cuts list (most expendable first)
1. Relaxed speed mode
2. Farm biome (keep kitchen only)
3. Extra hazard types
4. MenuStack (back navigation in menus)
5. Screen shake effect
6. Music and sound effects



Week 6 vertical-slice target
Week 6 slice: 1 biome, 10+ chunks, a mission system

Stretch goals
- Farm biome