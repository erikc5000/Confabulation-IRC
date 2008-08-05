using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Confabulation.Chat.Commands
{
	public static class PongCommand
	{
		public static void Execute(IrcClient client, string server1)
		{
			if (client == null)
				throw new ArgumentNullException("client");
			else if (server1 == null)
				throw new ArgumentNullException("server1");

			if (!Irc.IsValidMessageContent(server1))
				throw new IrcCommandException(IrcCommandExceptionType.InvalidParameter, "server1 is invalid");

			client.Send(new IrcMessage(command, Encoding.UTF8.GetBytes(server1)));
		}

		public static void Execute(IrcClient client, string server1, string server2)
		{
			if (client == null)
				throw new ArgumentNullException("client");
			else if (server1 == null)
				throw new ArgumentNullException("server1");
			else if (server2 == null)
				throw new ArgumentNullException("server2");
			
			if (!Irc.IsValidMessageContent(server1))
				throw new IrcCommandException(IrcCommandExceptionType.InvalidParameter, "server1 is invalid");
			else if (!Irc.IsValidMessageContent(server1))
				throw new IrcCommandException(IrcCommandExceptionType.InvalidParameter, "server1 is invalid");

			client.Send(new IrcMessage(command, Encoding.UTF8.GetBytes(server1), Encoding.UTF8.GetBytes(server2)));
		}

		private static readonly byte[] command = Encoding.UTF8.GetBytes("PONG");
	}
}
