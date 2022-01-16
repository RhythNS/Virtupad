using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

namespace Virtupad
{
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
           // VRMapperSpecificOffset headOffset = VRMapperSpecificOffset.RotY;// | VRMapperSpecificOffset.PosX | VRMapperSpecificOffset.PosY | VRMapperSpecificOffset.PosZ;
            VRMapper.Instance.AddMapNoOffset(ConstructorDict.Instance.head, VRController.Instance.head);
            
            //VRMapper.Instance.AddMap(ConstructorDict.Instance.rightArm, VRController.Instance.rightHandAttachment, false, false);
            VRMapper.Instance.AddMapCustomRotation(ConstructorDict.Instance.rightArm, VRController.Instance.rightHandAttachment,
                Quaternion.Euler(new Vector3(90.0f, 0.0f, -90.0f)));
            
            //VRMapper.Instance.AddMap(ConstructorDict.Instance.leftArm, VRController.Instance.leftHandAttachment, false, false);
            VRMapper.Instance.AddMapCustomRotation(ConstructorDict.Instance.leftArm, VRController.Instance.leftHandAttachment,
                Quaternion.Euler(new Vector3(90.0f, 0.0f, 90.0f)));

            ConstructorDict.Instance.LoadingCharacterAnimator.runtimeAnimatorController = ConstructorDict.Instance.UpperBody;

            List<VRTracker> trackers = VRController.Instance.trackers;
            for (int i = 0; i < trackers.Count; i++)
            {
                switch (trackers[i].TrackerType)
                {
                    case VRTrackerType.Unknown:
                        break;
                    case VRTrackerType.LeftFoot:
                        VRMapper.Instance.AddMapTracker(ConstructorDict.Instance.leftLeg, trackers[i].transform);
                        break;
                    case VRTrackerType.RightFoot:
                        VRMapper.Instance.AddMapTracker(ConstructorDict.Instance.rightLeg, trackers[i].transform);
                        break;
                    case VRTrackerType.Hip:
                        Transform hip = ConstructorDict.Instance.hip;
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
            ConstructorDict.Instance.LoadingCharacterAnimator.transform.position = VRController.Instance.bodyCollider.position;
            Vector3 vrRot = Player.instance.hmdTransform.rotation.eulerAngles;
            vrRot.x = 0.0f;
            vrRot.z = 0.0f;
            ConstructorDict.Instance.LoadingCharacterAnimator.transform.rotation = Quaternion.Euler(vrRot);
        }
    }
}
