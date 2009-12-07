using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Confabulation.Chat;

namespace Confabulation.Chat.Commands
{
	public class RawCommand : IrcCommand
	{
		public static new RawCommand Parse(string parameters)
		{
			if (parameters == null || parameters.Length == 0)
				throw new IrcCommandException(IrcCommandExceptionType.InvalidParameter, "No data to send");

			return new RawCommand(parameters);
		}

		static RawCommand()
		{
			IrcCommand.Register("raw", RawCommand.Parse);
		}

		public RawCommand(string message)
		{
			if (message == null)
				throw new ArgumentNullException("message");

			this.message = message;
		}

		public override void Execute(IrcClient client)
		{
			if (client == null)
				throw new ArgumentNullException("client");

			// TODO: Is there a better way to handle the encoding issue?  It's not really correct to
			// apply the same encoding to everything sent out.
			byte[] byteMessage = Encoding.UTF8.GetBytes(message);
			client.Send(new IrcMessage(byteMessage, 0, byteMessage.Length));
		}

		string message;

		private const string syntax = "/raw <message>";
	}
}
