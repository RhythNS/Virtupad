using UnityEngine;
using Valve.VR.InteractionSystem;

namespace Virtupad
{
    public class DefaultCameraMover : CameraMover
    {
        private bool isGrabbed = false;

        private Vector3 trackingPositionOffset;
        private Vector3 trackingDirectionOffset;

        private void OnAttachedToHand()
        {
            isGrabbed = true;
        }

        private void OnDetachedFromHand()
        {
            isGrabbed = false;
            OnFollowTypeChanged();
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

            trackingPositionOffset = toTrack.InverseTransformPoint(transform.position);
            trackingDirectionOffset = toTrack.InverseTransformDirection((toTrack.position - transform.position).normalized);
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

            transform.position = toTrack.TransformPoint(trackingPositionOffset);
            transform.rotation = Quaternion.LookRotation(toTrack.TransformDirection(trackingDirectionOffset));
        }

        private void Track()
        {
            Transform toTrack;

            VRMController controller = VRMController.Instance;
            toTrack = controller != null ? controller.Animator.GetBoneTransform(OnCamera.TrackingBone)
                : Player.instance.hmdTransform;

            transform.LookAt(toTrack, Vector3.up);
        }
    }
}
