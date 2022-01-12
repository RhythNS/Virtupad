using System;
using UnityEngine;

namespace Virtupad
{
    public class SalsaDict : MonoBehaviour
    {
        public static SalsaDict Instance { get; private set; }

        public AudioClip EmptyClip => emptyClip;
        [SerializeField] private AudioClip emptyClip;

        public string CurrentMicrophone => microphone;
        private string microphone;

        public event StringChanged OnMicrophoneChanged;

        private void Awake()
        {
            if (Instance)
            {
                Debug.LogWarning("SalsaDict already in scene. Deleting myself!");
                Destroy(this);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            SaveFileManager saveFileManager = SaveFileManager.Instance;
            string savedMic = saveFileManager.saveGame.microphone;

            if (Array.IndexOf(Microphone.devices, savedMic) != -1)
                microphone = savedMic;
            else
                microphone = Microphone.devices.Length > 0 ? Microphone.devices[0] : "";
        }

        public void SetMicrophone(string newValue, bool save = true)
        {
            if (newValue == microphone)
                return;

            microphone = newValue;

            OnMicrophoneChanged?.Invoke(microphone);

            if (save == false)
                return;

            SaveFileManager saveFileManager = SaveFileManager.Instance;
            saveFileManager.saveGame.microphone = microphone;
            saveFileManager.Save();
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }
    }
}
