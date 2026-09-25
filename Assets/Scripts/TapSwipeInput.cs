using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class TapSwipeInput : MonoBehaviour
{
    public float swipeDp = 50f;
    public float tapMax = 0.3f;
    public RunnerController runner; //This is where the player object is being dragged to in the inspector

    void OnEnable() => EnhancedTouchSupport.Enable();
    void OnDisable() => EnhancedTouchSupport.Disable();

    void Update()
    {

        if (LifecycleGuard.IsPaused) return;

        foreach (var t in Touch.activeTouches)
        {
            if (t.phase != TouchPhase.Ended) continue;

            float px = swipeDp * Mathf.Max(Screen.dpi, 160f) / 160f;
            Vector2 d = t.screenPosition - t.startScreenPosition;

            if (d.magnitude >= px)
            {
                Vector2 dir = d.normalized;
                if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
                {
                    if (dir.x > 0) runner.MoveRight();   //Move Right and 
                    else runner.MoveLeft();
                }
                else
                {
                    if (dir.y > 0) {

                        runner.Jump(); 
                        Haptics.Pulse();
                        }  //Jump
                    else runner.Slide();
                    
                }
            }
            // Tap is intentionally unused for this runner
        }
    }
}