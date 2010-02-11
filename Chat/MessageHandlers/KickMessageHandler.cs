using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Confabulation.Chat.MessageHandlers
{
	internal static class KickMessageHandler
	{
		static KickMessageHandler()
		{
			IrcMessageHandler.Register("KICK", Process);
		}

		internal static void Process(IrcConnection connection, IrcMessage message)
		{
			IrcMessagePrefix prefix = message.Prefix;

			if (prefix == null)
			{
				Log.WriteLine("KICK message is missing prefix");
				return;
			}

			if (message.Parameters.Count() < 2)
			{
				Log.WriteLine("KICK message is missing parameters");
				return;
			}

			byte[] byteKickedByNickname = prefix.ExtractNickname();
			string kickedByNickname = Encoding.UTF8.GetString(byteKickedByNickname);

			byte[] byteChannelName = message.Parameters.ElementAt(0);
			string channelName = Encoding.UTF8.GetString(byteChannelName);

			byte[] byteKickedNickname = message.Parameters.ElementAt(1);
			string kickedNickname = Encoding.UTF8.GetString(byteKickedNickname);

			string kickMessage = null;

			if (message.Parameters.Count() > 2)
			{
				byte[] byteKickMessage = message.Parameters.ElementAt(2);
				kickMessage = Encoding.UTF8.GetString(byteKickMessage);
			}

			if (connection.User == null)
			{
				Log.WriteLine("Received KICK message, but user isn't registered");
				return;
			}

			IrcChannel channel = connection.FindChannel(channelName);

			if (channel == null)
			{
				Log.WriteLine("Received KICK message for a channel we aren't in");
				return;
			}

			bool isSelf = connection.User.Equals(kickedNickname);

			if (isSelf)
			{
				// TODO: Add kick rejoin handling
				connection.LeaveChannel(channel, kickMessage);
			}
			else
			{
				IrcUser kicked = connection.FindUser(kickedNickname);
				IrcUser kickedBy = connection.FindUser(kickedByNickname);

				if (kicked == null || kickedBy == null)
				{
					Log.WriteLine("Received KICK message involving users that don't exist");
					return;
				}

				channel.OnKick(kicked, kickedBy, kickMessage);

				if (kicked.Channels.Count() == 0)
					connection.RemoveUser(kicked);
			}
		}
	}
}
