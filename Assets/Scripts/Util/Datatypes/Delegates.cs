using System.Collections.Generic;

namespace Virtupad
{
    public delegate void BoolChanged(bool changed);
    public delegate void VoidEvent();

    public delegate int GetInt();

    public delegate void SetDefinitionChanged(SetDefinition newDefinition);
    public delegate void StudioCameraChanged(StudioCamera newCamera);
    public delegate void StudioCamerasChanged(List<StudioCamera> newCamera);
}
