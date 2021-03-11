using UnityEngine;

public class ControllingHead : MonoBehaviour
{
    public Animator animator;
    public Vector3 offset;
    public Transform rigTrans;

    private void FixedUpdate()
    {
        rigTrans.rotation = transform.rotation;

        Vector3 currentPos = transform.position;
        animator.transform.position = currentPos - offset;
        float newRotY = transform.rotation.eulerAngles.y;
        Vector3 currentRot = animator.transform.rotation.eulerAngles;
        animator.transform.rotation = Quaternion.Euler(currentRot.x, newRotY, currentRot.z);
    }
}
