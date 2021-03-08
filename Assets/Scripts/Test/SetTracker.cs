using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class SetTracker : MonoBehaviour
{
    [SerializeField] private SteamVR_TrackedObject[] trackedObjects;
    [SerializeField] private VRAnimatorController animatonController;
    [SerializeField] private Transform pelvis;
    [SerializeField] private Transform leftFoot;
    [SerializeField] private Transform rightFoot;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitForSeconds(3.0f);

        bool hasOneTracker = false;
        int index = 0;
        var error = ETrackedPropertyError.TrackedProp_Success;
        List<SteamVR_TrackedObject> trackedObjects = new List<SteamVR_TrackedObject>();
        for (uint i = 0; i < 16; i++)
        {
            var result = new System.Text.StringBuilder(64);
            OpenVR.System.GetStringTrackedDeviceProperty(i, ETrackedDeviceProperty.Prop_RenderModelName_String, result, 64, ref error);
            if (result.ToString().Contains("tracker"))
            {
                if (OpenVR.System.IsTrackedDeviceConnected(i) == false)
                    continue;
                hasOneTracker = true;
                trackedObjects.Add(this.trackedObjects[index]);
                trackedObjects[index].gameObject.SetActive(true);
                trackedObjects[index++].index = (SteamVR_TrackedObject.EIndex)i;
            }
        }
        /*
        animatonController.SetAvatarMask(hasOneTracker);

        if (trackedObjects.Count != 3)
            yield break;

        Debug.Log("Full body detected!");

        Transform highestY = trackedObjects[0].transform;
        for (int i = 1; i < trackedObjects.Count; i++)
        {
            if (trackedObjects[i].transform.position.y > highestY.transform.position.y)
                highestY = trackedObjects[i].transform;
        }

        trackedObjects.RemoveAll(x => x.transform == highestY);

        Transform highestX, lowestX;

        if (trackedObjects[0].transform.localPosition.x < trackedObjects[1].transform.localPosition.x)
        {
            highestX = trackedObjects[1].transform;
            lowestX = trackedObjects[0].transform;
        }
        else
        {
            highestX = trackedObjects[0].transform;
            lowestX = trackedObjects[1].transform;
        }

        VRRig.Instance.fullBody = true;

        VRRig.Instance.pelivs.vrTarget = highestY;
        VRRig.Instance.pelivs.enabled = true;
        VRRig.Instance.pelivs.trackingRotationOffset =- highestY.rotation.eulerAngles;

        VRRig.Instance.leftFoot.vrTarget = lowestX;
        VRRig.Instance.leftFoot.enabled = true;
        VRRig.Instance.leftFoot.trackingRotationOffset = -lowestX.rotation.eulerAngles;
        
        VRRig.Instance.rightFoot.vrTarget = highestX;
        VRRig.Instance.rightFoot.enabled = true;
        VRRig.Instance.rightFoot.trackingRotationOffset = -highestX.rotation.eulerAngles;
         */
    }

}
