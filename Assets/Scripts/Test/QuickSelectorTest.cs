using UnityEngine;

namespace Virtupad
{
    public class QuickSelectorTest : MonoBehaviour
    {
        public void PreviewChanged(int newIndex)
        {
            Debug.Log("preview: " + newIndex);
        }

        public void SelectionChanged(int newIndex)
        {
            Debug.Log("selection: " + newIndex);
        }
    }
}
