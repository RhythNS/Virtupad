using UnityEngine;

public abstract class ShapesUtil
{
    public static void RectFloor(ref Rect rect)
    {
        rect.x = Mathf.Floor(rect.x);
        rect.y = Mathf.Floor(rect.y);
        rect.width = Mathf.Floor(rect.width);
        rect.height = Mathf.Floor(rect.height);
    }

    public static void RectRoundToNextInt(ref Rect rect)
    {
        rect.x = Mathf.Round(rect.x);
        rect.y = Mathf.Round(rect.y);
        rect.width = Mathf.Round(rect.width);
        rect.height = Mathf.Round(rect.height);
    }

    public static void SetPositionWithMidPoint(ref Rect rect, in Vector2 midPoint)
    {
        rect.x = midPoint.x - rect.width / 2;
        rect.y = midPoint.y - rect.height / 2;
    }
}
