using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class VRController : MonoBehaviour
{
    public static VRController Instance { get; private set; }

    public Transform head;
    public Hand leftHand, rightHand;
    private Transform player;

    public List<VRTracker> trackers = new List<VRTracker>();

    public Transform SteamVRObjects => steamVRObjects;
    [SerializeField] private Transform steamVRObjects;

    [SerializeField] private SteamVR_Action_Vector2 positionInput;
    [SerializeField] private SteamVR_Action_Vector2 lookingInput;

    public float playerHeight = 1.9f;

    private readonly float movementSpeed = 1.0f;
    private readonly float rotatingSpeed = 1.0f;

    private void Awake()
    {
        if (Instance)
        {
            Debug.LogWarning("VRDevicesDict already in scene. Deleting myself!");
            Destroy(this);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        player = GlobalsDict.Instance.Player.transform;
    }

    public void SizeToModelHeight(float modelHeight)
    {
        float toScale = modelHeight / playerHeight;
        transform.localScale = new Vector3(toScale, toScale, toScale);
    }

    private void FixedUpdate()
    {
        Vector2 axis = positionInput.axis;
        Vector3 movement = new Vector3(axis.x, 0.0f, axis.y);
        movement = head.rotation * movement;
        movement.y = 0.0f;
        movement.Normalize();
        transform.position += movementSpeed * Time.fixedDeltaTime * movement;

        axis = lookingInput.axis;
        player.rotation *= Quaternion.Euler(0.0f, axis.x, 0.0f);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}