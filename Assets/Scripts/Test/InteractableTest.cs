using System.Collections;
using UnityEngine;
using Valve.VR.InteractionSystem;

namespace Virtupad
{
    public class InteractableTest : MonoBehaviour
    {
        [SerializeField] private Transform attachmentOffset;
        [SerializeField] private Hand.AttachmentFlags attachmentFlags = Hand.AttachmentFlags.ParentToHand | Hand.AttachmentFlags.DetachFromOtherHand | Hand.AttachmentFlags.TurnOnKinematic;

        protected virtual void OnHandHoverBegin(Hand hand)
        {
            Debug.Log("OnHandHoverBegin");
        }

        protected virtual void OnHandHoverEnd(Hand hand)
        {
            Debug.Log("OnHandHoverEnd");
        }

        protected virtual void HandHoverUpdate(Hand hand)
        {
            Debug.Log("HandHoverUpdate");

            GrabTypes startingGrabType = hand.GetGrabStarting();

            if (startingGrabType != GrabTypes.None)
            {
                hand.AttachObject(gameObject, startingGrabType, attachmentFlags, attachmentOffset);
                hand.HideGrabHint();
            }
        }

        protected virtual void OnAttachedToHand(Hand hand)
        {
            Debug.Log("OnAttachedToHand");
        }

        protected virtual void OnDetachedFromHand(Hand hand)
        {
            Debug.Log("OnDetachedFromHand");
        }

        protected virtual void HandAttachedUpdate(Hand hand)
        {
            Debug.Log("HandAttachedUpdate");

            if (hand.IsGrabEnding(this.gameObject))
            {
                hand.DetachObject(gameObject, false);
            }
        }

        protected virtual IEnumerator LateDetach(Hand hand)
        {
            Debug.Log("LateDetach");
            yield break;
        }

        protected virtual void OnHandFocusAcquired(Hand hand)
        {
            Debug.Log("OnHandFocusAcquired");
        }

        protected virtual void OnHandFocusLost(Hand hand)
        {
            Debug.Log("OnHandFocusLost");
        }
    }
}
