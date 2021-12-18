namespace Virtupad
{
    public class CreateAutoRigInteractable : Interactable
    {
        public override void Select()
        {
            FindObjectOfType<FullRigCreator>()?.StartAutoSetup();
        }
    }
}
