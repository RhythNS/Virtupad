using System.Collections.Generic;
using UnityEngine;

public class NoRenderMarker : MonoBehaviour
{
    [SerializeField] private List<Renderer> renderes = new List<Renderer>();

    private void Awake()
    {
        GetComponents(renderes);
        GetComponentsInChildren(renderes);
    }

    public void ToggleRenderers(bool enable)
        => renderes.ForEach(x => x.enabled = enable);
}
