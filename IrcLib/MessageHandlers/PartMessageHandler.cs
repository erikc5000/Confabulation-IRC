using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Confabulation.Chat.MessageHandlers
{
	internal static class PartMessageHandler
	{
		internal static void Process(IrcConnection connection, IrcMessage message)
		{
			IrcMessagePrefix prefix = message.Prefix;

			if (prefix == null)
			{
				Log.WriteLine("PART message is missing prefix");
				return;
			}

			if (message.Parameters.Count() < 1)
			{
				Log.WriteLine("PART message is missing parameters");
				return;
			}

			byte[] byteNickname = prefix.ExtractNickname();
			string nickname = Encoding.UTF8.GetString(byteNickname);

			byte[] byteChannelName = message.Parameters.ElementAt(0);
			string channelName = Encoding.UTF8.GetString(byteChannelName);

			string partMessage = null;

			if (message.Parameters.Count() > 1)
			{
				byte[] bytePartMessage = message.Parameters.ElementAt(1);
				partMessage = Encoding.UTF8.GetString(bytePartMessage);
			}

			if (connection.User == null)
			{
				Log.WriteLine("Received PART message, but user isn't registered");
				return;
			}

			IrcChannel channel = connection.FindChannel(channelName);

			if (channel == null)
			{
				Log.WriteLine("Received PART message for a channel we aren't in");
				return;
			}

			bool isSelf = connection.User.Equals(nickname);

			if (isSelf)
			{
				IrcChannelEventArgs e = new IrcChannelEventArgs(channel, connection.User);
				e.Message = partMessage;
				connection.ChannelPartEvent(e);
				connection.RemoveChannel(channel);
			}
			else
			{
				IrcUser user = connection.FindUser(nickname);

				if (user == null)
				{
					Log.WriteLine("Received PART message from a user that doesn't exist");
					return;
				}

				channel.OnUserPart(user, partMessage);

				if (user.Channels.Count() == 0)
					connection.RemoveUser(user);
			}
		}
	}
}
