namespace Virtupad
{
    public class UISingleCameraSettingsPanel : UIAnimationPanel
    {
        public void Open()
        {
            RegainFocus(null);
        }

        public void Close()
        {
            LooseFocus(true);
        }

        protected override void OnHidden()
        {
            gameObject.SetActive(false);
        }
    }
}
