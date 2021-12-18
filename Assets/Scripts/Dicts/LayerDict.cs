using UnityEngine;

namespace Virtupad
{
    public class LayerDict : MonoBehaviour
    {
        public static LayerDict Instance { get; private set; }

        public int OnlyVisibleInVRLayer => onlyVisibleInVRLayer;
        [SerializeField] private int onlyVisibleInVRLayer;

        public int OnlyVisibleInDesktopLayer => onlyVisibleInDesktopLayer;
        [SerializeField] private int onlyVisibleInDesktopLayer;

        public int MaskEverythingButPlayer => ~(1 << playerCollider | 1 << playerHands);

        public int PlayerCollider => playerCollider;
        [SerializeField] private int playerCollider;

        public int PlayerHands => playerHands;
        [SerializeField] private int playerHands;

        private void Awake()
        {
            if (Instance)
            {
                Debug.LogWarning("LayerDict already in scene. Deleting myself!");
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
