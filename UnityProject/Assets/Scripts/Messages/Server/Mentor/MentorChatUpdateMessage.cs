using System.Collections;
using UnityEngine;
using Mirror;
using AdminTools;
using AdminTools.MentorChat;

namespace Messages.Server.Mentor
{
	public class MentorChatUpdateMessage : ServerMessage
	{
		public string JsonData;

		public override void Process()
		{
			UIManager.Instance.mentorChatWindows.mentorToMentorChat.ClientUpdateChatLog(JsonData);
		}

		public static MentorChatUpdateMessage SendSingleEntryToAdmins(MentorChatMessage chatMessage)
		{
			MentorChatUpdate update = new MentorChatUpdate();
			update.messages.Add(chatMessage);
			MentorChatUpdateMessage msg =
				new MentorChatUpdateMessage { JsonData = JsonUtility.ToJson(update) };

			msg.SendToAdmins();
			return msg;
		}

		public static MentorChatUpdateMessage SendLogUpdateToAdmin(NetworkConnection requestee, MentorChatUpdate update)
		{
			MentorChatUpdateMessage msg =
				new MentorChatUpdateMessage
				{
					JsonData = JsonUtility.ToJson(update),
				};

			msg.SendTo(requestee);
			return msg;
		}
	}
}