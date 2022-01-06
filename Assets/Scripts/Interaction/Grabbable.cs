using UnityEngine;
using Valve.VR.InteractionSystem;

namespace Virtupad
{
    public class Grabbable : MonoBehaviour, IGrabbable
    {
        protected virtual void OnHandHoverBegin(Hand hand) { }

        protected virtual void OnHandHoverEnd(Hand hand) { }

        protected virtual void HandHoverUpdate(Hand hand) { }

        /*
        protected virtual void OnHandFocusAcquired(Hand hand)
        {
            Debug.Log("OnHandFocusAcquired");
        }

        protected virtual void OnHandFocusLost(Hand hand)
        {
            Debug.Log("OnHandFocusLost");
        }
         */
    }
}
