using TMPro;
using UnityEngine;

namespace Virtupad
{
    public class UIFullBodyPanel : UIPanel
    {
        [SerializeField] private UIFullBodyTrackerDisplay display;
        [SerializeField] private TMP_Text countdownText;

        private void OnEnable()
        {
            OnRetry();
        }

        public void OnRetry()
        {
            RigMaker.Config? config = VRMController.Instance?.FullRigCreator?.GetConfig();
            display.Init(config);
        }

        public void OnStartAssign()
        {
            UICurrentModelPanel.Instance.Switcher.SwitchChild((int)ModelConfigSwitcherIndexes.Coutdown);
            VRMController.Instance.FullRigCreator.StartAutoSetup(5, OnSecondTickedOnCountdown, OnRigConstructed);
        }

        private void OnSecondTickedOnCountdown(int secondsRemaining)
        {
            countdownText.text = secondsRemaining.ToString();
        }

        private void OnRigConstructed()
        {
            UICurrentModelPanel.Instance.Switcher.SwitchChild((int)ModelConfigSwitcherIndexes.MainPanel);
            UIRoot.Instance.CloseRequest();
        }
    }
}
