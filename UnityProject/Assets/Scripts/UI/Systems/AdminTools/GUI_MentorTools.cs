using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DatabaseAPI;
using AdminCommands;
using UI.AdminTools;
using AdminTools.MentorChat;

namespace AdminTools
{
	public class GUI_MentorTools : MonoBehaviour
	{
		[SerializeField] private GameObject retrievingDataScreen = null;

		[SerializeField] private GameObject backBtn = null;
		[SerializeField] private GameObject mainPage = null;
		[SerializeField] private GameObject playerManagePage = null;
		[SerializeField] private GameObject playerChatPage = null;
		[SerializeField] private GameObject playersScrollView = null;

		private MentorPlayerChatPage playerChatPageScript;
		public AreYouSurePage areYouSurePage;

		[SerializeField] private Transform playerListContent = null;
		[SerializeField] private GameObject playerEntryPrefab = null;

		[SerializeField] private Text windowTitle = null;
		public Text WindowTitle => windowTitle;

		private List<MentorPlayerEntry> playerEntries = new List<MentorPlayerEntry>();
		public string SelectedPlayer { get; private set; }

		private void OnEnable()
		{
			playerChatPageScript = playerChatPage.GetComponent<MentorPlayerChatPage>();
			ShowMainPage();
		}

		public void ClosePanel()
		{
			gameObject.SetActive(false);
		}

		public void ToggleOnOff()
		{
			gameObject.SetActive(!gameObject.activeInHierarchy);
		}

		public void OnBackButton()
		{
			if (playerChatPage.activeInHierarchy)
			{
				playerChatPage.GetComponent<PlayerChatPage>().GoBack();
				return;
			}
			ShowMainPage();
		}

		public void ShowMainPage()
		{
			DisableAllPages();
			mainPage.SetActive(true);
			windowTitle.text = "MENTOR TOOL PANEL";
		}

		public void ShowPlayerChatPage()
		{
			DisableAllPages();
			playerChatPage.SetActive(true);
			backBtn.SetActive(true);
			windowTitle.text = "PLAYER BWOINK";
			playersScrollView.SetActive(true);
			retrievingDataScreen.SetActive(true);
		}

		private void DisableAllPages()
		{
			retrievingDataScreen.SetActive(false);
			mainPage.SetActive(false);
			backBtn.SetActive(false);
			playerManagePage.SetActive(false);
			playerChatPage.SetActive(false);
			playersScrollView.SetActive(false);
			areYouSurePage.gameObject.SetActive(false);
		}

		public void CloseRetrievingDataScreen()
		{
			retrievingDataScreen.SetActive(false);
		}

		public void RefreshOnlinePlayerList(MentorPageRefreshData data)
		{
			foreach (var e in playerEntries)
			{
				Destroy(e.gameObject);
			}

			playerEntries.Clear();

			foreach (var p in data.players)
			{
				var e = Instantiate(playerEntryPrefab, playerListContent);
				var entry = e.GetComponent<MentorPlayerEntry>();
				entry.UpdateButton(p, SelectPlayerInList);

				if (p.isOnline)
				{
					entry.button.interactable = true;
				}
				else
				{
					if (!playerChatPage.activeInHierarchy)
					{
						entry.button.interactable = false;
					}
				}

				playerEntries.Add(entry);
				if (SelectedPlayer == p.uid)
				{
					entry.SelectPlayer();
					if (playerChatPage.activeInHierarchy)
					{
						playerChatPageScript.SetData(entry);
						SelectedPlayer = entry.PlayerData.uid;
					}
				}
			}

			if (string.IsNullOrEmpty(SelectedPlayer))
			{
				SelectPlayerInList(playerEntries[0]);
			}
		}

		public void SelectPlayerInList(MentorPlayerEntry selectedEntry)
		{
			foreach (var p in playerEntries)
			{
				if (p != selectedEntry)
				{
					p.DeselectPlayer();
				}
				else
				{
					p.SelectPlayer();
					SelectedPlayer = selectedEntry.PlayerData.uid;
				}
			}

			SelectedPlayer = selectedEntry.PlayerData.uid;

			if (playerChatPage.activeInHierarchy)
			{
				playerChatPageScript.SetData(selectedEntry);
			}
		}

		public void AddPendingMessagesToLogs(string playerId, List<MentorChatMessage> pendingMessages)
		{
			if (pendingMessages.Count == 0) return;

			playerChatPageScript.AddPendingMessagesToLogs(playerId, pendingMessages);
			if (playerId == SelectedPlayer)
			{
				playerChatPageScript.SetData(null);
			}
		}
	}
}