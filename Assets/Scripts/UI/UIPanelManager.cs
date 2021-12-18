using System.Collections.Generic;
using UnityEngine;

namespace Virtupad
{
    public class UIPanelManager : MonoBehaviour
    {
        public static UIPanelManager Instance { get; private set; }

        [SerializeField] private List<UIPanel> menuStack = new List<UIPanel>();
        [SerializeField] private UIRoot root;

        private void Awake()
        {
            if (root == null)
                Debug.LogWarning("Root not set on panel manager");

            if (Instance)
            {
                Debug.LogWarning("UIPanelManager already in scene. Deleting myself!");
                Destroy(this);
                return;
            }
            Instance = this;
        }

        public void OnMenuOpened(UIPanel panel)
        {
            for (int i = 0; i < menuStack.Count; i++)
            {
                if (menuStack[i] == panel)
                    return;
            }

            if (menuStack.Count != 0)
                menuStack[menuStack.Count - 1].LooseFocus(false);
            menuStack.Add(panel);
            panel.RegainFocus(UIRegainFocusMessage.Empty);
        }

        public void OnMenuClosed(UIPanel menu, UIRegainFocusMessage message)
        {
            if (menuStack.Count == 0 || menuStack[menuStack.Count - 1] != menu)
            {
                Debug.LogWarning("Given panel " + menu.name + " was not the last panel or the panel stack is empty");
                return;
            }

            menu.LooseFocus(true);

            menuStack.RemoveAt(menuStack.Count - 1);
            if (menuStack.Count != 0)
                menuStack[menuStack.Count - 1].RegainFocus(message);
        }

        public UIPanel GetTopMenu()
        {
            if (menuStack.Count == 0)
                return null;
            return menuStack[menuStack.Count - 1];
        }

        public void OnMainMenuToggle()
        {
            UIPanel panel = GetTopMenu();
            if (panel != null)
            {
                panel.CloseRequest();
                return;
            }

            menuStack.Add(root);
            root.RegainFocus(UIRegainFocusMessage.GetBasic(UIRegainFocusMessage.RegainFocusMessageType.Restore));
        }

        public bool OnControllerAction(UIControllerAction action)
        {
            UIPanel menu = GetTopMenu();
            if (menu != null)
            {
                if (menu.InterceptAction(action) == true)
                    return true;
            }

            // something like a switch case that disables the menu

            return true;
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }
    }
}
