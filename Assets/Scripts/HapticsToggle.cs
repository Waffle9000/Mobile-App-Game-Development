using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class HapticsToggle : MonoBehaviour
{
    void Awake()
    {
        var toggle = GetComponent<Toggle>();
        toggle.isOn = Haptics.Enabled;
        toggle.onValueChanged.AddListener(v => Haptics.Enabled = v);
    }
}
