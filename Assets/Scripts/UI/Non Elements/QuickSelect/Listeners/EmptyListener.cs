using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Virtupad
{
    public class EmptyListener : MonoBehaviour, IUIQuickSelectListener
    {
        [SerializeField] private List<string> selections = new List<string>();
        [SerializeField] private int currentSelection;

        public int GetCurrentSelection()
        {
            return currentSelection;
        }

        public List<string> GetSelections()
        {
            return selections;
        }

        public void OnPreviewChanged(int newIndex)
        {
        }

        public void OnSelectionChanged(int newIndex)
        {
            currentSelection = newIndex;
        }

        public void OnStart()
        {
        }

        public void OnStop()
        {
        }
    }
}
