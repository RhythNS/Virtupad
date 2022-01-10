using System;
using System.Collections;
using System.Collections.Generic;
using uDesktopDuplication;
using UnityEngine;
using UnityEngine.UI;

namespace Virtupad
{
    public class UIMonitorPanel : UIAnimationPanel
    {
        public DesktopInWorld Screen => screen;
        [SerializeField] private DesktopInWorld screen;

        public bool IsOnStaticMonitor => isOnStaticMonitor;
        [SerializeField] private bool isOnStaticMonitor = true;

        public event BoolChanged screenActiveChanged;

        [SerializeField] private UISelector monitorIndexSelector;
        private int currentMonitorIndex = 0;

        [SerializeField] private Toggle displayDesktopToggle;

        [SerializeField] private Transform resizePanel;

        public void Open()
        {
            RegainFocus(null);
        }

        public void Close()
        {
            LooseFocus(true);
        }

        protected override void Awake()
        {
            base.Awake();

            int monitorCount = Manager.monitorCount;

            if (monitorCount != monitorIndexSelector.selections.Count)
            {
                currentMonitorIndex = monitorIndexSelector.Index;

                List<string> selections = monitorIndexSelector.selections;
                selections.Clear();
                for (int i = 0; i < monitorCount; i++)
                    selections.Add((i + 1).ToString());

                monitorIndexSelector.ManuallyChangedSelections();
                monitorIndexSelector.Index = currentMonitorIndex;
            }

            displayDesktopToggle.isOn = screen.gameObject.activeInHierarchy;

            resizePanel.gameObject.SetActive(IsOnStaticMonitor == false);
        }

        public void OnMonitorIndexChanged(int newIndex)
        {
            Screen.CurrentMonitor = newIndex;
        }

        public void OnShowDisplayChanged(bool shouldDisplay)
        {
            Screen.gameObject.SetActive(shouldDisplay);
            screenActiveChanged?.Invoke(shouldDisplay);
        }

        public void ToggleKeyboard()
        {
            VRKeys.Keyboard.Instance.ToggleEnable();
        }

        public void ToggleResize()
        {

        }
    }
}
