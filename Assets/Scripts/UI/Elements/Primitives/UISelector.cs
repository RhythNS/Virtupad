using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Virtupad
{
    public class UISelector : UIPrimitiveElement
    {
        public List<string> selections = new List<string>();

        public UnityEvent<int> selectionChanged;

        [SerializeField] private TMP_Text textDisplay;

        [SerializeField] private int index;
        public int Index
        {
            get => index;
            set
            {
                value = Mathf.Clamp(value, 0, selections.Count - 1);

                if (index == value)
                    return;

                index = value;
                OnUpdateValue();
            }
        }

        protected override void Start()
        {
            base.Start();

            OnUpdateValue();
        }

        private void OnUpdateValue()
        {
            selectionChanged?.Invoke(index);
            textDisplay.text = selections[index];
        }

        public void Next()
        {
            if (index + 1 >= selections.Count)
                Index = 0;
            else
                ++Index;
        }

        public void Prev()
        {
            if (Index - 1 < 0)
                Index = selections.Count - 1;
            else
                --Index;
        }
    }
}
