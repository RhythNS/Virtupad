using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class VRUIColliderCreator : MonoBehaviour
{
    private void Start()
    {
        Destroy(this);
    }
    public void AutoSize()
    {
        if (TryGetComponent(out RectTransform trans) == false)
            return;

        BoxCollider collider = GetComponent<BoxCollider>();
        if (collider == null)
            collider = gameObject.AddComponent<BoxCollider>();

        Rect rect = trans.rect;
        collider.size = rect.size;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(VRUIColliderCreator))]
public class VRUIColliderCreatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        (target as VRUIColliderCreator).AutoSize();
    }
}
#endif
