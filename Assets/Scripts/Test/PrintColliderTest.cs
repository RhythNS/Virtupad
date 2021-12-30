using UnityEngine;

namespace Virtupad
{
    public class PrintColliderTest : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            Debug.Log(gameObject.name + " has hit " + other.gameObject.layer);
        }
    }
}
