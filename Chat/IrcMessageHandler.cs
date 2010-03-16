using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Confabulation.Chat.MessageHandlers;

namespace Confabulation.Chat
{
	internal static class IrcMessageHandler
	{
		static IrcMessageHandler()
		{
			Register("JOIN", JoinMessageHandler.Process);
			Register("PART", PartMessageHandler.Process);
			Register("PING", PingMessageHandler.Process);
			Register("NICK", NickMessageHandler.Process);
			Register("PRIVMSG", PrivmsgMessageHandler.Process);
			Register("QUIT", QuitMessageHandler.Process);
			Register("KICK", KickMessageHandler.Process);
			Register("TOPIC", TopicMessageHandler.Process);
		}

		internal static void Process(IrcConnection connection, IrcMessage message)
		{
			if (message.IsNumericReply())
			{
				ReplyMessageHandler.Process(connection, message);
			}
			else
			{
				string command = Encoding.UTF8.GetString(message.GetCommand());

				if (commandMap.ContainsKey(command))
					commandMap[command](connection, message);
			}
		}

		static internal void Register(string command, Action<IrcConnection, IrcMessage> processFunction)
		{
			commandMap.Add(command, processFunction);
		}

		private static Dictionary<string, Action<IrcConnection, IrcMessage>> commandMap =
			new Dictionary<string, Action<IrcConnection, IrcMessage>>();
	}
}
