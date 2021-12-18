namespace Virtupad
{
    public abstract class UIPanel : UIPrimitiveElement, UIControllerIntercept
    {
        public virtual void CloseRequest()
        {
            NotifyClose(UIRegainFocusMessage.Empty);
        }

        public virtual void LooseFocus(bool closing)
        {

        }

        public virtual void RegainFocus(UIRegainFocusMessage msg)
        {

        }

        public bool Intercept(UIControllerAction controllerAction)
        {
            if (controllerAction == UIControllerAction.Cancel)
            {
                CloseRequest();
                return true;
            }

            return false;
        }

        public override bool InterceptAction(UIControllerAction action)
        {
            if (selected != null)
                return selected.InterceptAction(action);

            if (controllerIntercept != null && controllerIntercept.Intercept(action) == true)
                return true;

            return false;
        }

        public void NotifyClose(UIRegainFocusMessage msg)
        {
            UIPanelManager.Instance.OnMenuClosed(this, msg);
        }

        public void NotifyOpen()
        {
            UIPanelManager.Instance.OnMenuOpened(this);
        }
    }
}
