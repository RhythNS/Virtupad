using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Virtupad
{
    public class TwoBoneConstraintConstructor : MonoBehaviour
    {
        [SerializeField] private HumanBodyBones root;
        [SerializeField] private HumanBodyBones mid;
        [SerializeField] private HumanBodyBones tip;
        [SerializeField] private Transform target;
        [SerializeField] private Transform hint;

        private void Start()
        {
            Animator character = ConstructorDict.Instance.LoadingCharacterAnimator;
            character.enabled = false;

            Rig rig = ConstructorDict.Instance.rig = transform.parent.gameObject.AddComponent<Rig>();

            TwoBoneIKConstraint twoBoneIKConstraint = gameObject.AddComponent<TwoBoneIKConstraint>();
            twoBoneIKConstraint.Reset();
            var data = twoBoneIKConstraint.data;
            data.root = character.GetBoneTransform(root);
            data.mid = character.GetBoneTransform(mid);
            hint.transform.position = data.mid.transform.position + (-data.mid.transform.forward * 0.1f);
            data.tip = character.GetBoneTransform(tip);
            target.transform.position = data.tip.position;
            data.target = target;
            data.hint = hint;
            twoBoneIKConstraint.data = data;

            RigBuilder builder = ConstructorDict.Instance.rigBuilder = character.gameObject.AddComponent<RigBuilder>();
            builder.layers.Clear();
            builder.layers.Add(new RigLayer(rig, true));

            builder.Build();
            character.Rebind();

            character.enabled = true;

            //twoBoneIKConstraint.CreateJob(character);
        }
    }
}
