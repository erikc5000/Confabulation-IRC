using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Confabulation.Chat.Commands;

namespace Confabulation.Chat.MessageHandlers
{
	internal static class PingMessageHandler
	{
		static PingMessageHandler()
		{
			IrcMessageHandler.Register("PING", Process);
		}

		internal static void Process(IrcConnection connection, IrcMessage message)
		{
			// TODO: Revisit
			connection.Execute(new PongCommand(Encoding.UTF8.GetString(message.Parameters.First())));
		}
	}
}
