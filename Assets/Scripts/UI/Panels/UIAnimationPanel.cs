using System.Collections.Generic;
using UnityEngine;

namespace Virtupad
{
    public class UIAnimationPanel : UIPanel
    {
        [SerializeField] protected Vector3 normalScale;
        [SerializeField] protected Vector3 closingScale = new Vector3(0.01f, 0.01f, 0.01f);
        [SerializeField] protected float animationTime = 0.3f;

        private ExtendedCoroutine hideOrShowingCoroutine;

        protected override void Awake()
        {
            base.Awake();

            normalScale = transform.localScale;
            transform.localScale = closingScale;
        }

        protected virtual void OnEnable()
        {
            OnReset();
        }

        public override void LooseFocus(bool closing)
        {
            if (gameObject.activeInHierarchy == false)
                return;

            if (hideOrShowingCoroutine != null && hideOrShowingCoroutine.IsFinshed == false)
                hideOrShowingCoroutine.Stop(false);

            OnHidingAnimationStarted();

            hideOrShowingCoroutine = new ExtendedCoroutine(this,
                EnumeratorUtil.ScaleInSecondsCurve
                    (transform, closingScale, CurveDict.Instance.UIOutAnimation, animationTime),
                OnHidden,
                true
                );
        }

        protected virtual void OnHidingAnimationStarted()
        {

        }

        protected virtual void OnShowingAnimationStarting()
        {

        }

        public override void RegainFocus(UIRegainFocusMessage msg)
        {
            gameObject.SetActive(true);

            if (hideOrShowingCoroutine != null && hideOrShowingCoroutine.IsFinshed == false)
                hideOrShowingCoroutine.Stop(false);

            OnShowingAnimationStarting();

            hideOrShowingCoroutine = new ExtendedCoroutine(this,
                EnumeratorUtil.ScaleInSecondsCurve
                    (transform, normalScale, CurveDict.Instance.UIInAnimation, animationTime),
                OnAnimationFinished, true
                );
        }

        protected virtual void OnAnimationFinished()
        {
            OnInit();
        }

        protected virtual void OnHidden()
        {
            gameObject.SetActive(false);

            List<Interacter> interacters = GlobalsDict.Instance.Interacters;
            for (int i = 0; i < interacters.Count; i++)
                interacters[i].StopRequest();
        }
    }
}
