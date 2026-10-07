using System.Collections.Generic;
using UnityEngine;

public class HazardPool : MonoBehaviour
{
    [SerializeField] Hazard prefab;
    [SerializeField] int size = 16;
    readonly Stack<Hazard> free = new Stack<Hazard>();

    void Awake()
    {
        for (int i = 0; i < size; i++) free.Push(Create());
    }


    Hazard Create()
    {
        Hazard h = Instantiate(prefab, transform).Init(this);
        h.gameObject.SetActive(false);
        return h;
    }

    public Hazard Spawn(Vector3 position)
    {
        Hazard h = free.Count > 0 ? free.Pop() : Create();
        h.transform.position = position;
        h.gameObject.SetActive(true);
        return h;
    }

    public void Release(Hazard h)
    {
        h.gameObject.SetActive(false);
        free.Push(h);
    }
}