using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using VRKeys;

namespace Virtupad
{
    public class VRKeysTest : MonoBehaviour
    {
		/// <summary>
		/// Reference to the VRKeys keyboard.
		/// </summary>
		public Keyboard keyboard;

		private void OnEnable()
		{
			keyboard.Enable();
			//keyboard.SetPlaceholderMessage ("Please enter your email address");

			keyboard.OnUpdate.AddListener(HandleUpdate);
			keyboard.OnCancel.AddListener(HandleCancel);
		}

		private void OnDisable()
		{
			keyboard.OnUpdate.RemoveListener(HandleUpdate);
			keyboard.OnCancel.RemoveListener(HandleCancel);

			keyboard.Disable();
		}

		/// <summary>
		/// Press space to show/hide the keyboard.
		///
		/// Press Q for Qwerty keyboard, D for Dvorak keyboard, and F for French keyboard.
		/// </summary>
		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.N))
			{
				if (keyboard.disabled)
				{
					keyboard.Enable();
				}
				else
				{
					keyboard.Disable();
				}
			}

			if (keyboard.disabled)
			{
				return;
			}

			/*
			if (Input.GetKeyDown(KeyCode.Q))
			{
				keyboard.SetLayout(KeyboardLayout.Qwerty);
			}
			else if (Input.GetKeyDown(KeyCode.F))
			{
				keyboard.SetLayout(KeyboardLayout.French);
			}
			else if (Input.GetKeyDown(KeyCode.D))
			{
				keyboard.SetLayout(KeyboardLayout.Dvorak);
			}
			 */
		}

		/// <summary>
		/// Hide the validation message on update. Connect this to OnUpdate.
		/// </summary>
		public void HandleUpdate(string text)
		{
			Debug.Log("Update: " + text);
		}

		public void HandleCancel()
		{
			Debug.Log("Cancelled keyboard input!");
		}
	}
}
