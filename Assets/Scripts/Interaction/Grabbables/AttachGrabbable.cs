using UnityEngine;
using Valve.VR.InteractionSystem;

namespace Virtupad
{
    public class AttachGrabbable : Grabbable
    {
        [SerializeField] private Transform attachmentOffset;
        [SerializeField] private Hand.AttachmentFlags attachmentFlags = Hand.AttachmentFlags.ParentToHand | Hand.AttachmentFlags.DetachFromOtherHand | Hand.AttachmentFlags.TurnOnKinematic;

        [SerializeField] private Vector3 easyRotationDefaultAngle = Vector3.zero;
        [SerializeField, BitMask(typeof(XYZ))] private XYZ easyRotationLock = 0;

        [SerializeField] private Transform toMove;

        private Interacter onInteractor;

        protected virtual void Awake()
        {
        }

        protected override void HandHoverUpdate(Hand hand)
        {
            GrabTypes startingGrabType = hand.GetGrabStarting();

            if (startingGrabType != GrabTypes.None)
            {
                hand.AttachObject(gameObject, startingGrabType, attachmentFlags, attachmentOffset);
                hand.HideGrabHint();
            }
        }

        protected virtual void OnAttachedToHand(Hand hand)
        {
            onInteractor = GlobalsDict.Instance.Interacters.Find(x => x.ForSource == hand.handType);
            onInteractor.ChangeGrabbed(this);
        }

        protected virtual void OnDetachedFromHand(Hand hand)
        {
            onInteractor.ChangeGrabbed(null);
            onInteractor = null;
        }

        protected virtual void HandAttachedUpdate(Hand hand)
        {
            if (hand.IsGrabEnding(this.gameObject))
            {
                hand.DetachObject(gameObject, false);
            }
        }

        private void LateUpdate()
        {
            if (toMove)
                FixRotation();
        }

        /*
        private void FixedUpdate()
        {
            FixRotation();
        }
         */

        private void FixRotation()
        {
            Vector3 eulor = toMove.eulerAngles;

            if (easyRotationLock.HasFlag(XYZ.X))
                eulor.x = easyRotationDefaultAngle.x;
            if (easyRotationLock.HasFlag(XYZ.Y))
                eulor.y = easyRotationDefaultAngle.y;
            if (easyRotationLock.HasFlag(XYZ.Z))
                eulor.z = easyRotationDefaultAngle.z;

            toMove.rotation = Quaternion.Euler(eulor);
        }
    }
}
