using System.Collections.Generic;
using UnityEngine;

namespace Virtupad
{
    public class HelpPanel : UIAnimationPanel
    {
        private bool showing = false;
        private IInteractable lastInteractable;
        private int currentlyShowing = 0;

        [SerializeField] private List<Transform> panelChilds;

        protected override void Start()
        {
            base.Start();

            for (int i = 0; i < panelChilds.Count; i++)
                panelChilds[i].gameObject.SetActive(false);
         
            gameObject.SetActive(false);
            panelChilds[0].gameObject.SetActive(true);
        }

        public void ToggleHelp()
        {
            if (showing == true)
                Hide();
            else
                Show();
        }

        private void Show()
        {
            showing = true;
            RegainFocus(null);
        }

        protected override void OnHidingAnimationFinished()
        {
            base.OnHidingAnimationFinished();

            OnInteractableChanged(HelpID.Unknown);
            lastInteractable = null;
        }

        private void Hide()
        {
            showing = false;
            LooseFocus(false);
        }

        private void OnInteractableChanged(HelpID newId)
        {
            int nowShowing = (int)newId;
            panelChilds[currentlyShowing].gameObject.SetActive(false);
            panelChilds[nowShowing].gameObject.SetActive(true);
            currentlyShowing = nowShowing;
        }

        private void Update()
        {
            for (int i = 0; i < GlobalsDict.Instance.Interacters.Count; i++)
            {
                IInteractable newInteractable = GlobalsDict.Instance.Interacters[i].LastSelectedInteractable;
                if (newInteractable == null)
                    continue;
                if (newInteractable == lastInteractable)
                    return;

                HelpID id = newInteractable.GetHelpID();
                if (id == HelpID.Unknown)
                    continue;

                lastInteractable = newInteractable;
                OnInteractableChanged(id);
                return;
            }
        }
    }
}
