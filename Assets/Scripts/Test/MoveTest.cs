using System.Collections.Generic;
using UnityEngine;

namespace Virtupad
{
    public class MoveTest : MonoBehaviour
    {
        [SerializeField] private Transform toMove;

        private Collider ownCollider;

        [SerializeField] private List<Collider> intersecting = new List<Collider>();

        private void Awake()
        {
            ownCollider = GetComponent<Collider>();

            Physics.IgnoreCollision(ownCollider, toMove.GetComponent<Collider>());
        }

        private void FixedUpdate()
        {
            if (intersecting.Count > 0)
                return;

            toMove.position = transform.position;
            toMove.rotation = transform.rotation;
        }

        private void OnTriggerEnter(Collider other)
        {
            intersecting.Add(other);
        }

        private void OnTriggerExit(Collider other)
        {
            intersecting.Remove(other);
        }
    }
}
