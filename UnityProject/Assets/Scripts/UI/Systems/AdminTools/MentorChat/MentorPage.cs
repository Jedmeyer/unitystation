using System;
using System.Collections.Generic;
using DatabaseAPI;
using UnityEngine;

namespace AdminTools.MentorChat
{
	public class MentorPage : MonoBehaviour
	{
		protected MentorPageRefreshData currentData;
		protected GUI_MentorTools mentorTools;

		public virtual void OnEnable()
		{
			if (mentorTools == null)
			{
				mentorTools = FindObjectOfType<GUI_MentorTools>(); // TODO This causes a ~80ms frame hitch when the page is opened.
			}
			RefreshPage();
		}

		public void RefreshPage()
		{
			RequestAdminPageRefresh.Send(ServerData.UserID, PlayerList.Instance.AdminToken);
		}

		public virtual void OnPageRefresh(MentorPageRefreshData mentorPageRefreshData)
		{
			currentData = mentorPageRefreshData;
			mentorTools.RefreshOnlinePlayerList(mentorPageRefreshData);
			mentorTools.CloseRetrievingDataScreen();
		}
	}

	[Serializable]
	public class MentorPageRefreshData
	{
		//Mentors should have access to only Player Management:
		public List<MentorPlayerEntryData> players = new List<MentorPlayerEntryData>();
	}

	[Serializable]
	public class MentorPlayerEntryData
	{
		public string name;
		public string uid;
		public string currentJob;
		public string accountName;
		public bool isAlive;
		public bool isAdmin;
		public bool isOnline;
	}

	[Serializable]
	public class MentorChatMessage : ChatEntryData
	{
		public string fromUserid;
		public bool wasFromAdmin;
	}

	[Serializable]
	public class MentorChatUpdate
	{
		public List<MentorChatMessage> messages = new List<MentorChatMessage>();
	}
}