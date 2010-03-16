using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Confabulation.Chat;

namespace Confabulation.Chat.Commands
{
	public class PartAllCommand : IrcCommand
	{
		public static new PartAllCommand Parse(string parameters)
		{
			return new PartAllCommand();
		}

		public PartAllCommand()
		{
		}

		public override void Execute(IrcClient client)
		{
			if (client == null)
				throw new ArgumentNullException("client");

			client.Send(new IrcMessage(command, parameter));
		}

		private const string syntax = "/partall";
		private static readonly byte[] command = Encoding.UTF8.GetBytes("JOIN");
		private static readonly byte[] parameter = Encoding.UTF8.GetBytes("0");
	}
}
