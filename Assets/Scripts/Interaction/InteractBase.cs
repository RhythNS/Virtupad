using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractBase : MonoBehaviour
{
    public static InteractBase Instance { get; private set; }

    public float TimeBeforeQuickDeselectInSeconds => timeBeforeQuickDeselectInSeconds;
    [SerializeField] private float timeBeforeQuickDeselectInSeconds = 1.0f;

    public Interactable LastSelected { get; private set; } = null;

    private void Awake()
    {
        if (Instance)
        {
            Debug.LogWarning("InteractBase already in scene. Deleting myself!");
            Destroy(this);
            return;
        }
        Instance = this;
    }

    public void Select(Interactable interactable)
    {
        interactable.Select();
    }

    public void DeSelect()
    {
        if (LastSelected == null)
            return;

        LastSelected.DeSelect();
        LastSelected = null;
    }


    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

}
