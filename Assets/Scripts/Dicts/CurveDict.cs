using UnityEngine;

public class CurveDict : MonoBehaviour
{
    public static CurveDict Instance { get; private set; }

    public AnimationCurve SelectedPosCurve => selectedPosCurve;
    [SerializeField] private AnimationCurve selectedPosCurve;
    
    public AnimationCurve UIInAnimation => uiIntAnimation;
    [SerializeField] private AnimationCurve uiIntAnimation;
    
    public AnimationCurve UIOutAnimation => uiOutAnimation;
    [SerializeField] private AnimationCurve uiOutAnimation;

    private void Awake()
    {
        if (Instance)
        {
            Debug.LogWarning("CurveDict already in scene. Deleting myself!");
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
