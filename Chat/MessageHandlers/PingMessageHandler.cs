using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Confabulation.Chat.Commands;

namespace Confabulation.Chat.MessageHandlers
{
	internal static class PingMessageHandler
	{
		internal static void Process(IrcConnection connection, IrcMessage message)
		{
			// TODO: Revisit
			connection.Execute(new PongCommand(Encoding.UTF8.GetString(message.Parameters.First())));
		}
	}
}
