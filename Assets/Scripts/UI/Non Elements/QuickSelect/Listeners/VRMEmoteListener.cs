using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Virtupad
{
    public class VRMEmoteListener : MonoBehaviour, IUIQuickSelectListener
    {
        public int GetCurrentSelection()
        {
            return -1;
        }

        public List<string> GetSelections()
        {
            return new List<string>();
        }

        public void OnPreviewChanged(int newIndex)
        {
        }

        public void OnSelectionChanged(int newIndex)
        {
        }

        public void OnStart()
        {
        }

        public void OnStop()
        {
        }
    }
}
