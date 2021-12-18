using UnityEngine;

namespace Virtupad
{
    public class VRMLookTarget : MonoBehaviour
    {
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, 0.1f);
        }
    }
}
