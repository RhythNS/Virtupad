using System.Collections.Generic;
using UnityEngine;

namespace Virtupad
{
    public abstract class Interactable : MonoBehaviour, IInteractable
    {
        public bool SnapToObject { get => snapToObject; set => snapToObject = value; }
        private bool snapToObject;

        public ushort HapticFeedbackDuration { get => hapticFeedbackDuration; set => hapticFeedbackDuration = value; }
        private ushort hapticFeedbackDuration = 1000;

        public List<Interacter> CurrentlyInteracting => currentlyInteracting;

        private List<Interacter> currentlyInteracting = new List<Interacter>();

        public abstract void Select();

        public void BeginHover(Interacter interacter, Vector3 impactPoint)
        {
            if (currentlyInteracting.Count == 0)
                OnBeginHover(impactPoint);

            currentlyInteracting.Add(interacter);
        }

        protected virtual void OnBeginHover(Vector3 impactPoint) { }

        public void StayHover(Interacter interacter, Vector3 impactPoint)
        {
            OnStayHover(impactPoint);
        }

        protected virtual void OnStayHover(Vector3 impactPoint) { }

        public void LeaveHover(Interacter interacter)
        {
            currentlyInteracting.Remove(interacter);
            if (currentlyInteracting.Count == 0)
                OnLeaveHover();
        }

        public virtual void OnBeginSelecting(Vector3 impactPoint) { }

        public virtual void OnStaySelecting(Vector3 impactPoint) { }

        public virtual void OnEndSelecting() { }

        protected virtual void OnLeaveHover() { }

        public ushort HapticFeedbackOnSelect()
        {
            return HapticFeedbackDuration;
        }
    }
}
