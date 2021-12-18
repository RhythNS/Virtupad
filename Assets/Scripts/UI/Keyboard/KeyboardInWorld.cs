using System.Collections;
using System.Collections.Generic;
using uDesktopDuplication;
using UnityEngine;

namespace Virtupad
{
    public class KeyboardInWorld : MonoBehaviour
    {
        public delegate void KeyEvent(KeyBoard.Keys key);

        public static KeyboardInWorld Instance { get; private set; }

        private List<KeyboardButtonEvent> keys = new List<KeyboardButtonEvent>();

        public event KeyEvent OnKeyDown;
        public event KeyEvent OnKeyUp;

        public int CurrentLayer { get; private set; } = 0;

        private void Awake()
        {
            if (Instance)
            {
                Debug.LogWarning("KeyboardInWorld already in scene. Deleting myself!");
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void Register(KeyboardButtonEvent buttonEvent)
        {
            keys.Add(buttonEvent);
        }

        public void DeRegister(KeyboardButtonEvent buttonEvent)
        {
            keys.Remove(buttonEvent);
        }

        public void KeyDown(KeyBoard.Keys key)
        {
            Manager.keyBoard.SendKeyDown(key);
            OnKeyDown?.Invoke(key);
        }

        public void KeyUp(KeyBoard.Keys key)
        {
            Manager.keyBoard.SendKeyUp(key);
            OnKeyUp?.Invoke(key);
        }

        public void OnChangeLayer(int layer)
        {
            if (layer < 0 || layer == CurrentLayer)
                return;

            for (int i = 0; i < keys.Count; i++)
            {
                keys[i].ChangeLayer(layer);
            }

            CurrentLayer = layer;
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }
    }
}
