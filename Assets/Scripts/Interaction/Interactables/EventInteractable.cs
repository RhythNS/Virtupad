using UnityEngine.Events;

public class EventInteractable : Interactable
{
    public UnityEvent onSelect;

    public override void Select()
    {
        onSelect?.Invoke();
    }
}
