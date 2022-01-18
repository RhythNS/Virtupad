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

        [SerializeField] private Vector3 outLocalSpeed;
        [SerializeField] private Vector3 outSpeed;

        void Update()
        {
            if (!VRController.Instance.head)
                return;

            Transform headTrans = VRController.Instance.head;
            Vector3 headsetSpeed = (headTrans.position - previousPos) * (1 / Time.deltaTime);
            headsetSpeed /= 1.0f; // divide by max movement 
            Vector3 headSetlocalSpeed = transform.InverseTransformDirection(headsetSpeed);

            outLocalSpeed = headSetlocalSpeed;
            outSpeed = headsetSpeed;

            previousPos = headTrans.position;

            /*
            float dirX = headSetlocalSpeed.x * VRController.InvMovementSpeed;
            float dirY = headSetlocalSpeed.z * VRController.InvMovementSpeed;
             */
            float dirX = headSetlocalSpeed.x;
            float dirY = headSetlocalSpeed.z;

            if (float.IsNaN(dirX))
                dirX = 0.0f;
            if (float.IsInfinity(dirY))
                dirY = 0.0f;

            animator.SetFloat("directionX", dirX, 0.1f, Time.deltaTime);
            animator.SetFloat("directionY", dirY, 0.1f, Time.deltaTime);
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }
    }
}
