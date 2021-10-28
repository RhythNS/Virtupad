using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClickUIElementWithRaycastTest : MonoBehaviour
{
    [SerializeField] private Vector3 direction;
    [SerializeField] private float distance;

    //https://github.com/Sergey-Shamov/Unity-VR-InputModule/blob/master/VRControllerInputModule.cs

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Ray ray = new Ray(transform.position, direction);
            if (Physics.Raycast(ray, out RaycastHit hit, distance, 1 << 5) == false
                || hit.collider.TryGetComponent(out RectTransform trans) == false)
                return;

            Debug.Log("Hit " + hit.collider.name);

            /*
            hit.collider.TryGetComponent(out GraphicRaycaster canvas);
            UnityEngine.EventSystems.PointerEventData eventData = new UnityEngine.EventSystems.PointerEventData(EventSystem.current);
            eventData.button = PointerEventData.InputButton.Left;
            
            Vector2 hitPoint = trans.InverseTransformPoint(hit.point);
            Vector2 size = trans.sizeDelta;
            eventData.position = hitPoint + (size * 0.5f);
            eventData.eligibleForClick = true;
            ExecuteEvents.ExecuteHierarchy(hit.collider.gameObject, eventData, ExecuteEvents.pointerDownHandler);
             */
        }
    }



    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + (direction.normalized * distance));
    }
}
