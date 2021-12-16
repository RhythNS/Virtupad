using UnityEngine;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class UIInteractable : Interactable
{
    [SerializeField] private bool useAutoSize = true;
    [SerializeField] protected UIPrimitiveElement element;

    public virtual void Awake()
    {
        SnapToObject = false;

        if (element == null)
            element = GetComponent<UIPrimitiveElement>();

        element.OnInitEvent += OnInit;
    }

    private void OnInit()
    {
        AutoSize();
    }

    protected override void OnBeginHover()
    {
        element.Select();
        // UIEventThrower.GameobjectUIEvent(gameObject, ExecuteEvents.pointerEnterHandler);
    }

    protected override void OnLeaveHover()
    {
        element.DeSelect();
        // UIEventThrower.GameobjectUIEvent(gameObject, ExecuteEvents.pointerExitHandler);
    }

    public override void Select()
    {
        element.OnEvent(ExecuteEvents.pointerClickHandler);
        // UIEventThrower.GameobjectUIEvent(gameObject, ExecuteEvents.pointerClickHandler);
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

    private void OnDestroy()
    {
        if (element)
            element.OnInitEvent -= OnInit;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(UIInteractable), true)]
public class VRUIColliderCreatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        (target as UIInteractable).AutoSize();
    }
}
#endif
