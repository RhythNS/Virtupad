using UnityEngine;

namespace Virtupad
{
    public class PrintString : MonoBehaviour
    {
        [SerializeField] private string toPrint;

        public void Print() => Debug.Log(toPrint);
    }
}
