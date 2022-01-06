using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Virtupad
{
    public class UIWorldSettingsPanel : UIPanel
    {
        public static UIWorldSettingsPanel Instance { get; private set; }

        public UIElementSwitcher Switcher => switcher;
        [SerializeField] private UIElementSwitcher switcher;

        protected override void Awake()
        {
            if (Instance)
            {
                Debug.LogWarning("UIWorldSettingsPanel already in scene. Deleting myself!");
                Destroy(this);
                return;
            }
            Instance = this;

            base.Awake();
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }
    }
}
