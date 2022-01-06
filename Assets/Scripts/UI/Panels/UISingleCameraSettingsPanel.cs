using UnityEngine;

namespace Virtupad
{
    public class UISingleCameraSettingsPanel : UIAnimationPanel
    {
        public static UISingleCameraSettingsPanel Instance { get; private set; }

        public UICameraSetting CameraSetting => CameraSetting;
        [SerializeField] private UICameraSetting cameraSetting;

        protected override void Awake()
        {
            if (Instance)
            {
                Debug.LogWarning("UISingleCameraSettingsPanel already in scene. Deleting myself!");
                Destroy(this);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            gameObject.SetActive(false);
        }

        public void Open(StudioCamera camera)
        {
            if (cameraSetting.OnCamera != camera)
                cameraSetting.OnCamera = camera;

            RegainFocus(null);
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
