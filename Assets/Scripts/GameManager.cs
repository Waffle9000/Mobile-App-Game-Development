using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum State { Playing, Paused, Won, Lost }

    public static GameManager Instance { get; private set; }
    public static int LastScore;

    public State Current { get; private set; } = State.Playing;
    public event Action<State> StateChanged;

    void Awake() => Instance = this;

    public void SetState(State s)
    {
        if (s == Current) return;
        Current = s;
        StateChanged?.Invoke(s);
        if (s == State.Won || s == State.Lost)
            SceneManager.LoadScene("Result");   
    }
}