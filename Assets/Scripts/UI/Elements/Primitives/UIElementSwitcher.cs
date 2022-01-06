using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Virtupad
{
    public class UIElementSwitcher : UIPrimitiveElement
    {
        [System.Serializable]
        public class Element
        {
            public UIPrimitiveElement primitive;
            public bool alreadyInited = false;

            public Element(UIPrimitiveElement primitive, bool alreadyInited)
            {
                this.primitive = primitive;
                this.alreadyInited = alreadyInited;
            }
        }

        public int Length => switchingChildren.Count;
        [SerializeField] private List<Element> switchingChildren = new List<Element>();

        public int AtChild
        {
            get => atChild;
            private set
            {
                atChild = value;
                onChildSwitched.Invoke(value);
            }
        }

        [SerializeField] private int atChild = 0;

        [SerializeField] private int returnToChildAtReset = -1;

        public UnityEvent<int> onChildSwitched;

        protected override void Awake()
        {
            for (int i = 0; i < switchingChildren.Count; i++)
                switchingChildren[i].primitive.gameObject.SetActive(false);
        }

        private void Start()
        {
            AtChild = NormalizeChildValue(AtChild);

            switchingChildren[AtChild].primitive.gameObject.SetActive(true);
        }

        public void SwitchChild(int newIndex)
        {
            newIndex = NormalizeChildValue(newIndex);

            if (newIndex == AtChild)
                return;

            switchingChildren[AtChild].primitive.gameObject.SetActive(false);

            AtChild = newIndex;
            switchingChildren[AtChild].primitive.gameObject.SetActive(true);
            InitCurrentChild();
        }

        public override void OnInit()
        {
            InitCurrentChild();
        }

        public override void OnReset()
        {
            if (returnToChildAtReset != -1)
                AtChild = returnToChildAtReset;

            switchingChildren[AtChild].primitive.OnReset();
        }

        private void InitCurrentChild()
        {
            //   if (switchingChildren[AtChild].alreadyInited == true)
            //       return;

            switchingChildren[AtChild].alreadyInited = true;
            switchingChildren[AtChild].primitive.OnInit();
        }

        private int NormalizeChildValue(int value) => Mathf.Min(switchingChildren.Count - 1, Mathf.Max(0, value));

        public override void AddChild(UIPrimitiveElement element)
        {
            base.AddChild(element);

            switchingChildren.Add(new Element(element, false));
        }

        public override void RemoveAllChildren()
        {
            switchingChildren[AtChild].primitive.RemoveAllChildren();
        }

        public override void RemoveChild(UIPrimitiveElement element)
        {
            switchingChildren[AtChild].primitive.RemoveChild(element);
        }

        public override bool InterceptAction(UIControllerAction action)
        {
            return switchingChildren[AtChild].primitive.InterceptAction(action);
        }

        public override bool Move(Direction direction)
        {
            return switchingChildren[AtChild].primitive.Move(direction);
        }

        public override void Select()
        {
            switchingChildren[AtChild].primitive.Select();
        }

        public override void DeSelect()
        {
            switchingChildren[AtChild].primitive.DeSelect();
        }

        public override void FireDeselectEvent(bool shouldPassEvent = true)
        {
            switchingChildren[AtChild].primitive.FireDeselectEvent(shouldPassEvent);
        }

        public override void FireSelectionEvent(bool select, bool shouldPassEvent = true)
        {
            switchingChildren[AtChild].primitive.FireSelectionEvent(select, shouldPassEvent);
        }

        public override void FireSelectEvent(bool shouldPassEvent = true)
        {
            switchingChildren[AtChild].primitive.FireSelectEvent(shouldPassEvent);
        }

        public override void OnEvent<T>(ExecuteEvents.EventFunction<T> eventFunction, bool shouldPassEvent = true, PointerEventData ped = null)
        {
            switchingChildren[AtChild].primitive.OnEvent(eventFunction, shouldPassEvent, ped);
        }
    }
}
