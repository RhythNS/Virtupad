using System.Collections.Generic;
using UnityEngine;

namespace Virtupad
{
    public class VRAnimatorController : MonoBehaviour
    {
        public float speedTreshold = 0.1f;
        public float smoothing = 1f;
        public float rotateSpeed = 180.0f;
        public float moveSpeed = 2f;

        public static VRAnimatorController Instance { get; private set; }

        private readonly List<DoubleTransform> doubleTransforms = new List<DoubleTransform>();

        private Animator animator;
        private Vector3 previousPos;

        [System.Serializable]
        struct DoubleTransform
        {
            public Transform first, second;
            public bool useOffset;

            public DoubleTransform(Transform first, Transform second, bool useOffset)
            {
                this.first = first;
                this.second = second;
                this.useOffset = useOffset;
            }
        }

        private void Awake()
        {
            if (Instance)
            {
                Debug.LogWarning("VRAnimatorController already in scene. Deleting myself!");
                Destroy(this);
                return;
            }
            Instance = this;
            animator = GetComponent<Animator>();
        }

        void Update()
        {
            if (!VRController.Instance.head)
                return;

            /*
            Transform headTrans = VRController.Instance.head;

            float prevDirX = animator.GetFloat("directionX");
            float prevDirY = animator.GetFloat("directionY");

            Vector3 headsetSpeed = (headTrans.position - previousPos) / Time.deltaTime;
            headsetSpeed.y = 0;
            Vector3 headSetlocalSpeed = transform.InverseTransformDirection(headsetSpeed);

            previousPos = headTrans.position;

            animator.SetBool("isMoving", headSetlocalSpeed.magnitude > speedTreshold);
            animator.SetFloat("directionX", Mathf.Lerp(prevDirX, Mathf.Clamp(headSetlocalSpeed.x, -1, 1), smoothing));
            animator.SetFloat("directionY", Mathf.Lerp(prevDirY, Mathf.Clamp(headSetlocalSpeed.z, -1, 1), smoothing));
             */


            Transform headTrans = VRController.Instance.head;
            Vector3 headsetSpeed = (headTrans.position - previousPos) / Time.deltaTime;
            Vector3 headSetlocalSpeed = transform.InverseTransformDirection(headsetSpeed);

            previousPos = headTrans.position;

            float dirX = headSetlocalSpeed.x * VRController.invMovementSpeed;
            float dirY = headSetlocalSpeed.z * VRController.invMovementSpeed;

            if (float.IsNaN(dirX))
                dirX = 0.0f;
            if (float.IsInfinity(dirY))
                dirY = 0.0f;

            animator.SetFloat("directionX", dirX, 0.1f, Time.deltaTime);
            animator.SetFloat("directionY", dirY, 0.1f, Time.deltaTime);
        }

        private void LateUpdate()
        {
            for (int i = 0; i < doubleTransforms.Count; i++)
            {
                doubleTransforms[i].second.rotation = doubleTransforms[i].useOffset ?
                    doubleTransforms[i].first.rotation * Quaternion.Euler(0, 180, 0) :
                    doubleTransforms[i].first.rotation;
            }
        }

        public void Register(HumanBodyBones humanBodyBones, Transform transform, bool useOffset)
        {
            //   doubleTransforms.Add(new DoubleTransform(transform, animator.GetBoneTransform(humanBodyBones), useOffset));
        }

        public void DeRegister(Transform transform)
        {
            for (int i = 0; i < doubleTransforms.Count; i++)
            {
                if (doubleTransforms[i].first == transform)
                {
                    doubleTransforms.RemoveAt(i);
                    return;
                }
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }
    }
}
