/**
 * Copyright (c) 2017 The Campfire Union Inc - All Rights Reserved.
 *
 * Licensed under the MIT license. See LICENSE file in the project root for
 * full license information.
 *
 * Email:   info@campfireunion.com
 * Website: https://www.campfireunion.com
 */

using UnityEngine;
using System.Collections;
using TMPro;
using Virtupad;

namespace VRKeys
{
    /// <summary>
    /// An individual key in the VR keyboard.
    /// </summary>
    public class Key : Interactable, IStayOnInteractable
    {
        public Keyboard keyboard;

        public TextMeshPro label;

        public Material inactiveMat;

        public Material activeMat;

        public Material disabledMat;

        public Vector3 defaultPosition;

        public Vector3 pressedPosition;

        public Vector3 pressDirection = Vector3.down;

        public float pressMagnitude = 0.1f;

        public bool autoInit = false;

        private bool disabled = false;

        protected MeshRenderer meshRenderer;

        private ExtendedCoroutine activateFor;
        private ExtendedCoroutine press;
        private Collider lastPressed;

        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            SnapToObject = false;

            if (autoInit)
            {
                Init(transform.localPosition);
            }
        }

        /// <summary>
        /// Initialize the key with a default position and pressed position.
        /// </summary>
        /// <param name="defaultPos">Default position.</param>
        public void Init(Vector3 defaultPos)
        {
            defaultPosition = defaultPos;
            pressedPosition = defaultPos + (Vector3.down * 0.01f);
        }

        private void OnEnable()
        {
            disabled = false;
            transform.localPosition = defaultPosition;
            meshRenderer.material = inactiveMat;

            OnEnableExtras();
        }

        /// <summary>
        /// Override this to add custom logic on enable.
        /// </summary>
        protected virtual void OnEnableExtras()
        {
            // Override me!
        }

        public void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out KeyStabCollider keyStab) == true)
                Press(keyStab, other);
        }

        private void OnTriggerExit(Collider other)
        {
            if (lastPressed == other)
            {
                lastPressed = null;
            }
        }

        public void Press(KeyStabCollider keyStab = null, Collider other = null)
        {
            if ((press != null && press.IsFinshed == false) || disabled || keyboard.disabled || !keyboard.initialized)
                return;

            if (keyStab != null)
                keyStab.OnKeyHit();

            lastPressed = other;

            if (press != null && press.IsFinshed == false)
            {
                press.Stop(false);
            }
            press = new ExtendedCoroutine(this, OnPress(other));
        }

        private IEnumerator OnPress(Collider other)
        {
            HandleTriggerEnter(other);

            transform.localPosition = pressedPosition;

            yield return new WaitForSeconds(0.125f);
            while (lastPressed != null)
            {
                yield return null;
            }

            transform.localPosition = defaultPosition;
        }

        /// <summary>
        /// Override this to handle trigger events. Only fires when
        /// a downward trigger event occurred from the collider
        /// matching keyboard.colliderName.
        /// </summary>
        /// <param name="other">Collider.</param>
        public virtual void HandleTriggerEnter(Collider other)
        {
            // Override me!
        }

        /// <summary>
        /// Show the active material for the specified length of time.
        /// </summary>
        /// <param name="seconds">Seconds.</param>
        public void ActivateFor(float seconds)
        {
            if (activateFor != null && activateFor.IsFinshed == false)
            {
                activateFor.Stop();
            }
            activateFor = new ExtendedCoroutine(this, DoActivateFor(seconds));
        }

        private IEnumerator DoActivateFor(float seconds)
        {
            meshRenderer.material = activeMat;

            yield return new WaitForSeconds(seconds);

            meshRenderer.material = inactiveMat;
        }

        /// <summary>
        /// Disable the key.
        /// </summary>
        public virtual void Disable()
        {
            disabled = true;

            if (meshRenderer != null)
            {
                meshRenderer.material = disabledMat;
            }
        }

        /// <summary>
        /// Re-enable a disabled key.
        /// </summary>
        public virtual void Enable()
        {
            disabled = false;

            if (meshRenderer != null)
            {
                meshRenderer.material = inactiveMat;
            }
        }

        /// <summary>
        /// Update the key's label from a new language.
        /// </summary>
        /// <param name="translation">Translation object.</param>
        public virtual void UpdateLayout(Layout translation)
        {
            // Override me!
        }

        public override void Select()
        {
            Press();
        }
    }
}