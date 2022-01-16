using System;
using System.Collections.Generic;
using UnityEngine;

namespace Virtupad
{
    public class VRMapper : MonoBehaviour
    {
        [System.Serializable]
        public struct Mapped
        {
            public Transform constrain;
            public Transform source;
            public Vector3 posOffset;
            public Quaternion rotOffset;
            public Quaternion sourceStartRot;

            public Mapped(Transform constrain, Transform source, Vector3 posOffset, Quaternion rotOffset, Quaternion sourceStartRot)
            {
                this.constrain = constrain;
                this.source = source;
                this.posOffset = posOffset;
                this.rotOffset = rotOffset;
                this.sourceStartRot = sourceStartRot;
            }
        }

        [System.Serializable]
        public struct CustomMapped
        {
            public Transform constrain;
            public Transform source;
            public Quaternion rotOffset;

            public CustomMapped(Transform constrain, Transform source, Quaternion rotOffset)
            {
                this.constrain = constrain;
                this.source = source;
                this.rotOffset = rotOffset;
            }
        }

        public static VRMapper Instance { get; private set; }

        [SerializeField] private List<Mapped> maps = new List<Mapped>();
        [SerializeField] private List<CustomMapped> customMapped = new List<CustomMapped>();

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

        public void AddMapCustomRotation(Transform constrain, Transform source, Quaternion rotOffset)
        {
            if (MapCheck(constrain, source) == false)
                return;

            customMapped.Add(new CustomMapped(constrain, source, rotOffset));
        }

        public void AddMapNoOffset(Transform constrain, Transform source)
        {
            if (MapCheck(constrain, source) == false)
                return;

            maps.Add(new Mapped(constrain, source, Vector3.zero, Quaternion.identity, Quaternion.identity));
        }

        public void AddMapTracker(Transform constrain, Transform source)
        {
            if (MapCheck(constrain, source) == false)
                return;

            Vector3 posOffset = source.position - constrain.position;
            Quaternion rotOffset = Quaternion.Inverse(source.rotation) * constrain.rotation;

            maps.Add(new Mapped(constrain, source, posOffset, rotOffset, source.rotation));
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
            for (int i = 0; i < customMapped.Count; i++)
            {
                if (customMapped[i].constrain == constrain || customMapped[i].source == source)
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
                Quaternion secOffset = maps[i].source.rotation * Quaternion.Inverse(maps[i].sourceStartRot);
                Vector3 toMove = secOffset * maps[i].posOffset;
                maps[i].constrain.position = maps[i].source.position - toMove;

                maps[i].constrain.rotation =  maps[i].source.rotation * maps[i].rotOffset;
            }

            for (int i = 0; i < customMapped.Count; i++)
            {
                customMapped[i].constrain.position = customMapped[i].source.position;
                customMapped[i].constrain.rotation = customMapped[i].source.rotation * customMapped[i].rotOffset;
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }
    }
}
