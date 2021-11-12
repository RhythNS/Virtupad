using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMoveManager : MonoBehaviour
{
    public static UIMoveManager Instance { get; private set; }

    public UIElement SelectedElement { get; private set; }

    private void Awake()
    {
        if (Instance)
        {
            Debug.LogWarning("UIMoveManager already in scene. Deleting myself!");
            Destroy(this);
            return;
        }
        Instance = this;
    }

    public void OnElementSelected(UIElement element)
    {
        if (element == SelectedElement)
            return;

        if (SelectedElement)
            SelectedElement.Deselect();

        SelectedElement = element;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
