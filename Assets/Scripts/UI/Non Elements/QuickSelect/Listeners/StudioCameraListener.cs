using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Virtupad
{
    public class StudioCameraListener : MonoBehaviour, IUIQuickSelectListener
    {
        [SerializeField] private RawImage previewOutput;

        public void OnSelectionChanged(int newIndex)
        {
            StudioCameraManager.Instance.ChangeActiveCamera(newIndex);
        }

        public int GetCurrentSelection()
        {
            StudioCamera activeCamera = StudioCameraManager.Instance.ActiveCamera;
            return activeCamera == null ? 0 : activeCamera.Id;
        }

        public List<string> GetSelections()
        {
            List<StudioCamera> cameras = StudioCameraManager.Instance.Cameras;
            List<string> strings = new List<string>();
            cameras.ForEach(cam => strings.Add((cam.Id + 1).ToString()));

            if (strings.Count == 0)
                strings.Add("No cameras");

            return strings;
        }

        public void OnPreviewChanged(int newIndex)
        {
            List<StudioCamera> cameras = StudioCameraManager.Instance.Cameras;
            if (cameras.Count == 0)
                return;

            previewOutput.texture = cameras[newIndex].PreviewTexture;

            //   DeOutlineCurrentCamera();
            //   highlightingCamera = StudioCameraManager.Instance.Cameras.Find(x => x.Id == newIndex);
            //   if (highlightingCamera != null)
            //       highlightingCamera.ActivateOutline();
        }

        public void OnStart()
        {
            StudioCameraManager.Instance.ForcePreviewRender = true;
            List<StudioCamera> cameras = StudioCameraManager.Instance.Cameras;
            if (cameras.Count == 0)
            {
                previewOutput.gameObject.SetActive(false);
                return;
            }
            previewOutput.gameObject.SetActive(true);
            previewOutput.texture = StudioCameraManager.Instance.ActiveCamera.PreviewTexture;
        }

        public void OnStop()
        {
            StudioCameraManager.Instance.ForcePreviewRender = false;
        }

        private void DeOutlineCurrentCamera()
        {
            //  if (highlightingCamera != null)
            //      highlightingCamera.DeActivateOutline();
            //  highlightingCamera = null;
        }
    }
}
