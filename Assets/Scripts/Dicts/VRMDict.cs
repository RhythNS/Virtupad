using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRM;

namespace Virtupad
{
    public class VRMDict : MonoBehaviour
    {
        public static VRMDict Instance { get; private set; }

        public List<BlendShapePreset> DefaultsPresets => defaultPresets;
        [SerializeField] private List<BlendShapePreset> defaultPresets = new List<BlendShapePreset>();

        public float ChangeEmoteInSeconds => changeEmoteInSeconds;
        [SerializeField] private float changeEmoteInSeconds;

        private void Awake()
        {
            if (Instance)
            {
                Debug.LogWarning("VRMDict already in scene. Deleting myself!");
                Destroy(this);
                return;
            }
            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }
    }
}
