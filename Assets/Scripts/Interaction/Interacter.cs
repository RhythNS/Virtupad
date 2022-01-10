using System;
using UnityEngine;
using Valve.VR;

namespace Virtupad
{
    public class Interacter : MonoBehaviour
    {
        [SerializeField] SteamVR_Action_Boolean interactButton;

        public SteamVR_Input_Sources ForSource => listenForSource;
        [SerializeField] SteamVR_Input_Sources listenForSource;

        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private float maxRange;

        public bool Started { get; private set; } = false;
        public float MaxRange => maxRange;

        public bool IsButtonPressed => buttonPressed;
        private bool buttonPressed = false;

        public event BoolChanged DownChanged;

        private IInteractable lastSelectedInteractable;

        private bool beginStopSelectRequest = false;
        private IInteractable beginStopSelecting;
        private Vector3 lastHitStopSelectingPoint = Vector3.zero;

        private IGrabbable grabbed;

        private void Start()
        {
            lineRenderer.enabled = false;
            lineRenderer.useWorldSpace = true;

            enabled = false;

            interactButton.AddOnChangeListener(ActiveChanged, listenForSource);

            GlobalsDict.Instance.Interacters.Add(this);
        }

        private void ActiveChanged(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool active)
        {
            if (active)
                StartInteract(fromAction, fromSource);
            else
                StopInteract(fromAction, fromSource);
            DownChanged?.Invoke(active);
        }

        private void StartInteract(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            if (grabbed != null)
                return;

            buttonPressed = true;

            if (enabled)
                beginStopSelectRequest = true;

            lineRenderer.enabled = true;
            enabled = true;
        }

        private void StopInteract(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            if (enabled == false)
                return;

            buttonPressed = false;

            if (beginStopSelecting != null)
            {
                if (beginStopSelecting == lastSelectedInteractable)
                    beginStopSelecting.Select();

                beginStopSelecting.OnEndSelecting();

                if (lastSelectedInteractable != null)
                    lastSelectedInteractable.LeaveHover(this);

                if (ShouldGoOff())
                    Stop(false);

                lastSelectedInteractable = null;
                beginStopSelecting = null;

                return;
            }

            if (lastSelectedInteractable != null)
                lastSelectedInteractable.Select();

            if (ShouldGoOff())
            {
                Stop();
                return;
            }
        }

        private bool ShouldGoOff() => lastSelectedInteractable == null || !(lastSelectedInteractable is IStayOnInteractable);

        private void Stop(bool issueStopEvents = true)
        {
            lineRenderer.enabled = false;
            Started = false;
            enabled = false;

            if (lastSelectedInteractable == null || issueStopEvents == false)
                return;

            lastSelectedInteractable.LeaveHover(this);
            lastSelectedInteractable = null;
        }

        public void ChangeGrabbed(IGrabbable grabbed)
        {
            this.grabbed = grabbed;
        }

        public void StopRequest()
        {
            if (buttonPressed == true)
                return;

            Stop();
        }

        private void Update()
        {
            IInteractable closestInteractable;
            Vector3 impactPoint = Vector3.zero;

            closestInteractable = grabbed != null ? null : GetClosest<IInteractable>(out impactPoint);

            if (beginStopSelectRequest == true)
                HandleBeginStopRequest(closestInteractable, impactPoint);
            else if (beginStopSelecting != null)
                HandleBeginStopStay(closestInteractable, impactPoint);

            UpdateLineRenderer(closestInteractable, impactPoint);
            IssueEvents(closestInteractable, impactPoint);

            lastSelectedInteractable = closestInteractable;

            if (grabbed != null)
                Stop();
        }

        private void HandleBeginStopRequest(IInteractable interactable, Vector3 impactPoint)
        {
            beginStopSelectRequest = false;

            if (interactable == null)
                return;

            beginStopSelecting = interactable;
            beginStopSelecting.OnBeginSelecting(impactPoint);
        }

        private void HandleBeginStopStay(IInteractable closestInteractable, Vector3 impactPoint)
        {
            if (closestInteractable == beginStopSelecting)
            {
                beginStopSelecting.OnStaySelecting(impactPoint);
                lastHitStopSelectingPoint = impactPoint;
                return;
            }

            beginStopSelecting.OnStaySelecting(lastHitStopSelectingPoint);
        }

        public T GetClosest<T>(out Vector3 impactPoint)
        {
            RaycastHit[] raycastHits = Physics.RaycastAll(transform.position, transform.forward, maxRange, ~0);

            T closest = default;
            float closestLength = float.MaxValue;
            Vector3 ownPos = transform.position;
            impactPoint = Vector3.zero;
            for (int i = 0; i < raycastHits.Length; i++)
            {
                if (raycastHits[i].collider.TryGetComponent(out T t) == false)
                    continue;

                float lengthAway = (ownPos - raycastHits[i].point).sqrMagnitude;

                if (closest != null && lengthAway > closestLength)
                    continue;

                closest = t;
                closestLength = lengthAway;
                impactPoint = raycastHits[i].point;
            }

            return closest;
        }

        private void UpdateLineRenderer(IInteractable closestInteractable, Vector3 impactPoint)
        {
            Vector3 ownPos = transform.position;

            Vector3 toPos;
            if (closestInteractable == null)
                toPos = ownPos + transform.forward * maxRange;
            else if (closestInteractable.SnapToObject)
                toPos = (closestInteractable as Component).transform.position;
            else
                toPos = impactPoint;

            lineRenderer.SetPositions(new Vector3[2] { ownPos, toPos });
        }

        private void IssueEvents(IInteractable closestInteractable, Vector3 impactPoint)
        {
            // is it the same since last frame?
            if (lastSelectedInteractable == closestInteractable)
            {
                // have we selected anything?
                if (closestInteractable != null)
                    closestInteractable.StayHover(this, impactPoint);
                return;
            }

            // not the same
            // have we selected something in the last frame?
            if (lastSelectedInteractable != null)
                lastSelectedInteractable.LeaveHover(this);
            // have we selected something this frame?
            if (closestInteractable != null)
                closestInteractable.BeginHover(this, impactPoint);
        }

        private void OnDestroy()
        {
            if (GlobalsDict.Instance)
                GlobalsDict.Instance.Interacters.Remove(this);
        }
    }
}
