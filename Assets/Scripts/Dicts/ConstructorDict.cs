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

        public List<Tuple<Transform, HumanBodyBones, bool>> ToSetDoubleTrans => toSetDoubleTrans;
        private readonly List<Tuple<Transform, HumanBodyBones, bool>> toSetDoubleTrans = new List<Tuple<Transform, HumanBodyBones, bool>>();

        public void RegisterFinger(HumanBodyBones humanBodyBones, Transform transform, bool useOffset)
        {
            toSetDoubleTrans.Add(new Tuple<Transform, HumanBodyBones, bool>(transform, humanBodyBones, useOffset));
        }

        public void DeRegisterFinger(Transform transform)
        {
            for (int i = 0; i < toSetDoubleTrans.Count; i++)
            {
                if (toSetDoubleTrans[i].Item1 == transform)
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
