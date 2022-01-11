using System.Collections.Generic;
using UnityEngine;

namespace VRKeys
{
    public class KeyStabManager : MonoBehaviour
    {
        public static KeyStabManager Instance { get; private set; }

        public List<KeyStab> Stabs => stabs;
        [SerializeField] private List<KeyStab> stabs;

        private void Awake()
        {
            if (Instance)
            {
                Debug.LogWarning("KeyStabManager already in scene. Deleting myself!");
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
