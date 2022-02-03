using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

        [SerializeField] private Toggle rotationLock;

        private Transform toTrack;
        private Vector3 forwardVec;
        // private Transform toTrackRot;

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

        private void Update()
        {
            if (toTrack == null)
                return;

            if (rotationLock.isOn == false)
                transform.position = toTrack.position + forwardVec;
        }

        protected override void OnShowingAnimationStarting()
        {
            toTrack = VRController.Instance?.bodyCollider;
            Quaternion rot = Quaternion.AngleAxis(VRController.Instance.head.rotation.eulerAngles.y, Vector3.up);
            transform.rotation = rot * trackingRotation;
            forwardVec = rot * forwardTrackingPosition;
            transform.position = toTrack.position + forwardVec;
        }

        protected override void OnShowingAnimationFinished()
        {
            base.OnShowingAnimationFinished();
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
