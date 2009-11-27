using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Confabulation.Chat.MessageHandlers
{
	internal static class TopicMessageHandler
	{
		internal static void Process(IrcConnection connection, IrcMessage message)
		{
			IrcMessagePrefix prefix = message.Prefix;

			if (prefix == null)
			{
				Log.WriteLine("TOPIC message is missing prefix");
				return;
			}

			if (message.Parameters.Count() < 1)
			{
				Log.WriteLine("TOPIC message is missing parameters");
				return;
			}

			if (connection.User == null)
			{
				Log.WriteLine("Received TOPIC message, but user isn't registered");
				return;
			}

			byte[] byteNickname = prefix.ExtractNickname();
			string nickname = Encoding.UTF8.GetString(byteNickname);

			byte[] byteChannelName = message.Parameters.ElementAt(0);
			string channelName = Encoding.UTF8.GetString(byteChannelName);

			string topic;

			if (message.Parameters.Count() > 1)
			{
				byte[] bytetopic = message.Parameters.ElementAt(1);
				topic = Encoding.UTF8.GetString(bytetopic);
			}
			else
			{
				topic = "";
			}

			IrcChannel channel = connection.FindChannel(channelName);

			if (channel == null)
			{
				Log.WriteLine("Received TOPIC message for a channel we aren't in");
				return;
			}

			IrcUser user = connection.FindUser(nickname);

			if (user == null)
			{
				Log.WriteLine("Received TOPIC message from a user that doesn't exist");
				return;
			}

			channel.ChangeTopic(topic, user);
		}
	}
}
