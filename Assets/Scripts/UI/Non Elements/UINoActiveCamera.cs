using UnityEngine;

namespace Virtupad
{
    public class UINoActiveCamera : MonoBehaviour
    {
        private void Start()
        {
            StudioCameraManager.Instance.OnActiveStudioCameraChanged += OnCameraChanged;
            OnCameraChanged(StudioCameraManager.Instance.ActiveCamera);
        }

        private void OnCameraChanged(StudioCamera newCamera)
        {
            gameObject.SetActive(newCamera == null);
        }

        private void OnDestroy()
        {
            if (StudioCameraManager.Instance)
                StudioCameraManager.Instance.OnActiveStudioCameraChanged -= OnCameraChanged;
        }
    }
}
