using UnityEngine;
using Valve.VR;

namespace Virtupad
{
    public class SkeletonPrinterTEst : MonoBehaviour
    {
        private SteamVR_Behaviour_Skeleton skeleton;

        void Start()
        {
            skeleton = GetComponent<SteamVR_Behaviour_Skeleton>();
        }

        private void Update()
        {
            float[] fingerCurls = skeleton.fingerCurls;
            Debug.Log(string.Join(" ", fingerCurls));
        }
    }
}
