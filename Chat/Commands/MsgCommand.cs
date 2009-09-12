using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Confabulation.Chat.Commands
{
	public static class MsgCommand
	{
		public static void ParseAndExecute(IrcClient client, string parameters)
		{
			if (parameters == null || parameters.Length == 0)
				throw new IrcCommandException(IrcCommandExceptionType.TooFewParameters, syntax);

			string[] splitParams = parameters.Split(new char[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);

			if (splitParams.Length < 2)
				throw new IrcCommandException(IrcCommandExceptionType.TooFewParameters, syntax);

			Execute(client, splitParams[0], splitParams[1]);
		}

		public static void Execute(IrcClient client, string target, string message)
		{
			if (client == null)
				throw new ArgumentNullException("client");
			else if (target == null)
				throw new ArgumentNullException("target");
			else if (message == null)
				throw new ArgumentNullException("message");

			if (!Irc.IsValidMessageTarget(target))
			{
				throw new IrcCommandException(IrcCommandExceptionType.InvalidParameter,
											  "'" + target + "' is not a valid message target");
			}

			if (!Irc.IsValidMessageContent(message))
			{
				throw new IrcCommandException(IrcCommandExceptionType.InvalidParameter,
											  "Invalid message content");
			}

			client.Send(new IrcMessage(command, Encoding.UTF8.GetBytes(target), Encoding.UTF8.GetBytes(message)));
		}

		private const string syntax = "/msg <target> <message>";
		private static readonly byte[] command = Encoding.UTF8.GetBytes("PRIVMSG");
	}
}
