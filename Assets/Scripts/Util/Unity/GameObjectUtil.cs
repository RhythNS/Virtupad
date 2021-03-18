using UnityEngine;

public static class GameObjectUtil
{
    public static void SetLayerRecursively(Transform transform, int layer)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            SetLayerRecursively(transform.GetChild(i), layer);
        }
        transform.gameObject.layer = layer;
    }
}
