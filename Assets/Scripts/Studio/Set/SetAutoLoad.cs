using System.Collections;
using UnityEngine;

namespace Virtupad
{
    public class SetAutoLoad : MonoBehaviour
    {
        [SerializeField] private string sceneToLoad;

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(0.1f);
            if (GlobalsDict.Instance.CurrentDefinition == null)
                SetSceneLoader.Instance.LoadScene(sceneToLoad);

            Destroy(gameObject);
        }
    }
}
