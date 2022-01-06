using System.Collections.Generic;
using UnityEngine;

namespace Virtupad
{
    public class StudioCameraListener : MonoBehaviour, IUIQuickSelectListener
    {
        private StudioCamera highlightingCamera;

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
         //   DeOutlineCurrentCamera();
         //   highlightingCamera = StudioCameraManager.Instance.Cameras.Find(x => x.Id == newIndex);
         //   if (highlightingCamera != null)
         //       highlightingCamera.ActivateOutline();
        }

        public void OnStart() { }

        public void OnStop()
        {
            DeOutlineCurrentCamera();
        }

        private void DeOutlineCurrentCamera()
        {
          //  if (highlightingCamera != null)
          //      highlightingCamera.DeActivateOutline();
          //  highlightingCamera = null;
        }
    }
}
