using System;
using System.Collections;
using UnityEngine;

public class ExtendedCoroutine
{
    public bool IsFinshed { get; private set; } = false;
    public Coroutine Coroutine { get; private set; }

    private readonly IEnumerator enumerator;
    private readonly Action onFinished;
    private readonly MonoBehaviour onScript;

    public ExtendedCoroutine(MonoBehaviour onScript, IEnumerator enumerator, Action onFinished = null, bool startNow = false)
    {
        this.onScript = onScript;
        this.enumerator = enumerator;
        this.onFinished = onFinished;

        if (startNow)
            Start();
    }

    public void Start()
    {
        Coroutine = onScript.StartCoroutine(InnerEnumerator());
    }

    private IEnumerator InnerEnumerator()
    {
        while (enumerator.MoveNext())
        {
            yield return enumerator.Current;
        }
        IsFinshed = true;
        if (onFinished != null)
            onFinished.Invoke();
    }

    public void Stop(bool invokeOnFinished = true)
    {
        if (Coroutine != null)
            onScript.StopCoroutine(Coroutine);
        if (invokeOnFinished && onFinished != null)
            onFinished.Invoke();
    }

    public static ExtendedCoroutine ActionAfterSeconds(MonoBehaviour onScript, float seconds, Action onFinished, bool startNow = false)
        => new ExtendedCoroutine(onScript, EnumeratorUtil.WaitForSeconds(seconds), onFinished, startNow);
}
