using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class ShakeToggle : MonoBehaviour
{
    void Awake()
    {
        var toggle = GetComponent<Toggle>();
        toggle.isOn = PlayerPrefs.GetInt("shake", 1) == 1;
        toggle.onValueChanged.AddListener(v => PlayerPrefs.SetInt("shake", v ? 1 : 0));
    }
}