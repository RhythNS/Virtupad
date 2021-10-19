using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAnimation : Interactable
{
    [SerializeField] private Animation toControl;
    [SerializeField] private string animationName;
    [SerializeField] private PlayMode mode;

    public override void DeSelect()
    {
    }

    public override void Select()
    {
        toControl.Play(animationName);
    }
}
