using UnityEngine;

namespace Virtupad
{
    public class PlayAnimation : Interactable
    {
        [SerializeField] private Animation toControl;
        [SerializeField] private string animationName;
        [SerializeField] private PlayMode mode;

        public override void Select()
        {
            toControl.Play(animationName);
        }
    }
}
