using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace Virtupad
{
    public class UIComplexInteractable : UIInteractable
    {
        private Vector2 prevPos = default;
        [SerializeField] private Canvas canvas;
        [SerializeField] private GraphicRaycaster caster;

        protected override void OnBeginHover()
        {
            PointerEventData ped = GetPED(prevPos);

            ExecuteEvents.Execute(canvas.gameObject, ped, ExecuteEvents.pointerEnterHandler);


            //            element.Select();
            // UIEventThrower.GameobjectUIEvent(gameObject, ExecuteEvents.pointerEnterHandler);
        }

        protected override void OnLeaveHover()
        {
            PointerEventData ped = GetPED(prevPos);

            ExecuteEvents.Execute(canvas.gameObject, ped, ExecuteEvents.pointerExitHandler);

            //    element.DeSelect();
            // UIEventThrower.GameobjectUIEvent(gameObject, ExecuteEvents.pointerExitHandler);
        }

        public override void Select()
        {
            PointerEventData ped = GetPED(prevPos);

            ExecuteEvents.Execute(canvas.gameObject, ped, ExecuteEvents.pointerClickHandler);

            //   element.OnEvent(ExecuteEvents.pointerClickHandler);
            // UIEventThrower.GameobjectUIEvent(gameObject, ExecuteEvents.pointerClickHandler);
        }

        protected override void OnStayHover(Vector3 impactPoint)
        {
            PointerEventData ped = GetPED(impactPoint);

            ExecuteEvents.Execute(canvas.gameObject, ped, ExecuteEvents.updateSelectedHandler);

            List<RaycastResult> res = new List<RaycastResult>();
            caster.Raycast(ped, res);

            Debug.Log(string.Join(Environment.NewLine, res));
        }

        private PointerEventData GetPED(Vector3 impactPoint)
        {
            Vector2 local = canvas.transform.InverseTransformPoint(impactPoint);

            PointerEventData ped = new PointerEventData(EventSystem.current)
            {
                position = local,
                delta = local - prevPos
            };

            prevPos = local;
            return ped;
        }
    }
}
