using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Confabulation.Chat;

namespace Confabulation.Chat.Commands
{
	public class JoinCommand : IrcCommand
	{
		public static new JoinCommand Parse(string parameters)
		{
			if (parameters == null || parameters.Length == 0)
				throw new IrcCommandException(IrcCommandExceptionType.TooFewParameters, syntax);

			string[] splitParams = parameters.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

			if (splitParams.Length > 2)
				throw new IrcCommandException(IrcCommandExceptionType.TooManyParameters, syntax);

			string[] channelNames = splitParams.First().Split(',');

			if (splitParams.Length == 2)
			{
				string[] keys = splitParams.Last().Split(',');
				return new JoinCommand(channelNames, keys);
			}

			return new JoinCommand(channelNames);
		}

		static JoinCommand()
		{
			IrcCommand.Register("join", JoinCommand.Parse);
		}

		public JoinCommand(string channelName)
		{
			if (channelName == null)
				throw new ArgumentNullException("channelName");

			this.channelNames = new string[] { channelName };
		}

		public JoinCommand(string channelName, string key)
		{
			if (channelName == null)
				throw new ArgumentNullException("channelName");
			else if (key == null)
				throw new ArgumentNullException("key");

			this.channelNames = new string[] { channelName };
			this.keys = new string[] { key };
		}

		public JoinCommand(string[] channelNames)
		{
			if (channelNames == null)
				throw new ArgumentNullException("channelNames");
			else if (channelNames.Count() == 0)
				throw new ArgumentException("No channel names provided", "channelNames");

			this.channelNames = channelNames;
		}

		public JoinCommand(string[] channelNames, string[] keys)
		{
			if (channelNames == null)
				throw new ArgumentNullException("channelNames");
			else if (keys == null)
				throw new ArgumentNullException("keys");
			else if (channelNames.Count() == 0)
				throw new ArgumentException("No channel names provided", "channelNames");

			this.channelNames = channelNames;
			this.keys = keys;
		}

		public override void Execute(IrcClient client)
		{
			foreach (string channel in channelNames)
			{
				if (!Irc.IsValidChannelName(channel))
				{
					throw new IrcCommandException(IrcCommandExceptionType.InvalidParameter,
												  "Invalid channel name");
				}
			}

			string combinedChannelNames = Utilities.GetCommaSeparatedList(channelNames);

			if (keys == null || keys.Count() == 0)
			{
				// TODO: Add server-wide encoding setting and use here
				client.Send(new IrcMessage(command, Encoding.UTF8.GetBytes(combinedChannelNames)));
			}
			else
			{
				foreach (string key in keys)
				{
					if (key.Length > 0 && !Irc.IsValidChannelKey(key))
					{
						throw new IrcCommandException(IrcCommandExceptionType.InvalidParameter,
													  "Invalid channel key");
					}
				}

				string combinedKeys = Utilities.GetCommaSeparatedList(keys);

				// TODO: Add server-wide encoding setting and use here
				client.Send(new IrcMessage(command,
				                           Encoding.UTF8.GetBytes(combinedChannelNames),
				                           Encoding.UTF8.GetBytes(combinedKeys)));
			}
		}

		private string[] channelNames = null;
		private string[] keys = null;

		private static readonly byte[] command = Encoding.UTF8.GetBytes("JOIN");
		private const string syntax = "/join <channel>[,<channel>...] [<key>[,<key>...]]";
	}
}
