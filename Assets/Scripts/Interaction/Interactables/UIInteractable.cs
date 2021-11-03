using UnityEngine;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class UIInteractable : Interactable
{
    [SerializeField] private bool useAutoSize = true;

    public virtual void Awake()
    {
        SnapToObject = false;
    }

    protected override void OnBeginHover()
    {
        PointerEventData ped = new PointerEventData(EventSystem.current);
        ExecuteEvents.Execute(gameObject, ped, ExecuteEvents.pointerEnterHandler);
    }

    protected override void OnLeaveHover()
    {
        PointerEventData ped = new PointerEventData(EventSystem.current);
        ExecuteEvents.Execute(gameObject, ped, ExecuteEvents.pointerExitHandler);
    }

    public override void Select()
    {
        PointerEventData ped = new PointerEventData(EventSystem.current);
        ExecuteEvents.Execute(gameObject, ped, ExecuteEvents.pointerClickHandler);
    }

    public void AutoSize()
    {
        if (useAutoSize == false)
            return;

        if (TryGetComponent(out RectTransform trans) == false)
            return;

        BoxCollider collider = GetComponent<BoxCollider>();
        if (collider == null)
            collider = gameObject.AddComponent<BoxCollider>();

        Rect rect = trans.rect;
        collider.size = rect.size;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(UIInteractable))]
public class VRUIColliderCreatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        (target as UIInteractable).AutoSize();
    }
}
#endif
