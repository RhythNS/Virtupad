using UnityEngine;

namespace Virtupad
{
    public class AudioOutputPlayer : Interactable
    {
        [SerializeField] private int index;

        private void Awake()
        {
            SnapToObject = true;
        }

        public override void Select()
        {
            AudioOutput.Instance.ChangeIndex(index);
        }
    }
}
