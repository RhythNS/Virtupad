using UnityEngine;
using UnityEngine.EventSystems;

public class UIElement : MonoBehaviour
{
    [System.Serializable]
    [System.Flags]
    public enum EventInterception
    {
        Passthrough = 1 << 0,
        Recieve = 1 << 1
    }

    public Vector2Int uiPos;
    public UIElement parent;
    private Fast2DArray<UIElement> children;
    [SerializeField] private UIElement selected = null;

    [SerializeField, BitMask(typeof(EventInterception))] private EventInterception eventInterception = EventInterception.Recieve | EventInterception.Passthrough;

    private void Awake()
    {
        if (parent)
            parent.AddChild(this);
    }

    private void AddChild(UIElement element)
    {
        if (element.uiPos.x < 0 || element.uiPos.y < 0)
        {
            Debug.LogWarning("Element " + element.name + " had an invalid pos! Ignoring it!");
            return;
        }

        Vector2Int pos = element.uiPos;

        if (children == null)
        {
            children = new Fast2DArray<UIElement>(pos.x + 1, pos.y + 1);
            children[pos.x, pos.y] = element;
            return;
        }

        if (children.InBounds(pos) == false)
        {
            Vector2Int newSize = children.Size;
            newSize.x = Mathf.Max(newSize.x, pos.x + 1);
            newSize.y = Mathf.Max(newSize.y, pos.y + 1);
            children.Resize(newSize.x, newSize.y);
        }

        if (children[pos.x, pos.y] != null)
        {
            Debug.LogWarning("Element " + element.name + " has the same position as " + children[pos.x, pos.y].name + "!");
            return;
        }

        children[pos.x, pos.y] = element;
    }

    public bool Move(Direction direction)
    {
        if (children != null)
        {
            if (selected == null)
            {
                Debug.LogWarning("Could not move. Nothing is selected!");
                return false;
            }

            if (selected.Move(direction) == true)
                return true;

            return MoveSelf(direction);
        }

        return false;
    }

    private bool MoveSelf(Direction direction)
    {
        Vector2Int coordDirection;
        bool isHori = false;
        switch (direction)
        {
            case Direction.Up:
                coordDirection = Vector2Int.up;
                break;

            case Direction.Down:
                coordDirection = Vector2Int.down;
                break;

            case Direction.Left:
                coordDirection = Vector2Int.left;
                isHori = true;
                break;

            case Direction.Right:
                coordDirection = Vector2Int.right;
                isHori = true;
                break;

            default:
                Debug.LogWarning("Could not get direction : " + direction);
                return false;
        }

        Vector2Int currentPos = selected.uiPos;
        UIElement bestElement = null;
        while (bestElement == null)
        {
            currentPos += coordDirection;
            if (children.InBounds(currentPos) == false)
                break;

            float bestPos = float.MaxValue;

            if (isHori)
            {
                for (int y = 0; y < children.YSize; y++)
                    GetBetterElement(in currentPos, currentPos.x, y, ref bestElement, ref bestPos);
            }
            else
            {
                for (int x = 0; x < children.XSize; x++)
                    GetBetterElement(in currentPos, x, currentPos.y, ref bestElement, ref bestPos);
            }
        }

        if (bestElement == null)
            return false;

        selected.FireDeselectEvent();
        bestElement.FireSelectEvent();
        selected = bestElement;
        return true;
    }

    private void GetBetterElement(in Vector2Int currentPos, int x, int y, ref UIElement bestElement, ref float bestPos)
    {
        UIElement newElement = children[x, y];
        if (newElement == null)
            return;

        float newPos = (newElement.uiPos - currentPos).sqrMagnitude;
        if (newPos < bestPos)
        {
            bestElement = newElement;
            bestPos = newPos;
        }
    }

    private void ChildSelection(UIElement newSelected, UIElement invoker, bool select)
    {
        if (selected != newSelected)
        {
            if (selected != null)
                selected.FireDeselectEvent();

            selected = newSelected;
            if (selected)
                selected.FireSelectionEvent(select, false);
        }

        SelectionUpwards(invoker, select);
    }

    private void SelectionUpwards(UIElement invoker, bool select)
    {
        if (parent)
            parent.ChildSelection(select ? this : null, invoker, select);
        else
        {
            FireSelectEvent(false);
            UIMoveManager.Instance.OnElementSelected(select ? this : null, invoker);
        }
    }

    public void Select()
    {
        SelectionUpwards(this, true);
    }

    public void DeSelect()
    {
        SelectionUpwards(this, false);
    }

    public void FireSelectionEvent(bool select, bool shouldPassEvent = true)
    {
        if (select)
            OnEvent(ExecuteEvents.pointerEnterHandler, shouldPassEvent);
        else
            OnEvent(ExecuteEvents.pointerExitHandler, shouldPassEvent);
    }

    public void FireDeselectEvent(bool shouldPassEvent = true)
    {
        OnEvent(ExecuteEvents.pointerExitHandler, shouldPassEvent);
    }

    public void FireSelectEvent(bool shouldPassEvent = true)
    {
        OnEvent(ExecuteEvents.pointerEnterHandler, shouldPassEvent);
    }

    public void OnEvent<T>(ExecuteEvents.EventFunction<T> eventFunction, bool shouldPassEvent = true) where T : IEventSystemHandler
    {
        bool passedEvent = true;

        if (shouldPassEvent && eventInterception.HasFlag(EventInterception.Passthrough))
            passedEvent = PassEvent(eventFunction);

        if (passedEvent && eventInterception.HasFlag(EventInterception.Recieve))
            UIEventThrower.GameobjectUIEvent(gameObject, eventFunction);
    }

    private bool PassEvent<T>(ExecuteEvents.EventFunction<T> eventFunction) where T : IEventSystemHandler
    {
        if (selected)
        {
            selected.OnEvent(eventFunction);
            return true;
        }

        UIEventThrower.GameobjectUIEvent(gameObject, eventFunction);
        return false;
    }

    protected virtual void OnValidate()
    {
        if (uiPos.x < 0)
        {
            Debug.LogWarning("UIElement can not have a value below 0.");
            uiPos.x = 0;
        }
        if (uiPos.y < 0)
        {
            Debug.LogWarning("UIElement can not have a value below 0.");
            uiPos.y = 0;
        }
    }
}
