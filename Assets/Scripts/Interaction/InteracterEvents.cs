using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Virtupad
{
    public class InteracterEvents : MonoBehaviour
    {
        public static InteracterEvents Instance { get; private set; }

        public event VoidEvent OnInteractBegin;
        public event VoidEvent OnInteractEnd;

        private void Awake()
        {
            if (Instance)
            {
                Debug.LogWarning("InteracterEvents already in scene. Deleting myself!");
                Destroy(this);
                return;
            }
            Instance = this;
        }

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(0.5f);

            foreach (Interacter interacter in GlobalsDict.Instance.Interacters)
            {
                interacter.DownChanged += OnActiveChanged;
            }
        }

        private void OnActiveChanged(bool on)
        {
            if (on)
                CheckIssueBegin();
            else
                CheckIssueEnd();
        }

        private void CheckIssueBegin()
        {
            int interactersOn = 0;

            foreach (Interacter interacter in GlobalsDict.Instance.Interacters)
            {
                if (interacter.enabled == true)
                {
                    ++interactersOn;
                    if (interactersOn > 1)
                        return;
                }
            }

            OnInteractBegin?.Invoke();
        }

        private void CheckIssueEnd()
        {
            foreach (Interacter interacter in GlobalsDict.Instance.Interacters)
            {
                if (interacter.enabled == true)
                    return;
            }

            OnInteractEnd?.Invoke();
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;

            List<Interacter> interacters = GlobalsDict.Instance?.Interacters;
            if (interacters == null)
                return;

            for (int i = 0; i < interacters.Count; i++)
            {
                if (!interacters[i])
                    continue;

                interacters[i].DownChanged -= OnActiveChanged;
            }
        }

    }
}
