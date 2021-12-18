using UnityEngine;

namespace Virtupad
{
    public class UIAutoAddToParentFromChildIndexElement : UIPrimitiveElement
    {
        [SerializeField] private bool isHori = false;

        void Start()
        {
            autoRemoveFromParentOnDestroy = true;

            parent = transform.parent.GetComponent<UIPrimitiveElement>();
            if (parent == null)
            {
                Debug.LogWarning(gameObject.name + " ui primitive could not get parent!");
                return;
            }

            int ownIndex = -1;
            for (int i = 0; i < parent.transform.childCount; i++)
            {
                if (parent.transform.GetChild(i) == transform)
                {
                    ownIndex = i;
                    break;
                }
            }

            if (ownIndex == -1)
            {
                Debug.LogWarning(gameObject.name + " ui primitive trasnform parent is not a ui element!");
                return;
            }

            if (isHori)
                uiPos.Set(ownIndex, 0);
            else
                uiPos.Set(0, ownIndex);

            parent.AddChild(this);
            OnInit();
        }
    }
}
