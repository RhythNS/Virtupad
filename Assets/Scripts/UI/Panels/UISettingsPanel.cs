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
        [SerializeField] private UISelector rotateTypeSelector;

        [SerializeField] private UISelector lipSyncSelector;
        [SerializeField] private string noMicrophoneFound = "No input sources";

        [SerializeField] private Slider cameraMovementSpeedSlider;
        [SerializeField] private Slider cameraRotationSpeedSlider;

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
            moveTypeSelector.Index = (int)vrCon.MovementMode;
            rotateTypeSelector.Index = (int)vrCon.RotationMode;

            StudioCameraManager cameraManager = StudioCameraManager.Instance;
            cameraMovementSpeedSlider.value = cameraManager.MovingMetersPerSecond;
            cameraRotationSpeedSlider.value = cameraManager.RotatingAnglesPerSecond;

            lipSyncSelector.selections.Clear();
            Array.ForEach(Microphone.devices, x => lipSyncSelector.selections.Add(x));
            lipSyncSelector.ManuallyChangedSelections();

            resolutionSelector.selections.Clear();
            Resolution currentResolution = Screen.currentResolution;
            resolutions = Screen.resolutions;
            int selectedIndex = resolutions.Length - 1;
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

        public void ApplyResolutionChanges()
        {
            Screen.fullScreenMode = (FullScreenMode)screenModeSelector.Index;
            int resIndex = resolutionSelector.Index;
            Screen.SetResolution(resolutions[resIndex].width, resolutions[resIndex].height, Screen.fullScreenMode, resolutions[resIndex].refreshRate);
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

            VRController.Instance.ChangeSpeed(movementMode: (VRControllerMovementMode)newValue);
        }

        public void OnRotationTypeChanged(int newValue)
        {
            if (initing == true)
                return;

            VRController.Instance.ChangeSpeed(rotationMode: (VRControllerRotationMode)newValue);
        }

        public void OnLipSyncChanged(int newValue)
        {
            if (initing == true || lipSyncSelector.selections[0] == noMicrophoneFound)
                return;

            SalsaDict.Instance.SetMicrophone(lipSyncSelector.selections[newValue]);
        }

        public void OnCameraMovementSpeedChanged(float newValue)
        {
            if (initing == true)
                return;

            StudioCameraManager.Instance.ChangeSpeed(movementSpeed: newValue);
        }

        public void OnCameraRotationSpeedChanged(float newValue)
        {
            if (initing == true)
                return;

            StudioCameraManager.Instance.ChangeSpeed(rotationSpeed: newValue);
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
