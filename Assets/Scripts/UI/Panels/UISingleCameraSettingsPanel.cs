using UnityEngine;

namespace Virtupad
{
    public class UISingleCameraSettingsPanel : UIAnimationPanel
    {
        public static UISingleCameraSettingsPanel Instance { get; private set; }

        [SerializeField] private Vector3 forwardTrackingPosition = new Vector3(0.0f, 0.8f, 1.25f);
        [SerializeField] private Quaternion trackingRotation;

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

        private void Start()
        {
            gameObject.SetActive(false);
        }

        protected override void OnEnable()
        {
            toTrack = VRController.Instance?.bodyCollider;

            if (toTrack != null)
                transform.rotation = toTrack.rotation * trackingRotation;

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

            transform.position = toTrack.position + toTrack.TransformDirection(forwardTrackingPosition);
            //transform.rotation = toTrack.rotation * trackingRotation;
        }

        public void Close()
        {
            LooseFocus(true);
        }

        protected override void OnHidden()
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
