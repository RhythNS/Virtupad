using UnityEngine;

namespace Virtupad
{
    public class PrintLayer : MonoBehaviour
    {
        [SerializeField] private GameObject target;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.H))
                Debug.Log(target.layer);
        }
    }
}
