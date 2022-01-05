using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Virtupad
{
    public class UICameraPanel : UIPanel
    {
        public static UICameraPanel Instance { get; private set; }

        public UIElementSwitcher Switcher => switcher;
        [SerializeField] private UIElementSwitcher switcher;

        [SerializeField] private Transform buttonPanel;
        [SerializeField] private UICameraSelection selectionPrefab;
        [SerializeField] private UIPrimitiveElement cameraAddButton;
        [SerializeField] private UIPrimitiveElement cameraParent;

        [SerializeField] private UIElementSwitcher cameraSettingSwitcher;
        [SerializeField] private UICameraSetting cameraSetting;

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
            OnChangeCamera(null);

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
                    UICameraSelection selection = trans.GetComponent<UICameraSelection>();
                    selection.parent.RemoveChild(selection);
                    trans.SetParent(null, false);
                    Destroy(trans.gameObject);
                }
            }

            bool showCameraAddButton = cameras.Count < 9;
            cameraAddButton.gameObject.SetActive(showCameraAddButton);
            cameraAddButton.transform.SetAsLastSibling();

            UIUtil.GetColAndRowsOfGridLayoutGroup(buttonPanel.GetComponent<GridLayoutGroup>(), out int colNum, out int rowNum);
            cameraParent.RemoveAllChildren();

            for (int i = 0; i < cameras.Count; i++)
            {
                UICameraSelection selection = buttonPanel.GetChild(i).GetComponent<UICameraSelection>();

                selection.uiPos = new Vector2Int(i % colNum, rowNum - (i / colNum) - 1);
                cameraParent.AddChild(selection);
                selection.Init(cameras[i]);
            }

            if (showCameraAddButton == true)
            {
                cameraAddButton.uiPos = new Vector2Int((cameras.Count + 1) % colNum, rowNum - ((cameras.Count + 1) / colNum) - 1);
                cameraParent.AddChild(cameraAddButton);
            }
        }

        public void OnChangeCamera(StudioCamera studioCamera)
        {
            cameraSetting.OnCamera = studioCamera;
            cameraSettingSwitcher.SwitchChild(studioCamera == null ? 1 : 0);
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
