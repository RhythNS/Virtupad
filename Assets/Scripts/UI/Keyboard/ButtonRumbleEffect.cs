using UnityEngine;
using Valve.VR.InteractionSystem;

public class ButtonRumbleEffect : MonoBehaviour
{
    [SerializeField] private ushort microseconds = 1000;

    public void OnButtonDown(Hand fromHand)
    {
        fromHand.TriggerHapticPulse(microseconds);
    }
}
