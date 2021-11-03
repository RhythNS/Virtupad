using UnityEngine;
using UnityEngine.EventSystems;

// Modified version of:
// https://github.com/Sergey-Shamov/Unity-VR-InputModule

public class UIInteracter : BaseInputModule
{
    public enum State
    {
        JustUp, Up, JustDown, Down
    }

    [SerializeField] private float dragThreshold = 0.5f;

    [SerializeField] private bool handledAbort = true;
    [SerializeField] private bool currentlyUsing = false;
    private Interacter onInteracter;

    private Vector2 scrollDelta = Vector2.zero;

    private float pressedDistance;
    private PointerEventData eventData;
    private Vector3 lastHit;
    private State currentState = State.Up;

    public bool Activate(Interacter onInteracter)
    {
        if (this.onInteracter == onInteracter)
            return true;

        if (currentlyUsing == true || handledAbort == false)
        {
            if (this.onInteracter.Started == true)
                return false;

            Interactable closestInteractable = this.onInteracter.GetClosestInteractable();
            if (closestInteractable != null && closestInteractable is UIInteractable)
                return false;
        }

        currentlyUsing = true;
        handledAbort = false;
        UnSubscribeFromButton();
        this.onInteracter = onInteracter;
        SubscribeToButton();
        return true;
    }

    public void Deactivate()
    {
        currentlyUsing = false;
        UnSubscribeFromButton();
        onInteracter = null;
    }

    private void SubscribeToButton()
    {
        if (onInteracter != null)
            onInteracter.DownChanged += OnDownChanged;
    }

    private void UnSubscribeFromButton()
    {
        if (onInteracter != null)
            onInteracter.DownChanged -= OnDownChanged;
    }

    public void OnDownChanged(bool changed)
    {
        currentState = changed ? State.JustDown : State.JustUp;
    }

    public override void Process()
    {
        UpdateSelectedObject();

        PointerEventData pointerEventData = GetPointerEventData();

        ProcessPress(pointerEventData);
        ProcessMove(pointerEventData);
        ProcessDrag(pointerEventData);
    }

