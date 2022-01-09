using UnityEngine;
using UnityEngine.Events;
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
        [SerializeField] private Toggle previewToggle;
        [SerializeField] private Toggle easyRotateToggle;
        [SerializeField] private Slider fovSlider;
        [SerializeField] private UISelector cameraTypeSelector;

        [SerializeField] private Transform autoFollowPanel;
        [SerializeField] private Toggle autoFollowToggle;
        [SerializeField] private UISelector followSelector;

        [SerializeField] private Transform trackingBodyPartPanel;
        [SerializeField] private Toggle autoTrackToggle;
        [SerializeField] private UISelector trackingBodyPartSelector;


        [SerializeField] private bool onMainMenu = false;
        [SerializeField] private Transform forSmallPanel;
        [SerializeField] private Transform forMainMenuPanel;

        public UnityEvent shouldClose;

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

            previewToggle.isOn = onCamera.IsPreviewOutputting;
            easyRotateToggle.isOn = onCamera.Grabbable.EasyRotationLock != 0;
            fovSlider.value = onCamera.OutputCamera.fieldOfView;
            cameraTypeSelector.Index = (int)onCamera.PrefabType;

            autoFollowToggle.isOn = onCamera.AutoFollow;
            autoFollowPanel.gameObject.SetActive(onCamera.AutoFollow);
            followSelector.Index = (int)onCamera.TrackingSpace;

            autoTrackToggle.isOn = onCamera.Tracking;
            trackingBodyPartPanel.gameObject.SetActive(OnCamera.Tracking);
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
            if (onCamera == null)
                return;

            OnCamera.SetToTrack(newValue);
            trackingBodyPartPanel.gameObject.SetActive(newValue);
            OnInit();
        }

        public void OnTrackingBodyPartChanged(int newValue)
        {
            if (onCamera == null)
                return;

            StudioCamera.ToTrack toTrack = (StudioCamera.ToTrack)newValue;
            OnCamera.SetTrackingBodyPart(toTrack);
        }

        public void OnCameraTypeChanged(int newValue)
        {
            if (onCamera == null)
                return;

            StudioCamera.CameraType cameraType = (StudioCamera.CameraType)newValue;
            OnCamera.ChangeType(cameraType);
        }

        public void OnAutoFollowChanged(bool newValue)
        {
            if (onCamera == null)
                return;

            OnCamera.SetAutoFollow(newValue);
            autoFollowPanel.gameObject.SetActive(newValue);
            OnInit();
        }

        public void OnFollowTypeChanged(int newValue)
        {
            if (onCamera == null)
                return;

            OnCamera.SetFollowType((CameraMover.TrackingSpace)newValue);
        }

        public void OnEasyMoveChanged(bool newValue)
        {
            if (onCamera == null)
                return;

            onCamera.Grabbable.EasyRotationLock = newValue ? XYZ.X : 0;
        }

        public void OnDeletePressed()
        {
            if (onCamera == null)
                return;

            Destroy(OnCamera.gameObject);
            shouldClose?.Invoke();
        }
    }
}
