using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRMapper : MonoBehaviour
{
    public Transform PositionTransform;

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

    public void StartAutoSetup()
    {
        StartCoroutine(AutoSetup());
    }

    private IEnumerator AutoSetup()
    {
        yield return new WaitForSeconds(3.0f);
        VRSetTracker.RegisterAutoAssignTrackers();

    }

    private void Update()
    {
        for (int i = 0; i < Maps.Count; i++)
        {
            // do the thing
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

}
