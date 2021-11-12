using UnityEngine;
using UnityEngine.EventSystems;

public class UIEventThrower
{
    public static void GameobjectUIEvent<T>(GameObject gObject, ExecuteEvents.EventFunction<T> pointerEnterHandler) where T : IEventSystemHandler
    {
        PointerEventData ped = new PointerEventData(EventSystem.current);
        ExecuteEvents.Execute(gObject, ped, pointerEnterHandler);
    }
}
