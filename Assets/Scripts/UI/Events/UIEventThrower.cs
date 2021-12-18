using UnityEngine;
using UnityEngine.EventSystems;

namespace Virtupad
{
    public class UIEventThrower
    {
        public static void GameobjectUIEvent<T>(GameObject gObject, ExecuteEvents.EventFunction<T> pointerEnterHandler, PointerEventData ped = null) where T : IEventSystemHandler
        {
            if (ped == null)
                ped = GetDefaultPed();

            ExecuteEvents.Execute(gObject, ped, pointerEnterHandler);
        }

        public static PointerEventData GetDefaultPed()
        {
            PointerEventData ped = new PointerEventData(EventSystem.current)
            {
                button = PointerEventData.InputButton.Left
            };
            return ped;
        }

        public static PointerEventData GetDefaultPed(Vector3 impactPoint)
        {
            PointerEventData ped = GetDefaultPed();
            ped.position = impactPoint;
            return ped;
        }
    }
}
