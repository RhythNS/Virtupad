using UnityEngine;
using Valve.VR.InteractionSystem;

namespace Virtupad
{
    public class DefaultCameraMover : CameraMover
    {
        private bool isGrabbed = false;

        private Vector3 trackingPositionOffset;
        private Vector3 trackingDirectionOffset;

        private Vector3 velocity;
        [SerializeField] private float smoothness = 0.01f;

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
            trackingDirectionOffset = toTrack.InverseTransformDirection(transform.forward);
        }

        public override void OnExternalMoved(Vector3 newPos)
        {
            Transform toTrack = GetFollowTrans();

            if (toTrack == null)
                return;

            trackingPositionOffset = toTrack.InverseTransformPoint(newPos);
        }

        public override void OnExternalRotated(Quaternion newRot)
        {
            Transform toTrack = GetFollowTrans();

            if (toTrack == null)
                return;

            trackingDirectionOffset = toTrack.InverseTransformDirection(newRot * Vector3.forward);
        }

        private void Update()
        {
            if (isGrabbed == true)
                return;

            Vector3 toGoTo = transform.position;
            Quaternion toRotateTo = transform.rotation;

            // Set position
            if (OnCamera.AutoFollow == true)
                AutoFollow(ref toGoTo, ref toRotateTo);

            // Set rotation
            if (OnCamera.Tracking == true)
                Track(ref toGoTo, ref toRotateTo);

            Vector3.SmoothDamp(transform.position, toGoTo, ref velocity, smoothness);
            transform.position = transform.position + (velocity * Time.deltaTime);
            transform.rotation = toRotateTo;
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

        private void AutoFollow(ref Vector3 toGoTo, ref Quaternion toRotateTo)
        {
            Transform toTrack = GetFollowTrans();

            if (toTrack == null)
                return;

            toGoTo = toTrack.TransformPoint(trackingPositionOffset);
            toRotateTo = Quaternion.LookRotation(toTrack.TransformDirection(trackingDirectionOffset));
        }

        private void Track(ref Vector3 toGoTo, ref Quaternion toRotateTo)
        {
            Transform toTrack;

            VRMController controller = VRMController.Instance;
            toTrack = controller != null ? controller.Animator.GetBoneTransform(OnCamera.TrackingBone)
                : Player.instance.hmdTransform;

            // transform.LookAt(toTrack, Vector3.up);
            toRotateTo = Quaternion.LookRotation((toTrack.position - transform.position).normalized, Vector3.up);
        }
    }
}
