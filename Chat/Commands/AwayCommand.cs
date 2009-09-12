using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Confabulation.Chat.Commands
{
	public static class AwayCommand
	{
		public static void ParseAndExecute(IrcClient client, string parameters)
		{
			if (parameters == null || parameters.Length == 0)
				Execute(client);

			Execute(client, parameters);
		}

		public static void Execute(IrcClient client)
		{
			if (client == null)
				throw new ArgumentNullException("client");

			client.Send(new IrcMessage(command));
		}

		public static void Execute(IrcClient client, string message)
		{
			if (client == null)
				throw new ArgumentNullException("client");
			else if (message == null)
				throw new ArgumentNullException("message");

			if (!Irc.IsValidMessageContent(message))
			{
				throw new IrcCommandException(IrcCommandExceptionType.InvalidParameter,
				                              "Away message contains invalid characters");
			}

			// TODO: User server-wide setting for encoding
			client.Send(new IrcMessage(command, Encoding.UTF8.GetBytes(message)));
		}

		private const string syntax = "/away [<message>]";
		private static readonly byte[] command = Encoding.UTF8.GetBytes("AWAY");
	}
}
