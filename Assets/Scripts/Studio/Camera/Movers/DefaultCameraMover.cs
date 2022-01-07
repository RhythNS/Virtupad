using UnityEngine;

namespace Virtupad
{
    public class DefaultCameraMover : CameraMover
    {
        private bool isGrabbed = false;

        private Vector3 trackingPositionOffset;
        public Quaternion trackingRotationOffset;

        private void OnAttachedToHand()
        {
            isGrabbed = true;
        }

        private void OnDetachedFromHand()
        {
            isGrabbed = false;
        }

        public override void OnInit()
        {
            OnCamera.Grabbable.onAttachedToHand += OnAttachedToHand;
            OnCamera.Grabbable.onDetachedFromHand += OnDetachedFromHand;

            isGrabbed = OnCamera.Grabbable.IsAttached;
            OnFollowTypeChanged();
        }

        public override void OnRemove()
        {
            OnCamera.Grabbable.onAttachedToHand -= OnAttachedToHand;
            OnCamera.Grabbable.onDetachedFromHand -= OnDetachedFromHand;
        }

        public override void OnFollowTypeChanged()
        {
            Transform toTrack = GetFollowTrans();

            if (toTrack == null)
                return;

            trackingPositionOffset = transform.position - toTrack.position;
            trackingRotationOffset = toTrack.rotation * Quaternion.Inverse(transform.rotation);
        }

        private void Update()
        {
            if (isGrabbed == true)
                return;

            // Set position
            if (OnCamera.AutoFollow == true)
                AutoFollow();

            // Set rotation
            if (OnCamera.Tracking == true)
                Track();
        }

        private Transform GetFollowTrans()
        {
            return OnCamera.TrackingSpace switch
            {
                TrackingSpace.Playspace => VRController.Instance.transform,
                TrackingSpace.ModelSpace => VRController.Instance.bodyCollider,
                _ => null,
            };
        }

        private void AutoFollow()
        {
            Transform toTrack = GetFollowTrans();

            if (toTrack == null)
                return;

            transform.position = toTrack.position + trackingPositionOffset;
            transform.rotation = toTrack.rotation * trackingRotationOffset;
        }

        private void Track()
        {
            VRMController controller = VRMController.Instance;
            if (controller == null)
                return;

            Transform trackingBone = controller.Animator.GetBoneTransform(OnCamera.TrackingBone);
            transform.LookAt(trackingBone, Vector3.up);
        }
    }
}
