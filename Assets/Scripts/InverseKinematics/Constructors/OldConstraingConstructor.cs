using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Virtupad
{
    public class OldConstraingConstructor : MonoBehaviour
    {
        private Animator character;
        private Rig rig;
        private RigBuilder builder;

        private void Start()
        {
            Make6TrackingPointsCharacter(ConstructorDict.Instance.LoadingCharacterAnimator);
        }

        public void Make3TrackingPointsCharacter(Animator character)
        {
            this.character = character;
            character.runtimeAnimatorController = ConstructorDict.Instance.UpperBody;
            Prep();

            MakeArms();
            MakeHead();

            Finish();
        }

        public void Make4TrackingPointsCharacter(Animator character)
        {
            this.character = character;
            character.runtimeAnimatorController = ConstructorDict.Instance.UpperBody;
            Prep();

            MakeArms();
            MakeHead();
            MakeHip();

            Finish();
        }

        public void Make5TrackingPointsCharacter(Animator character)
        {
            this.character = character;
            character.runtimeAnimatorController = ConstructorDict.Instance.FullBody;
            Prep();

            MakeArms();
            MakeLegs();
            MakeHead();

            Finish();
        }

        public void Make6TrackingPointsCharacter(Animator character)
        {
            this.character = character;
            character.runtimeAnimatorController = ConstructorDict.Instance.FullBody;
            Prep();

            MakeArms();
            MakeLegs();
            ConstructorDict.Instance.head = MakeTwoBoneConstraint("Head", false, HumanBodyBones.UpperChest, HumanBodyBones.Neck, HumanBodyBones.Head);
            MakeHip();

            Finish();
        }

        private void MakeHead()
        {
            ConstructorDict.Instance.head = MakeMultiParentConstraint("Head", HumanBodyBones.Head);
        }

        private void MakeHip()
        {
            ConstructorDict.Instance.hip = MakeMultiParentConstraint("Hip", HumanBodyBones.Spine);
        }

        private void MakeArms()
        {
            ConstructorDict.Instance.rightArm = MakeTwoBoneConstraint("Right Arm", false, HumanBodyBones.RightUpperArm, HumanBodyBones.RightLowerArm, HumanBodyBones.RightHand);
            ConstructorDict.Instance.leftArm = MakeTwoBoneConstraint("Left Arm", false, HumanBodyBones.LeftUpperArm, HumanBodyBones.LeftLowerArm, HumanBodyBones.LeftHand);
        }

        private void MakeLegs()
        {
            ConstructorDict.Instance.rightLeg = MakeTwoBoneConstraint("Right Leg", true, HumanBodyBones.RightUpperLeg, HumanBodyBones.RightLowerLeg, HumanBodyBones.RightFoot);
            ConstructorDict.Instance.leftLeg = MakeTwoBoneConstraint("Left Leg", true, HumanBodyBones.LeftUpperLeg, HumanBodyBones.LeftLowerLeg, HumanBodyBones.LeftFoot);
        }

        private void Prep()
        {
            character.enabled = false;

            rig = ConstructorDict.Instance.rig = gameObject.AddComponent<Rig>();
        }

        private void Finish()
        {
            builder = ConstructorDict.Instance.rigBuilder = character.gameObject.AddComponent<RigBuilder>();
            builder.layers.Clear();
            builder.layers.Add(new RigLayer(rig, true));

            builder.Build();
            character.Rebind();

            character.enabled = true;

            Destroy(this);
        }

        private Transform MakeTwoBoneConstraint(string name, bool useForward, HumanBodyBones root, HumanBodyBones mid, HumanBodyBones tip)
        {
            GameObject constraintParent = new GameObject(name);
            constraintParent.transform.parent = transform;

            TwoBoneIKConstraint twoBoneIKConstraint = constraintParent.gameObject.AddComponent<TwoBoneIKConstraint>();
            twoBoneIKConstraint.Reset();
            TwoBoneIKConstraintData data = twoBoneIKConstraint.data;

            data.root = character.GetBoneTransform(root);
            data.mid = character.GetBoneTransform(mid);
            data.tip = character.GetBoneTransform(tip);

            GameObject hint = new GameObject("Hint");
            hint.transform.parent = constraintParent.transform;
            hint.transform.position = data.mid.transform.position + (data.mid.transform.forward * 0.1f * (useForward ? 1f : -1f));
            hint.transform.rotation = data.mid.transform.rotation;
            data.hint = hint.transform;

            GameObject target = new GameObject("Target");
            target.transform.parent = constraintParent.transform;
            target.transform.position = data.tip.position;
            target.transform.rotation = data.tip.rotation;
            data.target = target.transform;

            twoBoneIKConstraint.data = data;

            return target.transform;
        }

        private Transform MakeMultiParentConstraint(string name, HumanBodyBones bone)
        {
            GameObject multiParent = new GameObject(name);
            multiParent.transform.parent = transform;

            MultiParentConstraint multiParentConstraint = multiParent.AddComponent<MultiParentConstraint>();
            multiParentConstraint.Reset();
            MultiParentConstraintData data = multiParentConstraint.data;

            data.constrainedObject = character.GetBoneTransform(bone);
            multiParent.transform.position = data.constrainedObject.position;
            WeightedTransformArray sourceObjects = data.sourceObjects;
            sourceObjects.Clear();
            sourceObjects.Add(new WeightedTransform(multiParent.transform, 1.0f));
            data.sourceObjects = sourceObjects;

            multiParentConstraint.data = data;

            return multiParent.transform;
        }

        private Transform MakeOverrideTransform(string name, HumanBodyBones bone)
        {
            GameObject overrideObj = new GameObject(name);
            overrideObj.transform.parent = transform;

            OverrideTransform overrideTransform = overrideObj.AddComponent<OverrideTransform>();
            overrideTransform.Reset();
            OverrideTransformData data = overrideTransform.data;

            data.sourceObject = overrideObj.transform;
            data.constrainedObject = character.GetBoneTransform(bone);
            overrideObj.transform.position = data.constrainedObject.position;
            data.space = OverrideTransformData.Space.Local;

            overrideTransform.data = data;
            return overrideObj.transform;
        }
    }
}
