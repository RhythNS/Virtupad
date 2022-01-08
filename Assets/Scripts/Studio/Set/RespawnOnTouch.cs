using UnityEngine;
using Valve.VR.InteractionSystem;

namespace Virtupad
{
    public class RespawnOnTouch : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.attachedRigidbody.gameObject == Player.instance.gameObject)
            {
                SetDefinition definition = GlobalsDict.Instance.CurrentDefinition;
                other.attachedRigidbody.MovePosition(definition.StartPoint);
            }
        }
    }
}
