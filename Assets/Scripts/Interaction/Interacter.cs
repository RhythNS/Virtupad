using UnityEngine;
using Valve.VR;

public class Interacter : MonoBehaviour
{
    [SerializeField] SteamVR_Action_Boolean interactButton;
    [SerializeField] SteamVR_Input_Sources listenForSource;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private float maxRange;

    public bool Started { get; private set; } = false;
    public float MaxRange => maxRange;

    public event BoolChanged DownChanged;

    private Interactable lastSelectedInteractable;

    private void Start()
    {
        lineRenderer.enabled = false;
        lineRenderer.useWorldSpace = true;

        interactButton.AddOnChangeListener(ActiveChanged, listenForSource);
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
        lineRenderer.enabled = true;
        enabled = true;
    }

    private void StopInteract(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        lineRenderer.enabled = false;
        Started = false;
        enabled = false;

        if (lastSelectedInteractable == null)
            return;

        lastSelectedInteractable.Select();
        lastSelectedInteractable.LeaveHover(this);
        lastSelectedInteractable = null;
    }

    private void Update()
    {
        Interactable closestInteractable = GetClosestInteractable(out Vector3 impactPoint);
        UpdateLineRenderer(closestInteractable, impactPoint);
        IssueEvents(closestInteractable);

        lastSelectedInteractable = closestInteractable;
    }

    public Interactable GetClosestInteractable(out Vector3 impactPoint)
    {
        RaycastHit[] raycastHits = Physics.RaycastAll(transform.position, transform.forward, maxRange, ~0);

        Interactable closestInteractable = null;
        float closestLength = float.MaxValue;
        Vector3 ownPos = transform.position;
        impactPoint = Vector3.zero;
        for (int i = 0; i < raycastHits.Length; i++)
        {
            if (raycastHits[i].collider.TryGetComponent(out Interactable interactable) == false)
                continue;

            float lengthAway = (ownPos - raycastHits[i].point).sqrMagnitude;

            if (closestInteractable != null && lengthAway > closestLength)
                continue;

            closestInteractable = interactable;
            closestLength = lengthAway;
            impactPoint = raycastHits[i].point;
        }

        return closestInteractable;
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

    private void IssueEvents(Interactable closestInteractable)
    {
        // is it the same since last frame?
        if (lastSelectedInteractable == closestInteractable)
        {
            // have we selected anything?
            if (closestInteractable)
                closestInteractable.StayHover(this);
            return;
        }

        // not the same
        // have we selected something in the last frame?
        if (lastSelectedInteractable)
            lastSelectedInteractable.LeaveHover(this);
        // have we selected something this frame?
        if (closestInteractable)
            closestInteractable.BeginHover(this);
    }
}
