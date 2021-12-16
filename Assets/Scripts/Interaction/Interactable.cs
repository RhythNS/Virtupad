using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public bool SnapToObject { get; protected set; } = true;

    private List<Interacter> currentlyInteracting = new List<Interacter>();

    public abstract void Select();

    public void BeginHover(Interacter interacter)
    {
        if (currentlyInteracting.Count == 0)
            OnBeginHover();

        currentlyInteracting.Add(interacter);
    }

    protected virtual void OnBeginHover() { }

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

    protected virtual void OnLeaveHover() { }
}
