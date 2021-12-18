using UnityEngine;
using Valve.VR.InteractionSystem;

namespace Virtupad
{
    public class LogOnEvent : MonoBehaviour
    {
        private void Start()
        {
            GetComponent<HoverButton>().onButtonDown.AddListener(OnHover);
        }

        public void Print()
        {
            Debug.Log("Event fired!");
        }

        public void OnHover(Hand hand)
        {
            hand.TriggerHapticPulse(10000);
        }
    }
}
