using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Virtupad
{
    public class MonitorInWorld : Interactable
    {
        public UIMonitorPanel Panel => panel;
        [SerializeField] private UIMonitorPanel panel;

        private Collider OwnCollider => ownCollider;
        [SerializeField] private Collider ownCollider;

        private void Awake()
        {
            SnapToObject = true;

            Panel.screenActiveChanged += ScreenActiveChanged;
        }

        private void ScreenActiveChanged(bool active)
        {
            ownCollider.enabled = active == false;
        }

        private void Start()
        {
            InteracterEvents.Instance.OnInteractBegin += OnInteractBegin;
            InteracterEvents.Instance.OnInteractEnd += OnInteractEnd;
        }

        private void OnInteractBegin()
        {
            if (Panel.Screen.gameObject.activeInHierarchy)
            {
                panel.Open();
                return;
            }
        }

        private void OnInteractEnd()
        {
            if (Panel.Screen.gameObject.activeInHierarchy)
            {
                panel.Close();
                return;
            }
        }

        public override void Select()
        {
            panel.Open();
        }

        private void OnDestroy()
        {
            if (Panel)
                Panel.screenActiveChanged -= ScreenActiveChanged;

            InteracterEvents instance = InteracterEvents.Instance;
            if (!instance)
                return;

            InteracterEvents.Instance.OnInteractBegin -= OnInteractBegin;
            InteracterEvents.Instance.OnInteractEnd -= OnInteractEnd;
        }
    }
}
