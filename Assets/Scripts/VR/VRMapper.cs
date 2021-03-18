using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRMapper : MonoBehaviour
{
    public bool IsFullBody { get; private set; } = false;
    public static VRMapper Instance { get; private set; }

    [SerializeField] private List<MapTransform> maps = new List<MapTransform>();


    [System.Serializable]
    public struct MapTransform
    {
        public Transform source, constrain;
        public Vector3 offsetPos;
        public Vector3 offsetRot;

        public MapTransform(Transform constrain, Transform source, Vector3 offsetPos, Vector3 offsetRot)
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

    public void AddMap(Transform constrain, Transform source, bool useOffset = true)
    {
        for (int i = 0; i < maps.Count; i++)
        {
            if (maps[i].constrain == constrain || maps[i].source == source)
            {
                throw new System.Exception(source.name + " or " + constrain.name + " is already mapped!");
            }
        }

        if (useOffset == false)
        {
            maps.Add(new MapTransform(constrain, source, Vector3.zero, Vector3.zero));
        }
        else
        {
            Vector3 offsetPos = constrain.position - source.position;
            maps.Add(new MapTransform(constrain, source, offsetPos, constrain.rotation.eulerAngles));
        }
    }

    private void Update()
    {
        for (int i = 0; i < maps.Count; i++)
        {
            maps[i].constrain.position = maps[i].source.position + maps[i].offsetPos;
            maps[i].constrain.rotation = maps[i].source.rotation * Quaternion.Euler(maps[i].offsetRot);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
