using UnityEngine;

public class IKFootGroundChecker : MonoBehaviour
{
    public FootOffset leftFoot, rightFoot;

    private Animator animator;

    [System.Serializable]
    public struct FootOffset
    {
        public float posWeight;
        public float rotWeight;
        public Vector3 offset;

        public FootOffset(float posWeight, float rotWeight, Vector3 offset)
        {
            this.posWeight = posWeight;
            this.rotWeight = rotWeight;
            this.offset = offset;
        }
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnAnimatorIK(int layerIndex)
    {
        DoFoot(AvatarIKGoal.LeftFoot, leftFoot);
        DoFoot(AvatarIKGoal.LeftFoot, rightFoot);
    }

    private void DoFoot(AvatarIKGoal goal, FootOffset foot)
    {
        Vector3 footPos = animator.GetIKPosition(goal);

        if (Physics.Raycast(footPos + Vector3.up, Vector3.down, out RaycastHit hit) == false)
        {
            animator.SetIKPositionWeight(goal, 0);
        }
        else
        {
            animator.SetIKPositionWeight(goal, foot.posWeight);
            animator.SetIKPosition(goal, hit.point + foot.offset);

            Quaternion rightFootRotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward, hit.normal), hit.normal);
            animator.SetIKRotationWeight(goal, foot.rotWeight);
            animator.SetIKRotation(goal, rightFootRotation);
        }
    }
}
