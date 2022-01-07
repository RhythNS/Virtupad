using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

namespace Virtupad
{
    public delegate void BoolChanged(bool changed);
    public delegate void VoidEvent();

    public delegate int GetInt();

    public delegate void SetDefinitionChanged(SetDefinition newDefinition);
    public delegate void StudioCameraChanged(StudioCamera newCamera);
    public delegate void StudioCamerasChanged(List<StudioCamera> newCamera);

    public delegate void InteracterEvent(Interacter interacter);

    public delegate void OnRenderTextureChanged(RenderTexture newTexture);
}
