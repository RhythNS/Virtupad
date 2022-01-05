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
        private Transform player;

        public List<VRTracker> trackers = new List<VRTracker>();

        public Transform SteamVRObjects => steamVRObjects;
        [SerializeField] private Transform steamVRObjects;

        [SerializeField] private SteamVR_Action_Vector2 primaryWalkingDirectionInput;
        
        // 1 = Left Controller; 2 = HMD
        [Range(1, 2)]public int secondaryWalkingDirectionInputOption; 
        [SerializeField] private SteamVR_Action_Vector2 lookingInput;
        [SerializeField][Range(2, 4)] private float walkingSpeed;

        public float playerHeight = 1.9f;

        public static readonly float movementSpeed = 1.0f;
        public static readonly float invMovementSpeed = 1.0f / movementSpeed;
        public static readonly float rotatingSpeed = 0.01f;

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
            walkingSpeed *= 100;
        }

        public void SizeToModelHeight(float modelHeight)
        {
            float toScale = modelHeight / playerHeight;
            transform.localScale = new Vector3(toScale, toScale, toScale);
        }

        private void Update()
        {
            //GetComponentInChildren<CharacterController>().Move(new Vector3(hmdTransform.position.x, hmdTransform.position.y, hmdTransform.position.z));
        }

        private void FixedUpdate()
        {
            
            Vector2 walkingDirection2D = primaryWalkingDirectionInput.axis;
            Vector3 walkingDirection = new Vector3(walkingDirection2D.x, 0, walkingDirection2D.y);
            if (secondaryWalkingDirectionInputOption == 1)
                walkingDirection = Vector3.Scale(leftHand.transform.rotation * walkingDirection, new Vector3(1, 0, 1));
            else if (secondaryWalkingDirectionInputOption == 2)
                walkingDirection = Vector3.Scale(head.rotation * walkingDirection, new Vector3(1, 0, 1));
            Vector3 additionalVelocity = walkingDirection * walkingSpeed * Time.deltaTime;
            player.GetComponentInChildren<Rigidbody>().velocity = Vector3.Scale(player.GetComponentInChildren<Rigidbody>().velocity, new Vector3(0, 1, 0)) + additionalVelocity;

     
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
