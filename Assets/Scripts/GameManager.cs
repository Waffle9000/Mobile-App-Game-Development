using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum State { Playing, Paused, Won, Lost }

    public static GameManager Instance { get; private set; }
    public static int LastScore;

    public State Current { get; private set; } = State.Playing;
    public float Score { get; private set; }
    public event Action<State> StateChanged;

    void Awake() => Instance = this;
    void OnEnable()  => LifecycleGuard.PausedChanged += OnPaused;
    void OnDisable() => LifecycleGuard.PausedChanged -= OnPaused;

    void Update()
    {
        if (Current == State.Playing) Score += Time.deltaTime * 10f;
    }

    void OnPaused(bool paused)
    {
        if (paused && Current == State.Playing) SetState(State.Paused);
        else if (!paused && Current == State.Paused) SetState(State.Playing);
    }

    public void SetState(State s)
    {
        if (s == Current) return;
        Current = s;
        StateChanged?.Invoke(s);
        if (s == State.Won || s == State.Lost)
        {
            LastScore = (int)Score;
            SceneManager.LoadScene("Result");
        }
    }
}