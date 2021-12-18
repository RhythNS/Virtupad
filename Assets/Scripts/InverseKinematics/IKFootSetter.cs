using UnityEngine;

namespace Virtupad
{
    public class IKFootSetter : MonoBehaviour
    {
        private Animator animator;
        private Transform rightFoot;
        private Transform leftFoot;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            rightFoot = ConstructorDict.Instance.rightLeg;
            leftFoot = ConstructorDict.Instance.leftLeg;
        }

        private void OnAnimatorIK(int layerIndex)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1.0f);
            animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1.0f);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1.0f);
            animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1.0f);

            animator.SetIKPosition(AvatarIKGoal.RightFoot, rightFoot.position);
            animator.SetIKRotation(AvatarIKGoal.RightFoot, rightFoot.rotation);

            animator.SetIKPosition(AvatarIKGoal.LeftFoot, leftFoot.position);
            animator.SetIKRotation(AvatarIKGoal.LeftFoot, leftFoot.rotation);
        }
    }
}
