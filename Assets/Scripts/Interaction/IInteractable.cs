using UnityEngine;

namespace Virtupad
{
    public interface IInteractable
    {
        public bool SnapToObject { get; set; }

        public void Select();

        public ushort HapticFeedbackOnSelect();

        public void BeginHover(Interacter interacter, Vector3 impactPoint);

        public void StayHover(Interacter interacter, Vector3 impactPoint);

        public void LeaveHover(Interacter interacter);

        public void OnBeginSelecting(Vector3 impactPoint);

        public void OnStaySelecting(Vector3 impactPoint);

        public void OnEndSelecting();
    }
}
