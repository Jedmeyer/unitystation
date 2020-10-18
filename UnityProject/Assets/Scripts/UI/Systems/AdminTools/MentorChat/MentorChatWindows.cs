using Scripts.UI.Systems.AdminTools.MentorChat;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AdminTools.MentorChat
{
	public class MentorChatWindows : MonoBehaviour
	{
		public MentorPlayerChat mentorPlayerChat = null;
		public MentorToMentorChat mentorToMentorChat = null;
		public List<AdminPlayersScrollView> playerListViews = new List<AdminPlayersScrollView>();

		public MentorChatWindow SelectedWindow { get; private set; }

		public event Action<MentorChatWindow> WindowChangeEvent;

		private void Awake()
		{
			ToggleWindows(MentorChatWindow.None);
		}

		//This is for onclick button events, hence the int param
		public void ToggleWindows(int option)
		{
			ToggleWindows((MentorChatWindow)option);
		}

		private void ToggleWindows(MentorChatWindow window)
		{
			mentorPlayerChat.gameObject.SetActive(false);
			mentorToMentorChat.gameObject.SetActive(false);

			switch (window)
			{
				case MentorChatWindow.MentorPlayerChat:
					mentorPlayerChat.gameObject.SetActive(true);
					SelectedWindow = MentorChatWindow.MentorPlayerChat;
					break;

				case MentorChatWindow.MentorToMentorChat:
					mentorPlayerChat.gameObject.SetActive(true);
					SelectedWindow = MentorChatWindow.MentorToMentorChat;
					break;

				default:
					SelectedWindow = MentorChatWindow.None;
					break;
			}

			if (WindowChangeEvent != null)
			{
				WindowChangeEvent.Invoke(SelectedWindow);
			}
		}

		public void ResetAll()
		{
			mentorPlayerChat.ClearLogs();
			mentorToMentorChat.ClearLogs();
		}
	}

	public enum MentorChatWindow
	{
		None,
		MentorPlayerChat,
		MentorToMentorChat,
	}
}