using TMPro;
using UnityEngine;

/// <summary>
/// Displays an integer as a tmp text.
/// </summary>
public class IntAsText : IntAs
{
    [SerializeField] private TMP_Text text;

    public override void UpdateValue(int value)
    {
        text.text = value.ToString();
    }
}
