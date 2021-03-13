using UnityEngine;

public static class GizmosUtil
{
    public static void DrawPoint(Vector2 point, float length = 0.125f)
    {
        Vector3 position = point;
        Gizmos.DrawLine(position + new Vector3(-length, -length), position + new Vector3(length, length));
        Gizmos.DrawLine(position + new Vector3(-length, length), position + new Vector3(length, -length));
    }
}
