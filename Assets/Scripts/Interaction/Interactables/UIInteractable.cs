public class UIInteractable : Interactable
{
    public virtual void Awake()
    {
        SnapToObject = false;
    }

    public override void DeSelect() { }

    public override void Select() { }
}
