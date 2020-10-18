using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AdminTools;
using Messages.Client;
using Mirror;

namespace Messages.Client.Mentor
{
	public class MentorCheckMentorMessages : ClientMessage
	{
		public string PlayerId;
		public int CurrentCount;

		public override void Process()
		{
			UIManager.Instance.mentorChatWindows.mentorToMentorChat.ServerGetUnreadMessages(PlayerId, CurrentCount, SentByPlayer.Connection);
		}

		public static MentorCheckMentorMessages Send(string playerId, int currentCount)
		{
			MentorCheckMentorMessages msg = new MentorCheckMentorMessages
			{
				PlayerId = playerId,
				CurrentCount = currentCount
			};
			msg.Send();
			return msg;
		}

		public override void Deserialize(NetworkReader reader)
		{
			base.Deserialize(reader);
			PlayerId = reader.ReadString();
			CurrentCount = reader.ReadInt32();
		}

		public override void Serialize(NetworkWriter writer)
		{
			base.Serialize(writer);
			writer.WriteString(PlayerId);
			writer.WriteInt32(CurrentCount);
		}
	}
}