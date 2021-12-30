using UnityEngine;

namespace Virtupad
{
    public class SetDefinition : MonoBehaviour
    {
        private void Start()
        {
            GlobalsDict.Instance.CurrentDefinition = this;
        }

        public string SceneName => sceneName;
        [SerializeField] protected string sceneName;

        public Vector3 StartPoint => startPoint;
        [SerializeField] private Vector3 startPoint;

        public Quaternion StartRotation => startRotation;
        [SerializeField] private Quaternion startRotation;

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(startPoint, 0.15f);
            Gizmos.DrawLine(startPoint, startPoint + (startRotation * Vector3.forward));
        }

        private void OnDestroy()
        {
            if (GlobalsDict.Instance && GlobalsDict.Instance.CurrentDefinition == this)
                GlobalsDict.Instance.CurrentDefinition = null;
        }

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(sceneName))
                Debug.LogWarning("Scene name is empty for the set definition!");
        }
    }
}
