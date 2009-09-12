using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Confabulation.Chat;

namespace Confabulation.Chat.Commands
{
	public class KickCommand : IrcCommand
	{
		public static new KickCommand Parse(string parameters)
		{
			if (parameters == null || parameters.Length == 0)
				throw new IrcCommandException(IrcCommandExceptionType.TooFewParameters, syntax);

			string[] splitParams = parameters.Split(new char[] { ' ' }, 3, StringSplitOptions.RemoveEmptyEntries);

			if (splitParams.Length < 2)
				throw new IrcCommandException(IrcCommandExceptionType.TooFewParameters, syntax);

			string[] channelNames = splitParams[0].Split(',');
			string[] nicknames = splitParams[1].Split(',');

			if (splitParams.Length == 3)
				return new KickCommand(channelNames, nicknames, splitParams[2]);
			else
				return new KickCommand(channelNames, nicknames);
		}

		public KickCommand(string channelName, string nickname)
		{
			if (channelName == null)
				throw new ArgumentNullException("channelName");
			else if (nickname == null)
				throw new ArgumentNullException("nickname");

			this.channelNames = new string[] { channelName };
			this.nicknames = new string[] { nickname };
		}

		public KickCommand(string channelName, string nickname, string message)
		{
			if (channelName == null)
				throw new ArgumentNullException("channelName");
			else if (nickname == null)
				throw new ArgumentNullException("nickname");
			else if (message == null)
				throw new ArgumentNullException("message");

			this.channelNames = new string[] { channelName };
			this.nicknames = new string[] { nickname };
			this.message = message;
		}

		public KickCommand(string channelName, string[] nicknames)
		{
			if (channelName == null)
				throw new ArgumentNullException("channelName");
			else if (nicknames == null)
				throw new ArgumentNullException("nicknames");
			else if (nicknames.Count() == 0)
				throw new ArgumentException("No nicknames provided");

			this.channelNames = new string[] { channelName };
			this.nicknames = nicknames;
		}

		public KickCommand(string channelName, string[] nicknames, string message)
		{
			if (channelName == null)
				throw new ArgumentNullException("channelName");
			else if (nicknames == null)
				throw new ArgumentNullException("nicknames");
			else if (nicknames.Count() == 0)
				throw new ArgumentException("No nicknames provided");
			else if (message == null)
				throw new ArgumentNullException("message");

			this.channelNames = new string[] { channelName };
			this.nicknames = nicknames;
			this.message = message;
		}

		public KickCommand(string[] channelNames, string[] nicknames)
		{
			if (channelNames == null)
				throw new ArgumentNullException("channelNames");
			else if (channelNames.Count() == 0)
				throw new ArgumentException("No channel names provided");
			else if (nicknames == null)
				throw new ArgumentNullException("nicknames");
			else if (nicknames.Count() == 0)
				throw new ArgumentException("No nicknames provided");
			else if (channelNames.Count() > 1 && channelNames.Count() != nicknames.Count())
				throw new ArgumentException("The number of channels must be 1, or the number of nicks");

			this.channelNames = channelNames;
			this.nicknames = nicknames;
		}

		public KickCommand(string[] channelNames, string[] nicknames, string message)
		{
			if (channelNames == null)
				throw new ArgumentNullException("channelNames");
			else if (channelNames.Count() == 0)
				throw new ArgumentException("No channel names provided");
			else if (nicknames == null)
				throw new ArgumentNullException("nicknames");
			else if (nicknames.Count() == 0)
				throw new ArgumentException("No nicknames provided");
			else if (message == null)
				throw new ArgumentNullException("message");
			else if (channelNames.Count() > 1 && channelNames.Count() != nicknames.Count())
				throw new ArgumentException("The number of channels must be 1, or the number of nicks");

			this.channelNames = channelNames;
			this.nicknames = nicknames;
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

		private string[] channelNames = null;
		private string[] nicknames = null;
		private string message = null;

		private static readonly byte[] command = Encoding.UTF8.GetBytes("KICK");
		private const string syntax = "/kick channel[,channel...] nick[,nick...] [message]";
	}
}
