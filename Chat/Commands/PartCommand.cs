using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Confabulation.Chat.Commands
{
	public static class PartCommand
	{
		public static void ParseAndExecute(IrcClient client, string parameters)
		{
			if (parameters == null || parameters.Length == 0)
				throw new IrcCommandException(IrcCommandExceptionType.TooFewParameters, syntax);

			string[] splitParams = parameters.Split(new char[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);

			if (splitParams.Length == 0)
				throw new IrcCommandException(IrcCommandExceptionType.TooFewParameters, syntax);

			string[] channelNames = splitParams.First().Split(',');

			if (splitParams.Length == 2)
				Execute(client, channelNames, splitParams.Last());
			else
				Execute(client, channelNames);
		}

		public static void Execute(IrcClient client, string channelName)
		{
			if (client == null)
				throw new ArgumentNullException("client");
			else if (channelName == null)
				throw new ArgumentNullException("channelName");

			DoExecute(client, new string[] { channelName }, null);
		}

		public static void Execute(IrcClient client, string channelName, string message)
		{
			if (client == null)
				throw new ArgumentNullException("client");
			else if (channelName == null)
				throw new ArgumentNullException("channelName");
			else if (message == null)
				throw new ArgumentNullException("message");

			DoExecute(client, new string[] { channelName }, message);
		}

		public static void Execute(IrcClient client, IEnumerable<string> channelNames)
		{
			if (client == null)
				throw new ArgumentNullException("client");
			else if (channelNames == null)
				throw new ArgumentNullException("channelNames");
			else if (channelNames.Count() == 0)
				throw new ArgumentException("No channel names provided");

			DoExecute(client, channelNames, null);
		}

		public static void Execute(IrcClient client, IEnumerable<string> channelNames, string message)
		{
			if (client == null)
				throw new ArgumentNullException("client");
			else if (channelNames == null)
				throw new ArgumentNullException("channelNames");
			else if (message == null)
				throw new ArgumentNullException("message");
			else if (channelNames.Count() == 0)
				throw new ArgumentException("No channel names provided");

			DoExecute(client, channelNames, null);
		}

		private static void DoExecute(IrcClient client, IEnumerable<string> channelNames, string message)
		{
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

		private static readonly byte[] command = Encoding.UTF8.GetBytes("PART");
		private const string syntax = "/part <channel>[,<channel>...] [<message>]";
	}
}
