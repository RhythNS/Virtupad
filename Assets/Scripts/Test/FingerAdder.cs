using UnityEngine;

public class FingerAdder : MonoBehaviour
{
    [SerializeField] private HumanBodyBones bone;
    [SerializeField] private bool useOffset;

    private void Start()
    {
        VRAnimatorController.Instance.Register(bone, transform, useOffset);
    }

    private void OnDestroy()
    {
        if (VRAnimatorController.Instance)
            VRAnimatorController.Instance.DeRegister(transform);
    }
}
