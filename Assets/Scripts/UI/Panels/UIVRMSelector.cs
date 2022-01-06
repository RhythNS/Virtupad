using SimpleFileBrowser;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRM;
using static Virtupad.LastLoadedVRMs;

namespace Virtupad
{
    public class UIVRMSelector : UIPanel
    {
        public static UIVRMSelector Instance { get; private set; }

        public UIElementSwitcher Switcher => switcher;
        [SerializeField] private UIElementSwitcher switcher;

        [SerializeField] private int maxVRMElementsInPanel = 16;
        [SerializeField] private RectTransform vrmPanel;

        public LastLoadedVRMs LastLoadedVRMs => lastLoadedVRMs;
        [SerializeField] private LastLoadedVRMs lastLoadedVRMs;

        [SerializeField] private UIPrimitiveElement lastLoadedVRMsPanel;
        [SerializeField] private GridLayoutGroup lastLoadedGroup;

        public Sprite NoTextureFound => noTextureFound;
        [SerializeField] private Sprite noTextureFound;

        public Sprite[] LoadingSpriteAnimation => loadingSpriteAnimation;
        [SerializeField] private Sprite[] loadingSpriteAnimation;

        public float LoadingAnimationSecondsPerFrame => loadingAnimationSecondsPerFrame;
        [SerializeField] private float loadingAnimationSecondsPerFrame = 0.2f;

        [SerializeField] private UILoadVRMElement prefabElement;
        [SerializeField] private GameObject noCustomModelsFound;

        [SerializeField] private FileBrowser fileBrowser;

        private int atPage = 0;

        protected override void Awake()
        {
            if (Instance)
            {
                Debug.LogWarning("UIVRMSelector already in scene. Deleting myself!");
                Destroy(this);
                return;
            }
            Instance = this;

            base.Awake();

            FileBrowser.OverideInstance(fileBrowser);
        }

        private void OnEnable()
        {
            List<SaveGame.LoadVRM> vrms = SaveFileManager.Instance.saveGame.vrms;
            if (vrms == null)
                vrms = new List<SaveGame.LoadVRM>();
            lastLoadedVRMs.Init(vrms);

            SetCurrentLastLoadedVRMs();
        }

        public void LoadVRMFromFilePath(LoadVRM vrm)
        {
            Switcher.SwitchChild((int)VRMSelectorSwitcherIndexes.LoadingInProgress);
            UILoadVRMInProgressPanel.Instance.TryToLoad(vrm.filepath);
        }

        public void LoadVRMFromPrefab(VRMHumanoidDescription prefab)
        {
            Switcher.SwitchChild((int)VRMSelectorSwitcherIndexes.LoadingInProgress);
            UILoadVRMInProgressPanel.Instance.TryToLoad(prefab);
        }

        public override void OnInit()
        {
            base.OnInit();
            InitPositionOfLastLoaded();

            for (int i = 0; i < lastLoadedGroup.transform.childCount; i++)
            {
                UILoadVRMElement loadElement = lastLoadedGroup.transform.GetChild(i).GetComponent<UILoadVRMElement>();
                loadElement.OnInit();
            }
        }

        public void Refresh()
        {
            atPage = 0;
            SetCurrentLastLoadedVRMs();
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
                UILoadVRMElement loadElement = vrmPanel.GetChild(++elementIndex).GetComponent<UILoadVRMElement>();
                loadElement.LoadVRM = lastLoadedVRMs.LastLoaded[i];
            }

            InitPositionOfLastLoaded();
        }

        private void InitPositionOfLastLoaded()
        {
            lastLoadedVRMsPanel.RemoveAllChildren();

            UIUtil.GetColAndRowsOfGridLayoutGroup(lastLoadedGroup, out int colNum, out int rowNum);

            for (int i = 0; i < lastLoadedGroup.transform.childCount; i++)
            {
                UILoadVRMElement loadElement = lastLoadedGroup.transform.GetChild(i).GetComponent<UILoadVRMElement>();

                loadElement.uiPos = new Vector2Int(i % colNum, rowNum - (i / colNum) - 1);
                lastLoadedVRMsPanel.AddChild(loadElement);
            }
        }

        private void CreateOrDeletePrefabs(int count)
        {
            if (count == vrmPanel.childCount)
                return;

            if (count > vrmPanel.childCount)
            {
                while (count > vrmPanel.childCount)
                {
                    Instantiate(prefabElement, vrmPanel);
                }
                return;
            }

            while (count < vrmPanel.childCount)
            {
                Transform trans = vrmPanel.GetChild(vrmPanel.childCount - 1);
                UILoadVRMElement loadElement = trans.GetComponent<UILoadVRMElement>();
                loadElement.parent.RemoveChild(loadElement);
                trans.SetParent(null, false);
                Destroy(trans.gameObject);
            }
        }

        public void ShowFileDialog()
        {
            Switcher.SwitchChild((int)VRMSelectorSwitcherIndexes.FileBrowser);
            FileBrowser.SetFilters(true, new FileBrowser.Filter("VRM model files", ".vrm"));
            FileBrowser.ShowLoadDialog(OnFileSuccess, OnFileCancel, FileBrowser.PickMode.Files);
        }

        private void OnFileSuccess(string[] paths)
        {
            if (paths == null || paths.Length == 0 || string.IsNullOrEmpty(paths[0]))
            {
                OnFileCancel();
                return;
            }

            Switcher.SwitchChild((int)VRMSelectorSwitcherIndexes.LoadingInProgress);
            UILoadVRMInProgressPanel.Instance.TryToLoad(paths[0]);
        }

        public IEnumerator DoLoadingAnimation(Image image)
        {
            int at = 0;

            while (true)
            {
                if (++at >= loadingSpriteAnimation.Length)
                    at = 0;

                image.sprite = loadingSpriteAnimation[at];
                yield return new WaitForSeconds(loadingAnimationSecondsPerFrame);
            }
        }

        public void OnClearHistory()
        {
            lastLoadedVRMs.DeleteAllFromHistory();
            Refresh();
        }

        private void OnFileCancel()
        {
            gameObject.SetActive(true);
            Switcher.SwitchChild((int)VRMSelectorSwitcherIndexes.LastLoaded);
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
}
