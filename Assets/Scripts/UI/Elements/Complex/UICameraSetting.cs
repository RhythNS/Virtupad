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

        private bool overwritingControllerInputs = false;
        float movingAngleOffset;

        private void OnEnable()
        {
            forSmallPanel.gameObject.SetActive(onMainMenu == false);
            forMainMenuPanel.gameObject.SetActive(onMainMenu == true);

            Init();
        }

        private void OnDisable()
        {
            DeRegisterInput();
        }

        private void Init()
        {
            if (onCamera == null)
                return;

            RegisterInput();

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

        private void RegisterInput()
        {
            DeRegisterInput();

            overwritingControllerInputs = true;

            VRController.Instance.RegisterRotation(Rotate);
            VRController.Instance.RegisterWalking(Move);

            Vector3 ownPos = Player.instance.hmdTransform.position;
            Vector3 cameraPos = OnCamera.transform.position;

            ownPos.y = 0.0f;
            cameraPos.y = 0.0f;

            movingAngleOffset = Vector3.SignedAngle(ownPos, cameraPos, Vector3.up);
        }

        private void DeRegisterInput()
        {
            if (overwritingControllerInputs == false)
                return;

            overwritingControllerInputs = false;

            VRController.Instance.DeRegisterRotation(Rotate);
            VRController.Instance.DeRegisterWalking(Move);
        }

        private bool Rotate(SteamVR_Action_Vector2 input)
        {
            if (onCamera == null)
            {
                DeRegisterInput();
                return false;
            }

            float rotatingAnglesPerSecond = StudioCameraManager.Instance.RotatingAnglesPerSecond;
            Vector2 axis = -input.axis * (rotatingAnglesPerSecond * Time.fixedDeltaTime);
            Vector3 prevAngle = OnCamera.Body.rotation.eulerAngles;
            OnCamera.Body.MoveRotation(Quaternion.Euler(prevAngle.x + axis.y, prevAngle.y + axis.x, prevAngle.z));

            return true;
        }

        private bool Move(SteamVR_Action_Vector2 input)
        {
            if (onCamera == null)
            {
                DeRegisterInput();
                return false;
            }

            float toMove = StudioCameraManager.Instance.MovingMetersPerSecond * Time.fixedDeltaTime;
            Vector3 moveVec = Quaternion.AngleAxis(movingAngleOffset, Vector3.up)
                * new Vector3(-input.axis.y * toMove, 0.0f, input.axis.x * toMove);
            OnCamera.Body.MovePosition(OnCamera.Body.position + moveVec);

            return true;
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
