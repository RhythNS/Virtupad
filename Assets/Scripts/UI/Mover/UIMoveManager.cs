using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Valve.VR;

public class UIMoveManager : MonoBehaviour
{
    public static UIMoveManager Instance { get; private set; }

    public UIElement LowestPrevSelected { get; private set; }
    public UIElement SelectedElement { get; private set; }
    public UIMover UIMover { get; private set; }

    [SerializeField] private SteamVR_Action_Vector2 uiMoveInput;
    [SerializeField] private SteamVR_Action_Boolean uiSelectInput;

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
        UIMover = GetComponent<UIMover>();
        if (UIMover == null)
            return;

        UIMover.SubscribeToEvents(uiMoveInput, uiSelectInput);
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

    public void OnElementSelected(UIElement element, UIElement mostLowestSelected)
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
    }
}
