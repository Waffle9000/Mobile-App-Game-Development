using UnityEngine;

public class Hazard : MonoBehaviour
{
    [SerializeField] float speed = 12f;
    [SerializeField] float despawnZ = -10f;
    HazardPool pool;

    public Hazard Init(HazardPool p) { pool = p; return this; }

    void Update()
    {
        transform.Translate(0f, 0f, -speed * Time.deltaTime, Space.World);
        if (transform.position.z < despawnZ) Despawn();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        Haptics.Pulse();
        GameManager.Instance.SetState(GameManager.State.Lost);
    }

    public void Despawn() => pool.Release(this);
}