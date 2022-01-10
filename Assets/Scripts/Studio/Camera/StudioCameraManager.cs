using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

namespace Virtupad
{
    public class StudioCameraManager : MonoBehaviour
    {
        public static StudioCameraManager Instance { get; private set; }

        public List<StudioCamera> Cameras { get => cameras; set => cameras = value; }
        private List<StudioCamera> cameras = new List<StudioCamera>();

        public event StudioCamerasChanged OnCamerasChanged;
        public event StudioCameraChanged OnActiveStudioCameraChanged;

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

                OnActiveStudioCameraChanged?.Invoke(value);
            }
        }
        private List<StudioCamera> prevCameras = new List<StudioCamera>();

        [System.Serializable]
        public struct StudioCameraPrefabType
        {
            public StudioCamera prefab;
            public StudioCamera.CameraType cameraType;
        }

        public StudioCameraPrefabType[] PrefabCameras => prefabCameras;
        [SerializeField] private StudioCameraPrefabType[] prefabCameras;
        [SerializeField] private int defaultSpawningCamera = 0;

        public CameraMover.Type DefaultMover => defaultMover;
        [SerializeField] public CameraMover.Type defaultMover;

        [SerializeField] private float previewResolutionMultiplier = 0.5f;

        public float MovingMetersPerSecond => movingMetersPerSecond;
        [SerializeField] private float movingMetersPerSecond;

        public float RotatingAnglesPerSecond => rotatingAnglesPerSecond;
        [SerializeField] private float rotatingAnglesPerSecond;

        public bool ForcePreviewRender
        {
            get => currentForcePreviewRenderRequests > 0;
            set
            {
                if (value)
                    ++currentForcePreviewRenderRequests;
                else
                    --currentForcePreviewRenderRequests;
            }
        }
        private int currentForcePreviewRenderRequests = 0;

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
            StartCoroutine(RenderCameras());
        }

        private IEnumerator RenderCameras()
        {
            while (true)
            {
                for (int i = 0; i < cameras.Count; i++)
                {
                    if (cameras[i].IsPreviewOutputting == true || ForcePreviewRender == true)
                    {
                        cameras[i].PreviewCamera.Render();
                        yield return null;
                    }
                }

                yield return null;
            }
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

        public void ChangeActiveCamera(int id)
        {
            for (int i = 0; i < cameras.Count; i++)
            {
                if (cameras[i].Id == id)
                {
                    ActiveCamera = cameras[i];
                    return;
                }
            }
        }

        public void CreateNewCamera()
        {
            Transform hmdTransform = Player.instance.hmdTransform;
            Vector3 position = hmdTransform.position + hmdTransform.forward * 2.0f;
            Quaternion rotation = Quaternion.LookRotation((hmdTransform.position - position).normalized, Vector3.up);
            StudioCamera studioCamera = Instantiate(prefabCameras[defaultSpawningCamera].prefab, position, rotation);
            UIRoot.Instance.CloseRequest();
        }

        public Vector2 GetPreviewResolution => DesiredResolution;

        public void Register(StudioCamera studioCamera)
        {
            studioCamera.Id = cameras.Count == 0 ? 0 : cameras[cameras.Count - 1].Id + 1;
            cameras.Add(studioCamera);

            if (ActiveCamera == null)
                ActiveCamera = studioCamera;

            OnCamerasChanged?.Invoke(cameras);
        }

        public void DeRegister(StudioCamera studioCamera)
        {
            cameras.Remove(studioCamera);
            for (int i = studioCamera.Id; i < cameras.Count; i++)
                cameras[i].Id = i;

            if (prevCameras.Contains(studioCamera))
                prevCameras.Remove(studioCamera);

            if (ActiveCamera == studioCamera)
                ActiveCamera = null;

            OnCamerasChanged?.Invoke(cameras);
        }

        private void SortCameras()
        {
            cameras.Sort((StudioCamera a, StudioCamera b) => { return a == b ? 0 : a.Id < b.Id ? -1 : 1; });
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
