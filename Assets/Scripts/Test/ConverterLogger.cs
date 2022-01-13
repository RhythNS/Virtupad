using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Virtupad
{
    public class ConverterLogger : MonoBehaviour
    {
        void Start()
        {
            Quaternion inv = Quaternion.Inverse(Quaternion.Euler(new Vector3(90.0f, 0.0f, -90.0f)));
            Quaternion inv2 = Quaternion.Inverse(Quaternion.Euler(new Vector3(90.0f, 0.0f, 90.0f)));
            Debug.Log(inv.x + " " + inv.y + " " + inv.z + " " + inv.w);
            Debug.Log(inv2.x + " " + inv2.y + " " + inv2.z + " " + inv2.w);
        }
    }
}
