using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR.InteractionSystem;
using VRM;

namespace Virtupad
{
    public class UILoadVRMInProgressPanel : UIPanel
    {
        public static UILoadVRMInProgressPanel Instance { get; private set; }

        [SerializeField] private Image image;
        [SerializeField] UIElementSwitcher elementSwitcher;
        [SerializeField] private TMP_Text errorText;

        private string currentPath;

        protected override void Awake()
        {
            if (Instance)
            {
                Debug.LogWarning("UIVRMSelector already in scene. Deleting myself!");
                Destroy(this);
                return;
            }
            Instance = this;

            base.Awake();
        }

        private void OnEnable()
        {
            StartCoroutine(UIVRMSelector.Instance.DoLoadingAnimation(image));
            elementSwitcher.SwitchChild(0);
        }

        public void TryToLoad(string path)
        {
            currentPath = path;

            GetPositionAndRotation(out Vector3 position, out Quaternion rotation);
            VRMLoader.Instance.SpawnModel(path, position, rotation, OnSuccess, OnFailure);
        }

        public void TryToLoad(VRMHumanoidDescription prefab)
        {
            GetPositionAndRotation(out Vector3 position, out Quaternion rotation);
            VRMLoader.Instance.LoadPrefab(prefab, position, rotation);
            OnSuccess();
        }

        private void GetPositionAndRotation(out Vector3 position, out Quaternion rotation)
        {
            position = Player.instance.transform.position + Player.instance.transform.forward;
            rotation = Quaternion.identity;
        }

        public void OnOkayButtonPressed()
        {
            UIVRMSelector.Instance.Switcher.SwitchChild((int)VRMSelectorSwitcherIndexes.LastLoaded);
        }

        private void OnSuccess()
        {
            Texture2D preview = null;
            if (VRMLoader.Instance?.Meta != null)
                preview = VRMLoader.Instance.Meta.Thumbnail;

            UIVRMSelector.Instance.LastLoadedVRMs.Add(currentPath, preview);

            RigMaker.Config? config = VRMController.Instance?.FullRigCreator?.GetConfig();
            if (config == null || config.Value != RigMaker.Config.ThreePoints)
            {
                UIRoot.Instance.MainSwitcher.SwitchChild((int)MidPanelSwitcherIndexes.VRMModelConfigSettings);
                UICurrentModelPanel.Instance.Switcher.SwitchChild((int)ModelConfigSwitcherIndexes.FullBodySetup);
                return;
            }

            UIRoot.Instance.CloseRequest();
            VRMController.Instance.FullRigCreator.StartAutoSetup(0);
        }

        private void OnFailure(string errorMessage)
        {
            elementSwitcher.SwitchChild(1);
            errorText.text = errorMessage;
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }
    }
}
