using UnityEngine;
using Valve.VR.InteractionSystem;

namespace Virtupad
{
    public class AutoHeightInteractable : Interactable
    {
        public override void Select()
        {
            Vector3 headDir = VRController.Instance.head.transform.position - Player.instance.transform.position;
            headDir.x = 0.0f;
            headDir.z = 0.0f;
            VRController.Instance.playerHeight = headDir.magnitude;
        }
    }
}
