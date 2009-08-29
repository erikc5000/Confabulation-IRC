using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Confabulation.Chat;

namespace Confabulation.Chat.Commands
{
	public class UserCommand : IrcCommand
	{
		public UserCommand(string userName, InitialUserMode mode, string realName)
		{
			if (userName == null)
				throw new ArgumentNullException("userName");
			else if (realName == null)
				throw new ArgumentNullException("realName");

			this.userName = userName;
			this.mode = mode;
			this.realName = realName;
		}

		public override void Execute(IrcClient client)
		{
			if (client == null)
				throw new ArgumentNullException("client");

			if (!Irc.IsValidUserName(userName))
			{
				throw new IrcCommandException(IrcCommandExceptionType.InvalidParameter,
											  "User name is invalid");
			}

			if (!Irc.IsValidMessageContent(realName))
			{
				throw new IrcCommandException(IrcCommandExceptionType.InvalidParameter,
											  "Real name is invalid");
			}

			byte[] byteUserName = Encoding.UTF8.GetBytes(userName);
			byte[] byteMode = Encoding.UTF8.GetBytes(mode.ToString("d"));
			byte[] unused = new byte[] { (byte)'*' };
			byte[] byteRealName = Encoding.UTF8.GetBytes(realName);

			client.Send(new IrcMessage(command, byteUserName, byteMode, unused, byteRealName));
		}

		private string userName;
		private InitialUserMode mode;
		private string realName;

		private static readonly byte[] command = Encoding.UTF8.GetBytes("USER");
	}
}
