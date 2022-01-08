using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using UniGLTF;
using UnityEngine;
using VRM;

namespace Virtupad
{
    public class VRMLoader : MonoBehaviour
    {
        [SerializeField] private string[] paths;
        
        public VRMMetaObject Meta => meta;
        [SerializeField] private VRMMetaObject meta = default;

        public static VRMLoader Instance { get; private set; }

        private void Awake()
        {
            if (Instance)
            {
                Debug.LogWarning("VRMLoader already in scene. Deleting myself!");
                Destroy(this);
                return;
            }
            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        public void DebugSpawnModel(int modelIndex, Vector3 position, Quaternion rotation, Action onSuccess = null, Action<string> onFailure = null)
            => StartCoroutine(InnerSpawnModel(paths[modelIndex], true, position, rotation, onSuccess, onFailure));

        public void SpawnModel(string path, Vector3 position, Quaternion rotation, Action onSuccess, Action<string> onFailure)
            => StartCoroutine(InnerSpawnModel(path, true, position, rotation, onSuccess, onFailure));

        private IEnumerator InnerSpawnModel(string path, bool isMain, Vector3 position, Quaternion rotation, Action onSuccess, Action<string> onFailure)
        {
            Task<Tuple<bool, string>> task = LoadModel(path, isMain, position, rotation);
            yield return new WaitUntil(() => task.IsCompleted);

            if (task.Result.Item1 == true)
                onSuccess?.Invoke();
            else
                onFailure?.Invoke(task.Result.Item2);
        }

        private async Task<Tuple<bool, string>> LoadModel(string path, bool isMain, Vector3 position, Quaternion rotation)
        {
            var ext = Path.GetExtension(path).ToLower();
            try
            {
                switch (ext)
                {
                    case ".vrm":
                        {
                            byte[] file = File.ReadAllBytes(path);

                            GltfParser parser = new GltfParser();
                            parser.ParseGlb(file);

                            using VRMImporterContext context = new VRMImporterContext(parser);
                            meta = await context.ReadMetaAsync(default(TaskCaller), true);
                            await context.LoadAsync();
                            context.EnableUpdateWhenOffscreen();
                            context.ShowMeshes();
                            context.DisposeOnGameObjectDestroyed();
                            SetModel(context.Root, isMain, position, rotation);
                            return new Tuple<bool, string>(true, default);
                        }

                    case ".glb":
                        {
                            byte[] file = File.ReadAllBytes(path);

                            GltfParser parser = new GltfParser();
                            parser.ParseGlb(file);

                            UniGLTF.ImporterContext context = new UniGLTF.ImporterContext(parser);
                            context.Load();
                            context.EnableUpdateWhenOffscreen();
                            context.ShowMeshes();
                            context.DisposeOnGameObjectDestroyed();
                            SetModel(context.Root, isMain, position, rotation);
                            return new Tuple<bool, string>(true, default);
                        }

                    case ".gltf":
                    case ".zip":
                        {
                            GltfParser parser = new GltfParser();
                            parser.ParsePath(path);

                            UniGLTF.ImporterContext context = new UniGLTF.ImporterContext(parser);
                            context.Load();
                            context.EnableUpdateWhenOffscreen();
                            context.ShowMeshes();
                            context.DisposeOnGameObjectDestroyed();
                            SetModel(context.Root, isMain, position, rotation);
                            return new Tuple<bool, string>(true, default);
                        }

                    default:
                        string errorMessage = string.Format("Unknown file type: {0}", path);
                        Debug.LogWarning(errorMessage);
                        return new Tuple<bool, string>(false, errorMessage);
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning(string.Join(Environment.NewLine, e.Message, e.StackTrace));
                return new Tuple<bool, string>(false, "Error loading file." + Environment.NewLine + e.Message);
            }
        }

        public void LoadPrefab(VRMHumanoidDescription prefab, Vector3 position, Quaternion rotation)
        {
            VRMHumanoidDescription inst = Instantiate(prefab);
            SetModel(inst.gameObject, true, position, rotation);
        }

        private void SetModel(GameObject go, bool isMain, Vector3 position, Quaternion rotation)
        {
            go.transform.SetPositionAndRotation(position, rotation);

            if (isMain == false)
                return;

            DontDestroyOnLoad(go);

            go.AddComponent<VRMController>();
        }
    }
}
