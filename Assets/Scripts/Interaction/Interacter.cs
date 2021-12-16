using UnityEngine;
using UnityEngine.UI;
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

        enabled = false;

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
        Interactable closestInteractable = GetClosest<Interactable>(out Vector3 impactPoint);

        UpdateLineRenderer(closestInteractable, impactPoint);
        IssueEvents(closestInteractable, impactPoint);

        lastSelectedInteractable = closestInteractable;
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
            if (raycastHits[i].collider.TryGetComponent(out T interactable) == false)
                continue;

            float lengthAway = (ownPos - raycastHits[i].point).sqrMagnitude;

            if (closest != null && lengthAway > closestLength)
                continue;

            closest = interactable;
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
            closestInteractable.BeginHover(this);
    }
}
