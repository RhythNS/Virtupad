using UnityEngine;

namespace Virtupad
{
    public class EmptyStayOnInteractable : Interactable, IStayOnInteractable
    {
        [SerializeField] private bool snapTo = false;

        private void Awake()
        {
            SnapToObject = snapTo;
            HapticFeedbackDuration = 0;
        }

        public override void Select() { }
    }
}
