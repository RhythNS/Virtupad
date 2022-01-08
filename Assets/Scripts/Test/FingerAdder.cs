using UnityEngine;

namespace Virtupad
{
    public class FingerAdder : MonoBehaviour
    {
        [SerializeField] private HumanBodyBones bone;
        [SerializeField] private bool useOffset;

        private void Start()
        {
            ConstructorDict.Instance.RegisterFinger(bone, transform, useOffset);
        }

        private void OnDestroy()
        {
            ConstructorDict.Instance?.DeRegisterFinger(transform);
        }
    }
}
