using UnityEngine;

// Taken from: https://answers.unity.com/questions/393992/custom-inspector-multi-select-enum-dropdown.html

public class BitMaskAttribute : PropertyAttribute
{
    public System.Type propType;
    public BitMaskAttribute(System.Type aType)
    {
        propType = aType;
    }
}
