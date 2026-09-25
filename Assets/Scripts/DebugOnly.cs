using UnityEngine;

public class DebugOnly : MonoBehaviour
{
    void Awake() => gameObject.SetActive(Debug.isDebugBuild);

}