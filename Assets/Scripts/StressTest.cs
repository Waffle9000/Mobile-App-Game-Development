using UnityEngine;

public class StressTest : MonoBehaviour
{
    public int count = 200;
    Transform[] cubes;

    void Start()
    {
        cubes = new Transform[count];
        for (int i = 0; i < count; i++)
        {
            var c = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
            c.position = new Vector3(Random.Range(-5f, 5f), Random.Range(0f, 5f), Random.Range(5f, 30f));
            cubes[i] = c;
        }
    }

    void Update()
    {
        for (int i = 0; i < cubes.Length; i++)
            cubes[i].Rotate(0f, 90f * Time.deltaTime, 0f);
    }
}