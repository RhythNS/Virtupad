using UnityEngine;

public class PrintLayer : MonoBehaviour
{
    [SerializeField] private GameObject target;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
            Debug.Log(target.layer);
    }
}
