using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Virtupad
{
    public class MonitorInWorld : MonoBehaviour
    {
        public UIMonitorPanel Panel => panel;
        [SerializeField] private UIMonitorPanel panel;

        private void Start()
        {
            InteracterEvents.Instance.OnInteractBegin += OnInteractBegin;
            InteracterEvents.Instance.OnInteractEnd += OnInteractEnd;
        }

        private void OnInteractBegin()
        {
            panel.Open();
        }

        private void OnInteractEnd()
        {
            panel.Close();
        }

        private void OnDestroy()
        {
            InteracterEvents instance = InteracterEvents.Instance;
            if (!instance)
                return;

            InteracterEvents.Instance.OnInteractBegin -= OnInteractBegin;
            InteracterEvents.Instance.OnInteractEnd -= OnInteractEnd;
        }
    }
}
