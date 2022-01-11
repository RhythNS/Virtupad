using System.Collections;
using UnityEngine;
using VRM;

namespace Virtupad
{
    public class LayLoadVRMTest : MonoBehaviour
    {
        [SerializeField] private VRMHumanoidDescription prefab;

        IEnumerator Start()
        {
            yield return new WaitForSeconds(0.1f);
            VRMLoader.Instance.LoadPrefab(prefab, Vector3.zero, Quaternion.identity);
        }
    }
}
