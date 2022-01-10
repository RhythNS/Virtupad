using System.Collections.Generic;
using UnityEngine;

namespace Virtupad
{
    public class GlobalsDict : MonoBehaviour
    {
        public static GlobalsDict Instance { get; private set; }

        public List<Interacter> Interacters => interacters;
        [SerializeField] private List<Interacter> interacters = new List<Interacter>();

        public SetDefinition CurrentDefinition
        {
            get => currentDefinition;
            set
            {
                currentDefinition = value;
                onSetDefinitionChanged?.Invoke(value);
            }
        }
        private SetDefinition currentDefinition;
        public event SetDefinitionChanged onSetDefinitionChanged;

        private void Awake()
        {
            if (Instance)
            {
                Debug.LogWarning("GlobalsDict already in scene. Deleting myself!");
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
