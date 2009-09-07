using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Confabulation.Chat
{
    public class IrcChannel
    {
		public IrcChannel(string name)
		{
			if (name == null)
				throw new ArgumentNullException("name");

			this.Name = name;
		}



		public string Name
		{
			get;
			private set;
		}

		public string Topic
		{
			get
			{
				lock (topicLock)
				{
					return topic;
				}
			}
		}

		public List<IrcUser> Users
		{
			get
			{
				return users;
			}
		}

		public event EventHandler<IrcChannelEventArgs> UserJoined;
		public event EventHandler<IrcChannelEventArgs> UserParted;
		public event EventHandler<IrcChannelEventArgs> MessageReceived;
		public event EventHandler<IrcChannelEventArgs> NoticeReceived;
		public event EventHandler<IrcChannelEventArgs> TopicChanged;

		internal void ChangeTopic(string topic)
		{
			lock (topicLock)
			{
				this.topic = topic;

			}
		}

		private readonly Object topicLock = new Object();
		private string topic = null;
		private List<IrcUser> users = new List<IrcUser>();
    }
}
