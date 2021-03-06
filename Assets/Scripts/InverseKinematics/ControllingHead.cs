using UnityEngine;

public class ControllingHead : MonoBehaviour
{
    public Animator animator;
    public Vector3 offset;
    public float turnSmoothness = 5.0f;

    private void FixedUpdate()
    {
        Vector3 currentPos = transform.position;

        animator.transform.position = currentPos - offset;

        animator.transform.rotation = Quaternion.AngleAxis(transform.rotation.eulerAngles.y, Vector3.up);
        /*
        float newRotY = transform.rotation.eulerAngles.y;
        Vector3 currentRot = animator.transform.rotation.eulerAngles;
        animator.transform.rotation = Quaternion.Euler(currentRot.x, newRotY, currentRot.z);
         */
        //animator.transform.forward = Vector3.Lerp(animator.transform.forward, Vector3.ProjectOnPlane(transform.up, Vector3.up).normalized, Time.deltaTime * turnSmoothness);
    }
}
