using System;
using System.Collections;
using UnityEngine;

namespace Virtupad
{
    public class FullRigCreator : MonoBehaviour
    {
        [SerializeField] private int defaultSeconds = 5;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                StartAutoSetup();
            }
        }

        public void StartAutoSetup(int seconds = -1, Action<int> callbackForSecondsLeft = null, Action onFinished = null)
        {
            StartCoroutine(AutoSetup(seconds, callbackForSecondsLeft, onFinished));
        }

        public RigMaker.Config? GetConfig()
        {
            VRSetTracker.RegisterTrackers();
            return VRSetTracker.GetConfig();
        }

        private IEnumerator AutoSetup(int seconds, Action<int> callbackForSecondsLeft, Action onFinished)
        {
            if (seconds == -1)
                seconds = defaultSeconds;

            if (VRAnimatorController.Instance)
                VRAnimatorController.Instance.enabled = false;
            VRSetTracker.RegisterTrackers();
            RigMaker.Config? config = VRSetTracker.AutoAssignTrackers();
            if (config.HasValue == false)
                yield break;

            ConstructorDict.Instance.LoadingCharacterAnimator = GetComponent<Animator>();
            ConstructorDict.Instance.vrmController = GetComponent<VRMController>();

            VRToRig.CharacterToTPose();
            VRToRig.CharacterToVRPlayer();
            VRController.Instance.SizeToModelHeight(VRMController.Instance.Height);

            for (int i = 0; i < seconds; i++)
            {
                callbackForSecondsLeft?.Invoke(seconds - i);
                yield return new WaitForSeconds(1.0f);
            }

            VRToRig.PrepareRig();
            VRToRig.MakeCharacter(config.Value);
            VRToRig.AssignTrackers();

            VRMController.Instance.OnTakenControl();

            onFinished?.Invoke();
        }
    }
}
