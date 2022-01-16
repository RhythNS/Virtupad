using UnityEngine;

namespace Virtupad
{
    public class Testtracking : MonoBehaviour
    {
        [SerializeField] private Transform source;
        [SerializeField] private Transform constrain;

        private VRMapper mapper;

        private void Start()
        {
            mapper = gameObject.AddComponent<VRMapper>();
            mapper.AddMapTracker(constrain, source);
        }
    }
}
