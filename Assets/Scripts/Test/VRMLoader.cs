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
        [SerializeField] private int selectedPath;

        [SerializeField] private GameObject currentLoadedModel = default;
        [SerializeField] private GameObject lookAtObject = default;
        [SerializeField] private RuntimeAnimatorController animatorController;
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


        void Start() => StartCoroutine(InnerSpawnModel(paths[selectedPath], true, Vector3.zero, Quaternion.identity));

        public void SpawnModel(int modelIndex, Vector3 position, Quaternion rotation)
            => StartCoroutine(InnerSpawnModel(paths[modelIndex], false, position, rotation));

        public void SpawnModel(string path, Vector3 position, Quaternion rotation)
            => StartCoroutine(InnerSpawnModel(path, false, position, rotation));

        private IEnumerator InnerSpawnModel(string path, bool isMain, Vector3 position, Quaternion rotation)
        {
            Task task = LoadModel(path, isMain, position, rotation);
            yield return new WaitUntil(() => task.IsCompleted);
        }

        private async Task LoadModel(string path, bool isMain, Vector3 position, Quaternion rotation)
        {
            Debug.LogFormat("{0}", path);
            var ext = Path.GetExtension(path).ToLower();
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
                        break;
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
                        break;
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
                        break;
                    }

                default:
                    Debug.LogWarningFormat("unknown file type: {0}", path);
                    break;
            }
        }

        private void SetModel(GameObject go, bool isMain, Vector3 position, Quaternion rotation)
        {
            go.transform.SetPositionAndRotation(position, rotation);

            if (isMain == false)
                return;

            // cleanup
            if (currentLoadedModel != null)
            {
                Debug.LogFormat("destroy {0}", currentLoadedModel.name);
                GameObject.Destroy(currentLoadedModel);
                currentLoadedModel = null;
            }

            if (go == null)
                return;

            go.AddComponent<VRMController>();
        }

    }
}
