using System.Collections.Generic;
using UnityEngine;

namespace Virtupad
{
    public class UIRoot : UIPanel
    {
        [SerializeField] private Vector3 normalScale;
        [SerializeField] private Vector3 closingScale = new Vector3(0.01f, 0.01f, 0.01f);
        [SerializeField] private float animationTime = 0.3f;

        [SerializeField] private Vector3 forwardTrackingPosition = new Vector3(0.0f, 1.0f, 1.5f);
        [SerializeField] private Quaternion trackingRotation;

        private ExtendedCoroutine hideOrShowingCoroutine;
        private Transform toTrack;

        private void Awake()
        {
            trackingRotation = transform.rotation;
            normalScale = transform.localScale;
            transform.localScale = closingScale;
            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            toTrack = VRController.Instance?.transform;

            if (toTrack == null)
                return;
        }

        private void Update()
        {
            if (toTrack == null)
                return;

            transform.position = toTrack.position + toTrack.TransformDirection(forwardTrackingPosition);
            transform.rotation = toTrack.rotation * trackingRotation;
        }

        public override void LooseFocus(bool closing)
        {
            if (hideOrShowingCoroutine != null && hideOrShowingCoroutine.IsFinshed == false)
                hideOrShowingCoroutine.Stop(false);

            hideOrShowingCoroutine = new ExtendedCoroutine(this,
                EnumeratorUtil.ScaleInSecondsCurve
                    (transform, closingScale, CurveDict.Instance.UIOutAnimation, animationTime),
                OnHidden,
                true
                );
        }

        public override void RegainFocus(UIRegainFocusMessage msg)
        {
            gameObject.SetActive(true);

            if (hideOrShowingCoroutine != null && hideOrShowingCoroutine.IsFinshed == false)
                hideOrShowingCoroutine.Stop(false);

            hideOrShowingCoroutine = new ExtendedCoroutine(this,
                EnumeratorUtil.ScaleInSecondsCurve
                    (transform, normalScale, CurveDict.Instance.UIInAnimation, animationTime),
                OnAnimationFinished, true
                );
        }

        private void OnAnimationFinished()
        {
            OnInit();
        }

        private void OnHidden()
        {
            gameObject.SetActive(false);

            List<Interacter> interacters = GlobalsDict.Instance.Interacters;
            for (int i = 0; i < interacters.Count; i++)
                interacters[i].StopRequest();
        }
    }
}
