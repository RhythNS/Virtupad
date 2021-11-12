using System.Collections.Generic;
using UnityEngine;

public class NoRenderObjectsCloseToCamera : MonoBehaviour
{
    [SerializeField] private Camera onCamera;
    private readonly List<NoRenderMarker> noRender = new List<NoRenderMarker>();

    private void Awake()
    {
        if (onCamera != null || TryGetComponent(out onCamera) == true)
            return;

        Debug.LogWarning("Expected a camera! Deleting myself!");
        Destroy(this);
    }

    void Start()
    {
        Camera.onPreCull += OnPreCullCallback;
        Camera.onPreRender += OnPreRenderCallback;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out NoRenderMarker noRenderMarker) == false)
            return;

        noRender.Add(noRenderMarker);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out NoRenderMarker noRenderMarker) == false)
            return;

        noRender.Remove(noRenderMarker);
    }

    private void OnPreCullCallback(Camera camera)
    {
        if (camera == onCamera)
            noRender.ForEach(x => x.ToggleRenderers(false));
    }

    private void OnPreRenderCallback(Camera camera)
    {
        if (camera == onCamera)
            noRender.ForEach(x => x.ToggleRenderers(true));
    }

    private void OnDestroy()
    {
        Camera.onPreCull -= OnPreCullCallback;
        Camera.onPreRender -= OnPreRenderCallback;
    }
}
