using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Valve.VR;
using Valve.VR.InteractionSystem;

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

        [SerializeField] private TMP_Text cameraName;

        [SerializeField] private Toggle outputToDesktopToggle;
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

        [SerializeField] private SteamVR_Action_Vector2 walkingInput;
        [SerializeField] private SteamVR_Action_Vector2 lookingInput;

        public UnityEvent shouldClose;

        private bool overwritingControllerInputs = false;
        private float movingAngleOffset;

        private bool initing = false;

        private void OnEnable()
        {
            forSmallPanel.gameObject.SetActive(onMainMenu == false);
            forMainMenuPanel.gameObject.SetActive(onMainMenu == true);

            Init();

            if (StudioCameraManager.Instance && onCamera)
                StudioCameraManager.Instance.OnActiveStudioCameraChanged += OnActiveStudioCameraChanged;
        }

        private void OnDisable()
        {
            DeRegisterInput();

            if (StudioCameraManager.Instance)
                StudioCameraManager.Instance.OnActiveStudioCameraChanged -= OnActiveStudioCameraChanged;

        }

        private void Init()
        {
            if (onCamera == null)
                return;

            initing = true;

            RegisterInput();

            cameraName.text = "Camera " + (onCamera.Id + 1);

            outputToDesktopToggle.isOn = onCamera == StudioCameraManager.Instance.ActiveCamera;
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

            initing = false;
        }

        public void RegisterInput()
        {
            DeRegisterInput();

            overwritingControllerInputs = true;

            VRController.Instance.RegisterRotation(Rotate);
            VRController.Instance.RegisterWalking(Move);

            Vector3 ownPos = Player.instance.hmdTransform.position;
            Vector3 cameraPos = OnCamera.transform.position;

            ownPos.y = 0.0f;
            cameraPos.y = 0.0f;

            Vector3 toCameraDir = (cameraPos - ownPos).normalized;

            movingAngleOffset = Vector3.SignedAngle(Vector3.forward, toCameraDir, Vector3.up);
        }

        public void DeRegisterInput()
        {
            if (overwritingControllerInputs == false)
                return;

            overwritingControllerInputs = false;

            VRController.Instance?.DeRegisterRotation(Rotate);
            VRController.Instance?.DeRegisterWalking(Move);
        }

        private void Update()
        {
            if (onCamera == null || overwritingControllerInputs == false)
                return;

            float toMove = StudioCameraManager.Instance.MovingMetersPerSecond * Time.deltaTime;
            Vector3 moveVec = Quaternion.AngleAxis(movingAngleOffset, Vector3.up)
                * new Vector3(walkingInput.axis.x * toMove, 0.0f, walkingInput.axis.y * toMove);
            //.MoveTo(OnCamera.Body.position + moveVec);
            onCamera.transform.position = onCamera.transform.position + moveVec;

            float rotatingAnglesPerSecond = StudioCameraManager.Instance.RotatingAnglesPerSecond;
            Vector2 axis = -lookingInput.axis * (rotatingAnglesPerSecond * Time.deltaTime);
            Vector3 prevAngle = OnCamera.transform.rotation.eulerAngles;
            onCamera.transform.rotation = Quaternion.Euler(prevAngle.x + axis.y, prevAngle.y + axis.x, prevAngle.z);
        }

        private bool Rotate(SteamVR_Action_Vector2 input)
        {
            if (onCamera == null)
            {
                DeRegisterInput();
                return false;
            }
            return true;
        }

        private bool Move(SteamVR_Action_Vector2 input)
        {
            if (onCamera == null)
            {
                DeRegisterInput();
                return false;
            }
            return true;
        }

        private void OnActiveStudioCameraChanged(StudioCamera newCamera)
        {
            outputToDesktopToggle.isOn = newCamera == onCamera;
        }

        public void OnOutputToDesktopChanged(bool newValue)
        {
            if (onCamera == null || initing == true)
                return;

            if (newValue == true && onCamera == StudioCameraManager.Instance.ActiveCamera)
                return;
            if (newValue == false && onCamera != StudioCameraManager.Instance.ActiveCamera)
                return;

            StudioCameraManager.Instance.ActiveCamera = newValue ? onCamera : null;
        }

        public void OnPreviewToggleChanged(bool newValue)
        {
            if (onCamera == null || initing == true)
                return;

            if (newValue)
                onCamera.ActivatePreview();
            else
                onCamera.DeactivatePreview();
        }

        public void OnFOVChanged(float newValue)
        {
            if (onCamera == null || initing == true)
                return;

            onCamera.OutputCamera.fieldOfView = newValue;
            onCamera.PreviewCamera.fieldOfView = newValue;
        }

        public void OnAutoTrackChanged(bool newValue)
        {
            if (onCamera == null || initing == true)
                return;

            OnCamera.SetToTrack(newValue);
            trackingBodyPartPanel.gameObject.SetActive(newValue);
            OnInit();
        }

        public void OnTrackingBodyPartChanged(int newValue)
        {
            if (onCamera == null || initing == true)
                return;

            StudioCamera.ToTrack toTrack = (StudioCamera.ToTrack)newValue;
            OnCamera.SetTrackingBodyPart(toTrack);
        }

        public void OnCameraTypeChanged(int newValue)
        {
            if (onCamera == null || initing == true)
                return;

            StudioCamera.CameraType cameraType = (StudioCamera.CameraType)newValue;
            OnCamera = OnCamera.ChangeType(cameraType);
        }

        public void OnAutoFollowChanged(bool newValue)
        {
            if (onCamera == null || initing == true)
                return;

            OnCamera.SetAutoFollow(newValue);
            autoFollowPanel.gameObject.SetActive(newValue);
            OnInit();
        }

        public void OnFollowTypeChanged(int newValue)
        {
            if (onCamera == null || initing == true)
                return;

            OnCamera.SetFollowType((CameraMover.TrackingSpace)newValue);
        }

        public void OnEasyMoveChanged(bool newValue)
        {
            if (onCamera == null || initing == true)
                return;

            onCamera.Grabbable.EasyRotationLock = newValue ? XYZ.Z : 0;
        }

        public void OnDeletePressed()
        {
            if (onCamera == null || initing == true)
                return;

            Destroy(OnCamera.gameObject);
            shouldClose?.Invoke();
        }
    }
}
