using UnityEngine;
using Valve.VR;

public class Interacter : MonoBehaviour
{
    [SerializeField] SteamVR_Action_Boolean interactButton;
    [SerializeField] SteamVR_Input_Sources listenForSource;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private float maxRange;

    private Interactable lastSelectedInteractable;
    private bool started = false;
    private float timer;

    private void Start()
    {
        lineRenderer.enabled = false;
        lineRenderer.useWorldSpace = true;

        interactButton.AddOnChangeListener(ActiveChanged, listenForSource);
    }

    private void ActiveChanged(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool active)
    {
        if (active)
            StartInteract(fromAction, fromSource);
        else
            StopInteract(fromAction, fromSource);
    }

    private void StartInteract(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        timer = 0.0f;
        lineRenderer.enabled = true;
        started = true;
    }

    private void StopInteract(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        lineRenderer.enabled = false;
        started = false;

        if (lastSelectedInteractable == null)
        {
            if (timer < InteractBase.Instance.TimeBeforeQuickDeselectInSeconds)
            {
                InteractBase.Instance.DeSelect();
            }
            return;
        }

        InteractBase.Instance.Select(lastSelectedInteractable);

        Debug.Log("Interact with " + (lastSelectedInteractable == null ? "nothing" : lastSelectedInteractable.name));
    }

    private void FixedUpdate()
    {
        if (started == false)
            return;

        timer += Time.deltaTime;

        RaycastHit[] raycastHits = Physics.RaycastAll(transform.position, transform.forward, maxRange);

        Interactable closestInteractable = null;
        float closestLength = float.MaxValue;
        Vector3 ownPos = transform.position;

        for (int i = 0; i < raycastHits.Length; i++)
        {
            if (raycastHits[i].collider.TryGetComponent(out Interactable interactable) == false)
                continue;

            float lengthAway = (ownPos - interactable.transform.position).sqrMagnitude;

            if (closestInteractable != null && lengthAway > closestLength)
                continue;

            closestInteractable = interactable;
            closestLength = lengthAway;
        }

        Vector3[] positions = new Vector3[2];
        positions[0] = ownPos;
        positions[1] =
            closestInteractable == null || closestInteractable.SnapToObject == false
            ? ownPos + transform.forward * maxRange
            : closestInteractable.transform.position;
        lineRenderer.SetPositions(positions);

        lastSelectedInteractable = closestInteractable;
    }
}
