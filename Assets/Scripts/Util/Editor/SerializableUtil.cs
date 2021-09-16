using UnityEditor;
using UnityEngine;

/// <summary>
/// Helper class for methods related to the unity serializable system.
/// </summary>
public class SerializableUtil : MonoBehaviour
{
    /// <summary>
    /// Swaps two array elements.
    /// </summary>
    /// <param name="array">The array as property.</param>
    /// <param name="index1">The index of the first item to be swapped.</param>
    /// <param name="index2">The index of the second item to be swapped.</param>
    /// <param name="applyChanges">Wheter to instanly apply the changes to the property.</param>
    public static void ArraySwapElement(SerializedProperty array, int index1, int index2, bool applyChanges = true)
    {
        Object prop1 = array.GetArrayElementAtIndex(index1).objectReferenceValue;
        array.GetArrayElementAtIndex(index1).objectReferenceValue = array.GetArrayElementAtIndex(index2).objectReferenceValue;
        array.GetArrayElementAtIndex(index2).objectReferenceValue = prop1;

        if (applyChanges)
            array.serializedObject.ApplyModifiedProperties();
    }

    /// <summary>
    /// Removes an element from an array.
    /// </summary>
    /// <param name="array">The array as property.</param>
    /// <param name="index">The index of the item to be removed.</param>
    /// <param name="applyChanges">Wheter to instanly apply the changes to the property.</param>
    public static void ArrayRemoveAtIndex(SerializedProperty array, int index, bool applyChanges = true)
    {
        if (array.arraySize - 1 != index)
        {
            for (int i = index + 1; i < array.arraySize; i++)
            {
                array.MoveArrayElement(i, i - 1);
            }
        };
        array.arraySize--;

        if (applyChanges)
            array.serializedObject.ApplyModifiedProperties();
    }

}
