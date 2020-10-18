using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AdminTools.MentorChat
{
	public class MentorChatEntry : MonoBehaviour
	{
		[SerializeField] private Text msgText = null;

		public void SetText(string msg)
		{
			msgText.text = msg;
		}
	}
}