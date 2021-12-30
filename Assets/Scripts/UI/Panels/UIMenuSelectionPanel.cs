using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Virtupad
{
    public class UIMenuSelectionPanel : UIPanel
    {
        public bool CanSelect() => gameObject.activeInHierarchy;

        public void SetSelectable(bool selectable) => gameObject.SetActive(selectable);

        public void OnPressed(int index)
        {
            UIRoot.Instance.MainSwitcher.SwitchChild(index);
        }

        public void OnElementSelected(int index)
        {
        }
    }
}
