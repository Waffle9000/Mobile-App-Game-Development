using UnityEngine;

public class Hazard : MonoBehaviour
{
    void OnTriggerEnter(Collider other) => Check(other.gameObject);
    void OnTriggerEnter2D(Collider2D other) => Check(other.gameObject);

    void Check(GameObject other)
    {
        if (other.CompareTag("Player"))
            GameManager.Instance.SetState(GameManager.State.Lost);
    }
}