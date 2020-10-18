using System.Collections;
using UnityEngine;
using Mirror;
using AdminTools;
using AdminTools.MentorChat;

namespace Messages.Server.Mentor
{
	public class MentorPlayerChatUpdateMessage : ServerMessage
	{
		public string JsonData;
		public string PlayerId;

		public override void Process()
		{
			UIManager.Instance.mentorChatWindows.mentorPlayerChat.ClientUpdateChatLog(JsonData, PlayerId);
		}

		public static MentorPlayerChatUpdateMessage SendSingleEntryToAdmins(MentorChatMessage chatMessage, string playerId)
		{
			MentorChatUpdate update = new MentorChatUpdate();
			update.messages.Add(chatMessage);
			MentorPlayerChatUpdateMessage msg =
				new MentorPlayerChatUpdateMessage { JsonData = JsonUtility.ToJson(update), PlayerId = playerId };

			msg.SendToAdmins();
			return msg;
		}

		public static MentorPlayerChatUpdateMessage SendLogUpdateToAdmin(NetworkConnection requestee, MentorChatUpdate update, string playerId)
		{
			MentorPlayerChatUpdateMessage msg =
				new MentorPlayerChatUpdateMessage
				{
					JsonData = JsonUtility.ToJson(update),
					PlayerId = playerId
				};

			msg.SendTo(requestee);
			return msg;
		}
	}
}