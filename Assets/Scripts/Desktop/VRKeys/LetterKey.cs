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
using uDesktopDuplication;

namespace VRKeys
{

    /// <summary>
    /// An individual letter key.
    /// </summary>
    public class LetterKey : Key
    {
        public TextMeshPro shiftedLabel;

        private KeyBoard.Keys key;
        private KeyBoard.Keys shiftedKey;
        bool hasShiftedKey;

        private string character = "";
        private string shiftedChar = "";

        private bool _shifted = false;

        public void Set(Layout.LayoutKey layoutKey)
        {
            label.text = character = layoutKey.key;
            shiftedLabel.text = shiftedChar = layoutKey.shifted;
            key = layoutKey.keyValue;
            shiftedKey = layoutKey.keyShiftedValue;
            hasShiftedKey = layoutKey.hasShiftedValue;
            Shifted = false;
        }

        public bool Shifted
        {
            get { return _shifted; }
            set
            {
                _shifted = value;
                label.text = _shifted ? shiftedChar : character;
                shiftedLabel.text = _shifted ? character : shiftedChar;
            }
        }

        public override void HandleTriggerEnter(Collider other)
        {
            if (Shifted && hasShiftedKey == false)
                keyboard.AddCharacter(key, Shifted);
            else
                keyboard.AddCharacter(shiftedKey, false);

            ActivateFor(0.3f);
        }
    }
}