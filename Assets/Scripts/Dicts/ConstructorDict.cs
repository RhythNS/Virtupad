using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class ConstructorDict : MonoBehaviour
{
    public static ConstructorDict Instance { get; private set; }

    private void Awake()
    {
        if (Instance)
        {
            Debug.LogWarning("ConstructorDict already in scene. Deleting myself!");
            Destroy(this);
            return;
        }
        Instance = this;
    }

    public Animator LoadingCharacterAnimator;

    public RigBuilder rigBuilder;

    public Rig rig;

    public Transform rightArm, leftArm, head, rightLeg, leftLeg, hip;

    public RigMaker rigMaker;

    public VRMController vrmController;

    public RuntimeAnimatorController TPoseController => tPoseController;
    [SerializeField] private RuntimeAnimatorController tPoseController;

    public RuntimeAnimatorController FullBody => fullBody;
    [SerializeField] private RuntimeAnimatorController fullBody;

    public RuntimeAnimatorController UpperBody => upperBody;
    [SerializeField] private RuntimeAnimatorController upperBody;

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
