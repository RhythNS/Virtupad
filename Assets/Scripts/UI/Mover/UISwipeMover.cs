using UnityEngine;
using Valve.VR;

namespace Virtupad
{
    public class UISwipeMover : UIMover
    {
        private enum SelectState
        {
            NotPressed, Pressed, MovedWhilePressed
        }
        private enum MoveState
        {
            NotPressed, Pressing, Released
        }
        private enum RepeatingState
        {
            NoDirection, NotRepeating, Repeated
        }

        private SelectState selectState = SelectState.NotPressed;
        private MoveState moveState = MoveState.NotPressed;
        private RepeatingState repeatingState = RepeatingState.NoDirection;

        [SerializeField] private float deltaLengthSquaredRequired;

        [SerializeField] private float timeUntilRepeat = 1.0f;
        [SerializeField] private float timeAfterRepeat = 0.7f;
        private float timer = 0.0f;

        private Direction? direction = null;

        private Vector2 startPoint = Vector2.zero;
        private Vector2 currentPoint = Vector2.zero;

        public override void SubscribeToEvents(SteamVR_Action_Vector2 uiMoveInput, SteamVR_Action_Boolean uiSelectInput)
        {
            uiMoveInput.AddOnChangeListener(UIMoveInputChanged, SteamVR_Input_Sources.Any);
            uiSelectInput.AddOnChangeListener(UISelectInputChanged, SteamVR_Input_Sources.Any);
        }

        private void UIMoveInputChanged(SteamVR_Action_Vector2 fromAction, SteamVR_Input_Sources fromSource, Vector2 axis, Vector2 delta)
        {
            if (axis == Vector2.zero)
            {
                moveState = MoveState.Released;
                return;
            }

            if (moveState != MoveState.Pressing)
            {
                startPoint = axis;
                moveState = MoveState.Pressing;
                gameObject.SetActive(true);
            }

            currentPoint = axis;
        }

        private void UISelectInputChanged(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool newState)
        {
            if (newState)
            {
                selectState = SelectState.Pressed;
                return;
            }

            if (selectState != SelectState.MovedWhilePressed)
                UIMoveManager.Instance.Click();

            selectState = SelectState.NotPressed;
        }

        public override void UnSubscribeFromEvents(SteamVR_Action_Vector2 uiMoveInput, SteamVR_Action_Boolean uiSelectInput)
        {
            uiMoveInput.RemoveOnChangeListener(UIMoveInputChanged, SteamVR_Input_Sources.Any);
            uiSelectInput.RemoveOnChangeListener(UISelectInputChanged, SteamVR_Input_Sources.Any);
        }

        private void Update()
        {
            switch (moveState)
            {
                case MoveState.Pressing:
                    OnPressing();
                    break;

                case MoveState.Released:

                    moveState = MoveState.NotPressed;
                    goto case MoveState.NotPressed;

                case MoveState.NotPressed:
                    repeatingState = RepeatingState.NoDirection;
                    timer = 0.0f;
                    gameObject.SetActive(false);
                    break;
            }
        }

        private void OnPressing()
        {
            Direction? newDirection = GetDirection();
            bool changedDirection = newDirection != direction;

            if (changedDirection)
                direction = newDirection;

            switch (repeatingState)
            {
                case RepeatingState.NoDirection:
                    if (direction == null)
                        return;

                    UIMoveManager.Instance.Move(direction.Value);
                    repeatingState = RepeatingState.NotRepeating;
                    timer = 0.0f;
                    break;

                case RepeatingState.NotRepeating:
                    timer += Time.deltaTime;

                    if (changedDirection)
                    {
                        PressedChangedDirection();
                        return;
                    }

                    if (timer >= timeUntilRepeat)
                    {
                        UIMoveManager.Instance.Move(direction.Value);
                        repeatingState = RepeatingState.Repeated;
                        timer = 0.0f;
                        return;
                    }
                    break;

                case RepeatingState.Repeated:
                    timer += Time.deltaTime;

                    if (changedDirection)
                    {
                        PressedChangedDirection();
                        return;
                    }

                    if (timer >= timeAfterRepeat)
                    {
                        UIMoveManager.Instance.Move(direction.Value);
                        timer = 0.0f;
                        return;
                    }

                    break;
            }
        }

        private void PressedChangedDirection()
        {
            if (direction == null)
            {
                repeatingState = RepeatingState.NoDirection;
                return;
            }

            repeatingState = RepeatingState.NotRepeating;
            UIMoveManager.Instance.Move(direction.Value);
            timer = 0.0f;
        }

        private Direction? GetDirection()
        {
            Vector2 difVec = currentPoint - startPoint;
            if (difVec.sqrMagnitude < deltaLengthSquaredRequired)
                return null;

            if (Mathf.Abs(difVec.x) > Mathf.Abs(difVec.y))
                return difVec.x > 0 ? Direction.Right : Direction.Left;
            else
                return difVec.y > 0 ? Direction.Up : Direction.Down;
        }
    }
}
