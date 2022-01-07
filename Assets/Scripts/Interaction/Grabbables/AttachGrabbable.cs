using UnityEngine;
using Valve.VR.InteractionSystem;

namespace Virtupad
{
    public class AttachGrabbable : Grabbable
    {
        public bool IsAttached { get; private set; } = false;

        [SerializeField] private Transform attachmentOffset;
        [SerializeField] private Hand.AttachmentFlags attachmentFlags = Hand.AttachmentFlags.ParentToHand | Hand.AttachmentFlags.DetachFromOtherHand | Hand.AttachmentFlags.TurnOnKinematic;

        [SerializeField] private Vector3 easyRotationDefaultAngle = Vector3.zero;

        public XYZ EasyRotationLock { get => easyRotationLock; set => easyRotationLock = value; }
        [SerializeField, BitMask(typeof(XYZ))] private XYZ easyRotationLock = 0;

        [SerializeField] private Transform toMove;

        private Interacter onInteractor;

        public VoidEvent onAttachedToHand;
        public VoidEvent onDetachedFromHand;

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

            IsAttached = true;
            onAttachedToHand?.Invoke();
        }

        protected virtual void OnDetachedFromHand(Hand hand)
        {
            onInteractor.ChangeGrabbed(null);
            onInteractor = null;

            IsAttached = false;
            onDetachedFromHand?.Invoke();
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

            if (EasyRotationLock.HasFlag(XYZ.X))
                eulor.x = easyRotationDefaultAngle.x;
            if (EasyRotationLock.HasFlag(XYZ.Y))
                eulor.y = easyRotationDefaultAngle.y;
            if (EasyRotationLock.HasFlag(XYZ.Z))
                eulor.z = easyRotationDefaultAngle.z;

            toMove.rotation = Quaternion.Euler(eulor);
        }
    }
}
