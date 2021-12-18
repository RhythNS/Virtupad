using UnityEngine;
using UnityEngine.EventSystems;

namespace Virtupad
{
    public class SliderTest : MonoBehaviour, IDragHandler
    {
        public void OnDrag(PointerEventData eventData)
        {
            int x = 1;
            x--;
        }
    }
}
