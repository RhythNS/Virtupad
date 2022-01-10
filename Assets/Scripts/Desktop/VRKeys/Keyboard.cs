/**
 * Copyright (c) 2017 The Campfire Union Inc - All Rights Reserved.
 *
 * Licensed under the MIT license. See LICENSE file in the project root for
 * full license information.
 *
 * Email:   info@campfireunion.com
 * Website: https://www.campfireunion.com
 */

using System;
using System.Collections;
using System.Collections.Generic;
using uDesktopDuplication;
using UnityEngine;
using UnityEngine.Events;
using Virtupad;

namespace VRKeys
{

    /// <summary>
    /// Keyboard input system for use with NewtonVR. To use, drop the VRKeys prefab
    /// into your scene and activate as needed. Listen for OnUpdate and OnSubmit events,
    /// and set the text via SetText(string).
    ///
    /// Input validation can be done during OnUpdate and OnSubmit events by calling
    /// ShowValidationMessage(msg) and HideValidationMessage(). The keyboard does not
    /// automatically hide OnSubmit, but rather you should call SetActive(false) when
    /// you have finished validating the submitted text.
    /// </summary>
    public class Keyboard : MonoBehaviour
    {
        public static Keyboard Instance { get; private set; }

        public Vector3 positionRelativeToUser = new Vector3(0f, 1.35f, 2f);

        [Space(15)]
        public GameObject keyPrefab;

        public Transform keysParent;

        public float keyWidth = 0.16f;

        public float keyHeight = 0.16f;

        [Space(15)]
        public GameObject keyboardWrapper;

        public ShiftKey shiftKey;

        public Key[] extraKeys;

        [Space(15)]
        public bool initialized = false;

        public bool disabled = true;

        [Serializable]
        public class KeyboardUpdateEvent : UnityEvent<string> { }


        [Space(15)]

        /// <summary>
        /// Listen for events whenever the text changes.
        /// </summary>
        public KeyboardUpdateEvent OnUpdate = new KeyboardUpdateEvent();

        /// <summary>
        /// Listen for events when Cancel() is called.
        /// </summary>
        public UnityEvent OnCancel = new UnityEvent();

        private LetterKey[] keys;
        private int keyCount;

        private bool shifted = false;

        private Layout currentLayout;

        [SerializeField] private string defaultLayout;

        private void Awake()
        {
            if (Instance)
            {
                Debug.LogWarning("Keyboard already in scene. Deleting myself!");
                Destroy(this);
                return;
            }
            Instance = this;
        }

        /// <summary>
        /// Initialization.
        /// </summary>
        private IEnumerator Start()
        {
            yield return StartCoroutine(DoSetLanguage(LayoutDict.Instance.GetLayout(defaultLayout)));

            initialized = true;
            keysParent.gameObject.SetActive(false);
        }

        /// <summary>
        /// Make sure mallets don't stay attached if VRKeys is disabled without
        /// calling Disable().
        /// </summary>
        private void OnDisable()
        {
            Disable();
        }

        public void ToggleEnable()
        {
            if (disabled == true)
                Enable();
            else
                Disable();
        }

        /// <summary>
        /// Enable the keyboard.
        /// </summary>
        public void Enable()
        {
            if (!initialized)
            {
                // Make sure we're initialized first.
                StartCoroutine(EnableWhenInitialized());
                return;
            }

            disabled = false;

            if (keysParent != null)
            {
                keysParent.gameObject.SetActive(true);
            }

            EnableInput();

            KeyStabManager.Instance.Stabs.ForEach(x => x.gameObject.SetActive(true));
        }

        private IEnumerator EnableWhenInitialized()
        {
            yield return new WaitUntil(() => initialized);

            Enable();
        }

        /// <summary>
        /// Disable the keyboard.
        /// </summary>
        public void Disable()
        {
            disabled = true;

            if (keysParent != null)
            {
                keysParent.gameObject.SetActive(false);
            }

            KeyStabManager.Instance?.Stabs.ForEach(x => { if (x) x.gameObject.SetActive(false); });
        }

