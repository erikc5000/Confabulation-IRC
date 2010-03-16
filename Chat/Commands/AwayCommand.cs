using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Confabulation.Chat;

namespace Confabulation.Chat.Commands
{
	public class AwayCommand : IrcCommand
	{
		public static new AwayCommand Parse(string parameters)
		{
			if (parameters == null || parameters.Length == 0)
				return new AwayCommand();

			return new AwayCommand(parameters);
		}

		public AwayCommand()
		{
		}

		public AwayCommand(string message)
		{
			if (message == null)
				throw new ArgumentNullException("message");

			this.message = message;
		}

		public override void Execute(IrcClient client)
		{
			if (client == null)
				throw new ArgumentNullException("client");

			if (message == null)
			{
				client.Send(new IrcMessage(command));
			}
			else
			{
				if (!Irc.IsValidMessageContent(message))
				{
					throw new IrcCommandException(IrcCommandExceptionType.InvalidParameter,
												  "Away message contains invalid characters");
				}

				// TODO: Use server-wide setting for encoding
				client.Send(new IrcMessage(command, Encoding.UTF8.GetBytes(message)));
			}

		}

		private string message = null;

		private const string syntax = "/away [<message>]";
		private static readonly byte[] command = Encoding.UTF8.GetBytes("AWAY");
	}
}
