using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class VRTracker : MonoBehaviour
{
    public VRTrackerType TrackerType = VRTrackerType.Unknown;

    public SteamVR_TrackedObject TrackedObject { get; private set; }

    private void Awake()
    {
        TrackedObject = GetComponent<SteamVR_TrackedObject>();
    }
}
