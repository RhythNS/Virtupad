using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public static class VRToRig
{
    public static void PrepareRig()
    {
        ConstructorDict.Instance.LoadingCharacterAnimator.gameObject.AddComponent<VRMapper>();
        GameObject builder = new GameObject("Rig");
        builder.transform.parent = ConstructorDict.Instance.LoadingCharacterAnimator.transform;
        ConstructorDict.Instance.rigMaker = builder.AddComponent<RigMaker>();
    }

    public static void MakeCharacter(RigMaker.Config config)
    {
        ConstructorDict.Instance.rigMaker.MakeCharacter(config);
    }

    public static void AssignTrackers()
    {
        //VRMapper.Instance.AddMap(ConstructorDict.Instance.head, VRDevicesDict.Instance.head, false, false);
        VRMapperSpecificOffset headOffset = VRMapperSpecificOffset.RotY;// | VRMapperSpecificOffset.PosX | VRMapperSpecificOffset.PosY | VRMapperSpecificOffset.PosZ;
        VRMapper.Instance.AddMap(ConstructorDict.Instance.head, VRController.Instance.head, headOffset);
        VRMapper.Instance.AddMap(ConstructorDict.Instance.rightArm, VRController.Instance.rightHand.transform, false);
        VRMapper.Instance.AddMap(ConstructorDict.Instance.leftArm, VRController.Instance.leftHand.transform, false);

        ConstructorDict.Instance.LoadingCharacterAnimator.runtimeAnimatorController = ConstructorDict.Instance.UpperBody;

        List<VRTracker> trackers = VRController.Instance.trackers;
        for (int i = 0; i < trackers.Count; i++)
        {
            switch (trackers[i].TrackerType)
            {
                case VRTrackerType.Unknown:
                    break;
                case VRTrackerType.LeftFoot:
                    VRMapper.Instance.AddMap(ConstructorDict.Instance.leftLeg, trackers[i].transform);
                    break;
                case VRTrackerType.RightFoot:
                    VRMapper.Instance.AddMap(ConstructorDict.Instance.rightLeg, trackers[i].transform);
                    break;
                case VRTrackerType.Hip:
                    Transform hip = ConstructorDict.Instance.hip;
                    //VRMapper.Instance.AddMap(hip, trackers[i].transform, hip.position - trackers[i].transform.position, (trackers[i].transform.rotation * hip.rotation).eulerAngles);
                    //VRMapper.Instance.AddMap(hip, trackers[i].transform, hip.position - trackers[i].transform.position, Quaternion.Inverse(trackers[i].transform.rotation * hip.rotation).eulerAngles);
                    //VRMapper.Instance.AddMap(hip, trackers[i].transform, hip.position - trackers[i].transform.position, Quaternion.Inverse(trackers[i].transform.rotation).eulerAngles);
                    VRMapper.Instance.AddMapTracker(hip, trackers[i].transform);
                    break;
                default:
                    throw new System.Exception("TrackerType not implemented: " + trackers[i].TrackerType);
            }
        }
    }

    public static void CharacterToTPose()
    {
        ConstructorDict.Instance.LoadingCharacterAnimator.runtimeAnimatorController = ConstructorDict.Instance.TPoseController;
        ConstructorDict.Instance.LoadingCharacterAnimator.Update(0.1f);
    }

    public static void CharacterToVRPlayer()
    {
        ConstructorDict.Instance.LoadingCharacterAnimator.transform.position = Player.instance.transform.position;
        Vector3 vrRot = Player.instance.hmdTransform.rotation.eulerAngles;
        vrRot.x = 0.0f;
        vrRot.z = 0.0f;
        ConstructorDict.Instance.LoadingCharacterAnimator.transform.rotation = Quaternion.Euler(vrRot);
    }
}
