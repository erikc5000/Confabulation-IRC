using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Confabulation.Chat.Commands
{
	public static class KickCommand
	{
		public static void ParseAndExecute(IrcClient client, string parameters)
		{
			if (parameters == null || parameters.Length == 0)
				throw new IrcCommandException(IrcCommandExceptionType.TooFewParameters, syntax);

			string[] splitParams = parameters.Split(new char[] { ' ' }, 3, StringSplitOptions.RemoveEmptyEntries);

			if (splitParams.Length < 2)
				throw new IrcCommandException(IrcCommandExceptionType.TooFewParameters, syntax);

			string[] channelNames = splitParams[0].Split(',');
			string[] nicknames = splitParams[1].Split(',');

			if (splitParams.Length == 3)
				Execute(client, channelNames, nicknames, splitParams[2]);
			else
				Execute(client, channelNames, nicknames);
		}

		public static void Execute(IrcClient client, string channelName, string nickname)
		{
			if (client == null)
				throw new ArgumentNullException("client");
			else if (channelName == null)
				throw new ArgumentNullException("channelName");
			else if (nickname == null)
				throw new ArgumentNullException("nickname");

			DoExecute(client, new string[] { channelName }, new string[] { nickname }, null);
		}

		public static void Execute(IrcClient client, string channelName, string nickname, string message)
		{
			if (client == null)
				throw new ArgumentNullException("client");
			else if (channelName == null)
				throw new ArgumentNullException("channelName");
			else if (nickname == null)
				throw new ArgumentNullException("nickname");
			else if (message == null)
				throw new ArgumentNullException("message");

			DoExecute(client, new string[] { channelName }, new string[] { nickname }, message);
		}


		public static void Execute(IrcClient client,
								   string channelName,
								   IEnumerable<string> nicknames)
		{
			if (client == null)
				throw new ArgumentNullException("client");
			else if (channelName == null)
				throw new ArgumentNullException("channelName");
			else if (nicknames == null)
				throw new ArgumentNullException("nicknames");

			DoExecute(client, new string[] { channelName }, nicknames, null);
		}

		public static void Execute(IrcClient client,
								   string channelName,
								   IEnumerable<string> nicknames,
		                           string message)
		{
			if (client == null)
				throw new ArgumentNullException("client");
			else if (channelName == null)
				throw new ArgumentNullException("channelName");
			else if (nicknames == null)
				throw new ArgumentNullException("nicknames");
			else if (message == null)
				throw new ArgumentNullException("message");

			DoExecute(client, new string[] { channelName }, nicknames, message);
		}

		public static void Execute(IrcClient client,
		                           IEnumerable<string> channelNames,
		                           IEnumerable<string> nicknames)
		{
			if (client == null)
				throw new ArgumentNullException("client");
			else if (channelNames == null)
				throw new ArgumentNullException("channelNames");
			else if (nicknames == null)
				throw new ArgumentNullException("nicknames");

			DoExecute(client, channelNames, nicknames, null);
		}

		public static void Execute(IrcClient client,
								   IEnumerable<string> channelNames,
								   IEnumerable<string> nicknames,
		                           string message)
		{
			if (client == null)
				throw new ArgumentNullException("client");
			else if (channelNames == null)
				throw new ArgumentNullException("channelNames");
			else if (nicknames == null)
				throw new ArgumentNullException("nicknames");
			else if (message == null)
				throw new ArgumentNullException("message");

			DoExecute(client, channelNames, nicknames, message);
		}

		private static void DoExecute(IrcClient client,
		                              IEnumerable<string> channelNames,
		                              IEnumerable<string> nicknames,
		                              string message)
		{
			if (channelNames.Count() == 0)
				throw new ArgumentException("No channel names provided");
			else if (nicknames.Count() == 0)
				throw new ArgumentException("No nicknames provided");
			else if (channelNames.Count() > 1 && channelNames.Count() != nicknames.Count())
				throw new ArgumentException("The number of channels must be 1, or the number of nicks");

			foreach (string channel in channelNames)
			{
				if (!Irc.IsValidChannelName(channel))
				{
					throw new IrcCommandException(IrcCommandExceptionType.InvalidParameter,
												  "Invalid channel name");
				}
			}

			byte[] channelList = Encoding.UTF8.GetBytes(Utilities.GetCommaSeparatedList(channelNames));

			foreach (string nick in nicknames)
			{
				if (!Irc.IsValidNickname(nick))
				{
					throw new IrcCommandException(IrcCommandExceptionType.InvalidParameter,
												  "Invalid nickname");
				}
			}

			byte[] nickList = Encoding.UTF8.GetBytes(Utilities.GetCommaSeparatedList(nicknames));

			if (message != null)
			{
				if (!Irc.IsValidMessageContent(message))
					throw new IrcCommandException(IrcCommandExceptionType.InvalidParameter, "Message is invalid");

				client.Send(new IrcMessage(command, channelList, nickList, Encoding.UTF8.GetBytes(message)));
			}
			else
			{
				client.Send(new IrcMessage(command, channelList, nickList));
			}
		}

		private static readonly byte[] command = Encoding.UTF8.GetBytes("KICK");
		private const string syntax = "/kick channel[,channel...] nick[,nick...] [message]";
	}
}
