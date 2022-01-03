using UnityEngine;

/// <summary>
/// Abstract class for displaying an integer as whatever.
/// </summary>
public abstract class IntAs : MonoBehaviour
{
    /// <summary>
    /// Called when the value changed.
    /// </summary>
    /// <param name="value">The new value.</param>
    public abstract void UpdateValue(int value);
}
