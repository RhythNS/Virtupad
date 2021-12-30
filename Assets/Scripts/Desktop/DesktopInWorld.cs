using uDesktopDuplication;
using UnityEngine;

namespace Virtupad
{
    public class DesktopInWorld : Interactable, IStayOnInteractable
    {
        private uDesktopDuplication.Texture texture;

        private Vector2 monitorSize;

        private int currentMonitor;
        public int CurrentMonitor
        {
            get => currentMonitor;
            set
            {
                currentMonitor = value;
                Monitor monitor = Manager.GetMonitor(value);
                if (monitor == null)
                    return;

                monitorSize = new Vector2(monitor.width, monitor.height);
            }
        }

        private BoxCollider boxCollider;

        private void Awake()
        {
            boxCollider = GetComponent<BoxCollider>();
            texture = GetComponent<uDesktopDuplication.Texture>();
            SnapToObject = false;
        }

        private void Start()
        {
            currentMonitor = 0;
        }

        protected override void OnBeginHover(Vector3 impactPoint)
        {
            SetPosition(impactPoint);
        }

        protected override void OnStayHover(Vector3 impactPoint)
        {
            SetPosition(impactPoint);
        }

        protected override void OnLeaveHover()
        {

        }

        public override void Select()
        {
            texture.monitor.MouseButtonDown(0);
        }

        public override void OnEndSelecting()
        {
            texture.monitor.MouseButtonUp(0);
        }

        public override void OnStaySelecting(Vector3 impactPoint)
        {
            SetPosition(impactPoint);
        }

        private void SetPosition(Vector3 impactPoint)
        {
            Vector2 adjImpactPoint = transform.InverseTransformPoint(impactPoint);
            Vector2 colliderSize = boxCollider.size;
            adjImpactPoint += colliderSize * 0.5f;

            Vector2 multiplier = adjImpactPoint / colliderSize;

            Monitor currentMonitor = texture.monitor;
            adjImpactPoint = new Vector2(currentMonitor.width, currentMonitor.height) * multiplier;

            texture.monitor.SetMousePosition(currentMonitor.left + Mathf.RoundToInt(adjImpactPoint.x),
                currentMonitor.top + currentMonitor.height - Mathf.RoundToInt(adjImpactPoint.y));
        }
    }
}
