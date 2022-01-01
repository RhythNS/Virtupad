using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Virtupad
{
    public class SelfieCameraMover : CameraMover
    {
        public override void OnInit()
        {
            OnCamera.Grabbable = true;
        }

        public override void Play()
        {
            
        }
    }
}
