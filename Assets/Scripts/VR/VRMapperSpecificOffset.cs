using System;

namespace Virtupad
{
    [Flags]
    public enum VRMapperSpecificOffset
    {
        None = 0,
        RotX = 1,
        RotY = 2,
        RotZ = 4,
        PosX = 8,
        PosY = 16,
        PosZ = 32
    }
}
