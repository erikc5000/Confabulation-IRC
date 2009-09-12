using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Confabulation.Chat.Commands
{
	public static class TopicCommand
	{
		public static void ParseAndExecute(IrcClient client, string parameters)
		{
			if (parameters == null || parameters.Length == 0)
				throw new IrcCommandException(IrcCommandExceptionType.TooFewParameters, syntax);

			string[] splitParams = parameters.Split(new char[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);

			if (splitParams.Length == 0)
				throw new IrcCommandException(IrcCommandExceptionType.TooFewParameters, syntax);

			if (splitParams.Length == 1)
				Execute(client, splitParams.First());
			else
				Execute(client, splitParams.First(), splitParams.Last());
		}

		public static void Execute(IrcClient client, string channelName)
		{
			if (client == null)
				throw new ArgumentNullException("client");
			else if (channelName == null)
				throw new ArgumentNullException("channelName");

			DoExecute(client, channelName, null);
		}

		public static void Execute(IrcClient client, string channelName, string topic)
		{
			if (client == null)
				throw new ArgumentNullException("client");
			else if (channelName == null)
				throw new ArgumentNullException("channelName");
			else if (topic == null)
				throw new ArgumentNullException("topic");

			DoExecute(client, channelName, topic);
		}

		private static void DoExecute(IrcClient client, string channelName, string topic)
		{
			if (!Irc.IsValidChannelName(channelName))
			{
				throw new IrcCommandException(IrcCommandExceptionType.InvalidParameter,
											  "Invalid channel name");
			}

			// TODO: Add server-wide encoding setting and use here
			byte[] byteChannelName = Encoding.UTF8.GetBytes(channelName);

			if (topic != null)
			{
				if (!Irc.IsValidMessageContent(topic))
				{
					throw new IrcCommandException(IrcCommandExceptionType.InvalidParameter,
												  "Invalid topic");
				}

				byte[] byteTopic = Encoding.UTF8.GetBytes(topic);

				client.Send(new IrcMessage(command, byteChannelName, byteTopic));
			}
			else
			{
				client.Send(new IrcMessage(command, byteChannelName));
			}
		}

		private static readonly byte[] command = Encoding.UTF8.GetBytes("TOPIC");
		private const string syntax = "/topic <channel> [<topic>]";
	}
}
