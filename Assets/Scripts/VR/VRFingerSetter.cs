using UnityEngine;
using Valve.VR;

namespace Virtupad
{
    public class VRFingerSetter : MonoBehaviour
    {
        [SerializeField] private bool isRightHand = false;

        private SteamVR_Behaviour_Skeleton skeleton;
        private VRMFingerAnimator fingerAnimator;

        void Start()
        {
            skeleton = GetComponent<SteamVR_Behaviour_Skeleton>();

            VRMController.onVRMCreated += OnVRMCreated;
            VRMController.onVRMDeleted += OnVRMDeleted;

            VRMController vrmController = VRMController.Instance;
            if (vrmController)
            {
                vrmController.ResetFingerAnimators();
                GetFingerAnimator(vrmController);
            }
        }

        private void OnVRMDeleted(VRMController newController)
        {
            fingerAnimator = null;
        }

        private void OnVRMCreated(VRMController newController)
        {
            GetFingerAnimator(newController);
        }

        private void GetFingerAnimator(VRMController controller)
        {
            fingerAnimator = isRightHand ? controller.RightFingerAnimator : controller.LeftFingerAnimator;
        }

        void LateUpdate()
        {
            if (fingerAnimator == null)
                return;

            fingerAnimator.OnFingerUpdate(skeleton.fingerCurls);
        }

        private void OnDestroy()
        {
            VRMController.onVRMCreated -= OnVRMCreated;
            VRMController.onVRMDeleted -= OnVRMDeleted;
        }
    }
}
