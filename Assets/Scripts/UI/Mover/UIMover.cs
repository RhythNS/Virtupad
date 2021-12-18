using UnityEngine;
using Valve.VR;

namespace Virtupad
{
    public abstract class UIMover : MonoBehaviour
    {
        public abstract void SubscribeToEvents(SteamVR_Action_Vector2 uiMoveInput, SteamVR_Action_Boolean uiSelectInput);

        public abstract void UnSubscribeFromEvents(SteamVR_Action_Vector2 uiMoveInput, SteamVR_Action_Boolean uiSelectInput);

    }
}
