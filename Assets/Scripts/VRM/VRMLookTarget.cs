using System.Collections;
using UnityEngine;

namespace Virtupad
{
    public class VRMLookTarget : MonoBehaviour
    {
        private enum State
        {
            Forward,
            Camera,
            Interacter,
            Gaze
        }

        [SerializeField] private float gazeDistance = 4.0f;
        [SerializeField] private Vector2 gazeAngleSize = new Vector2(3.0f, 5.0f);

        [SerializeField] private float minTimeNoGaze = 8.0f;
        [SerializeField] private float gazeChanceIncreasePerSecond = 0.1f;
        [SerializeField] private Vector2 gazeDuration = new Vector2(1.0f, 2.0f);

        [SerializeField] private float maxAngle = 80.0f;

        private State CurrentState { get => currentState; set => currentState = value; }
        [SerializeField] private State currentState = State.Forward;

        private Interacter interacter;
        private Transform head;

        private ExtendedCoroutine currentRoutine;
        private float gazeTimer = 0;

        private void Start()
        {
            head = VRMController.Instance.Animator.GetBoneTransform(HumanBodyBones.Head);

            InteracterEvents.Instance.OnInteractBegin += OnInteractBegin;
            currentRoutine = new ExtendedCoroutine(this, LookForward());
        }

        private void OnInteractBegin()
        {
            foreach (Interacter item in GlobalsDict.Instance.Interacters)
            {
                if (item.enabled == false)
                    continue;

                interacter = item;
                return;
            }
        }

        private void LateUpdate()
        {
            if (HandleGaze() == true)
                return;

            if (HandleInteracter() == true)
                return;

            if (HandleCamera() == true)
                return;

            HandleForward();
        }

        private bool HandleGaze()
        {
            if (CurrentState == State.Gaze)
                return currentRoutine.IsFinshed == false;

            gazeTimer += Time.deltaTime;
            if (gazeTimer < minTimeNoGaze)
                return false;

            if (Random.value > (gazeTimer - minTimeNoGaze * gazeChanceIncreasePerSecond))
                return false;

            gazeTimer = 0.0f;
            CurrentState = State.Gaze;
            currentRoutine.Stop();
            currentRoutine = new ExtendedCoroutine(this, RandomGaze());
            return true;
        }

        private bool HandleInteracter()
        {
            if (interacter == null)
                return false;

            if (AngleInRange(interacter.ImpactPoint) == false)
                return false;

            if (CurrentState == State.Interacter)
                return true;

            CurrentState = State.Interacter;
            currentRoutine.Stop();
            currentRoutine = new ExtendedCoroutine(this, LookAtInteraction());
            return true;
        }

        private bool HandleCamera()
        {
            StudioCamera activeCamera = StudioCameraManager.Instance?.ActiveCamera;
            if (!activeCamera)
                return false;

            if (AngleInRange(activeCamera.transform.position) == false)
                return false;

            if (CurrentState == State.Camera)
            {
                if (currentRoutine.IsFinshed == true)
                    return false;

                return true;
            }

            CurrentState = State.Camera;
            currentRoutine.Stop();
            currentRoutine = new ExtendedCoroutine(this, LookAtCamera());
            return true;
        }

        private void HandleForward()
        {
            if (CurrentState == State.Forward)
                return;

            CurrentState = State.Forward;
            currentRoutine.Stop();
            currentRoutine = new ExtendedCoroutine(this, LookForward());
        }

        private bool AngleInRange(Vector3 to) =>
            Vector3.Angle(head.forward, (to - head.position).normalized) < maxAngle;

        private IEnumerator LookForward()
        {
            while (true)
            {
                transform.position = head.position + (head.forward * 2.0f);
                yield return null;
            }
        }

        private IEnumerator LookAtCamera()
        {
            while (true)
            {
                StudioCamera active = StudioCameraManager.Instance?.ActiveCamera;
                if (active == null)
                    yield break;

                transform.position = active.transform.position;

                yield return null;
            }
        }

        private IEnumerator LookAtInteraction()
        {
            while (true)
            {
                if (interacter == null)
                    yield break;

                transform.position = interacter.ImpactPoint;

                yield return null;
            }
        }

        private IEnumerator RandomGaze()
        {
            Vector3 distance = Vector3.forward * gazeDistance;
            float randAngle = Random.Range(-180.0f, 180.0f);
            Vector3 adder = Quaternion.AngleAxis(randAngle, Vector3.forward) * 
                (Vector3.up * Random.Range(gazeAngleSize.x, gazeAngleSize.y));
            Vector3 target = distance + adder;

            float duration = Random.Range(gazeDuration.x, gazeDuration.y);

            while (true)
            {
                transform.localPosition = transform.parent.InverseTransformPoint(head.position) + target;
                yield return null;
                duration -= Time.deltaTime;
                if (duration < 0)
                    yield break;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, 0.1f);
        }

        private void OnDestroy()
        {
            if (InteracterEvents.Instance)
                InteracterEvents.Instance.OnInteractBegin -= OnInteractBegin;
        }
    }
}
