using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Confabulation.Chat.Events
{
	public class TopicEventArgs : EventArgs
	{
		public TopicEventArgs(IrcChannel channel, string topic)
		{
			if (channel == null)
				throw new ArgumentNullException("channel");

			Channel = channel;
			Topic = topic;
			TopicInfo = null;
			User = null;
		}

		public TopicEventArgs(IrcChannel channel, string topic, IrcTopicInfo topicInfo)
		{
			if (channel == null)
				throw new ArgumentNullException("channel");
			else if (topicInfo == null)
				throw new ArgumentNullException("topicInfo");

			Channel = channel;
			Topic = topic;
			TopicInfo = topicInfo;
			User = null;
		}

		public TopicEventArgs(IrcChannel channel, string topic, IrcUser user)
		{
			if (channel == null)
				throw new ArgumentNullException("channel");
			else if (topic == null)
				throw new ArgumentNullException("topic");
			else if (user == null)
				throw new ArgumentNullException("user");

			Channel = channel;
			Topic = topic;
			TopicInfo = null;
			User = user;
		}

		public IrcChannel Channel
		{
			get;
			private set;
		}

		public string Topic
		{
			get;
			private set;
		}

		public IrcTopicInfo TopicInfo
		{
			get;
			private set;
		}

		public IrcUser User
		{
			get;
			private set;
		}
	}
}
