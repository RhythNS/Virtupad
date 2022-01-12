using System;
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

                if (value == null && prevCameras.Count > 0)
                {
                    value = prevCameras[0];
                    prevCameras.RemoveAt(0);
                }

                if (activeCamera != null)
                {
                    activeCamera.OnDeActive();
                    int indexOf = prevCameras.IndexOf(activeCamera);
                    if (indexOf != -1)
                        prevCameras.RemoveAt(indexOf);
                    prevCameras.Insert(0, activeCamera);
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

        private IEnumerator Start()
        {
            ResolutionChange.Instance.OnResolutionChanged += OnResolutionChanged;
            SetSceneLoader.Instance.OnSceneAboutToChange += OnSceneAboutToChange;
            SetSceneLoader.Instance.OnSceneChanged += OnSceneChanged;
            StartCoroutine(RenderCameras());
            yield return new WaitForEndOfFrame();
            OnSceneChanged();
        }

        private void OnSceneChanged()
        {
            SaveFileManager instance = SaveFileManager.Instance;
            if (instance == null)
                return;

            string sceneName = GlobalsDict.Instance.CurrentDefinition.SceneName;
            int index = instance.saveGame.camerasOnScenes.FindIndex(x => x.sceneName == sceneName);

            if (index == -1)
                return;

            SaveGame.CamerasOnScene camerasOnScene = instance.saveGame.camerasOnScenes[index];
            camerasOnScene.definitions.Sort((x, y) => x.id.CompareTo(y.id));

            foreach (StudioCamera.Definition definition in camerasOnScene.definitions)
            {
                Vector3 pos = new Vector3(definition.position[0], definition.position[1], definition.position[2]);
                Quaternion rot = Quaternion.Euler(definition.rotation[0], definition.rotation[1], definition.rotation[2]);

                StudioCamera camera = Instantiate(
                    Array.Find(prefabCameras, x => x.cameraType == definition.prefabType).prefab, pos, rot);
                camera.FromDefinition(definition);
            }
        }

        private void OnApplicationQuit()
        {
            SaveCameras();
        }

        private void OnSceneAboutToChange()
        {
            SaveCameras();

            // TODO: Maybe delete cameras and clean everything up
        }

        private void SaveCameras()
        {
            string sceneName = GlobalsDict.Instance.CurrentDefinition.SceneName;

            List<StudioCamera.Definition> definitions = new List<StudioCamera.Definition>();
            for (int i = 0; i < cameras.Count; i++)
                definitions.Add(cameras[i].ToDefinition());

            SaveFileManager instance = SaveFileManager.Instance;
            if (instance == null)
                return;

            SaveGame.CamerasOnScene toSave = new SaveGame.CamerasOnScene(sceneName, definitions);

            List<SaveGame.CamerasOnScene> camerasOnScenes = instance.saveGame.camerasOnScenes;
            int index = camerasOnScenes.FindIndex(x => x.sceneName == sceneName);
            if (index == -1)
                camerasOnScenes.Add(toSave);
            else
                camerasOnScenes[index] = toSave;

            instance.Save();
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
            CreateCamera(prefabCameras[defaultSpawningCamera].prefab, position, rotation);
            UIRoot.Instance.CloseRequest();
        }

        public StudioCamera ReplaceCamera(StudioCamera.CameraType newType, StudioCamera toReplace)
        {
            StudioCameraPrefabType cameraPrefab = Array.Find(PrefabCameras, x => x.cameraType == newType);
            StudioCamera newCamera = CreateCamera(cameraPrefab.prefab, toReplace.Body.position, toReplace.Body.rotation);

            bool wasActiveCamera = ActiveCamera == toReplace;
            newCamera.CopyValues(toReplace);

            Destroy(toReplace.gameObject);

            if (wasActiveCamera)
                ActiveCamera = newCamera;

            return newCamera;
        }

        private StudioCamera CreateCamera(StudioCamera prefab, Vector3 position, Quaternion rotation)
        {
            StudioCamera studioCamera = Instantiate(prefab, position, rotation);

            return studioCamera;
        }

        public Vector2 GetPreviewResolution => DesiredResolution;

        public void Register(StudioCamera studioCamera)
        {
            if (studioCamera.Id == -1)
                studioCamera.Id = cameras.Count == 0 ? 0 : cameras[cameras.Count - 1].Id + 1;

            cameras.Add(studioCamera);
            SortCameras();

            if (ActiveCamera == null)
                ActiveCamera = studioCamera;

            OnCamerasChanged?.Invoke(cameras);
        }

        public void DeRegister(StudioCamera studioCamera)
        {
            if (cameras.Remove(studioCamera) == false)
                return;

            if (studioCamera.Id != -1)
                for (int i = studioCamera.Id; i < cameras.Count; i++)
                    cameras[i].Id = i;

            if (ActiveCamera == studioCamera)
                ActiveCamera = null;

            if (prevCameras.Contains(studioCamera))
                prevCameras.Remove(studioCamera);

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
            if (SetSceneLoader.Instance)
                SetSceneLoader.Instance.OnSceneAboutToChange -= OnSceneAboutToChange;
        }
    }
}
