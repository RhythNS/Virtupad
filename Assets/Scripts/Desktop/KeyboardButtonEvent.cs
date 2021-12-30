using TMPro;
using uDesktopDuplication;
using UnityEngine;
using Valve.VR.InteractionSystem;

namespace Virtupad
{
    public class KeyboardButtonEvent : MonoBehaviour
    {
        [System.Serializable]
        public struct KeyAndCharacter
        {
            public KeyBoard.Keys key;
            public char toDisplay;
            public int layer;
        }

        [SerializeField] protected KeyAndCharacter[] layers;
        [SerializeField] protected TMP_Text text;
        [SerializeField] protected HoverButton onButton;
        [SerializeField] protected bool updateTextOnLayerChange = true;

        private void Start()
        {
            if (text && updateTextOnLayerChange)
                text.text = layers[0].toDisplay.ToString();
            KeyboardInWorld.Instance.Register(this);
        }

        public virtual void ChangeLayer(int layer)
        {
            if (onButton.buttonDown)
                OnButtonUp();

            if (updateTextOnLayerChange == false)
                return;

            for (int i = 0; i < layers.Length; i++)
            {
                if (layers[i].layer == layer)
                {
                    text.text = layers[layer].toDisplay.ToString();
                    break;
                }
            }
        }

        public virtual void OnButtonDown()
        {
            KeyboardInWorld.Instance.KeyDown(layers[GetLayerToPress(KeyboardInWorld.Instance.CurrentLayer)].key);
        }

        public virtual void OnButtonUp()
        {
            KeyboardInWorld.Instance.KeyUp(layers[GetLayerToPress(KeyboardInWorld.Instance.CurrentLayer)].key);
        }

        protected virtual int GetLayerToPress(int currentLayer)
        {
            for (int i = 0; i < layers.Length; i++)
            {
                if (layers[i].layer == currentLayer)
                    return i;
            }
            return 0;
        }

        private void OnDestroy()
        {
            if (KeyboardInWorld.Instance)
                KeyboardInWorld.Instance.DeRegister(this);
        }
    }
}
