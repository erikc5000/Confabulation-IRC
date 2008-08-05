using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Confabulation.Chat.Commands
{
	public static class PartAllCommand
	{
		public static void ParseAndExecute(IrcClient client, string parameters)
		{
			Execute(client);
		}

		public static void Execute(IrcClient client)
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
