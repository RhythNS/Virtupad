using UnityEngine;

namespace Virtupad
{
    public class KeyboardButtonLayerChanger : KeyboardButtonEvent
    {
        [SerializeField] private bool canSticky = true;
        [SerializeField] private bool layerHasKey = true;
        [SerializeField] private int layerToChangeTo;
        [SerializeField] private SpriteRenderer stickyLight;

        protected enum State
        {
            NotInLayer,
            InLayer,
            Sticky
        }

        protected State CurrentState = State.NotInLayer;

        public override void ChangeLayer(int layer)
        {

        }

        public override void OnButtonDown()
        {
            switch (CurrentState)
            {
                case State.NotInLayer:
                    if (layerHasKey == true)
                        KeyboardInWorld.Instance.KeyDown(layers[0].key);

                    KeyboardInWorld.Instance.OnKeyDown += OnKeyDown;
                    KeyboardInWorld.Instance.OnChangeLayer(layerToChangeTo);
                    CurrentState = State.InLayer;
                    break;

                case State.InLayer:
                    KeyboardInWorld.Instance.OnKeyDown -= OnKeyDown;

                    if (canSticky == true)
                    {
                        CurrentState = State.Sticky;
                        stickyLight.enabled = true;
                        break;
                    }

                    ChangeBackToNotInLayer();
                    break;

                case State.Sticky:
                    KeyboardInWorld.Instance.OnKeyDown -= OnKeyDown;
                    ChangeBackToNotInLayer();
                    break;

                default:
                    Debug.LogError("Unknown state in keyboard: " + CurrentState);
                    break;
            }
        }

        private void ChangeBackToNotInLayer()
        {
            if (layerHasKey == true)
                KeyboardInWorld.Instance.KeyUp(layers[0].key);

            stickyLight.enabled = false;
            KeyboardInWorld.Instance.OnChangeLayer(0);
            CurrentState = State.NotInLayer;
        }

        private void OnKeyDown(uDesktopDuplication.KeyBoard.Keys key)
        {
            if (CurrentState != State.Sticky)
                ChangeBackToNotInLayer();
        }

        public override void OnButtonUp()
        {

        }
    }
}
