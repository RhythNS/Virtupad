public class CreateAutoRigInteractable : Interactable
{
    public override void Select()
    {
        FindObjectOfType<FullRigCreator>()?.StartAutoSetup();
    }

    public override void DeSelect()
    {

    }

}
