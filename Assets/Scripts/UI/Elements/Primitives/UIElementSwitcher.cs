using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIElementSwitcher : UIElement
{
    public int Length => switchingChildren.Count;
    [SerializeField] private List<UIElement> switchingChildren = new List<UIElement>();

    public int AtChild => atChild;
    [SerializeField] private int atChild = 0;

    [SerializeField] private int defaultChild = 0;

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
    }

    private int NormalizeChildValue(ref int value) => value = Mathf.Min(switchingChildren.Count - 1, Mathf.Max(0, defaultChild));

    protected override void AddChild(UIElement element)
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

    public override void OnEvent<T>(ExecuteEvents.EventFunction<T> eventFunction, bool shouldPassEvent = true)
    {
        switchingChildren[atChild].OnEvent(eventFunction, shouldPassEvent);
    }
}
