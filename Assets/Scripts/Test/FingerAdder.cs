using UnityEngine;

namespace Virtupad
{
    public class FingerAdder : MonoBehaviour
    {
        [SerializeField] private HumanBodyBones bone;
        [SerializeField] private bool useOffset;
        [SerializeField] private Quaternion customOffset;
        [SerializeField] private bool useCustomOffset = false;

        private void Start()
        {
            ConstructorDict.Instance.RegisterFinger(bone, transform, useOffset, customOffset, useCustomOffset);
        }

        private void OnDestroy()
        {
            ConstructorDict.Instance?.DeRegisterFinger(transform);
        }
    }
}
