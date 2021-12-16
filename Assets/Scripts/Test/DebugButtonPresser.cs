using UnityEngine;
using UnityEngine.EventSystems;

namespace Virtupad
{
    public class DebugButtonPresser : MonoBehaviour
    {
        [SerializeField] private bool fireOnElementInit = false;
        [SerializeField] private KeyCode key = KeyCode.K;

        private UIPrimitiveElement element;

        void Start()
        {
            Debug.Log("There is a debug presser on " + gameObject.name + " with key: " + key);
            
            element = GetComponent<UIPrimitiveElement>();
            if (element == null || fireOnElementInit == false)
                return;

            element.OnInitEvent += OnInit;
        }

        private void OnInit()
        {
            FireEvent();
        }

        private void OnDestroy()
        {
            element.OnInitEvent -= OnInit;
        }

        void Update()
        {
            if (Input.GetKeyDown(key))
            {
                FireEvent();
            }
        }

        private void FireEvent()
        {
            UIEventThrower.GameobjectUIEvent(gameObject, ExecuteEvents.pointerClickHandler);
        }
    }
}
