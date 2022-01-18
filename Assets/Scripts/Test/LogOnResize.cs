using UnityEngine;

namespace Virtupad
{
    public class LogOnResize : MonoBehaviour
    {
        private void OnRectTransformDimensionsChange()
        {
            Debug.Log(gameObject.name + " resized!");
        }
    }
}
