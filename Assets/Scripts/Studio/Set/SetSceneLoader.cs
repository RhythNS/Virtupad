using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Virtupad
{
    public class SetSceneLoader : MonoBehaviour
    {
        public static SetSceneLoader Instance { get; private set; }

        [System.Serializable]
        public struct SetDescription
        {
            public string title;
            public string description;
            public Sprite previewSprite;

            public string sceneName;
        }

        public SetDescription[] Sets => sets;
        [SerializeField] SetDescription[] sets;

        public event VoidEvent OnSceneAboutToChange;
        public event VoidEvent OnSceneChanged;

        private void Awake()
        {
            if (Instance)
            {
                Debug.LogWarning("SetSceneLoader already in scene. Deleting myself!");
                Destroy(this);
                return;
            }
            Instance = this;
        }

        private int numberOfOperationsNotDone = 0;

        public void LoadScene(int index) => StartCoroutine(AsyncLoadUnLoad(sets[index].sceneName));

        public void LoadScene(string sceneName) => StartCoroutine(AsyncLoadUnLoad(sceneName));

        private IEnumerator AsyncLoadUnLoad(string sceneName)
        {
            OnSceneAboutToChange?.Invoke();

            SetDefinition currentDefinition = GlobalsDict.Instance.CurrentDefinition;
            if (currentDefinition != null)
            {
                AsyncOperation unLoad = SceneManager.UnloadSceneAsync(currentDefinition.SceneName);
                unLoad.completed += OperationFinished;
                numberOfOperationsNotDone++;
            }

            AsyncOperation load = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            load.completed += OperationFinished;
            numberOfOperationsNotDone++;

            while (numberOfOperationsNotDone != 0)
                yield return null;

            SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
            OnSceneChanged?.Invoke();
        }

        private void OperationFinished(AsyncOperation operation)
        {
            operation.completed -= OperationFinished;
            numberOfOperationsNotDone--;
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }
    }
}
