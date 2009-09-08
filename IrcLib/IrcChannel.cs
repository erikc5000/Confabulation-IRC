using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Confabulation.Chat
{
	//public enum IrcChannelType
	//{
	//    Secret,
	//    Private,
	//    Public
	//}

	//public enum IrcChannelMode
	//{

	//}
	public class IrcTopicInfo
	{
		public IrcTopicInfo(string setBy, DateTime time)
		{
			SetBy = setBy;
			Time = time;
		}

		public String SetBy
		{
			get;
			private set;
		}

		public DateTime Time
		{
			get;
			private set;
		}
	}

    public class IrcChannel
    {
		public IrcChannel(string name, IrcConnection connection)
		{
			if (name == null)
				throw new ArgumentNullException("name");
			else if (connection == null)
				throw new ArgumentNullException("connection");

			this.Name = name;
			this.Connection = connection;
			//this.Type = IrcChannelType.Public;

			this.users = new Dictionary<string, IrcChannelUser>(new IrcNameComparer(connection.ServerProperties));
		}

		public string Name
		{
			get;
			private set;
		}

		//public IrcChannelType Type
		//{
		//    get;
		//    private set;
		//}

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

		public IrcTopicInfo TopicInfo
		{
			get
			{
				lock (topicLock)
				{
					return topicInfo;
				}
			}
		}

		public IEnumerable<IrcChannelUser> Users
		{
			get
			{
				return users.Values;
			}
		}

		public bool IsSecret
		{
			get;
			private set;
		}

		public bool IsPrivate
		{
			get;
			private set;
		}

		public bool RequiresKey
		{
			get;
			private set;
		}

		public string Key
		{
			get;
			private set;
		}

		public bool IsModerated
		{
			get;
			private set;
		}

		public bool IsInviteOnly
		{
			get;
			private set;
		}

		public bool OnlyOperatorsSetTopic
		{
			get;
			private set;
		}

		public uint UserLimit
		{
			get;
			private set;
		}

		public IrcConnection Connection
		{
			get;
			private set;
		}

		public event EventHandler<IrcChannelEventArgs> TopicSet;
		public event EventHandler<IrcChannelEventArgs> TopicInfoSet;
		public event EventHandler<IrcChannelEventArgs> UsersAdded;

		public event EventHandler<IrcChannelEventArgs> UserJoined;
		public event EventHandler<IrcChannelEventArgs> UserParted;
		public event EventHandler<IrcChannelEventArgs> MessageReceived;
		public event EventHandler<IrcChannelEventArgs> NoticeReceived;
		public event EventHandler<IrcChannelEventArgs> TopicChanged;

		internal void SetTopic(string topic)
		{
			lock (topicLock)
			{
				this.topic = topic;

				IrcChannelEventArgs e = new IrcChannelEventArgs(this);
				e.Message = topic;
				EventHandler<IrcChannelEventArgs> handler = TopicSet;

				if (handler != null)
					handler(this, e);
			}
		}

		internal void SetTopicInfo(string setBy, DateTime time)
		{
			lock (topicLock)
			{
				this.topicInfo = new IrcTopicInfo(setBy, time);

				IrcChannelEventArgs e = new IrcChannelEventArgs(this);
				EventHandler<IrcChannelEventArgs> handler = TopicInfoSet;

				if (handler != null)
					handler(this, e);
			}
		}

		internal void ChangeTopic(string topic, IrcUser user)
		{
			lock (topicLock)
			{
				this.topic = topic;
				this.topicInfo = new IrcTopicInfo(user.Nickname, DateTime.Now);

				IrcChannelEventArgs e = new IrcChannelEventArgs(this, user);
				e.Message = topic;
				EventHandler<IrcChannelEventArgs> handler = TopicChanged;

				if (handler != null)
					handler(this, e);
			}
		}

		internal bool UsersInitialized
		{
			get
			{
				lock (usersLock)
				{
					return usersInitialized;
				}
			}
			set
			{
				lock (usersLock)
				{
					usersInitialized = value;
				}
			}
		}

		internal bool HasUser(string nickname)
		{
			lock (usersLock)
			{
				if (users.ContainsKey(nickname))
					return true;
			}

			return false;
		}

		internal void AddUser(IrcUser user, params char[] modes)
		{
			lock (usersLock)
			{
				if (!users.ContainsKey(user.Nickname))
					users[user.Nickname] = new IrcChannelUser(user, modes);

				IrcChannelEventArgs e = new IrcChannelEventArgs(this);
				EventHandler<IrcChannelEventArgs> handler = UsersAdded;

				if (handler != null)
					handler(this, e);
			}
		}

		internal void AddUsers(IrcUser[] newUsers, char[][] modes)
		{
			if (newUsers == null)
				throw new ArgumentNullException("newUsers");
			else if (modes == null)
				throw new ArgumentNullException("modes");
			else if (newUsers.Count() != modes.Count())
				throw new ArgumentException("Number of user should be equal to the number of modes");

			lock (usersLock)
			{
				for (int i = 0; i < newUsers.Count(); i++)
				{
					if (!users.ContainsKey(newUsers[i].Nickname))
						users[newUsers[i].Nickname] = new IrcChannelUser(newUsers[i], modes[i]);
				}

				IrcChannelEventArgs e = new IrcChannelEventArgs(this);
				EventHandler<IrcChannelEventArgs> handler = UsersAdded;

				if (handler != null)
					handler(this, e);
			}
		}

		internal void RemoveUser(IrcUser user)
		{
			lock (usersLock)
			{
				if (!users.ContainsKey(user.Nickname))
					return;

				users.Remove(user.Nickname);
			}
		}

		internal void OnUserJoin(IrcUser user)
		{
			lock (usersLock)
			{
				if (!users.ContainsKey(user.Nickname))
					users[user.Nickname] = new IrcChannelUser(user);

				IrcChannelEventArgs e = new IrcChannelEventArgs(this, user);
				EventHandler<IrcChannelEventArgs> handler = UserJoined;

				if (handler != null)
					handler(this, e);
			}
		}

		internal void OnUserPart(IrcUser user, string message)
		{
			lock (usersLock)
			{
				if (users.ContainsKey(user.Nickname))
					users.Remove(user.Nickname);

				IrcChannelEventArgs e = new IrcChannelEventArgs(this, user);
				e.Message = message;
				EventHandler<IrcChannelEventArgs> handler = UserParted;

				if (handler != null)
					handler(this, e);
			}
		}

		private readonly Object topicLock = new Object();
		private string topic = null;
		private IrcTopicInfo topicInfo = null;
		private readonly Object usersLock = new Object();
		private Dictionary<string, IrcChannelUser> users;
		private bool usersInitialized = false;
		private string rawModes = "";
    }
}
