using System.Collections.Generic;

namespace Virtupad
{
    public interface IUIQuickSelectListener
    {
        public List<string> GetSelections();

        public void OnPreviewChanged(int newIndex);

        public void OnSelectionChanged(int newIndex);

        public int GetCurrentSelection();

        public void OnStart();

        public void OnStop();
    }
}
