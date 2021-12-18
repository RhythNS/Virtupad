using UnityEngine.Events;

namespace Virtupad
{
    public class EventInteractable : Interactable
    {
        public UnityEvent onSelect;

        public override void Select()
        {
            onSelect?.Invoke();
        }
    }
}
