using System;
using System.Collections.Generic;
using UnityEngine;

namespace Virtupad
{
    public class VRMapper : MonoBehaviour
    {
        public bool IsFullBody { get; private set; } = false;
        public static VRMapper Instance { get; private set; }

        [SerializeField] private List<MapTransform> maps = new List<MapTransform>();
        [SerializeField] private List<MapTrackerTransform> trackerMaps = new List<MapTrackerTransform>();

        [Serializable]
        public struct MapTransform
        {
            public Transform source, constrain;
            public Vector3 offsetPos;
            public Quaternion offsetRot;
            public bool useSecondaryRotOffset;

            public MapTransform(Transform constrain, Transform source, Vector3 offsetPos, Quaternion offsetRot, bool useSecondaryRotOffset)
            {
                this.source = source;
                this.constrain = constrain;
                this.offsetPos = offsetPos;
                this.offsetRot = offsetRot;
                this.useSecondaryRotOffset = useSecondaryRotOffset;
            }
        }

        [Serializable]
        public struct MapTrackerTransform
        {
            public Transform source, constrain;
            public Vector3 offsetPos;
            public Quaternion offsetRot;

            public MapTrackerTransform(Transform constrain, Transform source, Vector3 offsetPos, Quaternion offsetRot)
            {
                this.source = source;
                this.constrain = constrain;
                this.offsetPos = offsetPos;
                this.offsetRot = offsetRot;
            }
        }

        private void Awake()
        {
            if (Instance)
            {
                Debug.LogWarning("VRMapper already in scene. Deleting myself!");
                Destroy(this);
                return;
            }
            Instance = this;
        }

        public void AddMap(Transform constrain, Transform source, bool usePosOffset = true, bool useRotationOffset = true)
        {
            if (MapCheck(constrain, source) == false)
                return;

            Vector3 offsetPos = constrain.position - source.position;
            Quaternion rotOffset = constrain.rotation * Quaternion.Inverse(source.rotation);
            maps.Add(new MapTransform(constrain, source, usePosOffset ? offsetPos : Vector3.zero,
                useRotationOffset ? rotOffset : Quaternion.identity, false));
        }

        public void AddMap(Transform constrain, Transform source, Vector3 posOffset, Quaternion rotOffset)
        {
            if (MapCheck(constrain, source) == false)
                return;

            maps.Add(new MapTransform(constrain, source, posOffset, rotOffset, true));
        }

        public void AddMap(Transform constrain, Transform source, VRMapperSpecificOffset specificOffset)
        {
            if (specificOffset == VRMapperSpecificOffset.None)
            {
                AddMap(constrain, source, false, false);
                return;
            }

            if (MapCheck(constrain, source) == false)
                return;

            Vector3 offsetPos = constrain.position - source.position;
            Vector3 offsetRot = (source.rotation * Quaternion.Inverse(constrain.rotation)).eulerAngles;
            if (specificOffset.HasFlag(VRMapperSpecificOffset.PosX) == false)
                offsetPos.x = 0.0f;
            if (specificOffset.HasFlag(VRMapperSpecificOffset.PosY) == false)
                offsetPos.y = 0.0f;
            if (specificOffset.HasFlag(VRMapperSpecificOffset.PosZ) == false)
                offsetPos.z = 0.0f;
            if (specificOffset.HasFlag(VRMapperSpecificOffset.RotX) == false)
                offsetRot.x = 0.0f;
            if (specificOffset.HasFlag(VRMapperSpecificOffset.RotY) == false)
                offsetRot.y = 0.0f;
            if (specificOffset.HasFlag(VRMapperSpecificOffset.RotZ) == false)
                offsetRot.z = 0.0f;

            maps.Add(new MapTransform(constrain, source, offsetPos, Quaternion.Euler(offsetRot), false));
        }

        public void AddMapTracker(Transform constrain, Transform source)
        {
            if (MapCheck(constrain, source) == false)
                return;

            Vector3 offsetPos = constrain.position - source.position;
            trackerMaps.Add(new MapTrackerTransform(constrain, source, offsetPos, source.rotation * Quaternion.Inverse(constrain.rotation)));
        }

        private bool MapCheck(Transform constrain, Transform source)
        {
            for (int i = 0; i < maps.Count; i++)
            {
                if (maps[i].constrain == constrain || maps[i].source == source)
                {
                    Debug.LogError(source.name + " or " + constrain.name + " is already mapped!");
                    return false;
                }
            }
            for (int i = 0; i < trackerMaps.Count; i++)
            {
                if (trackerMaps[i].constrain == constrain || trackerMaps[i].source == source)
                {
                    Debug.LogError(source.name + " or " + constrain.name + " is already mapped!");
                    return false;
                }
            }
            return true;
        }

        private void Update()
        {
            for (int i = 0; i < maps.Count; i++)
            {
                maps[i].constrain.position = maps[i].source.position + maps[i].offsetPos;
                maps[i].constrain.rotation = maps[i].useSecondaryRotOffset ? 
                    maps[i].source.rotation * maps[i].offsetRot : maps[i].offsetRot * maps[i].source.rotation;
            }
            for (int i = 0; i < trackerMaps.Count; i++)
            {
                trackerMaps[i].constrain.position = trackerMaps[i].source.position + trackerMaps[i].offsetPos;
                trackerMaps[i].constrain.rotation = trackerMaps[i].source.rotation * trackerMaps[i].offsetRot;
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }
    }
}
