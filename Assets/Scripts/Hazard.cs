using UnityEngine;

public class Hazard : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        Haptics.Pulse();
        GameManager.Instance.SetState(GameManager.State.Lost);
    }
}