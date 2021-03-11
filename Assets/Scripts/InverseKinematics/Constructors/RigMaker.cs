using UnityEngine;
using UnityEngine.Animations.Rigging;

public class RigMaker : MonoBehaviour
{
    private Animator character;
    private Rig rig;
    private RigBuilder builder;

    private void Start()
    {
        Animator character = transform.parent.GetComponent<Animator>();

        Make6TrackingPointsCharacter(character);
    }

    public void Make3TrackingPointsCharacter(Animator character)
    {
        this.character = character;
        character.runtimeAnimatorController = ConstructorDict.Instance.UpperBody;

        PrepRig();

        MakeArms();
        MakeControllingHead();
        MakeAutoLegs();

        FinishRig();
    }

    public void Make4TrackingPointsCharacter(Animator character)
    {
        this.character = character;
        character.runtimeAnimatorController = ConstructorDict.Instance.UpperBody;
        PrepRig();

        MakeArms();
        MakeHead();
        MakeControllingHip();
        MakeAutoLegs();

        FinishRig();
    }

    public void Make5TrackingPointsCharacter(Animator character)
    {
        this.character = character;
        character.runtimeAnimatorController = ConstructorDict.Instance.FullBody;

        PrepRig();

        MakeArms();
        MakeLegs();
        MakeControllingHead();

        FinishRig();
    }

    public void Make6TrackingPointsCharacter(Animator character)
    {
        this.character = character;
        character.runtimeAnimatorController = ConstructorDict.Instance.FullBody;
        PrepRig();

        MakeArms();
        MakeLegs();
        MakeHead();
        MakeControllingHip();

        FinishRig();
    }

    private void PrepRig()
    {
        character.enabled = false;

        rig = ConstructorDict.Instance.rig = gameObject.AddComponent<Rig>();
    }

    private void FinishRig()
    {
        builder = ConstructorDict.Instance.rigBuilder = character.gameObject.AddComponent<RigBuilder>();
        builder.layers.Clear();
        builder.layers.Add(new RigLayer(rig, true));

        builder.Build();
        character.Rebind();

        character.enabled = true;

        Destroy(this);
    }

    private void MakeControllingHead()
    {
        Transform rigTrans = ConstructorDict.Instance.head = MakeOverrideTransform("Head", HumanBodyBones.Head);

        ControllingHead controllingHead = new GameObject("VR Head").AddComponent<ControllingHead>();
        controllingHead.transform.position = rigTrans.position;
        controllingHead.animator = character;
        controllingHead.offset = rigTrans.localPosition;
        controllingHead.rigTrans = rigTrans;
    }

    private void MakeHead()
    {
        ConstructorDict.Instance.head = MakeChain("Head", HumanBodyBones.Hips, HumanBodyBones.Head);
    }

    private void MakeControllingHip()
    {
        Transform rigTrans = ConstructorDict.Instance.hip = MakeOverrideTransform("Hip", HumanBodyBones.Hips);

        ControllingHip controllingHip = rigTrans.gameObject.AddComponent<ControllingHip>();

        Transform hipTrans = character.GetBoneTransform(HumanBodyBones.Hips);
        Vector3 offset = hipTrans.position - character.transform.position;
        controllingHip.offset = offset;
        controllingHip.animator = character;

        /*
        Transform vrHip = new GameObject("VR Hip").transform;

        ControllingHip controllingHip = character.gameObject.AddComponent<ControllingHip>();
        vrHip.transform.position = hipTrans.position;
        controllingHip.hipTrans = hipTrans;
        controllingHip.vrHip = vrHip;
         */
    }

    private void MakeArms()
    {
        ConstructorDict.Instance.leftArm = MakeBodyPart("Left Arm", HumanBodyBones.LeftHand);
        ConstructorDict.Instance.rightArm = MakeBodyPart("Right Arm", HumanBodyBones.RightHand);
        character.gameObject.AddComponent<IKHandSetter>();
    }

    private void MakeLegs()
    {
        ConstructorDict.Instance.leftLeg = MakeBodyPart("Left Leg", HumanBodyBones.LeftFoot);
        ConstructorDict.Instance.rightLeg = MakeBodyPart("Right Leg", HumanBodyBones.RightFoot);
        character.gameObject.AddComponent<IKFootSetter>();
    }

    private void MakeAutoLegs()
    {
        character.gameObject.AddComponent<IKFootGroundChecker>();
    }

    private Transform MakeBodyPart(string name, HumanBodyBones bone)
    {
        GameObject bodyObject = new GameObject(name);
        bodyObject.transform.parent = transform;
        Transform boneTrans = character.GetBoneTransform(bone);
        bodyObject.transform.position = boneTrans.position;
        bodyObject.transform.rotation = boneTrans.rotation;
        return bodyObject.transform;
    }

    private Transform MakeChain(string name, HumanBodyBones root, HumanBodyBones tip)
    {
        GameObject chainObj = new GameObject(name);
        chainObj.transform.parent = transform;

        GameObject targetObject = new GameObject(name + " source");

        ChainIKConstraint chain = chainObj.AddComponent<ChainIKConstraint>();
        chain.Reset();
        ChainIKConstraintData data = chain.data;

        data.root = character.GetBoneTransform(root);
        data.tip = character.GetBoneTransform(tip);
        data.target = targetObject.transform;

        chain.data = data;
        chainObj.transform.position = targetObject.transform.position = data.tip.position;
        return chainObj.transform;
    }

    private Transform MakeOverrideTransform(string name, HumanBodyBones bone)
    {
        GameObject overObj = new GameObject(name);
        overObj.transform.parent = transform;
        GameObject targetObject = new GameObject(name + " source");

        OverrideTransform overrideTransform = overObj.AddComponent<OverrideTransform>();
        overrideTransform.Reset();
        OverrideTransformData data = overrideTransform.data;

        Transform boneTrans = character.GetBoneTransform(bone);
        data.constrainedObject = boneTrans;
        data.sourceObject = targetObject.transform;

        overrideTransform.data = data;
        overObj.transform.position = targetObject.transform.position = boneTrans.position;
        return targetObject.transform;
    }

    private Transform MakeMultiRotation(string name, HumanBodyBones bone)
    {
        GameObject multiRotObj = new GameObject(name);
        multiRotObj.transform.parent = transform;

        MultiRotationConstraint multiRotation = multiRotObj.AddComponent<MultiRotationConstraint>();
        multiRotation.Reset();
        MultiRotationConstraintData data = multiRotation.data;

        WeightedTransformArray sourceObjects = data.sourceObjects;
        sourceObjects.Add(new WeightedTransform(multiRotObj.transform, 1.0f));
        data.sourceObjects = sourceObjects;
        data.constrainedObject = character.GetBoneTransform(bone);

        multiRotation.data = data;
        multiRotObj.transform.position = data.constrainedObject.position;
        return multiRotObj.transform;
    }
}
