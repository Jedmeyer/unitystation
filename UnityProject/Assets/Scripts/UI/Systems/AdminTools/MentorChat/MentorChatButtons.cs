using Messages.Server.Mentor;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

namespace AdminTools.MentorChat
{
	public class MentorChatButtons : MonoBehaviour
	{
		public GUI_Notification mentorNotification = null;
		public GUI_Notification playerNotification = null;
		[SerializeField] private MentorChatWindows mentorChatWindows = null;
		[SerializeField] private Button mentorChatButton = null;
		[SerializeField] private Button playerChatButton = null;
		// Ignore default color warning
#pragma warning disable CS0649
		[SerializeField] private Color selectedColor;
		[SerializeField] private Color unSelectedColor;
		// Ignore default color warning
#pragma warning restore CS0649

		private void OnEnable()
		{
			mentorChatWindows.WindowChangeEvent += OnMentorChatWindowChange;
			ToggleButtons(MentorChatWindow.None);
		}

		private void OnDisable()
		{
			mentorChatWindows.WindowChangeEvent -= OnMentorChatWindowChange;
		}

		private void OnMentorChatWindowChange(MentorChatWindow selectedWindow)
		{
			ToggleButtons(selectedWindow);
		}

		private void ToggleButtons(MentorChatWindow selectedWindow)
		{
			mentorChatButton.image.color = unSelectedColor;
			playerChatButton.image.color = unSelectedColor;

			switch (selectedWindow)
			{
				case MentorChatWindow.MentorPlayerChat:
					playerChatButton.image.color = selectedColor;
					break;

				case MentorChatWindow.MentorToMentorChat:
					playerChatButton.image.color = selectedColor;
					break;
			}
		}

		public void ClearAllNotifications()
		{
			mentorNotification.ClearAll();
			playerNotification.ClearAll();
		}

		/// <summary>
		/// Use for initialization of admin chat notifications when the admin logs in
		/// </summary>
		/// <param name="adminConn"></param>
		public void ServerUpdateAdminNotifications(NetworkConnection adminConn)
		{
			var update = new MentorChatNotificationFullUpdate();

			foreach (var n in mentorNotification.notifications)
			{
				update.notificationEntries.Add(new MentorChatNotificationEntry
				{
					Amount = n.Value,
					Key = n.Key,
					TargetWindow = MentorChatWindow.MentorToMentorChat
				});
			}

			foreach (var n in playerNotification.notifications)
			{
				if (PlayerList.Instance.GetByUserID(n.Key) == null
					|| PlayerList.Instance.GetByUserID(n.Key).Connection == null) continue;

				update.notificationEntries.Add(new MentorChatNotificationEntry
				{
					Amount = n.Value,
					Key = n.Key,
					TargetWindow = MentorChatWindow.MentorPlayerChat
				});
			}

			MentorChatNotifications.Send(adminConn, update);
		}

		public void ClientUpdateNotifications(string notificationKey, MentorChatWindow targetWindow,
			int amt, bool clearAll)
		{
			switch (targetWindow)
			{
				case MentorChatWindow.MentorPlayerChat:
					if (clearAll)
					{
						playerNotification.RemoveNotification(notificationKey);
						if (amt == 0) return;
					}
					//No need to update notification if the player is already selected in admin chat
					if (mentorChatWindows.SelectedWindow == MentorChatWindow.MentorPlayerChat)
					{
						if (mentorChatWindows.mentorPlayerChat.SelectedPlayer != null
							&& mentorChatWindows.mentorPlayerChat.SelectedPlayer.uid == notificationKey)
						{
							break;
						}
					}
					playerNotification.AddNotification(notificationKey, amt);
					break;

				case MentorChatWindow.MentorToMentorChat:
					if (clearAll)
					{
						mentorNotification.RemoveNotification(notificationKey);
						if (amt == 0) return;
					}

					if (mentorChatWindows.mentorToMentorChat.gameObject.activeInHierarchy) return;

					mentorNotification.AddNotification(notificationKey, amt);
					break;
			}
		}
	}
}