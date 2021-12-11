using CrazyMinnow.SALSA;
using UnityEngine;
using static CrazyMinnow.SALSA.EventController;

public class EventLogger : MonoBehaviour
{
    private void Start()
    {
        AnimationStarting += EventController_AnimationStarting; ;
        AnimationON += EventController_AnimationON;
        AnimationOFF += EventController_AnimationOFF;
        AnimationEnding += EventController_AnimationEnding;
    }

    private void EventController_AnimationEnding(object sender, EventControllerNotificationArgs e)
    {
        Debug.Log("ending: " + e.eventName);
    }

    private void EventController_AnimationStarting(object sender, EventControllerNotificationArgs e)
    {
        Debug.Log("starting: " + e.eventName);
    }

    private void EventController_AnimationOFF(object sender, EventControllerNotificationArgs e)
    {
        Debug.Log("off: " + e.eventName);
    }

    private void EventController_AnimationON(object sender, EventControllerNotificationArgs e)
    {
        Debug.Log("on: " + e.eventName);
    }

    public void Test(EventControllerNotificationArgs args)
    {
        Debug.Log(args.eventName);
    }
}
