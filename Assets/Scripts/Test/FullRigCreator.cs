using System.Collections;
using UnityEngine;

public class FullRigCreator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float waitTime = 3.0f;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            StartAutoSetup();
        }
    }

    public void StartAutoSetup()
    {
        StartCoroutine(AutoSetup());
    }

    private IEnumerator AutoSetup()
    {
        Debug.Log("Starting rig constructor...");
        
        if (VRAnimatorController.Instance)
            VRAnimatorController.Instance.enabled = false;
        VRSetTracker.RegisterTrackers();
        RigMaker.Config? config = VRSetTracker.AutoAssignTrackers();
        if (config.HasValue == false)
            yield break;

        ConstructorDict.Instance.LoadingCharacterAnimator = animator;
        VRToRig.CharacterToTPose();
        VRToRig.CharacterToVRPlayer();

        yield return new WaitForSeconds(waitTime);

        VRToRig.PrepareRig();
        VRToRig.MakeCharacter(config.Value);
        VRToRig.AssignTrackers();

        if (VRAnimatorController.Instance)
            VRAnimatorController.Instance.enabled = true;
        Debug.Log("Finished rig constructor");
    }

}
