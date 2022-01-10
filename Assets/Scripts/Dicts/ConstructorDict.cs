using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Virtupad
{
    public class ConstructorDict : MonoBehaviour
    {
        public static ConstructorDict Instance { get; private set; }

        private void Awake()
        {
            if (Instance)
            {
                Debug.LogWarning("ConstructorDict already in scene. Deleting myself!");
                Destroy(this);
                return;
            }
            Instance = this;
        }

        public Animator LoadingCharacterAnimator;

        public RigBuilder rigBuilder;

        public Rig rig;

        public Transform rightArm, leftArm, head, rightLeg, leftLeg, hip;

        public RigMaker rigMaker;

        public VRMController vrmController;

        public RuntimeAnimatorController TPoseController => tPoseController;
        [SerializeField] private RuntimeAnimatorController tPoseController;

        public RuntimeAnimatorController FullBody => fullBody;
        [SerializeField] private RuntimeAnimatorController fullBody;

        public RuntimeAnimatorController UpperBody => upperBody;
        [SerializeField] private RuntimeAnimatorController upperBody;

        [System.Serializable]
        public struct ToSetDoubleTransform
        {
            public HumanBodyBones bodybones;
            public Transform transform;
            public bool useOffset;
            public Quaternion customOffset;
            public bool useCustomOffset;

            public ToSetDoubleTransform(HumanBodyBones bodybones, Transform transform, bool useOffset, Quaternion customOffset, bool useCustomOffset)
            {
                this.bodybones = bodybones;
                this.transform = transform;
                this.useOffset = useOffset;
                this.customOffset = customOffset;
                this.useCustomOffset = useCustomOffset;
            }
        }

        public List<ToSetDoubleTransform> ToSetDoubleTrans => toSetDoubleTrans;
        private readonly List<ToSetDoubleTransform> toSetDoubleTrans = new List<ToSetDoubleTransform>();

        public void RegisterFinger(HumanBodyBones humanBodyBones, Transform transform, bool useOffset, Quaternion customOffset, bool useCustomOffset)
        {
            toSetDoubleTrans.Add(new ToSetDoubleTransform(humanBodyBones, transform, useOffset, customOffset, useCustomOffset));
        }

        public void DeRegisterFinger(Transform transform)
        {
            for (int i = 0; i < toSetDoubleTrans.Count; i++)
            {
                if (toSetDoubleTrans[i].transform == transform)
                {
                    toSetDoubleTrans.RemoveAt(i);
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
