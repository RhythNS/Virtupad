using UnityEngine;

namespace Virtupad
{
    public class TestOutput : MonoBehaviour
    {
        [SerializeField] private Vector2 baseScale;
        [SerializeField] private Vector2 maxDesiredDimensions;

        void Start()
        {
            float xScale = maxDesiredDimensions.x / baseScale.x;
            if (baseScale.y * xScale < maxDesiredDimensions.y)
            {
                baseScale *= xScale;
                Debug.Log(baseScale);
                return;
            }

            float yScale = maxDesiredDimensions.y / baseScale.y;
            baseScale *= yScale;

            Debug.Log(baseScale);
            Debug.Log("alikjglksjdrgklj");
        }
    }
}
