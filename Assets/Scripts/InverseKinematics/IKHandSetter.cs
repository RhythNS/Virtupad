using UnityEngine;

namespace Virtupad
{
    public class IKHandSetter : MonoBehaviour
    {
        private Animator animator;
        private Transform rightHand, leftHand;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            rightHand = ConstructorDict.Instance.rightArm;
            leftHand = ConstructorDict.Instance.leftArm;
        }

        private void OnAnimatorIK(int layerIndex)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f);
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1.0f);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);

            animator.SetIKPosition(AvatarIKGoal.RightHand, rightHand.position);
            animator.SetIKRotation(AvatarIKGoal.RightHand, rightHand.rotation);

            animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHand.position);
            animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHand.rotation);
        }
    }
}
