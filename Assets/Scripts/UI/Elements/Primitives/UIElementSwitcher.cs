using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Virtupad
{
    public class UIElementSwitcher : UIPrimitiveElement
    {
        public int Length => switchingChildren.Count;
        [SerializeField] private List<UIPrimitiveElement> switchingChildren = new List<UIPrimitiveElement>();

        public int AtChild => atChild;
        [SerializeField] private int atChild = 0;

        private void Start()
        {
            for (int i = 0; i < switchingChildren.Count; i++)
            {
                switchingChildren[i].gameObject.SetActive(false);
            }

            NormalizeChildValue(ref atChild);

            switchingChildren[AtChild].gameObject.SetActive(true);
        }

        public void SwitchChild(int newIndex)
        {
            NormalizeChildValue(ref newIndex);

            switchingChildren[atChild].gameObject.SetActive(false);
            switchingChildren[atChild = newIndex].gameObject.SetActive(true);

            // maybe play animation
            switchingChildren[atChild].OnInit();
        }

        public override void OnInit()
        {
            switchingChildren[atChild].OnInit();
        }

        private int NormalizeChildValue(ref int value) => value = Mathf.Min(switchingChildren.Count - 1, Mathf.Max(0, value));

        public override void AddChild(UIPrimitiveElement element)
        {
            base.AddChild(element);

            switchingChildren.Add(element);
        }

        public override bool InterceptAction(UIControllerAction action)
        {
            return switchingChildren[atChild].InterceptAction(action);
        }

        public override bool Move(Direction direction)
        {
            return switchingChildren[atChild].Move(direction);
        }

        public override void Select()
        {
            switchingChildren[atChild].Select();
        }

        public override void DeSelect()
        {
            switchingChildren[atChild].DeSelect();
        }

        public override void FireDeselectEvent(bool shouldPassEvent = true)
        {
            switchingChildren[atChild].FireDeselectEvent(shouldPassEvent);
        }

        public override void FireSelectionEvent(bool select, bool shouldPassEvent = true)
        {
            switchingChildren[atChild].FireSelectionEvent(select, shouldPassEvent);
        }

        public override void FireSelectEvent(bool shouldPassEvent = true)
        {
            switchingChildren[atChild].FireSelectEvent(shouldPassEvent);
        }

        public override void OnEvent<T>(ExecuteEvents.EventFunction<T> eventFunction, bool shouldPassEvent = true, PointerEventData ped = null)
        {
            switchingChildren[atChild].OnEvent(eventFunction, shouldPassEvent, ped);
        }
    }
}
