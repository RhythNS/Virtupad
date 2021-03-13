using System;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public static class VRSetTracker
{
    public static void RegisterTrackers()
    {
        List<VRTracker> trackers = VRDevicesDict.Instance.trackers;

        for (int i = 0; i < trackers.Count; i++)
        {
            UnityEngine.Object.Destroy(trackers[i].gameObject);
        }
        trackers.Clear();

        var error = ETrackedPropertyError.TrackedProp_Success;
        for (uint i = 0; i < 16; i++)
        {
            System.Text.StringBuilder result = new System.Text.StringBuilder(64);
            OpenVR.System.GetStringTrackedDeviceProperty(i, ETrackedDeviceProperty.Prop_RenderModelName_String, result, 64, ref error);
            if (result.ToString().Contains("tracker"))
            {
                if (OpenVR.System.IsTrackedDeviceConnected(i) == false)
                    continue;

                GameObject tracker = new GameObject("Tracker");
                tracker.transform.parent = VRDevicesDict.Instance.SteamVRObjects;
                tracker.SetActive(true);

                VRTracker vrTracker = tracker.AddComponent<VRTracker>();
                vrTracker.TrackedObject.index = (SteamVR_TrackedObject.EIndex)i;
                trackers.Add(vrTracker);
            }
        }
    }

    public static bool RegisterAutoAssignTrackers()
    {
        RegisterTrackers();
        List<VRTracker> trackers = VRDevicesDict.Instance.trackers;

        if (trackers.Count == 0)
            return true;

        Debug.Log("Full body detected!");

        if (trackers.Count == 1)
        {
            trackers[0].TrackerType = VRTrackerType.Hip;
            return true;
        }

        List<VRTracker> toProcess = new List<VRTracker>();
        toProcess.AddRange(trackers);

        if (trackers.Count == 2)
        {
            ProcessLegs(toProcess);
            return true;
        }

        if (trackers.Count == 3)
        {
            ProcessHip(toProcess);
            ProcessLegs(toProcess);
            return true;
        }

        Debug.LogWarning("Could not auto assign trackers! Too many trackers!");

        return false;
    }

    private static void ProcessHip(List<VRTracker> toProcess)
    {
        int highestYIndex = 0;
        for (int i = 1; i < toProcess.Count; i++)
        {
            if (toProcess[i].transform.position.y > toProcess[highestYIndex].transform.position.y)
                highestYIndex = i;
        }

        toProcess[highestYIndex].TrackerType = VRTrackerType.Hip;
        toProcess.RemoveAt(highestYIndex);
    }

    private static void ProcessLegs(List<VRTracker> toProcess)
    {
        if (toProcess.Count != 2)
            throw new Exception("Tracker count not 2! Was: " + toProcess.Count);

        if (toProcess[0].transform.localPosition.x < toProcess[1].transform.localPosition.x)
        {
            toProcess[0].TrackerType = VRTrackerType.LeftFoot;
            toProcess[1].TrackerType = VRTrackerType.RightFoot;
        }
        else
        {
            toProcess[0].TrackerType = VRTrackerType.RightFoot;
            toProcess[1].TrackerType = VRTrackerType.LeftFoot;
        }

        toProcess.Clear();
    }

}
