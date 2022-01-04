using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Virtupad
{
    public class UIQuickSelectElement : MonoBehaviour
    {
        [SerializeField] private Image image;
        private bool isDefaultSelected;
        private UIQuickSelect onSelect;

        public void Init(UIQuickSelect onSelect, bool isDefaultSelected, int index, float percentile)
        {
            this.onSelect = onSelect;
            this.isDefaultSelected = isDefaultSelected;

            if (isDefaultSelected)
                Select();
            else
                Deselect();

            image.fillAmount = percentile;
            transform.localRotation = Quaternion.Euler(0, 0, (-index - 1) * percentile * 360.0f);
        }

        public void FixedInit(UIQuickSelect onSelect, bool isDefaultSelected)
        {
            this.onSelect = onSelect;
            this.isDefaultSelected = isDefaultSelected;

            if (isDefaultSelected)
                Select();
            else
                Deselect();
        }

        public void Select()
        {
            image.color = isDefaultSelected ? onSelect.DefaultSelectColor : onSelect.SelectColor;
        }

        public void Deselect()
        {
            image.color = isDefaultSelected ? onSelect.DefaultDeselectColor : onSelect.DeselectColor;
        }
    }
}
