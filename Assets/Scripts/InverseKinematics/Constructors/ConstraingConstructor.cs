using UnityEngine;
using UnityEngine.Animations.Rigging;

public class ConstraingConstructor : MonoBehaviour
{
    void Start()
    {
        Animator character = ConstructorDict.Instance.LoadingCharacterAnimator;
        character.enabled = false;

        Rig rig = ConstructorDict.Instance.rig = gameObject.AddComponent<Rig>();

        MakeArm(character, "Right Arm", HumanBodyBones.RightUpperArm, HumanBodyBones.RightLowerArm, HumanBodyBones.RightHand);
        MakeArm(character, "Left Arm", HumanBodyBones.LeftUpperArm, HumanBodyBones.LeftLowerArm, HumanBodyBones.LeftHand);
        MakeHead(character);

        RigBuilder builder = ConstructorDict.Instance.rigBuilder = character.gameObject.AddComponent<RigBuilder>();
        builder.layers.Clear();
        builder.layers.Add(new RigLayer(rig, true));

        builder.Build();
        character.Rebind();

        character.enabled = true;

        Destroy(this);
    }

    private void MakeArm(Animator character, string name, HumanBodyBones root, HumanBodyBones mid, HumanBodyBones tip)
    {
        GameObject rightArm = new GameObject(name);
        rightArm.transform.parent = transform;

        TwoBoneIKConstraint twoBoneIKConstraint = rightArm.gameObject.AddComponent<TwoBoneIKConstraint>();
        twoBoneIKConstraint.Reset();
        TwoBoneIKConstraintData data = twoBoneIKConstraint.data;

        data.root = character.GetBoneTransform(root);
        data.mid = character.GetBoneTransform(mid);
        data.tip = character.GetBoneTransform(tip);

        GameObject hint = new GameObject("Hint");
        hint.transform.parent = rightArm.transform;
        hint.transform.position = data.mid.transform.position + (-data.mid.transform.forward * 0.1f);
        hint.transform.rotation = data.mid.transform.rotation;
        data.hint = hint.transform;

        GameObject target = new GameObject("Target");
        target.transform.parent = rightArm.transform;
        target.transform.position = data.tip.position;
        target.transform.rotation = data.tip.rotation;
        data.target = target.transform;

        twoBoneIKConstraint.data = data;
    }

    private void MakeHead(Animator animator)
    {
        GameObject head = new GameObject("Head");
        head.transform.parent = transform;

        MultiParentConstraint multiParentConstraint = head.AddComponent<MultiParentConstraint>();
        multiParentConstraint.Reset();
        MultiParentConstraintData data = multiParentConstraint.data;

        data.constrainedObject = animator.GetBoneTransform(HumanBodyBones.Head);
        head.transform.position = data.constrainedObject.position;
        WeightedTransformArray sourceObjects = data.sourceObjects;
        sourceObjects.Clear();
        sourceObjects.Add(new WeightedTransform(head.transform, 1.0f));
        data.sourceObjects = sourceObjects;

        multiParentConstraint.data = data;
    }
}
