using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTest2 : MonoBehaviour
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.contactCount > 1)
            Debug.Log("Contact point was " + collision.contactCount);

        for (int i = 0; i < collision.contactCount; i++)
        {
            ContactPoint contactPoint = collision.GetContact(i);
            Vector3 toPush = (contactPoint.normal * contactPoint.separation);
            toPush.y = 0.0f;
            transform.position = transform.position + toPush;
        }
    }
}
