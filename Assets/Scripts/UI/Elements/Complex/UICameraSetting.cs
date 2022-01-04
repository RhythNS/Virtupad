using UnityEngine;
using UnityEngine.UI;

namespace Virtupad
{
    public class UICameraSetting : UIPrimitiveElement
    {
        public StudioCamera OnCamera
        {
            get => onCamera;
            set
            {
                onCamera = value;
                Init();
            }
        }

        [SerializeField] private StudioCamera onCamera;
        [SerializeField] private Slider fovSlider;
        [SerializeField] private Toggle previewToggle;
        [SerializeField] private UISelector trackingBodyPartSelector;

        [SerializeField] private Transform trackingBodyPartPanel;

        [SerializeField] private bool onMainMenu = false;
        [SerializeField] private Transform forSmallPanel;
        [SerializeField] private Transform forMainMenuPanel;

        private void OnEnable()
        {
            forSmallPanel.gameObject.SetActive(onMainMenu == false);
            forMainMenuPanel.gameObject.SetActive(onMainMenu == true);

            Init();
        }

        private void Init()
        {
            if (onCamera == null)
                return;

            fovSlider.value = onCamera.OutputCamera.fieldOfView;
            previewToggle.isOn = onCamera.IsPreviewOutputting;
            trackingBodyPartPanel.gameObject.SetActive(OnCamera.IsTracking);
            trackingBodyPartSelector.Index = (int)OnCamera.TrackingBodyPart;
        }

        public void OnPreviewToggleChanged(bool newValue)
        {
            if (onCamera == null)
                return;

            if (newValue)
                onCamera.ActivatePreview();
            else
                onCamera.DeactivatePreview();
        }

        public void OnFOVChanged(float newValue)
        {
            if (onCamera == null)
                return;

            onCamera.OutputCamera.fieldOfView = newValue;
            onCamera.PreviewCamera.fieldOfView = newValue;
        }

        public void OnAutoTrackChanged(bool newValue)
        {
            OnCamera.Tracking = newValue ? VRMController.Instance.transform : null;
            trackingBodyPartPanel.gameObject.SetActive(newValue);
        }

        public void OnTrackingBodyPartChanged(int newValue)
        {
            StudioCamera.ToTrack toTrack = (StudioCamera.ToTrack)newValue;
            OnCamera.TrackingBodyPart = toTrack;
        }

        public void OnDeletePressed()
        {
            Destroy(OnCamera.gameObject);
        }
    }
}
