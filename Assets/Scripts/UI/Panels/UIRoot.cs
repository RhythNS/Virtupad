using UnityEngine;

public class UIRoot : UIPanel
{
    [SerializeField] private Vector3 normalScale;
    [SerializeField] private Vector3 closingScale = new Vector3(0.01f, 0.01f, 0.01f);
    [SerializeField] private float animationTime = 0.3f;

    private ExtendedCoroutine hideOrShowingCoroutine;

    private void Awake()
    {
        normalScale = transform.localScale;
        transform.localScale = closingScale;
        gameObject.SetActive(false);
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
            startNow: true
            );
    }

    private void OnHidden()
    {
        gameObject.SetActive(false);
    }
}
