using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AdminTools;
using AdminTools.MentorChat;
using Mirror;

namespace Messages.Server.Mentor
{
	/// <summary>
	/// Notify the mentors when a message comes in
	/// </summary>
	public class MentorChatNotifications : ServerMessage
	{
		public string NotificationKey;
		public MentorChatWindow TargetWindow;
		public int Amount;
		public bool ClearAll;
		public bool IsFullUpdate;
		public string FullUpdateJson;

		public override void Process()
		{
			if (!IsFullUpdate)
			{
				UIManager.Instance.mentorChatButtons.ClientUpdateNotifications(NotificationKey, TargetWindow,
					Amount, ClearAll);
			}
			else
			{
				UIManager.Instance.mentorChatButtons.ClearAllNotifications();
				var notiUpdate = JsonUtility.FromJson<MentorChatNotificationFullUpdate>(FullUpdateJson);

				foreach (var n in notiUpdate.notificationEntries)
				{
					UIManager.Instance.mentorChatButtons.ClientUpdateNotifications(n.Key, n.TargetWindow,
						n.Amount, false);
				}
			}
		}

		/// <summary>
		/// Send notification updates to all admins
		/// </summary>
		public static MentorChatNotifications SendToAll(string notificationKey, MentorChatWindow targetWindow,
			int amt, bool clearAll = false)
		{
			MentorChatNotifications msg = new MentorChatNotifications
			{
				NotificationKey = notificationKey,
				TargetWindow = targetWindow,
				Amount = amt,
				ClearAll = clearAll,
				IsFullUpdate = false,
				FullUpdateJson = ""
			};
			msg.SendToAll();
			return msg;
		}

		/// <summary>
		/// Send full update to an admin client
		/// </summary>
		public static MentorChatNotifications Send(NetworkConnection adminConn, MentorChatNotificationFullUpdate update)
		{
			MentorChatNotifications msg = new MentorChatNotifications
			{
				IsFullUpdate = true,
				FullUpdateJson = JsonUtility.ToJson(update)
			};
			msg.SendTo(adminConn);
			return msg;
		}
	}

	[Serializable]
	public class MentorChatNotificationFullUpdate
	{
		public List<MentorChatNotificationEntry> notificationEntries = new List<MentorChatNotificationEntry>();
	}

	[Serializable]
	public class MentorChatNotificationEntry
	{
		public string Key;
		public int Amount;
		public MentorChatWindow TargetWindow;
	}
}