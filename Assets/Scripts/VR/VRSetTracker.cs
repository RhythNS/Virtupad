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

    public static RigMaker.Config? AutoAssignTrackers()
    {
        List<VRTracker> trackers = VRDevicesDict.Instance.trackers;

        if (trackers.Count == 0)
            return RigMaker.Config.ThreePoints;

        Debug.Log("Full body detected!");

        if (trackers.Count == 1)
        {
            trackers[0].TrackerType = VRTrackerType.Hip;
            return RigMaker.Config.FourPoints;
        }

        List<VRTracker> toProcess = new List<VRTracker>();
        toProcess.AddRange(trackers);

        if (trackers.Count == 2)
        {
            ProcessLegs(toProcess);
            return RigMaker.Config.FivePoints;
        }

        if (trackers.Count == 3)
        {
            ProcessHip(toProcess);
            ProcessLegs(toProcess);
            return RigMaker.Config.SixPoints;
        }

        Debug.LogWarning("Could not auto assign trackers! Too many trackers!");

        return null;
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

        /* Not sure how to do this mathematically better.
         * We get the right directional vector of the head and compare it with the direction
         * of tracker zero and one. Whatever has a smaller angle must be the right foot.
         */
        Transform head = VRDevicesDict.Instance.head;
        Vector3 rightDir = head.right;
        Vector3 zeroTrackerDir = (toProcess[0].transform.position - head.position).normalized;
        Vector3 oneTrackerDir = (toProcess[1].transform.position - head.position).normalized;
        float zeroAngle = Vector3.Angle(rightDir, zeroTrackerDir);
        float oneAngle = Vector3.Angle(rightDir, oneTrackerDir);

        if (zeroAngle < oneAngle)
        {
            toProcess[0].TrackerType = VRTrackerType.RightFoot;
            toProcess[1].TrackerType = VRTrackerType.LeftFoot;
        }
        else
        {
            toProcess[0].TrackerType = VRTrackerType.LeftFoot;
            toProcess[1].TrackerType = VRTrackerType.RightFoot;
        }

        toProcess.Clear();
    }

}
