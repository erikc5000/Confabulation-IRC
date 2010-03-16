using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Confabulation.Chat;

namespace Confabulation.Chat.Commands
{
	public class QuitCommand : IrcCommand
	{
		public static new QuitCommand Parse(string parameters)
		{
			if (parameters == null || parameters.Length == 0)
				return new QuitCommand();

			return new QuitCommand(parameters);
		}

		public QuitCommand()
		{
		}

		public QuitCommand(string message)
		{
			if (message == null)
				throw new ArgumentNullException("message");

			this.message = message;
		}

		public override void Execute(IrcClient client)
		{
			if (client == null)
				throw new ArgumentNullException("client");

			if (message != null)
			{
				if (!Irc.IsValidMessageContent(message))
				{
					throw new IrcCommandException(IrcCommandExceptionType.InvalidParameter,
												  "Quit message contains invalid characters");
				}

				// TODO: Use server-wide setting for encoding
				client.Send(new IrcMessage(command, Encoding.UTF8.GetBytes(message)));
			}
			else
			{
				client.Send(new IrcMessage(command));
			}
		}

		private string message = null;

		private const string syntax = "/quit [<message>]";
		private static readonly byte[] command = Encoding.UTF8.GetBytes("QUIT");
	}
}
