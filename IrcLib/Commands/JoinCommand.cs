using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Confabulation.Chat.Commands
{
	public static class JoinCommand
	{
		public static void ParseAndExecute(IrcClient client, string parameters)
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
				Execute(client, channelNames, keys);
			}

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

		public static void Execute(IrcClient client, string channelName, string key)
		{
			if (client == null)
				throw new ArgumentNullException("client");
			else if (channelName == null)
				throw new ArgumentNullException("channelName");
			else if (key == null)
				throw new ArgumentNullException("key");

			DoExecute(client, new string[] { channelName }, new string[] { key });
		}

		public static void Execute(IrcClient client, IEnumerable<string> channelNames)
		{
			if (client == null)
				throw new ArgumentNullException("client");
			else if (channelNames == null)
				throw new ArgumentNullException("channelNames");
			else if (channelNames.Count() == 0)
				throw new ArgumentException("No channel names provided", "channelNames");

			DoExecute(client, channelNames, null);
		}

		public static void Execute(IrcClient client, IEnumerable<string> channelNames, IEnumerable<string> keys)
		{
			if (client == null)
				throw new ArgumentNullException("client");
			else if (channelNames == null)
				throw new ArgumentNullException("channelNames");
			else if (keys == null)
				throw new ArgumentNullException("keys");
			else if (channelNames.Count() == 0)
				throw new ArgumentException("No channel names provided", "channelNames");

			DoExecute(client, channelNames, keys);
		}

		private static void DoExecute(IrcClient client, IEnumerable<string> channelNames, IEnumerable<string> keys)
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

		private static readonly byte[] command = Encoding.UTF8.GetBytes("JOIN");
		private const string syntax = "/join <channel>[,<channel>...] [<key>[,<key>...]]";
	}
}
