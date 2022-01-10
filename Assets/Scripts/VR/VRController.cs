using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

namespace Virtupad
{
    public delegate bool InputListener(SteamVR_Action_Vector2 input);

    public class VRController : MonoBehaviour
    {
        public static VRController Instance { get; private set; }

        public Transform head;
        public Hand leftHand, rightHand;

        public Transform leftHandAttachment;
        public Transform rightHandAttachment;

        public Transform bodyCollider;
        private Transform player;

        public List<VRTracker> trackers = new List<VRTracker>();

        public Transform SteamVRObjects => steamVRObjects;
        [SerializeField] private Transform steamVRObjects;

        [SerializeField] private SteamVR_Action_Vector2 primaryWalkingDirectionInput;

        // 1 = Left Controller; 2 = HMD
        [Range(1, 2)] public int secondaryWalkingDirectionInputOption;
        [SerializeField] private SteamVR_Action_Vector2 lookingInput;
        [SerializeField] private float walkingSpeed;
        [SerializeField] private float anglesPerSecond = 90.0f;

        private List<InputListener> walkingOverwrites = new List<InputListener>();
        private List<InputListener> rotationOverwrites = new List<InputListener>();

        [SerializeField] private Rigidbody[] bodiesToMove;

        public float playerHeight = 1.9f;

        public static float InvMovementSpeed { get; private set; }

        private void Awake()
        {
            if (Instance)
            {
                Debug.LogWarning("VRDevicesDict already in scene. Deleting myself!");
                Destroy(this);
                return;
            }
            Instance = this;
            InvMovementSpeed = 1.0f / walkingSpeed;
        }

        private void Start()
        {
            player = Player.instance.transform;
            bodiesToMove = new Rigidbody[]
            {
                GetComponent<Rigidbody>(),
                Player.instance.leftHand.GetComponent<HandPhysics>().handCollider.GetComponent<Rigidbody>(),
                Player.instance.rightHand.GetComponent<HandPhysics>().handCollider.GetComponent<Rigidbody>()
            };

            SaveGame saveGame = SaveFileManager.Instance.saveGame;
            ChangeSpeed(saveGame.playerMovePerSecond, saveGame.playerRotatePerSecond, saveGame.playerMoveType);
        }

        public void RegisterWalking(InputListener listener) => Register(listener, walkingOverwrites);
        public void RegisterRotation(InputListener listener) => Register(listener, rotationOverwrites);

        private void Register(InputListener listener, List<InputListener> onList)
        {
            if (onList.Contains(listener) == false)
                onList.Add(listener);
        }

        public void DeRegisterWalking(InputListener listener) => DeRegister(listener, walkingOverwrites);
        public void DeRegisterRotation(InputListener listener) => DeRegister(listener, rotationOverwrites);

        private void DeRegister(InputListener listener, List<InputListener> onList) => onList.Remove(listener);

        public void ChangeSpeed(float movementSpeed, float rotationSpeed, int movementType)
        {
            walkingSpeed = movementSpeed;
            InvMovementSpeed = 1.0f / movementSpeed;
            anglesPerSecond = rotationSpeed;
            secondaryWalkingDirectionInputOption = Mathf.Clamp(movementType, 1, 2);
        }

        public void SizeToModelHeight(float modelHeight)
        {
            float toScale = modelHeight / playerHeight;
            transform.localScale = new Vector3(toScale, toScale, toScale);
        }

        private void FixedUpdate()
        {
            Quaternion additionalRotation = GetRotation();
            Vector3 additionalVelocity = GetWalkingVelocity();
            for (int i = 0; i < bodiesToMove.Length; i++)
            {
                bodiesToMove[i].MovePosition(bodiesToMove[i].position + additionalVelocity);
                bodiesToMove[i].MoveRotation(bodiesToMove[i].rotation * additionalRotation);
            }

            // transform.position += additionalVelocity;
            // player.GetComponentInChildren<Rigidbody>().velocity = Vector3.Scale(player.GetComponentInChildren<Rigidbody>().velocity, new Vector3(0, 1, 0)) + additionalVelocity;
        }

        private Vector3 GetWalkingVelocity()
        {
            for (int i = 0; i < walkingOverwrites.Count; i++)
            {
                if (walkingOverwrites[i].Invoke(primaryWalkingDirectionInput) == true)
                    return Vector3.zero;
            }

            Vector2 walkingDirection2D = primaryWalkingDirectionInput.axis;
            Vector3 walkingDirection = new Vector3(walkingDirection2D.x, 0, walkingDirection2D.y);

            if (secondaryWalkingDirectionInputOption == 1)
                walkingDirection = Vector3.Scale(leftHand.transform.rotation * walkingDirection, new Vector3(1, 0, 1)).normalized;
            else if (secondaryWalkingDirectionInputOption == 2)
                walkingDirection = Vector3.Scale(head.rotation * walkingDirection, new Vector3(1, 0, 1)).normalized;

            return walkingDirection * (walkingSpeed * Time.fixedDeltaTime);
        }

        private Quaternion GetRotation()
        {
            for (int i = 0; i < rotationOverwrites.Count; i++)
            {
                if (rotationOverwrites[i].Invoke(lookingInput) == true)
                    return Quaternion.identity;
            }

            Vector2 lookingDirection = lookingInput.axis;
            float angle = lookingDirection.x * anglesPerSecond * Time.fixedDeltaTime;

            return Quaternion.AngleAxis(angle, Vector3.up);
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + transform.forward);
        }
    }
}
