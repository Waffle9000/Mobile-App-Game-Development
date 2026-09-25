using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class RenderScaleProbe : MonoBehaviour
{
    [Range(0.3f, 1f)] public float lowScale = 0.5f;
    UniversalRenderPipelineAsset _urp;
    float _full;

    void Start()
    {
        _urp = GraphicsSettings.currentRenderPipeline
               as UniversalRenderPipelineAsset;
        _full = _urp.renderScale;
    }

    public void Toggle()                  // bind to a debug button
    {
        bool atFull = Mathf.Approximately(_urp.renderScale, _full);
        _urp.renderScale = atFull ? lowScale : _full;
        Debug.Log($"[Probe] renderScale = {_urp.renderScale}");
    }
}