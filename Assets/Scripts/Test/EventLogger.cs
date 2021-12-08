using CrazyMinnow.SALSA;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CrazyMinnow.SALSA.EventController;

public class EventLogger : MonoBehaviour
{
    private void Start()
    {
        EventController.AnimationStarting += EventController_AnimationStarting; ;
        EventController.AnimationON += EventController_AnimationON;
        EventController.AnimationOFF += EventController_AnimationOFF;
        EventController.AnimationEnding += EventController_AnimationEnding;
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
