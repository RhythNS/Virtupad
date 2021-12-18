using System;
using UnityEngine;
using Valve.VR;

namespace Virtupad
{
    public class Interacter : MonoBehaviour
    {
        [SerializeField] SteamVR_Action_Boolean interactButton;
        [SerializeField] SteamVR_Input_Sources listenForSource;
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private float maxRange;

        public bool Started { get; private set; } = false;
        public float MaxRange => maxRange;

        public bool IsButtonPressed => buttonPressed;
        private bool buttonPressed = false;

        public event BoolChanged DownChanged;

        private Interactable lastSelectedInteractable;

        private bool beginStopSelectRequest = false;
        private Interactable beginStopSelecting;
        private Vector3 lastHitStopSelectingPoint = Vector3.zero;

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
            DownChanged?.Invoke(active);
            if (active)
                StartInteract(fromAction, fromSource);
            else
                StopInteract(fromAction, fromSource);
        }

        private void StartInteract(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            buttonPressed = true;

            if (enabled)
                beginStopSelectRequest = true;

            lineRenderer.enabled = true;
            enabled = true;
        }

        private void StopInteract(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            buttonPressed = false;

            if (beginStopSelecting != null)
            {
                beginStopSelecting.OnEndSelecting();

                if (beginStopSelecting == lastSelectedInteractable)
                    beginStopSelecting.Select();

                if (lastSelectedInteractable != null)
                    lastSelectedInteractable.LeaveHover(this);

                if (ShouldGoOff())
                    Stop(false);

                lastSelectedInteractable = null;
                beginStopSelecting = null;

                return;
            }

            if (ShouldGoOff())
            {
                Stop();
                return;
            }

            lastSelectedInteractable.Select();
        }

        private bool ShouldGoOff() => lastSelectedInteractable == null || lastSelectedInteractable.TryGetComponent<IStayOnInteractable>(out _) == false;

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

        public void StopRequest()
        {
            if (buttonPressed == true)
                return;

            Stop();
        }

        private void Update()
        {
            Interactable closestInteractable = GetClosest<Interactable>(out Vector3 impactPoint);

            if (beginStopSelectRequest == true)
                HandleBeginStopRequest(closestInteractable, impactPoint);
            else if (beginStopSelecting != null)
                HandleBeginStopStay(closestInteractable, impactPoint);

            UpdateLineRenderer(closestInteractable, impactPoint);
            IssueEvents(closestInteractable, impactPoint);

            lastSelectedInteractable = closestInteractable;
        }

        private void HandleBeginStopRequest(Interactable interactable, Vector3 impactPoint)
        {
            beginStopSelectRequest = false;

            if (interactable == null)
                return;

            beginStopSelecting = interactable;
            beginStopSelecting.OnBeginSelecting(impactPoint);
        }

        private void HandleBeginStopStay(Interactable closestInteractable, Vector3 impactPoint)
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

        private void UpdateLineRenderer(Interactable closestInteractable, Vector3 impactPoint)
        {
            Vector3 ownPos = transform.position;

            Vector3 toPos;
            if (closestInteractable == null)
                toPos = ownPos + transform.forward * maxRange;
            else if (closestInteractable.SnapToObject)
                toPos = closestInteractable.transform.position;
            else
                toPos = impactPoint;

            lineRenderer.SetPositions(new Vector3[2] { ownPos, toPos });
        }

        private void IssueEvents(Interactable closestInteractable, Vector3 impactPoint)
        {
            // is it the same since last frame?
            if (lastSelectedInteractable == closestInteractable)
            {
                // have we selected anything?
                if (closestInteractable)
                    closestInteractable.StayHover(this, impactPoint);
                return;
            }

            // not the same
            // have we selected something in the last frame?
            if (lastSelectedInteractable)
                lastSelectedInteractable.LeaveHover(this);
            // have we selected something this frame?
            if (closestInteractable)
                closestInteractable.BeginHover(this, impactPoint);
        }

        private void OnDestroy()
        {
            if (GlobalsDict.Instance)
                GlobalsDict.Instance.Interacters.Remove(this);
        }
    }
}
