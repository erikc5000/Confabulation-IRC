using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Confabulation.Chat.MessageHandlers;

namespace Confabulation.Chat
{
	internal static class IrcMessageHandler
	{
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

		static IrcMessageHandler()
		{
			commandMap.Add("JOIN", JoinMessageHandler.Process);
			commandMap.Add("PART", PartMessageHandler.Process);
			commandMap.Add("PING", PingMessageHandler.Process);
		}

		private static Dictionary<string, Action<IrcConnection, IrcMessage>> commandMap =
			new Dictionary<string, Action<IrcConnection, IrcMessage>>();
	}
}
