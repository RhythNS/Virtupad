using UnityEngine;
using Valve.VR.InteractionSystem;

public class GlobalsDict : MonoBehaviour
{
    public static GlobalsDict Instance { get; private set; }

    public Player Player => player;
    [SerializeField] private Player player;

    private void Awake()
    {
        if (Instance)
        {
            Debug.LogWarning("GlobalsDict already in scene. Deleting myself!");
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
