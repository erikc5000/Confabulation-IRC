using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Confabulation.Chat.MessageHandlers
{
	internal static class JoinMessageHandler
	{
		internal static void Process(IrcConnection connection, IrcMessage message)
		{
			IrcMessagePrefix prefix = message.Prefix;

			if (prefix == null)
			{
				Log.WriteLine("JOIN message is missing prefix");
				return;
			}

			if (message.Parameters.Count() < 1)
			{
				Log.WriteLine("JOIN message is missing parameters");
				return;
			}

			byte[] byteNickname = prefix.ExtractNickname();
			string nickname = Encoding.UTF8.GetString(byteNickname);

			byte[] byteChannelName = message.Parameters.ElementAt(0);
			string channelName = Encoding.UTF8.GetString(byteChannelName);

			if (connection.User == null)
			{
				Log.WriteLine("Received JOIN message, but user isn't registered");
				return;
			}

			bool isSelf = connection.User.Equals(nickname);
			IrcChannel channel = connection.FindChannel(channelName);

			if (isSelf)
			{
				if (channel != null)
				{
					Log.WriteLine("Received JOIN message for a channel the user is already in");
					return;
				}
			}
			else
			{
				if (channel == null)
				{
					Log.WriteLine("Received JOIN message for a user not in any existing channels");
					return;
				}
			}

			if (channel == null)
				channel = connection.AddChannel(channelName);

			if (isSelf)
			{
				IrcChannelEventArgs e = new IrcChannelEventArgs(channel, connection.User);
				connection.ChannelJoinEvent(e);
			}
			else
			{
				IrcUser user = connection.FindUser(nickname);

				if (user == null)
					user = connection.AddUser(nickname);

				channel.OnUserJoin(user);
			}
		}
	}
}
