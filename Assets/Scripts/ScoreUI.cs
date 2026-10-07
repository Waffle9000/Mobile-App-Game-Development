using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] TMP_Text label;
    int shown = -1;

    void Update()
    {
        int score = (int)GameManager.Instance.Score;
        if (score == shown) return;
        shown = score;
        label.SetText("{0}", score);
    }
}