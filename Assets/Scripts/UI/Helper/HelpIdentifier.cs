using UnityEngine;

namespace Virtupad
{
    [System.Serializable]
    public enum HelpID
    {
        Unknown = 0,
        LoadModelButton = 1,
        LoadModelFromFile = 2,
        ClearModel = 3,
        UnloadModel = 4,
        Cameras = 5,
        AddCamera = 6,
        OutputToDesktop = 7,
        Preview = 8,
        NoRoll = 9,
        FOV = 10,
        CameraType = 11,
        AutoFollow = 12,
        AutoTrack = 13,
        ShowNoOutputImageOnDesktop = 14,
        PlayerMove = 15,
        PlayerRotate = 16,
        MoveInDirectionOf = 17,
        LipSync = 18,
        CameraMove = 19,
        CameraRotate = 20,
        NoOutputLoad = 21,
        NoOutputReset = 22,
    }

    public class HelpIdentifier : MonoBehaviour
    {
        public HelpID ID => id;
        [SerializeField] private HelpID id;
    }
}
