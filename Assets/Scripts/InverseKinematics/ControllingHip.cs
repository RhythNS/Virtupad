using UnityEngine;

namespace Virtupad
{
    public class ControllingHip : MonoBehaviour
    {
        public Animator animator;
        public Vector3 offset;

        private void FixedUpdate()
        {
            Vector3 currentPos = transform.position;
            animator.transform.position = currentPos - offset;
        }
    }
}
