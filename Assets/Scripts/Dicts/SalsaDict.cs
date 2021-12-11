using UnityEngine;

public class SalsaDict : MonoBehaviour
{
    public static SalsaDict Instance { get; private set; }

    public float AnimationStartDuration => animationStartDuration;
    [SerializeField] private float animationStartDuration;

    public float AnimationEndDuration => animationEndDuration;
    [SerializeField] private float animationEndDuration;

    public AudioClip EmptyClip => emptyClip;
    [SerializeField] private AudioClip emptyClip;

    private void Awake()
    {
        if (Instance)
        {
            Debug.LogWarning("SalsaDict already in scene. Deleting myself!");
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
