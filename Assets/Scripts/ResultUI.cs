using TMPro;
using UnityEngine;

public class ResultUI : MonoBehaviour
{
    [SerializeField] TMP_Text scoreText;
    void Start() => scoreText.text = "Score: " + GameManager.LastScore;
}