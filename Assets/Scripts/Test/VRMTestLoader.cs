using System.IO;
using UniGLTF;
using UniHumanoid;
using UnityEngine;
using VRM;
using VRM.Samples;

public class VRMTestLoader : MonoBehaviour
{
    private HumanPoseTransfer loadedPose;
    private AIUEO lipSync;
    private Blinker blink;
    private VRMBlendShapeProxy m_proxy;

    [SerializeField] private string path = "V:\\VRM Models\\vroid\\Vivi.vrm";

    [SerializeField] private HumanPoseTransfer humanPoseSource = default;

    [SerializeField] private GameObject lookAtObject = default;

    [SerializeField] VRMMetaObject meta = default;

    private void Start()
    {
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
        var loaded = loadedPose;
        loadedPose = null;

        if (loaded != null)
        {
            Debug.LogFormat("destroy {0}", loaded);
            GameObject.Destroy(loaded.gameObject);
        }

        if (go != null)
        {
            var lookAt = go.GetComponent<VRMLookAtHead>();
            if (lookAt != null)
            {
                loadedPose = go.AddComponent<HumanPoseTransfer>();
                loadedPose.Source = humanPoseSource;
                loadedPose.SourceType = HumanPoseTransfer.HumanPoseTransferSourceType.HumanPoseTransfer;

                lipSync = go.AddComponent<AIUEO>();
                blink = go.AddComponent<Blinker>();

                lookAt.Target = lookAtObject.transform;
                lookAt.UpdateType = UpdateType.LateUpdate; // after HumanPoseTransfer's setPose
            }

            var animation = go.GetComponent<Animation>();
            if (animation && animation.clip != null)
            {
                animation.Play(animation.clip.name);
            }

            m_proxy = go.GetComponent<VRMBlendShapeProxy>();
        }
    }

}
