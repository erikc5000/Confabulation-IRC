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
			//IrcMessagePrefix prefix = message.Prefix;

			//if (prefix == null)
			//{
			//    Log.WriteLine("PART message is missing prefix");
			//    return;
			//}

			//if (message.Parameters.Count() < 1)
			//{
			//    Log.WriteLine("PART message is missing parameters");
			//    return;
			//}

			//byte[] byteNickname = prefix.ExtractNickname();
			//string nickname = Encoding.UTF8.GetString(byteNickname);

			//byte[] byteChannelName = message.Parameters.ElementAt(0);
			//string channelName = Encoding.UTF8.GetString(byteChannelName);

			//string partMessage = null;

			//if (message.Parameters.Count() > 1)
			//{
			//    byte[] bytePartMessage = message.Parameters.ElementAt(1);
			//    partMessage = Encoding.UTF8.GetString(bytePartMessage);
			//}

			//if (connection.self == null)
			//{
			//    Log.WriteLine("Received PART message, but user isn't registered");
			//    return;
			//}

			//if (nickname.Equals(connection.Self.Nickname))
			//{
			//    lock (connection.channelsLock)
			//    {
			//        if (!connection.channels.ContainsKey(channelName))
			//        {
			//            Log.WriteLine("Received PART message for a channel the user is not in");
			//            return;
			//        }
			//    }
			//}
			//else
			//{
			//    lock (connection.channelsLock)
			//    {
			//        if (connection.channels.ContainsKey(channelName))
			//        {
			//            Log.WriteLine("Received PART message for a user not in any existing channels");
			//            return;
			//        }
			//    }
			//}

			//IrcChannel channel = connection.GetChannel(channelName);
			//IrcUser user = connection.GetUser(nickname);

			//IrcChannelEventArgs e = new IrcChannelEventArgs(channel, user);
			//e.Message = partMessage;
			//EventHandler<IrcChannelEventArgs> handler = connection.OnChannelPart;

			//if (handler != null)
			//    handler(connection, e);

			//if (nickname.Equals(connection.Self.Nickname))
			//    connection.RemoveChannel(channel);
		}
	}
}