    private void UpdateSelectedObject()
    {
        if (eventSystem.currentSelectedGameObject == null)
            return;

        BaseEventData baseEventData = GetBaseEventData();
        ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, baseEventData, ExecuteEvents.updateSelectedHandler);
    }

    private PointerEventData GetPointerEventData()
    {
        if (eventData == null)
            eventData = new PointerEventData(eventSystem);

        if (currentlyUsing == false && handledAbort == true)
            return eventData;

        Vector2 pointerPos;
        Vector3 direction;

        if (currentlyUsing == true || currentState == State.JustDown || currentState == State.JustUp)
        {
            pointerPos = onInteracter.transform.position;
            direction = onInteracter.transform.forward;
        }
        else
        {
            pointerPos = new Vector2(0.0f, float.MinValue);
            direction = Vector3.down;
            handledAbort = true;
        }

        eventData.position = pointerPos;
        eventData.scrollDelta = scrollDelta;
        scrollDelta = Vector2.zero;
        eventData.button = PointerEventData.InputButton.Left;

        eventSystem.RaycastAll(eventData, m_RaycastResultCache);
        RaycastResult raycastResult = FindFirstRaycast(m_RaycastResultCache);

        Ray ray = new Ray(pointerPos, direction);
        Debug.DrawRay(pointerPos, direction, Color.black);
        Vector3 hit = ray.GetPoint(raycastResult.distance);
        eventData.delta = new Vector2((hit - lastHit).sqrMagnitude, 0);
        lastHit = hit;

        eventData.pointerCurrentRaycast = raycastResult;
        m_RaycastResultCache.Clear();

        return eventData;
    }

    private void ProcessPress(PointerEventData pointerEventData)
    {
        GameObject currentOverGo = pointerEventData.pointerCurrentRaycast.gameObject;

        // PointerDown notification
        if (currentState == State.JustDown)
        {
            currentState = State.Down;

            pointerEventData.eligibleForClick = true;
            pointerEventData.delta = Vector2.zero;
            pointerEventData.useDragThreshold = true;
            pointerEventData.pressPosition = pointerEventData.position;
            pointerEventData.pointerPressRaycast = pointerEventData.pointerCurrentRaycast;

            DeselectIfSelectionChanged(currentOverGo, pointerEventData);

            GameObject newPressed = ExecuteEvents.ExecuteHierarchy(currentOverGo, pointerEventData, ExecuteEvents.pointerDownHandler);

            // didnt find a press handler... search for a click handler
            if (newPressed == null)
                newPressed = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);

            pointerEventData.pointerPress = newPressed;    // TODO:remove?
            pressedDistance = 0;
            pointerEventData.rawPointerPress = currentOverGo;

            pointerEventData.clickTime = Time.unscaledTime;

            // Save the drag handler as well
            pointerEventData.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(currentOverGo);

            if (pointerEventData.pointerDrag != null)
                ExecuteEvents.Execute(pointerEventData.pointerDrag, pointerEventData, ExecuteEvents.initializePotentialDrag);
        }

        // PointerUp notification
        if (currentState == State.JustUp)
        {
            currentState = State.Up;

            ExecuteEvents.Execute(pointerEventData.pointerPress, pointerEventData, ExecuteEvents.pointerUpHandler);

            // see if we button up on the same element that we clicked on...
            GameObject pointerUpHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);

            // PointerClick and Drop events
            if (pointerEventData.pointerPress == pointerUpHandler && pointerEventData.eligibleForClick)
            {
                ExecuteEvents.Execute(pointerEventData.pointerPress, pointerEventData, ExecuteEvents.pointerClickHandler);
            }
            else if (pointerEventData.pointerDrag != null && pointerEventData.dragging)
            {
                ExecuteEvents.ExecuteHierarchy(currentOverGo, pointerEventData, ExecuteEvents.dropHandler);
            }

            pointerEventData.eligibleForClick = false;
            pointerEventData.pointerPress = null;
            pressedDistance = 0;
            pointerEventData.rawPointerPress = null;

            if (pointerEventData.pointerDrag != null && pointerEventData.dragging)
                ExecuteEvents.Execute(pointerEventData.pointerDrag, pointerEventData, ExecuteEvents.endDragHandler);

            pointerEventData.dragging = false;
            pointerEventData.pointerDrag = null;

            // redo pointer enter / exit to refresh state
            // so that if we hovered over something that ignored it before
            // due to having pressed on something else
            // it now gets it.
            if (currentOverGo != pointerEventData.pointerEnter)
            {
                HandlePointerExitAndEnter(pointerEventData, null);
                HandlePointerExitAndEnter(pointerEventData, currentOverGo);
            }
        }
    }

    private void DeselectIfSelectionChanged(GameObject currentOverGo, BaseEventData pointerEvent)
    {
        // Selection tracking
        GameObject selectHandlerGO = ExecuteEvents.GetEventHandler<ISelectHandler>(currentOverGo);
        // if we have clicked something new, deselect the old thing
        // leave 'selection handling' up to the press event though.
        if (selectHandlerGO != eventSystem.currentSelectedGameObject)
            eventSystem.SetSelectedGameObject(null, pointerEvent);
    }

    private void ProcessMove(PointerEventData pointerEventData)
    {
        GameObject targetGO = pointerEventData.pointerCurrentRaycast.gameObject;
        HandlePointerExitAndEnter(pointerEventData, targetGO);
    }

    private void ProcessDrag(PointerEventData pointerEventData)
    {
        // If pointer is not moving or if a button is not pressed (or pressed control did not return drag handler), do nothing
        if (!pointerEventData.IsPointerMoving() || pointerEventData.pointerDrag == null)
            return;

        // We are eligible for drag. If drag did not start yet, add drag distance
        if (!pointerEventData.dragging)
        {
            pressedDistance += pointerEventData.delta.x;

            if (ShouldStartDrag(pointerEventData))
            {
                ExecuteEvents.Execute(pointerEventData.pointerDrag, pointerEventData, ExecuteEvents.beginDragHandler);
                pointerEventData.dragging = true;
            }
        }

        // Drag notification
        if (pointerEventData.dragging)
        {
            // Before doing drag we should cancel any pointer down state
            // And clear selection!
            if (pointerEventData.pointerPress != pointerEventData.pointerDrag)
            {
                ExecuteEvents.Execute(pointerEventData.pointerPress, pointerEventData, ExecuteEvents.pointerUpHandler);

                pointerEventData.eligibleForClick = false;
                pointerEventData.pointerPress = null;
                pointerEventData.rawPointerPress = null;
            }
            ExecuteEvents.Execute(pointerEventData.pointerDrag, pointerEventData, ExecuteEvents.dragHandler);
        }
    }

    private bool ShouldStartDrag(PointerEventData pointerEventData)
    {
        return (currentState == State.Down || currentState == State.JustDown) && (pressedDistance > dragThreshold);
    }
}
