using UnityEngine;
using UnityEngine.Animations.Rigging;

public class RigMaker : MonoBehaviour
{
    private Animator character;
    private Rig rig;
    private RigBuilder builder;

    public void Make3TrackingPointsCharacter(Animator character)
    {
        this.character = character;
        character.runtimeAnimatorController = ConstructorDict.Instance.UpperBody;

        MakeArms();
        MakeControllingHead();
    }

    public void Make4TrackingPointsCharacter(Animator character)
    {
        this.character = character;
        character.runtimeAnimatorController = ConstructorDict.Instance.UpperBody;
        PrepRig();

        MakeArms();
        ConstructorDict.Instance.head = MakeChain("Head", HumanBodyBones.Spine, HumanBodyBones.Head);
        MakeControllingHip();

        FinishRig();
    }

    public void Make5TrackingPointsCharacter(Animator character)
    {
        this.character = character;
        character.runtimeAnimatorController = ConstructorDict.Instance.FullBody;

        MakeArms();
        MakeLegs();
        MakeControllingHead();
    }

    public void Make6TrackingPointsCharacter(Animator character)
    {
        this.character = character;
        character.runtimeAnimatorController = ConstructorDict.Instance.FullBody;
        PrepRig();

        MakeArms();
        MakeLegs();
        ConstructorDict.Instance.head = MakeChain("Head", HumanBodyBones.Spine, HumanBodyBones.Head);
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

    }

    private void MakeControllingHip()
    {

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

        ChainIKConstraint chain = chainObj.AddComponent<ChainIKConstraint>();
        chain.Reset();
        ChainIKConstraintData data = chain.data;

        data.root = character.GetBoneTransform(root);
        data.tip = character.GetBoneTransform(tip);
        data.target = chainObj.transform;
        chain.data = data;

        return chainObj.transform;
    }
}
