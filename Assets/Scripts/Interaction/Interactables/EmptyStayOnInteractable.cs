using UnityEngine;

namespace Virtupad
{
    public class EmptyStayOnInteractable : Interactable, IStayOnInteractable
    {
        [SerializeField] private bool snapTo = false;

        private void Awake()
        {
            SnapToObject = snapTo;
        }

        public override void Select() { }
    }
}
