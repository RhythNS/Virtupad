using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllingTest : MonoBehaviour
{
    [SerializeField] private Transform source;
    [SerializeField] private Vector3 offset;

    private void SetOffset()
    {
        offset = (source.position) - transform.position;
    }

    private void Start()
    {
        SetOffset();
    }

    private void Update()
    {
        Vector3 currentPos = source.position;

        transform.rotation = Quaternion.AngleAxis(source.rotation.eulerAngles.y, Vector3.up);

        float angle = Vector3.SignedAngle(transform.forward, Vector3.forward, Vector3.up);
        
        Debug.Log(angle);
        transform.position = currentPos - (Quaternion.Euler(0.0f, -angle, 0.0f) * offset);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + offset, new Vector3(0.1f,0.1f,0.1f));
    }
}
