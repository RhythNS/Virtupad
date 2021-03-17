using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRMapper : MonoBehaviour
{
    public bool IsFullBody { get; private set; } = false;
    public static VRMapper Instance { get; private set; }
    public List<MapTransform> Maps { get; private set; } = new List<MapTransform>();


    [System.Serializable]
    public struct MapTransform
    {
        public Transform source, constrain;
        public Vector3 offsetPos;
        public Quaternion offsetRot;

        public MapTransform(Transform source, Transform constrain, Vector3 offsetPos, Quaternion offsetRot)
        {
            this.source = source;
            this.constrain = constrain;
            this.offsetPos = offsetPos;
            this.offsetRot = offsetRot;
        }
    }

    private void Awake()
    {
        if (Instance)
        {
            Debug.LogWarning("VRMapper already in scene. Deleting myself!");
            Destroy(this);
            return;
        }
        Instance = this;
    }

    public void AddMap(Transform source, Transform constrain)
    {
        for (int i = 0; i < Maps.Count; i++)
        {
            if (Maps[i].constrain == constrain || Maps[i].source == source)
            {
                throw new System.Exception(source.name + " or " + constrain.name + " is already mapped!");
            }
        }

        Vector3 offsetPos = constrain.position - source.position;
        Quaternion offsetRot = constrain.rotation * Quaternion.Inverse(source.rotation);
        Maps.Add(new MapTransform(source, constrain, offsetPos, offsetRot));
    }

    private void Update()
    {
        for (int i = 0; i < Maps.Count; i++)
        {
            Maps[i].constrain.position = Maps[i].source.position + Maps[i].offsetPos;
            Maps[i].constrain.rotation = Maps[i].offsetRot * Maps[i].source.rotation;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
