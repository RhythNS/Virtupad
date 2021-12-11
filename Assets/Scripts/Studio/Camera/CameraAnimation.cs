using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAnimation : MonoBehaviour
{
    [System.Serializable]
    public class Keyframe
    {
        public Vector3 position;
        public Quaternion rotation;
    }

    public List<Keyframe> keys = new List<Keyframe>();


}
