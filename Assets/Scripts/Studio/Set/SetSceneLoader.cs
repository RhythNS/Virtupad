using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Virtupad
{
    public class SetSceneLoader : MonoBehaviour
    {
        public static SetSceneLoader Instance { get; private set; }

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

        public void LoadScene(string sceneName) => StartCoroutine(AsyncLoadUnLoad(sceneName));

        private IEnumerator AsyncLoadUnLoad(string sceneName)
        {
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
