using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public bool SnapToObject { get; protected set; } = true;
    public abstract void Select();
    public abstract void DeSelect();
}
