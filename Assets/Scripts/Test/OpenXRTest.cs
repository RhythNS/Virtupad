using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace Virtupad
{
    public class OpenXRTest : MonoBehaviour
    {
        private List<XRInputSubsystem> subsystems = new List<XRInputSubsystem>();
        private XRInputSubsystem inputSubsystem;

        private void Start()
        {
            SetTrackingOriginMode();
        }

        public void SetTrackingOriginMode()
        {
            if (inputSubsystem == null)
            {
                SubsystemManager.GetInstances(subsystems);
                if (subsystems.Count != 0)
                    inputSubsystem = subsystems[0];
            }

            Debug.Log($"Tracking Origin Mode is [{inputSubsystem.GetTrackingOriginMode()}]");
            //   inputSubsystem.
        }
    }
}
