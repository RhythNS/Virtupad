using System.Collections.Generic;
using UnityEngine;

namespace Virtupad
{
    public class UIRoot : UIAnimationPanel
    {
        public static UIRoot Instance { get; private set; }

        [SerializeField] private Vector3 forwardTrackingPosition = new Vector3(0.0f, 1.0f, 1.5f);
        [SerializeField] private Quaternion trackingRotation;

        public UIElementSwitcher MainSwitcher => mainSwitcher;
        [SerializeField] private UIElementSwitcher mainSwitcher;

        public UIMenuSelectionPanel SelectionPanel => selectionPanel;
        [SerializeField] private UIMenuSelectionPanel selectionPanel;

        private Transform toTrack;

        protected override void Awake()
        {
            if (Instance)
            {
                Debug.LogWarning("UIVRMSelector already in scene. Deleting myself!");
                Destroy(gameObject);
                return;
            }
            Instance = this;

            base.Awake();
            trackingRotation = transform.rotation;
            gameObject.SetActive(false);
        }

        protected override void OnEnable()
        {
            toTrack = VRController.Instance?.bodyCollider;

            base.OnEnable();
        }

        private void Update()
        {
            if (toTrack == null)
                return;

            transform.position = toTrack.position + toTrack.TransformDirection(forwardTrackingPosition);
            transform.rotation = toTrack.rotation * trackingRotation;
        }

        protected override void OnAnimationFinished()
        {
            base.OnAnimationFinished();
            mainSwitcher.OnInit();
            selectionPanel.OnInit();
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }
    }
}
