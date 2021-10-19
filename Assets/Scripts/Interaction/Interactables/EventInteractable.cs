using UnityEngine.Events;

public class EventInteractable : Interactable
{
    public UnityEvent onSelect;
    public UnityEvent onDeSelect;

    public override void DeSelect()
    {
        onDeSelect?.Invoke();
    }

    public override void Select()
    {
        onSelect?.Invoke();
    }
}
