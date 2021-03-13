using UnityEditor;
using UnityEngine;

public class SerializableUtil : MonoBehaviour
{
    public static void ArraySwapElement(SerializedProperty array, int index1, int index2, bool applyChanges = true)
    {
        Object prop1 = array.GetArrayElementAtIndex(index1).objectReferenceValue;
        array.GetArrayElementAtIndex(index1).objectReferenceValue = array.GetArrayElementAtIndex(index2).objectReferenceValue;
        array.GetArrayElementAtIndex(index2).objectReferenceValue = prop1;

        if (applyChanges)
            array.serializedObject.ApplyModifiedProperties();
    }

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
