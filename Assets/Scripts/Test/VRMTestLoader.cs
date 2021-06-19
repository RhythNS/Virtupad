using System.Collections;
using System.IO;
using UniGLTF;
using UnityEngine;
using VRM;

public class VRMTestLoader : MonoBehaviour
{
    [SerializeField] private string path = "V:\\VRM Models\\vroid\\Vivi.vrm";
    [SerializeField] private GameObject currentLoadedModel = default;
    [SerializeField] private GameObject lookAtObject = default;
    [SerializeField] private RuntimeAnimatorController animatorController;
    [SerializeField] private VRMMetaObject meta = default;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.2f);
        LoadModel(path);
    }

    async void LoadModel(string path)
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
                    SetModel(context.Root);
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
                    SetModel(context.Root);
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
                    SetModel(context.Root);
                    break;
                }

            default:
                Debug.LogWarningFormat("unknown file type: {0}", path);
                break;
        }
    }

    public void SetModel(GameObject go)
    {
        // cleanup
        if (currentLoadedModel != null)
        {
            Debug.LogFormat("destroy {0}", currentLoadedModel.name);
            GameObject.Destroy(currentLoadedModel);
            currentLoadedModel = null;
        }

        if (go == null)
            return;
        
        VRMLookAtHead lookAt = go.GetComponent<VRMLookAtHead>();
        if (lookAt != null)
        {
            go.AddComponent<Blinker>();

            lookAt.Target = lookAtObject.transform;
            lookAt.UpdateType = UpdateType.LateUpdate; // after HumanPoseTransfer's setPose
        }

        go.GetComponent<VRMFirstPerson>().Setup();

        go.GetComponent<Animator>().runtimeAnimatorController = animatorController;
        go.AddComponent<VRAnimatorController>();
        go.AddComponent<FullRigCreator>();
    }

}
