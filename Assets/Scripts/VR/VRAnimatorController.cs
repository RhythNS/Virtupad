using System;
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

        [SerializeField] private Quaternion fingerRotOffset = Quaternion.Euler(0, 180, 0);

        [SerializeField] private List<DoubleTransform> doubleTransforms = new List<DoubleTransform>();

        private Animator animator;
        private Vector3 previousPos;

        [System.Serializable]
        struct DoubleTransform
        {
            public Transform first, second;
            public bool useOffset;
            public Quaternion customOffset;
            public bool useCustomOffset;

            public DoubleTransform(Transform first, Transform second, bool useOffset, Quaternion customOffset, bool useCustomOffset)
            {
                this.first = first;
                this.second = second;
                this.useOffset = useOffset;
                this.customOffset = customOffset;
                this.useCustomOffset = useCustomOffset;
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

            List<ConstructorDict.ToSetDoubleTransform> toSetDoubleTrans = ConstructorDict.Instance.ToSetDoubleTrans;
            foreach (var item in toSetDoubleTrans)
            {
                doubleTransforms.Add(new DoubleTransform(item.transform, animator.GetBoneTransform(item.bodybones),
                    item.useOffset, item.customOffset, item.useCustomOffset));
            }
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

            float dirX = headSetlocalSpeed.x * VRController.InvMovementSpeed;
            float dirY = headSetlocalSpeed.z * VRController.InvMovementSpeed;

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
                Quaternion toSet = Quaternion.identity;

                if (doubleTransforms[i].useCustomOffset)
                    toSet *= doubleTransforms[i].customOffset;
                if (doubleTransforms[i].useOffset)
                {
                    toSet *= Quaternion.Inverse(doubleTransforms[i].first.localRotation);
                }
                else
                {
                    toSet *= doubleTransforms[i].first.localRotation;
                }

                doubleTransforms[i].second.localRotation = toSet;
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }
    }
}
