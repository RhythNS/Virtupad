using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class VRDevicesDict : MonoBehaviour
{
    public static VRDevicesDict Instance { get; private set; }

    private void Awake()
    {
        if (Instance)
        {
            Debug.LogWarning("VRDevicesDict already in scene. Deleting myself!");
            Destroy(this);
            return;
        }
        Instance = this;
    }

    public Transform head;

    public Hand leftHand, rightHand;

    public List<VRTracker> trackers = new List<VRTracker>();

    public Transform SteamVRObjects => steamVRObjects;
    [SerializeField] private Transform steamVRObjects;

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
