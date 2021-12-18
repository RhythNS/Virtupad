using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Valve.VR;

namespace Virtupad
{
    public class UIMoveManager : MonoBehaviour
    {
        public static UIMoveManager Instance { get; private set; }

        public UIPrimitiveElement LowestPrevSelected { get; private set; }
        public UIPrimitiveElement SelectedElement { get; private set; }

        public UIMover UIMover { get => uIMover; private set => uIMover = value; }
        [SerializeField] private UIMover uIMover;

        [SerializeField] private SteamVR_Action_Vector2 uiMoveInput;
        [SerializeField] private SteamVR_Action_Boolean uiSelectInput;
        [SerializeField] private SteamVR_Action_Boolean uiMainMenuInput;

        private void Awake()
        {
            if (Instance)
            {
                Debug.LogWarning("UIMoveManager already in scene. Deleting myself!");
                Destroy(this);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            if (UIMover == null && TryGetComponent(out uIMover) == false)
                return;

            UIMover.SubscribeToEvents(uiMoveInput, uiSelectInput);
            uiMainMenuInput.AddOnStateDownListener(OnMainMenuDown, SteamVR_Input_Sources.Any);
        }

        public void AddMover(Type type)
        {
            if (type.IsSubclassOf(typeof(UIMover)) == false)
            {
                Debug.LogWarning("Type " + type.FullName + " is not a subclass of UIMover!");
                return;
            }

            if (UIMover)
            {
                UIMover.UnSubscribeFromEvents(uiMoveInput, uiSelectInput);
                Destroy(UIMover);
            }

            UIMover = (UIMover)gameObject.AddComponent(type);
            UIMover.SubscribeToEvents(uiMoveInput, uiSelectInput);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
                OnMainMenuDown(null, SteamVR_Input_Sources.Waist);
        }

        private void OnMainMenuDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            UIPanelManager.Instance.OnMainMenuToggle();
        }

        public void OnElementSelected(UIPrimitiveElement element, UIPrimitiveElement mostLowestSelected)
        {
            LowestPrevSelected = mostLowestSelected;

            if (element == SelectedElement)
                return;

            if (SelectedElement)
                SelectedElement.FireDeselectEvent();

            SelectedElement = element;
        }

        public bool Move(Direction direction)
        {
            Debug.Log("UI wants to move " + direction);

            if (SelectedElement == null)
            {
                if (LowestPrevSelected == null)
                    return false;

                LowestPrevSelected.Select();
                return true;
            }

            return SelectedElement.Move(direction);
        }

        public void Click()
        {
            if (SelectedElement != null)
                SelectedElement.OnEvent(ExecuteEvents.pointerClickHandler);
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
            if (UIMover)
                UIMover.UnSubscribeFromEvents(uiMoveInput, uiSelectInput);

            uiMainMenuInput.RemoveAllListeners(SteamVR_Input_Sources.Any);
        }
    }
}
