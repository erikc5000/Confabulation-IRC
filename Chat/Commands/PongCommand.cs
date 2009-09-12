using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Confabulation.Chat;

namespace Confabulation.Chat.Commands
{
	public class PongCommand : IrcCommand
	{
		public PongCommand(string server1)
		{
			if (server1 == null)
				throw new ArgumentNullException("server1");

			this.server1 = server1;
		}

		public PongCommand(string server1, string server2)
		{
			if (server1 == null)
				throw new ArgumentNullException("server1");
			else if (server2 == null)
				throw new ArgumentNullException("server2");

			this.server1 = server1;
			this.server2 = server2;
		}

		public override void Execute(IrcClient client)
		{
			if (client == null)
				throw new ArgumentNullException("client");

			if (!Irc.IsValidMessageContent(server1))
				throw new IrcCommandException(IrcCommandExceptionType.InvalidParameter, "server1 is invalid");

			if (server2 != null)
			{
				if (!Irc.IsValidMessageContent(server1))
					throw new IrcCommandException(IrcCommandExceptionType.InvalidParameter, "server2 is invalid");

				client.Send(new IrcMessage(command, Encoding.UTF8.GetBytes(server1), Encoding.UTF8.GetBytes(server2)));
			}
			else
			{
				client.Send(new IrcMessage(command, Encoding.UTF8.GetBytes(server1)));
			}
		}

		private string server1;
		private string server2 = null;

		private static readonly byte[] command = Encoding.UTF8.GetBytes("PONG");
	}
}
