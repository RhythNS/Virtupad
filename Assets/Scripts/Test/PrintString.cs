using UnityEngine;

public class PrintString : MonoBehaviour
{
    [SerializeField] private string toPrint;

    public void Print() => Debug.Log(toPrint);
}
