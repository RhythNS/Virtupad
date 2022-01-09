using UnityEngine;

namespace Virtupad
{
    public class LogOnInit : MonoBehaviour
    {
        [SerializeField] private string message;

        private void Awake()
        {
            GetComponent<UIPrimitiveElement>().OnInitEvent += OnInit;
        }

        private void OnInit()
        {
            Debug.Log(gameObject.name + " was inited: " + message);
        }
    }
}
