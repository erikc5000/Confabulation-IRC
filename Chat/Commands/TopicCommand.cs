using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Confabulation.Chat;

namespace Confabulation.Chat.Commands
{
	public class TopicCommand : IrcCommand
	{
		public static new TopicCommand Parse(string parameters)
		{
			if (parameters == null || parameters.Length == 0)
				throw new IrcCommandException(IrcCommandExceptionType.TooFewParameters, syntax);

			string[] splitParams = parameters.Split(new char[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);

			if (splitParams.Length == 0)
				throw new IrcCommandException(IrcCommandExceptionType.TooFewParameters, syntax);

			if (splitParams.Length == 1)
				return new TopicCommand(splitParams.First());
			else
				return new TopicCommand(splitParams.First(), splitParams.Last());
		}

		public TopicCommand(string channelName)
		{
			if (channelName == null)
				throw new ArgumentNullException("channelName");

			this.channelName = channelName;
		}

		public TopicCommand(IrcChannel channel)
		{
			if (channel == null)
				throw new ArgumentNullException("channel");

			this.channelName = channel.Name;
		}

		public TopicCommand(string channelName, string topic)
		{
			if (channelName == null)
				throw new ArgumentNullException("channelName");
			else if (topic == null)
				throw new ArgumentNullException("topic");

			this.channelName = channelName;
			this.topic = topic;
		}

		public TopicCommand(IrcChannel channel, string topic)
		{
			if (channel == null)
				throw new ArgumentNullException("channel");
			else if (topic == null)
				throw new ArgumentNullException("topic");

			this.channelName = channel.Name;
			this.topic = topic;
		}

		public override void Execute(IrcClient client)
		{
			if (client == null)
				throw new ArgumentNullException("client");

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

		private string channelName;
		private string topic = null;

		private static readonly byte[] command = Encoding.UTF8.GetBytes("TOPIC");
		private const string syntax = "/topic <channel> [<topic>]";
	}
}
