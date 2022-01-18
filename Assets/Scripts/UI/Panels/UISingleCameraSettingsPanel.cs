using UnityEngine;

namespace Virtupad
{
    public class UISingleCameraSettingsPanel : UIAnimationPanel
    {
        public static UISingleCameraSettingsPanel Instance { get; private set; }

        [SerializeField] private Vector3 forwardTrackingPosition = new Vector3(0.0f, 0.8f, 1.25f);
        [SerializeField] private Quaternion trackingRotation;
        private Vector3 forwardVec;

        public UICameraSetting CameraSetting => CameraSetting;
        [SerializeField] private UICameraSetting cameraSetting;

        private Transform toTrack;

        protected override void Awake()
        {
            if (Instance)
            {
                Debug.LogWarning("UISingleCameraSettingsPanel already in scene. Deleting myself!");
                Destroy(this);
                return;
            }
            Instance = this;

            base.Awake();
            trackingRotation = transform.rotation;
        }

        protected override void Start()
        {
            base.Start();

            gameObject.SetActive(false);
        }

        protected override void OnEnable()
        {
            toTrack = VRController.Instance?.bodyCollider;
            Quaternion rot = Quaternion.AngleAxis(VRController.Instance.head.rotation.eulerAngles.y, Vector3.up);
            transform.rotation = rot * trackingRotation;
            forwardVec = rot * forwardTrackingPosition;

            base.OnEnable();
        }

        public void Open(StudioCamera camera)
        {
            if (cameraSetting.OnCamera != camera)
                cameraSetting.OnCamera = camera;

            RegainFocus(null);
        }

        private void Update()
        {
            if (toTrack == null)
                return;

            transform.position = toTrack.position + forwardVec;
            //transform.rotation = toTrack.rotation * trackingRotation;
        }

        protected override void OnHidingAnimationStarted()
        {
            cameraSetting.DeRegisterInput();
        }

        public void Close()
        {
            LooseFocus(true);
        }

        protected override void OnHidingAnimationFinished()
        {
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }
    }
}
