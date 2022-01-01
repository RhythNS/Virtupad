using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Virtupad
{
    public class StudioCameraManager : MonoBehaviour
    {
        public static StudioCameraManager Instance { get; private set; }

        public List<StudioCamera> Cameras { get => cameras; set => cameras = value; }
        private List<StudioCamera> cameras = new List<StudioCamera>();

        public StudioCameraChanged OnStudioCameraChanged;

        private StudioCamera activeCamera;
        public StudioCamera ActiveCamera
        {
            get => activeCamera;
            set
            {
                if (activeCamera == value)
                    return;

                if (activeCamera != null)
                {
                    activeCamera.OnDeActive();
                    int indexOf = prevCameras.IndexOf(activeCamera);
                    if (indexOf != -1)
                        prevCameras.RemoveAt(indexOf);
                    prevCameras.Insert(0, activeCamera);
                }

                if (value == null && prevCameras.Count > 0)
                {
                    value = prevCameras[0];
                    prevCameras.RemoveAt(0);
                }

                if (value != null)
                    value.OnActive();

                activeCamera = value;

                OnStudioCameraChanged?.Invoke(value);
            }
        }
        private List<StudioCamera> prevCameras = new List<StudioCamera>();

        public StudioCamera PrefabCamera => prefabCamera;
        [SerializeField] private StudioCamera prefabCamera;

        [SerializeField] private float previewResolutionMultiplier = 0.5f;

        public Vector2 DesiredResolution => new Vector2((float)Screen.width * previewResolutionMultiplier,
            (float)Screen.height * previewResolutionMultiplier);

        private ExtendedCoroutine resolutionChange;

        private void Awake()
        {
            if (Instance)
            {
                Debug.LogWarning("StudioCameraManager already in scene. Deleting myself!");
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            ResolutionChange.Instance.OnResolutionChanged += OnResolutionChanged;
        }

        private void OnResolutionChanged(Vector2Int newResolution)
        {
            if (resolutionChange != null && resolutionChange.IsFinshed == false)
                resolutionChange.Stop(false);

            resolutionChange = new ExtendedCoroutine(this, ChangePreviewResolutions(newResolution));
        }

        private IEnumerator ChangePreviewResolutions(Vector2Int _)
        {
            Vector2 resolution = DesiredResolution;

            for (int i = 0; i < cameras.Count; i++)
            {
                cameras[i].ChangePreviewResolution(resolution);
                yield return null;
            }
        }

        public Vector2 RegisterAndGetPreviewResolution(StudioCamera studioCamera)
        {
            cameras.Add(studioCamera);
            return DesiredResolution;
        }

        public void DeRegister(StudioCamera studioCamera)
        {
            cameras.Remove(studioCamera);

            if (prevCameras.Contains(studioCamera))
                prevCameras.Remove(studioCamera);
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
            if (ResolutionChange.Instance)
                ResolutionChange.Instance.OnResolutionChanged -= OnResolutionChanged;
        }
    }
}