        /// <summary>
        /// Add a character to the input text.
        /// </summary>
        /// <param name="character">Character.</param>
        public void AddCharacter(string character)
        {
            OnUpdate.Invoke(character);

            if (shifted && character != "" && character != " ")
            {
                StartCoroutine(DelayToggleShift());
            }
        }

        public void AddCharacter(KeyBoard.Keys keys, bool shifted)
        {
            List<KeyBoard.Keys> additionalKeys = new List<KeyBoard.Keys>();
            if (shifted)
                additionalKeys.Add(KeyBoard.Keys.SHIFT);

            additionalKeys.ForEach(x => Manager.keyBoard.SendKeyDown(x));
            Manager.keyBoard.SendKeyPress(keys);
            additionalKeys.ForEach(x => Manager.keyBoard.SendKeyUp(x));
        }

        /// <summary>
        /// Toggle whether the characters are shifted (caps).
        /// </summary>
        public bool ToggleShift()
        {
            if (keys == null)
            {
                return false;
            }

            shifted = !shifted;

            foreach (LetterKey key in keys)
            {
                key.Shifted = shifted;
            }

            shiftKey.Toggle(shifted);

            return shifted;
        }

        private IEnumerator DelayToggleShift()
        {
            yield return new WaitForSeconds(0.1f);

            ToggleShift();
        }

        /// <summary>
        /// Disable keyboard input.
        /// </summary>
        public void DisableInput()
        {
            if (keys != null)
            {
                foreach (LetterKey key in keys)
                {
                    if (key != null)
                    {
                        key.Disable();
                    }
                }
            }

            foreach (Key key in extraKeys)
            {
                key.Disable();
            }
        }

        /// <summary>
        /// Re-enable keyboard input.
        /// </summary>
        public void EnableInput()
        {
            if (keys != null)
            {
                foreach (LetterKey key in keys)
                {
                    if (key != null)
                    {
                        key.Enable();
                    }
                }
            }

            foreach (Key key in extraKeys)
            {
                key.Enable();
            }
        }


        /// <summary>
        /// Cancel input and close the keyboard.
        /// </summary>
        public void Cancel()
        {
            OnCancel.Invoke();
            Disable();
        }

        /// <summary>
        /// Set the language of the keyboard.
        /// </summary>
        /// <param name="layout">New language.</param>
        public void SetLayout(string newLayout)
        {
            StartCoroutine(DoSetLanguage(LayoutDict.Instance.GetLayout(newLayout)));
        }

        private IEnumerator DoSetLanguage(Layout lang)
        {
            currentLayout = lang;

            yield return StartCoroutine(SetupKeys());

            // Update extra keys
            foreach (Key key in extraKeys)
            {
                key.UpdateLayout(currentLayout);
            }
        }

