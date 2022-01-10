using UnityEngine;
using Valve.VR.InteractionSystem;

namespace VRKeys
{
    public class KeyStab : MonoBehaviour
    {
        [SerializeField] private Hand onHand;

        private void Start()
        {
            gameObject.SetActive(false);
        }

        public void OnKeyHit()
        {
            onHand.TriggerHapticPulse(1500);
        }
    }
}
