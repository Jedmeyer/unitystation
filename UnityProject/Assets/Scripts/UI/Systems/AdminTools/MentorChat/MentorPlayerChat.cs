using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DatabaseAPI;
using Mirror;
using UnityEngine;
using DiscordWebhook;
using Messages.Server.Mentor;
using Messages.Client.Mentor;

namespace AdminTools.MentorChat
{
	public class MentorPlayerChat : MonoBehaviour
	{
		[SerializeField] private ChatScroll chatScroll = null;
		private MentorPlayerEntryData selectedPlayer;

		public MentorPlayerEntryData SelectedPlayer
		{
			get { return selectedPlayer; }
		}

		/// <summary>
		/// All messages sent and recieved from players to admins
		/// </summary>
		private Dictionary<string, List<MentorChatMessage>> serverMentorPlayerChatLogs
			= new Dictionary<string, List<MentorChatMessage>>();

		/// <summary>
		/// The admins client local cache for admin to player chat
		/// </summary>
		private Dictionary<string, List<MentorChatMessage>> clientMentorPlayerChatLogs
			= new Dictionary<string, List<MentorChatMessage>>();

		public void ClearLogs()
		{
			serverMentorPlayerChatLogs.Clear();
			clientMentorPlayerChatLogs.Clear();
		}

		public void ServerAddChatRecord(string message, string playerId, string adminId = "")
		{
			if (!serverMentorPlayerChatLogs.ContainsKey(playerId))
			{
				serverMentorPlayerChatLogs.Add(playerId, new List<MentorChatMessage>());
			}

			var entry = new MentorChatMessage
			{
				fromUserid = playerId,
				Message = message
			};

			if (!string.IsNullOrEmpty(adminId))
			{
				entry.fromUserid = adminId;
				entry.wasFromAdmin = true;
			}
			serverMentorPlayerChatLogs[playerId].Add(entry);

			//Todo: Figure out what this does -
			//AdminPlayerChatUpdateMessage.SendSingleEntryToAdmins(entry, playerId);
			if (!string.IsNullOrEmpty(adminId))
			{
				AdminChatNotifications.SendToAll(playerId, AdminChatWindow.AdminPlayerChat, 0, true);
			}
			else
			{
				AdminChatNotifications.SendToAll(playerId, AdminChatWindow.AdminPlayerChat, 1);
			}

			ServerMessageRecording(playerId, entry);
		}

		private void ServerMessageRecording(string playerId, MentorChatMessage entry)
		{
			var chatlogDir = Path.Combine(Application.streamingAssetsPath, "chatlogs");
			if (!Directory.Exists(chatlogDir))
			{
				Directory.CreateDirectory(chatlogDir);
			}

			var filePath = Path.Combine(chatlogDir, $"{playerId}.txt");

			var connectedPlayer = PlayerList.Instance.GetByUserID(playerId);

			if (!File.Exists(filePath))
			{
				var stream = File.Create(filePath);
				stream.Close();
				string header = $"Username: {connectedPlayer.Username} Player Name: {connectedPlayer.Name} \r\n" +
								$"IsAntag: {PlayerList.Instance.AntagPlayers.Contains(connectedPlayer)}  role: {connectedPlayer.Job} \r\n" +
								$"-----Chat Log----- \r\n" +
								$" \r\n";
				File.AppendAllText(filePath, header);
			}

			string entryName = connectedPlayer.Name;
			if (entry.wasFromAdmin)
			{
				var adminPlayer = PlayerList.Instance.GetByUserID(entry.fromUserid);
				entryName = "[A] " + adminPlayer.Name;
			}

			DiscordWebhookMessage.Instance.AddWebHookMessageToQueue(DiscordWebhookURLs.DiscordWebhookAdminURL, entry.Message, entryName);

			File.AppendAllText(filePath, $"[{DateTime.Now.ToString("O")}] {entryName}: {entry.Message}");
		}

		public void ServerGetUnreadMessages(string playerId, int currentCount, NetworkConnection requestee)
		{
			if (!serverMentorPlayerChatLogs.ContainsKey(playerId))
			{
				serverMentorPlayerChatLogs.Add(playerId, new List<MentorChatMessage>());
			}

			if (currentCount >= serverMentorPlayerChatLogs[playerId].Count)
			{
				return;
			}

			MentorChatUpdate update = new MentorChatUpdate();

			update.messages = serverMentorPlayerChatLogs[playerId].GetRange(currentCount,
				serverMentorPlayerChatLogs[playerId].Count - currentCount);

			MentorPlayerChatUpdateMessage.SendLogUpdateToAdmin(requestee, update, playerId);
		}

		private void ClientGetUnreadMentorPlayerMessages(string playerId)
		{
			if (!clientMentorPlayerChatLogs.ContainsKey(playerId))
			{
				clientMentorPlayerChatLogs.Add(playerId, new List<MentorChatMessage>());
			}

			MentorCheckMessages.Send(playerId, clientMentorPlayerChatLogs[playerId].Count);
		}

		public void ClientUpdateChatLog(string unreadMessagesJson, string playerId)
		{
			if (string.IsNullOrEmpty(unreadMessagesJson)) return;

			if (!clientMentorPlayerChatLogs.ContainsKey(playerId))
			{
				clientMentorPlayerChatLogs.Add(playerId, new List<MentorChatMessage>());
			}

			var update = JsonUtility.FromJson<MentorChatUpdate>(unreadMessagesJson);
			clientMentorPlayerChatLogs[playerId].AddRange(update.messages);

			if (selectedPlayer != null && selectedPlayer.uid == playerId)
			{
				chatScroll.AppendChatEntries(update.messages.Cast<ChatEntryData>().ToList());
			}
		}

		public void OnPlayerSelect(MentorPlayerEntryData playerData)
		{
			selectedPlayer = playerData;
			ClientGetUnreadMentorPlayerMessages(playerData.uid);
			if (!clientMentorPlayerChatLogs.ContainsKey(playerData.uid))
			{
				clientMentorPlayerChatLogs.Add(playerData.uid, new List<MentorChatMessage>());
			}

			chatScroll.LoadChatEntries(clientMentorPlayerChatLogs[playerData.uid].Cast<ChatEntryData>().ToList());
		}

		private void OnEnable()
		{
			chatScroll.OnInputFieldSubmit += OnInputSend;
			if (selectedPlayer != null)
			{
				OnPlayerSelect(selectedPlayer);
			}
		}

		private void OnDisable()
		{
			chatScroll.OnInputFieldSubmit -= OnInputSend;
		}

		public void OnInputSend(string message)
		{
			var adminMsg = new MentorChatMessage
			{
				fromUserid = ServerData.UserID,
				Message = message,
				wasFromAdmin = true
			};

			var msg = $"{ServerData.Auth.CurrentUser.DisplayName}: {message}";
			RequestAdminBwoink.Send(ServerData.UserID, PlayerList.Instance.AdminToken, selectedPlayer.uid,
			msg);
		}
	}
}