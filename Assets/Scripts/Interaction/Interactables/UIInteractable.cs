using UnityEngine;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Virtupad
{
    public class UIInteractable : Interactable
    {
        [System.Serializable]
        [System.Flags]
        public enum UIInteractableSizer
        {
            XPlus = 1 << 0,
            XMinus = 1 << 1,
            YPlus = 1 << 2,
            YMinus = 1 << 3,
        }

        [SerializeField] private bool useAutoSize = true;
        [SerializeField] private UIInteractableSizer sizer = 0;

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

        protected override void OnBeginHover(Vector3 impactPoint)
        {
            element.Select();
        }

        protected override void OnLeaveHover()
        {
            element.DeSelect();
        }

        public override void Select()
        {
            element.OnEvent(ExecuteEvents.pointerClickHandler);
        }

        protected override void OnStayHover(Vector3 impactPoint)
        {
        }

        public override void OnBeginSelecting(Vector3 impactPoint)
        {
        }

        public override void OnStaySelecting(Vector3 impactPoint)
        {
            PointerEventData eventData = UIEventThrower.GetDefaultPed(impactPoint);
            element.OnEvent(ExecuteEvents.dragHandler, false, eventData);
        }

        public override void OnEndSelecting()
        {
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

            if (sizer == 0)
                return;

            Vector3 halfSize = rect.size * 0.5f;
            Vector3 center = Vector3.zero;
            if (sizer.HasFlag(UIInteractableSizer.XPlus))
                center.x = halfSize.x;
            else if (sizer.HasFlag(UIInteractableSizer.XMinus))
                center.x = -halfSize.x;
            if (sizer.HasFlag(UIInteractableSizer.YPlus))
                center.y = halfSize.y;
            else if (sizer.HasFlag(UIInteractableSizer.YMinus))
                center.y = -halfSize.y;

            collider.center = center;
        }

        private void OnDestroy()
        {
            if (element)
                element.OnInitEvent -= OnInit;
        }

        private void OnValidate()
        {
            if (element == null)
                Debug.LogWarning(gameObject.name + " does not have an element assigend!");
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
}