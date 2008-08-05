using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Confabulation.Chat.Commands
{
	public static class RawCommand
	{
		public static void ParseAndExecute(IrcClient client, string parameters)
		{
			if (parameters == null || parameters.Length == 0)
				throw new IrcCommandException(IrcCommandExceptionType.InvalidParameter, "No data to send");

			Execute(client, parameters);
		}

		public static void Execute(IrcClient client, string message)
		{
			if (client == null)
				throw new ArgumentNullException("client");
			else if (message == null)
				throw new ArgumentNullException("message");

			// TODO: Is there a better way to handle the encoding issue?  It's not really correct to
			// apply the same encoding to everything sent out.
			byte[] byteMessage = Encoding.UTF8.GetBytes(message);
			client.Send(new IrcMessage(byteMessage, 0, byteMessage.Length));
		}

		private const string syntax = "/raw <message>";
	}
}
