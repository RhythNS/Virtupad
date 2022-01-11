using UnityEngine;
using VRM;

namespace Virtupad
{
    public class VRMController : MonoBehaviour
    {
        public static VRMController Instance { get; private set; }

        public Animator Animator { get; private set; }
        public VRAnimatorController VRAnimatorController { get; private set; }
        public VRMFirstPerson VRMFirstPerson { get; private set; }
        public FullRigCreator FullRigCreator { get; private set; }
        public VRMLookTarget VRMLookTarget { get; private set; }
        //public VRMMouthMover VRMMouthMover { get; private set; }
        public VRMSalsa VRMSalsa { get; private set; }
        public VRMEmotionManager VRMEmotionManager { get; private set; }
        public VRMFingerAnimator LeftFingerAnimator { get; private set; }
        public VRMFingerAnimator RightFingerAnimator { get; private set; }
        public float Height { get; private set; }

        public static event OnVRMChanged onVRMCreated;
        public static event OnVRMChanged onVRMDeleted;

        private void Awake()
        {
            if (Instance)
            {
                Debug.LogWarning("VRMController already in scene. Deleting myself!");
                Destroy(this);
                return;
            }
            Instance = this;

            Animator = gameObject.GetComponent<Animator>();
            Animator.runtimeAnimatorController = ConstructorDict.Instance.TPoseController;
            Animator.Update(0.1f);

            VRMLookAtHead lookAt = gameObject.GetComponent<VRMLookAtHead>();
            if (lookAt != null)
            {
                gameObject.AddComponent<Blinker>();

                GameObject lookTargetObject = new GameObject("VRM Look Target");
                lookTargetObject.transform.parent = transform;
                lookAt.Target = lookTargetObject.transform;
                lookAt.UpdateType = UpdateType.LateUpdate; // after HumanPoseTransfer's setPose
                VRMLookTarget = lookTargetObject.AddComponent<VRMLookTarget>();

                Transform headTrans = Animator.GetBoneTransform(HumanBodyBones.Head).transform;
                lookTargetObject.transform.position = headTrans.position + (headTrans.forward * 3.0f);
            }

            VRMFirstPerson = gameObject.GetComponent<VRMFirstPerson>();
            VRMFirstPerson.Setup();

            Vector3 headPos = VRMFirstPerson.FirstPersonBone.transform.position + VRMFirstPerson.FirstPersonOffset;
            Vector3 headDir = headPos - transform.position;
            headDir.x = 0.0f;
            headDir.z = 0.0f;
            Height = headDir.magnitude;

            VRAnimatorController = gameObject.AddComponent<VRAnimatorController>();
            FullRigCreator = gameObject.AddComponent<FullRigCreator>();
            //VRMMouthMover = gameObject.AddComponent<VRMMouthMover>();
            VRMSalsa = gameObject.AddComponent<VRMSalsa>();
            VRMEmotionManager = gameObject.AddComponent<VRMEmotionManager>();
            VRAnimatorController.enabled = false;

            LeftFingerAnimator = gameObject.AddComponent<VRMFingerAnimator>();
            LeftFingerAnimator.onRightHand = false;
            RightFingerAnimator = gameObject.AddComponent<VRMFingerAnimator>();
            RightFingerAnimator.onRightHand = true;

            onVRMCreated?.Invoke(this);
        }

        public void ResetFingerAnimators()
        {
            LeftFingerAnimator.ResetFingers();
            RightFingerAnimator.ResetFingers();
        }

        public void OnTakenControl()
        {
            VRAnimatorController.enabled = true;
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
                onVRMDeleted?.Invoke(this);
            }

            ConstructorDict.Instance?.rigMaker?.CleanUp();
        }
    }
}
