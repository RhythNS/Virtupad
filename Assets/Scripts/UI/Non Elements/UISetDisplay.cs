using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Virtupad
{
    public class UISetDisplay : MonoBehaviour
    {
        [System.Serializable]
        public struct SetDescription
        {
            public string title;
            public string description;
            public Sprite previewSprite;

            public string sceneName;
        }

        [SerializeField] SetDescription[] sets;

        [SerializeField] private TMP_Text setHeader;
        [SerializeField] private TMP_Text setDescription;
        [SerializeField] private Image setPreview;

        [SerializeField] private RectTransform loadPanel;
        [SerializeField] private RectTransform alreadyLoadedPanel;

        private int loadedIndex = -1;
        private int currentIndex = 0;

        private void Start()
        {
            GlobalsDict.Instance.onSetDefinitionChanged += OnActiveChanged;
            InitIndex();
        }

        private void OnActiveChanged(SetDefinition newDefinition)
        {
            if (newDefinition == null)
                return;

            InitIndex();
        }

        private void InitIndex()
        {
            loadedIndex = -1;

            SetDefinition definition = GlobalsDict.Instance.CurrentDefinition;

            if (definition == null)
            {
                Debug.LogWarning("No active definition!");
                SetSetIndex(currentIndex);
                return;
            }

            for (int i = 0; i < sets.Length; i++)
            {
                if (sets[i].sceneName == definition.SceneName)
                {
                    loadedIndex = i;
                    break;
                }
            }

            if (loadedIndex == -1)
            {
                Debug.LogWarning("Active scene not found (" + definition.SceneName + "!)");
            }

            SetSetIndex(currentIndex);
        }

        public void OnForward()
        {
            ++currentIndex;
            if (currentIndex >= sets.Length)
                currentIndex = 0;

            SetSetIndex(currentIndex);
        }

        public void OnBackwards()
        {
            --currentIndex;
            if (currentIndex < 0)
                currentIndex = sets.Length - 1;

            SetSetIndex(currentIndex);
        }

        private void SetSetIndex(int newIndex)
        {
            setHeader.text = sets[newIndex].title;
            setDescription.text = sets[newIndex].description;
            setPreview.sprite = sets[newIndex].previewSprite;

            if (newIndex == loadedIndex)
            {
                loadPanel.gameObject.SetActive(false);
                alreadyLoadedPanel.gameObject.SetActive(true);
            }
            else
            {
                loadPanel.gameObject.SetActive(true);
                alreadyLoadedPanel.gameObject.SetActive(false);
            }
        }

        public void LoadSelectedIndex()
        {
            SetSceneLoader.Instance.LoadScene(sets[currentIndex].sceneName);
            UIRoot.Instance.CloseRequest();
        }

        private void OnDestroy()
        {
            if (GlobalsDict.Instance)
                GlobalsDict.Instance.onSetDefinitionChanged -= OnActiveChanged;
        }
    }
}
