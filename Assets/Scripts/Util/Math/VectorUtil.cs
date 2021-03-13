using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class VectorUtil
{
    public static Vector3Int FloorVectorToInt(in Vector3 input)
        => new Vector3Int(Mathf.FloorToInt(input.x), Mathf.FloorToInt(input.y), Mathf.FloorToInt(input.z));

}
