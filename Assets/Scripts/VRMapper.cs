using UnityEngine;

public class VRMapper : MonoBehaviour
{
    public Transform PositionTransform;
    public bool IsFullBody { get; private set; } = false;

    public static VRMapper Instance { get; private set; }

    private void Awake()
    {
        if (Instance)
        {
            Debug.LogWarning("VRMapper already in scene. Deleting myself!");
            Destroy(this);
            return;
        }
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

}
