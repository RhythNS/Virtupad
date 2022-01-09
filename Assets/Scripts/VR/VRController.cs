using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

namespace Virtupad
{
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

        [SerializeField] private Rigidbody[] bodiesToMove;

        public float playerHeight = 1.9f;

        /*
        public static readonly float movementSpeed = 1.0f;
        public static readonly float invMovementSpeed = 1.0f / movementSpeed;
        public static readonly float rotatingSpeed = 0.01f;
         */
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
            player = GlobalsDict.Instance.Player.transform;
            bodiesToMove = new Rigidbody[]
            {
                GetComponent<Rigidbody>(),
                Player.instance.leftHand.GetComponent<HandPhysics>().handCollider.GetComponent<Rigidbody>(),
                Player.instance.rightHand.GetComponent<HandPhysics>().handCollider.GetComponent<Rigidbody>()
            };

            SaveGame saveGame = SaveFileManager.Instance.saveGame;
            ChangeSpeed(saveGame.playerMovePerSecond, saveGame.playerRotatePerSecond, saveGame.playerMoveType);
        }

        public void ChangeSpeed(float movementSpeed, float rotationSpeed, int movementType)
        {
            walkingSpeed = movementSpeed;
            InvMovementSpeed = 1.0f/ movementSpeed;
            anglesPerSecond = rotationSpeed;
            secondaryWalkingDirectionInputOption = Mathf.Clamp(movementType, 1, 2);
        }

        public void SizeToModelHeight(float modelHeight)
        {
            float toScale = modelHeight / playerHeight;
            transform.localScale = new Vector3(toScale, toScale, toScale);
        }

        /*
        private void Update()
        {
            //GetComponentInChildren<CharacterController>().Move(new Vector3(hmdTransform.position.x, hmdTransform.position.y, hmdTransform.position.z));
        }
         */

        private void FixedUpdate()
        {
            Vector2 walkingDirection2D = primaryWalkingDirectionInput.axis;
            Vector3 walkingDirection = new Vector3(walkingDirection2D.x, 0, walkingDirection2D.y);

            if (secondaryWalkingDirectionInputOption == 1)
                walkingDirection = Vector3.Scale(leftHand.transform.rotation * walkingDirection, new Vector3(1, 0, 1)).normalized;
            else if (secondaryWalkingDirectionInputOption == 2)
                walkingDirection = Vector3.Scale(head.rotation * walkingDirection, new Vector3(1, 0, 1)).normalized;

            Vector2 lookingDirection = lookingInput.axis;
            float angle = lookingDirection.x * anglesPerSecond * Time.fixedDeltaTime;
            Quaternion additionalRotation = Quaternion.AngleAxis(angle, Vector3.up);

            Vector3 additionalVelocity = walkingDirection * (walkingSpeed * Time.fixedDeltaTime);
            for (int i = 0; i < bodiesToMove.Length; i++)
            {
                bodiesToMove[i].MovePosition(bodiesToMove[i].position + additionalVelocity);
                bodiesToMove[i].MoveRotation(bodiesToMove[i].rotation * additionalRotation);
            }

            // transform.position += additionalVelocity;
            // player.GetComponentInChildren<Rigidbody>().velocity = Vector3.Scale(player.GetComponentInChildren<Rigidbody>().velocity, new Vector3(0, 1, 0)) + additionalVelocity;
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
