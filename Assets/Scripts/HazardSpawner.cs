using System;
using System.Threading;
using UnityEngine;

public class HazardSpawner : MonoBehaviour
{
    [SerializeField] float interval = 1.5f;
    [SerializeField] float laneWidth = 2f;
    [SerializeField] float spawnY = 1f;
    [SerializeField] float spawnZ = 40f;
    [SerializeField] HazardPool pool;
    CancellationTokenSource cts;

    void OnEnable()
    {
        cts = CancellationTokenSource.CreateLinkedTokenSource(
            Application.exitCancellationToken);
        _ = RunAsync(cts.Token);
    }

    void OnDisable() { cts.Cancel(); cts.Dispose(); }

    async Awaitable RunAsync(CancellationToken ct)
    {
        try
        {
            while (true)
            {
                int lane = UnityEngine.Random.Range(0, 3);
                pool.Spawn(new Vector3((lane - 1) * laneWidth, spawnY, spawnZ));
                await Awaitable.WaitForSecondsAsync(interval, ct);
            }
        }
        catch (OperationCanceledException) { }
    }
}