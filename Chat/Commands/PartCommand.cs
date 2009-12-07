using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Confabulation.Chat;

namespace Confabulation.Chat.Commands
{
	public class PartCommand : IrcCommand
	{
		public static new PartCommand Parse(string parameters)
		{
			if (parameters == null || parameters.Length == 0)
				throw new IrcCommandException(IrcCommandExceptionType.TooFewParameters, syntax);

			string[] splitParams = parameters.Split(new char[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);

			if (splitParams.Length == 0)
				throw new IrcCommandException(IrcCommandExceptionType.TooFewParameters, syntax);

			string[] channelNames = splitParams.First().Split(',');

			if (splitParams.Length == 2)
				return new PartCommand(channelNames, splitParams.Last());
			else
				return new PartCommand(channelNames);
		}

		static PartCommand()
		{
			IrcCommand.Register("part", PartCommand.Parse);
		}

		public PartCommand(string channelName)
		{
			if (channelName == null)
				throw new ArgumentNullException("channelName");

			this.channelNames = new string[] { channelName };
		}

		public PartCommand(IrcChannel channel)
		{
			if (channel == null)
				throw new ArgumentNullException("channel");

			this.channelNames = new string[] { channel.Name };
		}

		public PartCommand(string channelName, string message)
		{
			if (channelName == null)
				throw new ArgumentNullException("channelName");
			else if (message == null)
				throw new ArgumentNullException("message");

			this.channelNames = new string[] { channelName };
			this.message = message;
		}

		public PartCommand(string[] channelNames)
		{
			if (channelNames == null)
				throw new ArgumentNullException("channelNames");
			else if (channelNames.Count() == 0)
				throw new ArgumentException("No channel names provided");

			this.channelNames = channelNames;
		}

		public PartCommand(string[] channelNames, string message)
		{
			if (channelNames == null)
				throw new ArgumentNullException("channelNames");
			else if (message == null)
				throw new ArgumentNullException("message");
			else if (channelNames.Count() == 0)
				throw new ArgumentException("No channel names provided");

			this.channelNames = channelNames;
			this.message = message;
		}

		public override void Execute(IrcClient client)
		{
			if (client == null)
				throw new ArgumentNullException("client");

			foreach (string channel in channelNames)
			{
				if (!Irc.IsValidChannelName(channel))
				{
					throw new IrcCommandException(IrcCommandExceptionType.InvalidParameter,
												  "Invalid channel name");
				}
			}

			byte[] channelList = Encoding.UTF8.GetBytes(Utilities.GetCommaSeparatedList(channelNames));

			if (message != null)
			{
				if (!Irc.IsValidMessageContent(message))
					throw new IrcCommandException(IrcCommandExceptionType.InvalidParameter, "Message is invalid");

				client.Send(new IrcMessage(command, channelList, Encoding.UTF8.GetBytes(message)));
			}
			else
			{
				client.Send(new IrcMessage(command, channelList));
			}
		}

		private string[] channelNames;
		private string message = null;

		private static readonly byte[] command = Encoding.UTF8.GetBytes("PART");
		private const string syntax = "/part <channel>[,<channel>...] [<message>]";
	}
}
