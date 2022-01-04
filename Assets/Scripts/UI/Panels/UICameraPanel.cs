using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Virtupad
{
    public class UICameraPanel : UIPanel
    {
        public static UICameraPanel Instance { get; private set; }

        public UIElementSwitcher Switcher => switcher;
        [SerializeField] private UIElementSwitcher switcher;


        [SerializeField] private Transform buttonPanel;
        [SerializeField] private UICameraSelection selectionPrefab;
        [SerializeField] private Transform cameraAddButton;

        private void Awake()
        {
            if (Instance)
            {
                Debug.LogWarning("UICameraPanel already in scene. Deleting myself!");
                Destroy(this);
                return;
            }
            Instance = this;
        }

        private void OnEnable()
        {
            if (StudioCameraManager.Instance == null)
                return;

            List<StudioCamera> cameras = StudioCameraManager.Instance.Cameras;
            OnStudioCamerasChanged(cameras);

            StudioCameraManager.Instance.OnCamerasChanged += OnStudioCamerasChanged;
        }

        private void OnDisable()
        {
            if (StudioCameraManager.Instance)
                StudioCameraManager.Instance.OnCamerasChanged -= OnStudioCamerasChanged;
        }

        private void OnStudioCamerasChanged(List<StudioCamera> cameras)
        {
            int childCount = buttonPanel.childCount - 1; // Do not count the camera add button
            if (childCount < cameras.Count)
            {
                for (int i = childCount; i < cameras.Count; i++)
                {
                    Instantiate(selectionPrefab, buttonPanel);
                }
            }
            else if (childCount > cameras.Count)
            {
                for (int i = childCount - 1; i >= cameras.Count; i--)
                {
                    Transform trans = buttonPanel.GetChild(i);
                    trans.parent = null;
                    Destroy(trans.gameObject);
                }
            }

            cameraAddButton.transform.SetAsLastSibling();
            for (int i = 0; i < cameras.Count; i++)
            {
                buttonPanel.GetChild(i).GetComponent<UICameraSelection>().Init(cameras[i]);
            }
        }

        public void OnChangeCamera(StudioCamera studioCamera)
        {

        }

        public void OnCameraAdd()
        {

        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }
    }
}
