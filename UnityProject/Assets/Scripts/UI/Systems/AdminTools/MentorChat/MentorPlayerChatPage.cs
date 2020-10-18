using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DatabaseAPI;
using UnityEngine;
using UnityEngine.UI;

namespace AdminTools.MentorChat
{
	public class MentorPlayerChatPage : MentorPage
	{
		[SerializeField] private InputField inputField = null;
		[SerializeField] private Transform chatContent = null;
		[SerializeField] private GameObject mentorChatEntryPrefab = null;
		private List<MentorChatEntry> loadedChatEntries = new List<MentorChatEntry>();
		private MentorPlayerEntry selectedPlayer;

		public override void OnPageRefresh(MentorPageRefreshData adminPageData)
		{
			base.OnPageRefresh(adminPageData);
		}

		private Dictionary<string, List<string>> chatLogs = new Dictionary<string, List<string>>();

		private bool refreshClock;
		private float waitTime;

		public void SetData(MentorPlayerEntry entry)
		{
			if (entry != null)
			{
				selectedPlayer = entry;
			}

			if (selectedPlayer == null) return;

			UIManager.IsInputFocus = true;
			UIManager.PreventChatInput = true;
			RefreshChatLog(selectedPlayer.PlayerData.uid);
			refreshClock = true;
			inputField.ActivateInputField();
		}

		private void Update()
		{
			if (refreshClock)
			{
				waitTime += Time.deltaTime;
				if (waitTime > 4f)
				{
					waitTime = 0f;
					RefreshPage();
				}
			}
		}

		public void AddPendingMessagesToLogs(string userID, List<MentorChatMessage> pendingMessages)
		{
			foreach (var msg in pendingMessages)
			{
				AddMessageToLogs(userID, msg.Message);
			}
		}

		public void AddMessageToLogs(string userID, string message)
		{
			if (!chatLogs.ContainsKey(userID))
			{
				chatLogs.Add(userID, new List<string>());
			}

			chatLogs[userID].Add(message);
		}

		private void RefreshChatLog(string userID)
		{
			foreach (var e in loadedChatEntries)
			{
				Destroy(e.gameObject);
			}

			loadedChatEntries.Clear();

			if (!chatLogs.ContainsKey(userID)) return;

			foreach (var s in chatLogs[userID])
			{
				var entry = Instantiate(mentorChatEntryPrefab, chatContent);
				var chatEntry = entry.GetComponent<MentorChatEntry>();
				chatEntry.SetText(s);
				loadedChatEntries.Add(chatEntry);
			}
		}

		public void OnInputSubmit()
		{
			if (string.IsNullOrEmpty(inputField.text)) return;

			AddMessageToLogs(selectedPlayer.PlayerData.uid, $"You: {inputField.text}");
			RefreshChatLog(selectedPlayer.PlayerData.uid);
			var message = $"{PlayerManager.CurrentCharacterSettings.Username}: {inputField.text}";
			RequestAdminBwoink.Send(ServerData.UserID, PlayerList.Instance.AdminToken, selectedPlayer.PlayerData.uid,
  message);
			inputField.text = "";
			inputField.ActivateInputField();
			StartCoroutine(AfterSubmit());
		}

		private IEnumerator AfterSubmit()
		{
			yield return WaitFor.EndOfFrame;
			UIManager.IsInputFocus = true;
		}

		public void GoBack()
		{
			refreshClock = false;
			UIManager.IsInputFocus = false;
			UIManager.PreventChatInput = false;
			mentorTools.ShowMainPage();
		}

		private void OnDisable()
		{
			UIManager.IsInputFocus = false;
			UIManager.PreventChatInput = false;
		}
	}
}