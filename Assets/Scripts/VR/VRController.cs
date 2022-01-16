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
        public float WalkingSpeed => walkingSpeed;
        [SerializeField] private float walkingSpeed;
        public float AnglesPerSecond => anglesPerSecond;
        [SerializeField] private float anglesPerSecond = 90.0f;

        private List<InputListener> walkingOverwrites = new List<InputListener>();
        private List<InputListener> rotationOverwrites = new List<InputListener>();

        [SerializeField] private Rigidbody[] bodiesToMove;
        [SerializeField] private HandPhysics[] handsToMove;

        private bool isCustomPlayerHeight = false;
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
             //   Player.instance.leftHand.GetComponent<HandPhysics>().handCollider.GetComponent<Rigidbody>(),
          //      Player.instance.rightHand.GetComponent<HandPhysics>().handCollider.GetComponent<Rigidbody>()
            };

            handsToMove = new HandPhysics[]
            {
                leftHand.GetComponent<HandPhysics>(),
                rightHand.GetComponent<HandPhysics>()
            };

            SaveGame saveGame = SaveFileManager.Instance.saveGame;
            ChangeSpeed(saveGame.playerMovePerSecond, saveGame.playerRotatePerSecond, saveGame.playerMoveType);

            VRMController.onVRMCreated += OnVRMCreated;
            VRMController.onVRMDeleted += OnVRMDeleted;
        }

        private void OnVRMDeleted(VRMController newController)
        {
            leftHand.ShowController(true);
            rightHand.ShowController(true);
        }

        private void OnVRMCreated(VRMController newController)
        {
            leftHand.HideController(true);
            rightHand.HideController(true);
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

        public void ChangeSpeed(float movementSpeed = -1.0f, float rotationSpeed = -1.0f, int movementType = 1)
        {
            SaveGame sg = SaveFileManager.Instance.saveGame;

            if (movementSpeed != -1.0f)
            {
                sg.playerMovePerSecond = walkingSpeed = movementSpeed;
                InvMovementSpeed = 1.0f / movementSpeed;
            }

            if (rotationSpeed != -1.0f)
                sg.playerRotatePerSecond = anglesPerSecond = rotationSpeed;

            if (movementType != -1)
                sg.playerMoveType = secondaryWalkingDirectionInputOption = Mathf.Clamp(movementType, 1, 2);

            SaveFileManager.Instance.Save();
        }

        public void AutoSetPlayerHeight()
        {

            Vector3 headDir = head.transform.position - Player.instance.transform.position;
            headDir.x = 0.0f;
            headDir.z = 0.0f;
            playerHeight = headDir.magnitude;
        }

        public void SizeToModelHeight(float modelHeight)
        {
            transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

            if (isCustomPlayerHeight == false)
                AutoSetPlayerHeight();

            float toScale = modelHeight / playerHeight;
            Debug.Log("s:" + toScale + ", m:" + modelHeight + ", p:" + playerHeight);
            transform.localScale = new Vector3(toScale, toScale, toScale);
        }

        private void FixedUpdate()
        {
            float additionalRotation = GetRotation();
            Vector3 additionalVelocity = GetWalkingVelocity();

            Quaternion q = Quaternion.AngleAxis(additionalRotation, Vector3.up);

            //.RotateAround(new Vector3(HeadTransform.position.x, 0, HeadTransform.position.z), Vector3.up, 45.0f);
            for (int i = 0; i < bodiesToMove.Length; i++)
            {
                bodiesToMove[i].MovePosition(bodiesToMove[i].position + additionalVelocity);
                //  bodiesToMove[i].MoveRotation(bodiesToMove[i].rotation * additionalRotation);
                bodiesToMove[i].MovePosition(q * (bodiesToMove[i].position - bodyCollider.position) + bodyCollider.position);
                bodiesToMove[i].MoveRotation(bodiesToMove[i].transform.rotation * q);
            }

            for (int i = 0; i < handsToMove.Length; i++)
                handsToMove[i].UpdateHand(default, default);

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

        private float GetRotation()
        {
            for (int i = 0; i < rotationOverwrites.Count; i++)
            {
                if (rotationOverwrites[i].Invoke(lookingInput) == true)
                    return 0.0f;
            }

            Vector2 lookingDirection = lookingInput.axis;
            float angle = lookingDirection.x * anglesPerSecond * Time.fixedDeltaTime;
            return angle;

            //return Quaternion.AngleAxis(angle, Vector3.up);
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;

            VRMController.onVRMCreated -= OnVRMCreated;
            VRMController.onVRMDeleted -= OnVRMDeleted;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + transform.forward);
        }
    }
}
