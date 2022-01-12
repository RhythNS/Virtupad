using SimpleFileBrowser;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Virtupad
{
    public class UISettingsPanel : UIPanel
    {
        public UIElementSwitcher Switcher => switcher;
        [SerializeField] private UIElementSwitcher switcher;

        [SerializeField] private FileBrowser fileBrowser;

        [SerializeField] private Slider movementSpeedSlider;
        [SerializeField] private Slider rotationSpeedSlider;
        [SerializeField] private UISelector moveTypeSelector;

        [SerializeField] private UISelector lipSyncSelector;
        [SerializeField] private string noMicrophoneFound = "No input sources";

        [SerializeField] private RawImage noOutputImage;

        [SerializeField] private UISelector screenModeSelector;
        [SerializeField] private UISelector resolutionSelector;
        private Resolution[] resolutions;
        private ExtendedCoroutine resolutionCoroutine;

        private bool initing = false;

        private void OnEnable()
        {
            FileBrowser.OverideInstance(fileBrowser);
            InitValues();
        }

        protected override void Start()
        {
            base.Start();

            OnOutputImageChanged(UINoActiveCamera.Instance.CustomTexture);
        }

        private void InitValues()
        {
            initing = true;

            VRController vrCon = VRController.Instance;
            movementSpeedSlider.value = vrCon.WalkingSpeed;
            rotationSpeedSlider.value = vrCon.AnglesPerSecond;
            moveTypeSelector.Index = vrCon.secondaryWalkingDirectionInputOption;

            lipSyncSelector.selections.Clear();
            Array.ForEach(Microphone.devices, x => lipSyncSelector.selections.Add(x));
            lipSyncSelector.ManuallyChangedSelections();

            resolutionSelector.selections.Clear();
            Resolution currentResolution = Screen.currentResolution;
            int selectedIndex = 0;
            resolutions = Screen.resolutions;
            for (int i = 0; i < resolutions.Length; i++)
            {
                Resolution res = resolutions[i];
                resolutionSelector.selections.Add(res.width + " x " + res.height + " "
                    + res.refreshRate + "Hz");

                if (res.width == currentResolution.width && res.width == currentResolution.height &&
                    currentResolution.refreshRate == currentResolution.refreshRate)
                    selectedIndex = i;
            }
            resolutionSelector.ManuallyChangedSelections();
            resolutionSelector.Index = selectedIndex;

            screenModeSelector.Index = (int)Screen.fullScreenMode;

            initing = false;
        }

        public void OnMovementSpeedChanged(float newValue)
        {
            if (initing == true)
                return;

            VRController.Instance.ChangeSpeed(movementSpeed: newValue);
        }

        public void OnRotationSpeedChanged(float newValue)
        {
            if (initing == true)
                return;

            VRController.Instance.ChangeSpeed(rotationSpeed: newValue);
        }

        public void OnMoveTypeChanged(int newValue)
        {
            if (initing == true)
                return;

            VRController.Instance.ChangeSpeed(movementType: newValue);
        }

        public void OnLipSyncChanged(int newValue)
        {
            if (initing == true || lipSyncSelector.selections[0] == noMicrophoneFound)
                return;

            SalsaDict.Instance.SetMicrophone(lipSyncSelector.selections[newValue]);
        }

        public void OnScreenModeChanged(int newValue)
        {
            if (initing == true)
                return;

            Screen.fullScreenMode = (FullScreenMode)newValue;
        }

        public void OnResolutionChanged(int newValue)
        {
            if (initing == true)
                return;

            if (resolutionCoroutine != null && resolutionCoroutine.IsFinshed == false)
                resolutionCoroutine.Stop(false);

            resolutionCoroutine = ExtendedCoroutine.ActionAfterSeconds(this, 3.0f,
                () => Screen.SetResolution(resolutions[newValue].width, resolutions[newValue].height, Screen.fullScreenMode, resolutions[newValue].refreshRate)
                );
        }

        public void ResetCustomNoCameraImage()
        {
            UINoActiveCamera.Instance.ResetCustomTexture();

            SaveFileManager saveFileManager = SaveFileManager.Instance;
            saveFileManager.saveGame.customNoCameraActivePath = "";
            saveFileManager.Save();

            OnOutputImageChanged(null);
        }

        public void LoadCustomNoCameraImageFromFile()
        {
            Switcher.SwitchChild(1);
            FileBrowser.SetFilters(true, new FileBrowser.Filter("Images", ".png", ".jpg"));
            FileBrowser.ShowLoadDialog(OnFileSuccess, OnFileCancel, FileBrowser.PickMode.Files);
        }

        private void OnFileSuccess(string[] paths)
        {
            if (paths == null || paths.Length == 0 || string.IsNullOrEmpty(paths[0]))
            {
                OnFileCancel();
                return;
            }

            Switcher.SwitchChild(0);
            string path = paths[0];
            if (UINoActiveCamera.Instance.SetCustomImage(path) == false)
            {
                // some error?
                return;
            }

            SaveFileManager saveFileManager = SaveFileManager.Instance;
            saveFileManager.saveGame.customNoCameraActivePath = path;
            saveFileManager.Save();

            OnOutputImageChanged(UINoActiveCamera.Instance.CustomTexture);
        }

        private void OnOutputImageChanged(Texture2D tex)
        {
            if (tex == null)
            {
                noOutputImage.texture = null;
                noOutputImage.color = Color.black;
                return;
            }

            noOutputImage.color = Color.white;
            noOutputImage.texture = tex;
        }

        private void OnFileCancel()
        {
            Switcher.SwitchChild(0);
        }
    }
}
