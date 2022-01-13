using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Virtupad
{
    public class AudioOutput : MonoBehaviour
    {
        public static AudioOutput Instance { get; private set; }

        [SerializeField] private AudioClip[] clips;
        [SerializeField] private AudioSource source;

        private void Awake()
        {
            if (Instance)
            {
                Debug.LogWarning("AudioOutput already in scene. Deleting myself!");
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void ChangeIndex(int i)
        {
            if (i == -1)
            {
                source.Stop();
                return;
            }

            source.clip = clips[i];
            source.Play();
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }
    }
}
