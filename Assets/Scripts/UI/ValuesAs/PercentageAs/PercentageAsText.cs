using TMPro;
using UnityEngine;

/// <summary>
/// Displays a percentage as a tmp text.
/// </summary>
public class PercentageAsText : PercentageAs
{
    [SerializeField] private TMP_Text percentageText;
    [SerializeField] private string appendToValue = "%";

    public override void UpdateValue(float value)
    {
        value = Mathf.Clamp(value, 0.0f, 1.0f);
        percentageText.text = Mathf.RoundToInt(value * 100).ToString() + appendToValue;
    }
}
