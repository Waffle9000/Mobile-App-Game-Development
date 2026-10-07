| Field | Row 1 | Row 2 (Week 5) |
|-------|-------|----------------|
| Date, commit: 7e84603, versionName / versionCode, Release or Dev, Unity version | 28 Sep 2026, COMMIT, 0.1.1 / CODE, Release, 6000.6.0f1 | 7 Oct 2026, COMMIT, 0.2.0 / 4, Dev, 6000.6.0f1 |
| Device model, Android version, SoC / GPU, graphics API, refresh rate | Samsung SM-A025F, Android 12, Adreno 506, OpenGL ES 3, 60 Hz | Same |
| Target fps | 60 | 60 |
| Menu: avg ms / p99 ms | 16.71 / 16.88 | |
| Steady gameplay: avg ms / p99 ms | 16.71 / 16.90 | 16.71 / 16.80 (pooled hazards spawning every 1.5 s) |
| Worst case: avg ms / p99 ms | 16.71 / 16.90 (200 stress cubes, always active) | |
| GC allocated per frame (bytes), allocating markers | 59 B: FinishFrameRendering 32 B, NewInputFixedUpdate 9 B, PlayerSendFrameStarted 9 B, NewInputUpdate 9 B (all Unity internal) | 18 B: PlayerSendFrameStarted 9 B, NewInputUpdate 9 B (both Unity internal). 0 B from spawner, pool and hazards |
| Peak Total Reserved (MB) / TOTAL PSS (MB) | 317.0 (Dev) / 194.3 (Release) | |
| Worst case: SetPass / batches / triangles | 11 / 0 (not counted with URP) / 3.6k | |
| Cold start ms (median of 3), first interactive s | 716 ms / 2.46 s | |
| APK size (MB) | 71.0 | |
| Thermal delta, throttling (Week 7) | | |
| Load time (Week 5) | | |

**Week 5 change:** hazards come from a prewarmed pool of 16 and the spawner uses Awaitable, so nothing is instantiated during play. Renderer switched from 2D to Universal (3D). Capture: `docs/CA2/baseline/w05-spawner.data`.