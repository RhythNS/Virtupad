using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Virtupad
{
    public class UIAnimator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        protected enum State
        {
            Idle, MovingClicked, Clicked, MovingIdle
        }

        [SerializeField] protected Vector3 localEndPoint;
        [SerializeField] protected float percentagePerSecond = 1.0f;
        [SerializeField] private UIPrimitiveElement toAnimate;

        [SerializeField] private bool manualStartPoint = false;
        [SerializeField] protected Vector3 startPoint;
        protected Vector3 endPoint;

        protected State currentState;

        [SerializeField] protected float atPercentage;

        protected ExtendedCoroutine moveCoroutine;

        protected virtual AnimationCurve GetCurve() => CurveDict.Instance.SelectedPosCurve;

        protected virtual void Awake()
        {
            if (!toAnimate)
                toAnimate = transform.GetComponent<UIPrimitiveElement>();

            endPoint = startPoint + localEndPoint;
        }

        public void SetStartAndCalcEndPoint(Vector3 startPoint)
        {
            this.startPoint = startPoint;
            endPoint = startPoint + localEndPoint;
        }

        public void AutoCalcStartAndEndPoint()
        {
            if (manualStartPoint == false && toAnimate != null)
                startPoint = toAnimate.transform.localPosition;
            endPoint = startPoint + localEndPoint;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            currentState = State.MovingClicked;
            StartMoveCoroutine();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            currentState = State.MovingIdle;
            StartMoveCoroutine();
        }

        protected void StartMoveCoroutine()
        {
            if (moveCoroutine == null || moveCoroutine.IsFinshed)
                moveCoroutine = new ExtendedCoroutine(this, Move());
        }

        protected virtual IEnumerator Move()
        {
            while (true)
            {
                switch (currentState)
                {
                    case State.Idle:
                    case State.Clicked:
                        yield break;

                    case State.MovingClicked:
                        atPercentage += percentagePerSecond * Time.deltaTime;
                        if (atPercentage >= 1.0f)
                        {
                            atPercentage = 1.0f;
                            currentState = State.Clicked;
                        }
                        break;

                    case State.MovingIdle:
                        atPercentage -= percentagePerSecond * Time.deltaTime;
                        if (atPercentage <= 0.0f)
                        {
                            atPercentage = 0.0f;
                            currentState = State.Idle;
                        }
                        break;
                }

                toAnimate.transform.localPosition = Vector3.Lerp(startPoint, endPoint, GetCurve().Evaluate(atPercentage));
                yield return null;
            }
        }

        private void OnDisable()
        {
            toAnimate.transform.localPosition = startPoint;

            if (moveCoroutine != null && moveCoroutine.IsFinshed == false)
                moveCoroutine.Stop(false);

            moveCoroutine = null;
            atPercentage = 0.0f;
        }

        protected void OnDrawGizmos()
        {
            Transform toShow = toAnimate ? toAnimate.transform : transform;
            Gizmos.color = Color.red;
            Vector3 endPoint = toShow.TransformPoint(localEndPoint);
            Gizmos.DrawLine(toShow.position, endPoint);
        }
    }
}