        /// <summary>
        /// Setup the keys.
        /// </summary>
        private IEnumerator SetupKeys()
        {
            bool activeState = keysParent.gameObject.activeSelf;

            // Hide everything before setting up the keys
            keysParent.gameObject.SetActive(false);

            // Remove previous keys
            if (keys != null)
            {
                foreach (Key key in keys)
                {
                    if (key != null)
                    {
                        Destroy(key.gameObject);
                    }
                }
            }

            keys = new LetterKey[currentLayout.TotalKeys()];
            keyCount = 0;

            yield return SetupKeyRow(currentLayout.row1Keys, currentLayout.row1Offset, 0);
            yield return SetupKeyRow(currentLayout.row2Keys, currentLayout.row2Offset, 1);
            yield return SetupKeyRow(currentLayout.row3Keys, currentLayout.row3Offset, 2);
            yield return SetupKeyRow(currentLayout.row4Keys, currentLayout.row4Offset, 3);

            #region old
            // Numbers row
            /*
            for (int i = 0; i < currentLayout.row1Keys.Length; i++)
            {
                GameObject obj = (GameObject)Instantiate(keyPrefab, keysParent);
                obj.transform.localPosition += (Vector3.right * ((keyWidth * i) - currentLayout.row1Offset));

                LetterKey key = obj.GetComponent<LetterKey>();
                Layout.LayoutKey layoutKey = currentLayout.row1Keys[i];
                key.Set(layoutKey);

                key.Init(obj.transform.localPosition);

                obj.name = "Key: " + layoutKey.key;
                obj.SetActive(true);

                keys[keyCount] = key;
                keyCount++;

                yield return null;
            }

            // QWERTY row
            for (int i = 0; i < currentLayout.row2Keys.Length; i++)
            {
                GameObject obj = (GameObject)Instantiate(keyPrefab, keysParent);
                obj.transform.localPosition += (Vector3.right * ((keyWidth * i) - currentLayout.row2Offset));
                obj.transform.localPosition += (Vector3.back * keyHeight * 1);

                LetterKey key = obj.GetComponent<LetterKey>();
                key.character = layout.row2Keys[i];
                key.shiftedChar = layout.row2Shift[i];
                key.Shifted = false;
                key.Init(obj.transform.localPosition);

                obj.name = "Key: " + layout.row2Keys[i];
                obj.SetActive(true);

                keys[keyCount] = key;
                keyCount++;

                yield return null;
            }

            // ASDF row
            for (int i = 0; i < layout.row3Keys.Length; i++)
            {
                GameObject obj = (GameObject)Instantiate(keyPrefab, keysParent);
                obj.transform.localPosition += (Vector3.right * ((keyWidth * i) - layout.row3Offset));
                obj.transform.localPosition += (Vector3.back * keyHeight * 2);

                LetterKey key = obj.GetComponent<LetterKey>();
                key.character = layout.row3Keys[i];
                key.shiftedChar = layout.row3Shift[i];
                key.Shifted = false;
                key.Init(obj.transform.localPosition);

                obj.name = "Key: " + layout.row3Keys[i];
                obj.SetActive(true);

                keys[keyCount] = key;
                keyCount++;

                yield return null;
            }

            // ZXCV row
            for (int i = 0; i < layout.row4Keys.Length; i++)
            {
                GameObject obj = (GameObject)Instantiate(keyPrefab, keysParent);
                obj.transform.localPosition += (Vector3.right * ((keyWidth * i) - currentLayout.row4Offset));
                obj.transform.localPosition += (Vector3.back * keyHeight * 3);

                LetterKey key = obj.GetComponent<LetterKey>();
                key.character = layout.row4Keys[i];
                key.shiftedChar = layout.row4Shift[i];
                key.Shifted = false;
                key.Init(obj.transform.localPosition);

                obj.name = "Key: " + layout.row4Keys[i];
                obj.SetActive(true);

                keys[keyCount] = key;
                keyCount++;

                yield return null;
            }
            */
            #endregion

            // Reset visibility of canvas and keyboard
            keysParent.gameObject.SetActive(activeState);
        }

        private IEnumerator SetupKeyRow(Layout.LayoutKey[] row, float rowOffset, int rowIndex)
        {
            for (int i = 0; i < row.Length; i++)
            {
                GameObject obj = Instantiate(keyPrefab, keysParent);
                obj.transform.localPosition += (Vector3.right * ((keyWidth * i) - rowOffset));
                obj.transform.localPosition += (Vector3.back * keyHeight * rowIndex);

                LetterKey key = obj.GetComponent<LetterKey>();
                Layout.LayoutKey layoutKey = row[i];
                key.Set(layoutKey);

                key.Init(obj.transform.localPosition);

                obj.name = "Key: " + layoutKey.key;
                obj.SetActive(true);

                keys[keyCount] = key;
                keyCount++;

                yield return null;
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }
    }
}