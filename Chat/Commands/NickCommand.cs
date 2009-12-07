using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Confabulation.Chat;

namespace Confabulation.Chat.Commands
{
	public class NickCommand : IrcCommand
	{
		public static new NickCommand Parse(string parameters)
		{
			if (parameters == null || parameters.Length == 0)
				throw new IrcCommandException(IrcCommandExceptionType.TooFewParameters, syntax);

			return new NickCommand(parameters);
		}

		static NickCommand()
		{
			IrcCommand.Register("nick", NickCommand.Parse);
		}

		public NickCommand(string nickname)
		{
			if (nickname == null)
				throw new ArgumentNullException("nickname");

			this.nickname = nickname;
		}

		public override void Execute(IrcClient client)
		{
			if (client == null)
				throw new ArgumentNullException("client");

			if (!Irc.IsValidNickname(nickname))
			{
				throw new IrcCommandException(IrcCommandExceptionType.InvalidParameter,
											  "'" + nickname + "' is not a valid nickname");
			}

			// TODO: Add server encoding setting and use that here
			client.Send(new IrcMessage(command, Encoding.UTF8.GetBytes(nickname)));
		}

		private string nickname;

		private const string syntax = "/nick <nick>";
		private static readonly byte[] command = Encoding.UTF8.GetBytes("NICK");
	}
}
