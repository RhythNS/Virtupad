using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllingHip : MonoBehaviour
{
    public Animator animator;
    public Vector3 offset;
    public Transform hipTrans;
    public Transform vrHip;

    private void FixedUpdate()
    {
        Vector3 currentPos = transform.position;
        animator.transform.position = currentPos + offset;
        /*
        float newRotY = transform.rotation.eulerAngles.y;
        Vector3 currentRot = animator.transform.rotation.eulerAngles;
        animator.transform.rotation = Quaternion.Euler(currentRot.x, newRotY, currentRot.z);
         */
    }
    /*
    private void OnAnimatorIK(int layerIndex)
    {
        Debug.Log("ik");

        hipTrans.rotation = vrHip.rotation;
        hipTrans.position = vrHip.position;

    }
     */
}
