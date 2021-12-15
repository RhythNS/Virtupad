using SimpleFileBrowser;
using UnityEngine;
using VRM;
using static LastLoadedVRMs;

public class UIVRMSelector : UIPanel
{
    public static UIVRMSelector Instance { get; private set; }

    [SerializeField] private int maxVRMElementsInPanel = 16;
    [SerializeField] private RectTransform vrmPanel;
    [SerializeField] private LastLoadedVRMs lastLoadedVRMs;

    public Sprite NoTextureFound => noTextureFound;
    [SerializeField] private Sprite noTextureFound;

    public Sprite[] LoadingSpriteAnimation => loadingSpriteAnimation;
    [SerializeField] private Sprite[] loadingSpriteAnimation;

    public float LoadingAnimationSecondsPerFrame => loadingAnimationSecondsPerFrame;
    [SerializeField] private float loadingAnimationSecondsPerFrame = 0.2f;

    [SerializeField] private UILoadVRMElement prefabElement;
    [SerializeField] private GameObject noCustomModelsFound;

    private int atPage = 0;

    private void Awake()
    {
        if (Instance)
        {
            Debug.LogWarning("UIVRMSelector already in scene. Deleting myself!");
            Destroy(this);
            return;
        }
        Instance = this;
    }

    private void OnEnable()
    {
        lastLoadedVRMs.Init(SaveFileManager.Instance.saveGame.vrms);
        SetCurrentLastLoadedVRMs();
    }


    public void LoadVRMFromFilePath(LoadVRM vrm)
    {
        Debug.Log("Now loading model: " + vrm.filepath);
    }

    public void LoadVRMFromPrefab(VRMHumanoidDescription prefab)
    {
        Debug.Log("Now loading overide model: " + prefab.name);
    }

    public void SetNextPage()
    {
        if (atPage <= 0)
            return;

        atPage--;
        SetCurrentLastLoadedVRMs();
    }

    public void SetPrevPage()
    {
        if (maxVRMElementsInPanel * atPage >= lastLoadedVRMs.LastLoaded.Count)
            return;

        atPage++;
        SetCurrentLastLoadedVRMs();
    }

    private void SetCurrentLastLoadedVRMs()
    {
        int from = maxVRMElementsInPanel * atPage;
        int to = Mathf.Min(maxVRMElementsInPanel * (atPage + 1), lastLoadedVRMs.LastLoaded.Count);

        StartCoroutine(lastLoadedVRMs.RequestPreviewImages(from, to));

        int count = to - from;
        CreateOrDeletePrefabs(count);

        if (count == 0)
        {
            noCustomModelsFound.SetActive(true);
            return;
        }

        noCustomModelsFound.SetActive(false);

        int elementIndex = -1;
        for (int i = from; i < to; i++)
        {
            vrmPanel.GetChild(++elementIndex).GetComponent<UILoadVRMElement>().LoadVRM = lastLoadedVRMs.LastLoaded[i];
        }
    }

    private void CreateOrDeletePrefabs(int count)
    {
        if (count == vrmPanel.childCount)
            return;

        if (count < vrmPanel.childCount)
        {
            while (count < vrmPanel.childCount)
            {
                Instantiate(prefabElement, vrmPanel);
            }
            return;
        }

        while (count > vrmPanel.childCount)
        {
            Destroy(vrmPanel.GetChild(count - 1));
        }
    }

    public void ShowFileDialog()
    {
        FileBrowser.SetFilters(true, new FileBrowser.Filter("VRM model files", ".vrm"));
        FileBrowser.ShowLoadDialog(OnFileSuccess, OnFileCancel, FileBrowser.PickMode.Files);
        (parent as UIElementSwitcher).SwitchChild((int)MidPanelSwitcherIndexes.FileBrowser);
        gameObject.SetActive(false);
    }

    private void OnFileSuccess(string[] paths)
    {
        gameObject.SetActive(true);
        (parent as UIElementSwitcher).SwitchChild((int)MidPanelSwitcherIndexes.UIVRMSelector);
    }

    private void OnFileCancel()
    {
        gameObject.SetActive(true);
        (parent as UIElementSwitcher).SwitchChild((int)MidPanelSwitcherIndexes.UIVRMSelector);
    }

    private void OnDisable()
    {
        lastLoadedVRMs.FreeAll();
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
