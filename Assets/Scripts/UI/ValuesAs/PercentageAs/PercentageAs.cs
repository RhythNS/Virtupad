using UnityEngine;

/// <summary>
/// Abstract class for displaying a percentage as whatever.
/// </summary>
public abstract class PercentageAs : MonoBehaviour
{
    /// <summary>
    /// Called when the value changed.
    /// </summary>
    /// <param name="value">The new value.</param>
    public abstract void UpdateValue(float value);
}
