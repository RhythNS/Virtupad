using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Displays a percentage as a slider.
/// </summary>
public class PercentageAsSlider : PercentageAs
{
    [SerializeField] private Slider slider;

    public override void UpdateValue(float value)
    {
        slider.value = Mathf.Clamp(value, 0.0f, 1.0f);
    }
}
