using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Confabulation.Chat.Commands
{
	public static class NickCommand
	{
		public static void ParseAndExecute(IrcClient client, string parameters)
		{
			if (parameters == null || parameters.Length == 0)
				throw new IrcCommandException(IrcCommandExceptionType.TooFewParameters, syntax);

			Execute(client, parameters);
		}

		public static void Execute(IrcClient client, string nickname)
		{
			if (client == null)
				throw new ArgumentNullException("client");

			if (nickname == null)
				throw new ArgumentNullException("nick");

			if (!Irc.IsValidNickname(nickname))
			{
				throw new IrcCommandException(IrcCommandExceptionType.InvalidParameter,
											  "'" + nickname + "' is not a valid nickname");
			}

			// TODO: Add server encoding setting and use that here
			client.Send(new IrcMessage(command, Encoding.UTF8.GetBytes(nickname)));
		}

		private const string syntax = "/nick <nick>";
		private static readonly byte[] command = Encoding.UTF8.GetBytes("NICK");
	}
}
