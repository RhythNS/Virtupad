using System.Collections.Generic;
using UnityEngine;

namespace Virtupad
{
    public class VRMFingerAnimator : MonoBehaviour
    {
        private List<FingerValue> fingerValues = new List<FingerValue>();

        private struct FingerValue
        {
            public Transform transform;
            public int index;
            public Vector3 from;
            public Vector3 to;

            public FingerValue(Transform transform, int index, Vector3 from, Vector3 to)
            {
                this.transform = transform;
                this.index = index;
                this.from = from;
                this.to = to;
            }
        }

        public bool onRightHand;

        public void ResetFingers()
        {
            Animator animator = GetComponent<Animator>();

            fingerValues.Clear();

            foreach (VRMDict.FingerAnimationValue finger in VRMDict.Instance.FingerAnimationValues)
            {
                if (finger.rightHand != onRightHand)
                    continue;

                fingerValues.Add(new FingerValue(animator.GetBoneTransform(finger.bone), finger.index,
                    finger.angleFrom, finger.angleTo));
            }
        }

        private void Start()
        {
            ResetFingers();    
        }

        public void OnFingerUpdate(float[] fingerCurls)
        {
            foreach (var finger in fingerValues)
            {
                finger.transform.localRotation = Quaternion.Euler(Vector3.Lerp(finger.from, finger.to, fingerCurls[finger.index]));
            }
        }
    }
}
