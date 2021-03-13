using System.Collections;
using UnityEngine;

public static class EnumeratorUtil
{
    public static IEnumerator WaitForSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }

    public static IEnumerator GoToInSecondsLerp(Transform transform, Vector2 position, float seconds)
    {
        Vector3 oldPos = transform.position;
        Vector3 newPos = position;
        newPos.z = oldPos.z;

        float timer = 0;
        do
        {
            timer += Time.deltaTime;
            transform.position = Vector3.Lerp(oldPos, newPos, timer / seconds);
            yield return null;
        } while (timer < seconds);
    }

    public static IEnumerator GoToInSecondsSlerp(Transform transform, Vector2 position, float seconds)
    {
        Vector3 oldPos = transform.position;
        Vector3 newPos = position;
        newPos.z = oldPos.z;

        float timer = 0;
        do
        {
            timer += Time.deltaTime;
            transform.position = Vector3.Slerp(oldPos, newPos, timer / seconds);
            yield return null;
        } while (timer < seconds);
    }
}
