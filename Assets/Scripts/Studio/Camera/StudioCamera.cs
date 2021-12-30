using System;
using UnityEngine;

namespace Virtupad
{
    public class StudioCamera : MonoBehaviour
    {
        public Transform Tracking { get; set; }
        public Transform Anchor { get; set; }
        public CameraMover Mover { get; private set; }

        public void SetCameraMover(CameraMover.Type type, bool autoPlay = true)
        {
            Type moverSystemType = CameraMover.GetSystemTypeForType(type);
            if (moverSystemType == null)
            {
                Debug.LogWarning("Could not get mover type for " + type + "!");
                return;
            }

            Component moverComp = GetComponent(moverSystemType);

            CameraMover newMover;
            if (moverComp == null)
                newMover = gameObject.AddComponent(moverSystemType) as CameraMover;
            else
                newMover = moverComp as CameraMover;

            if (newMover == Mover)
                return;

            Mover.enabled = false;

            newMover.Init(this);
            newMover.enabled = true;

            if (autoPlay == true)
                newMover.Play();

            Mover = newMover;
        }

    }
}
