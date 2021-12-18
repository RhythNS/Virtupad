using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

namespace Virtupad
{
    public class GlobalsDict : MonoBehaviour
    {
        public static GlobalsDict Instance { get; private set; }

        public Player Player => player;
        [SerializeField] private Player player;

        public List<Interacter> Interacters => interacters;
        [SerializeField] private List<Interacter> interacters = new List<Interacter>();

        private void Awake()
        {
            if (Instance)
            {
                Debug.LogWarning("GlobalsDict already in scene. Deleting myself!");
                Destroy(this);
                return;
            }

            if (!Player)
            {
                Debug.LogWarning("Player was not assigned! Searching for him instead!");
                player = FindObjectOfType<Player>();
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
