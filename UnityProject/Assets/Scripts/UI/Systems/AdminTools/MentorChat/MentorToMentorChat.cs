using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AdminTools.MentorChat;
using DatabaseAPI;
using Mirror;
using Messages.Server.Mentor;
using UnityEngine;
using Messages.Client.Mentor;

namespace Scripts.UI.Systems.AdminTools.MentorChat
{
	public class MentorToMentorChat : MonoBehaviour
	{
		[SerializeField] private ChatScroll chatScroll = null;
		private const string NotificationKey = "mentorchat";

		/// <summary>
		/// All messages sent and recieved between mentors
		/// </summary>
		private List<MentorChatMessage> serverMentorChatLogs
			= new List<MentorChatMessage>();

		/// <summary>
		/// The mentors client local cache for admin to admin chat
		/// </summary>
		private List<MentorChatMessage> clientMentorChatLogs
			= new List<MentorChatMessage>();

		public void ClearLogs()
		{
			serverMentorChatLogs.Clear();
			clientMentorChatLogs.Clear();
		}

		private void OnEnable()
		{
			chatScroll.OnInputFieldSubmit += OnInputSend;
			UIManager.Instance.mentorChatButtons.mentorNotification.ClearAll();
			chatScroll.LoadChatEntries(clientMentorChatLogs.Cast<ChatEntryData>().ToList());
			ClientGetUnreadAdminPlayerMessages(ServerData.UserID);
		}

		private void OnDisable()
		{
			chatScroll.OnInputFieldSubmit -= OnInputSend;
		}

		public void ServerAddChatRecord(string message, string userId)
		{
			var entry = new MentorChatMessage
			{
				fromUserid = userId,
				Message = message
			};

			serverMentorChatLogs.Add(entry);

			//TODO: figure out these events and where they come from.
			MentorChatUpdateMessage.SendSingleEntryToAdmins(entry);
			MentorChatNotifications.SendToAll(NotificationKey, MentorChatWindow.MentorToMentorChat, 1);
		}

		public void ServerGetUnreadMessages(string adminId, int currentCount, NetworkConnection requestee)
		{
			if (!PlayerList.Instance.IsAdmin(adminId)) return;

			if (currentCount >= serverMentorChatLogs.Count)
			{
				return;
			}

			MentorChatUpdate update = new MentorChatUpdate();

			update.messages = serverMentorChatLogs;

			MentorChatUpdateMessage.SendLogUpdateToAdmin(requestee, update);
		}

		private void ClientGetUnreadAdminPlayerMessages(string playerId)
		{
			AdminCheckAdminMessages.Send(playerId, clientMentorChatLogs.Count);
		}

		public void ClientUpdateChatLog(string unreadMessagesJson)
		{
			if (string.IsNullOrEmpty(unreadMessagesJson)) return;

			var update = JsonUtility.FromJson<MentorChatUpdate>(unreadMessagesJson);
			clientMentorChatLogs.AddRange(update.messages);

			chatScroll.AppendChatEntries(update.messages.Cast<ChatEntryData>().ToList());
		}

		public void OnInputSend(string message)
		{
			var adminMsg = new MentorChatMessage
			{
				fromUserid = ServerData.UserID,
				Message = message
			};

			var msg = $"{ServerData.Auth.CurrentUser.DisplayName}: {message}";
			RequestMentorChatMessage.Send(ServerData.UserID, PlayerList.Instance.AdminToken, msg);
		}
	}
}