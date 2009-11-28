using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Confabulation.Chat.Events;

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
			connection.StateChanged += new EventHandler<IrcConnectionEventArgs>(connection_StateChanged);

			this.IsSecret = false;
			this.IsPrivate = false;
			this.IsInviteOnly = false;
			this.IsModerated = false;
			this.Key = null;
			this.RequiresKey = false;
			this.OnlyOperatorsSetTopic = false;
			this.UserLimit = 0;
			//this.Type = IrcChannelType.Public;

			this.users = new Dictionary<string, IrcChannelUser>(new IrcNameComparer(connection.ServerProperties));
		}

		//public override bool Equals(object obj)
		//{
		//    if (obj == null)
		//        return false;

		//    if (obj is IrcChannel)
		//    {
		//        IrcChannel other = (IrcChannel)obj;
		//        return Irc.NamesEqual(other.Name, Name, Connection.ServerProperties);
		//    }

		//    return false;
		//}

		public bool Equals(string channelName)
		{
			return Irc.NamesEqual(channelName, Name, Connection.ServerProperties);
		}

		public IrcChannelUser GetChannelUser(IrcUser user)
		{
			if (user == null)
				throw new ArgumentNullException("user");

			return GetChannelUser(user.Nickname);
		}

		public IrcChannelUser GetChannelUser(string nickname)
		{
			if (nickname == null)
				throw new ArgumentNullException("user");

			lock (syncObject)
			{
				if (users.ContainsKey(nickname))
					return users[nickname];
			}

			return null;
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
				lock (syncObject)
				{
					return topic;
				}
			}
		}

		public IrcTopicInfo TopicInfo
		{
			get
			{
				lock (syncObject)
				{
					return topicInfo;
				}
			}
		}

		public IEnumerable<IrcChannelUser> Users
		{
			get
			{
                lock (syncObject)
                {
                    return new List<IrcChannelUser>(users.Values);
                }
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

		public event EventHandler<TopicEventArgs> TopicSet;
		public event EventHandler<TopicEventArgs> TopicInfoSet;
		public event EventHandler<ChannelEventArgs> UsersAdded;

		public event EventHandler<ChannelEventArgs> UserJoined;
		public event EventHandler<ChannelEventArgs> UserParted;
		public event EventHandler<KickEventArgs> UserKicked;
		public event EventHandler<UserEventArgs> MessageReceived;
		public event EventHandler<UserEventArgs> NoticeReceived;
		public event EventHandler<TopicEventArgs> TopicChanged;

		internal void SetTopic(string topic)
		{
			if (topic == null)
				throw new ArgumentNullException("topic");

			lock (syncObject)
			{
				this.topic = topic;
			}

			TopicEventArgs e = new TopicEventArgs(this, topic);
			EventHandler<TopicEventArgs> handler = TopicSet;

			if (handler != null)
				handler(this, e);
		}

		internal void SetTopicInfo(string setBy, DateTime time)
		{
			IrcTopicInfo info;
			string topic;

			lock (syncObject)
			{
				info = new IrcTopicInfo(setBy, time);
				this.topicInfo = info;
				topic = this.topic;
			}

			TopicEventArgs e = new TopicEventArgs(this, topic, info);
			EventHandler<TopicEventArgs> handler = TopicInfoSet;

			if (handler != null)
				handler(this, e);
		}

		internal void ChangeTopic(string topic, IrcUser user)
		{
			IrcTopicInfo info;

			lock (syncObject)
			{
				this.topic = topic;
				info = new IrcTopicInfo(user.Nickname, DateTime.Now);
				this.topicInfo = info;
			}

			TopicEventArgs e = new TopicEventArgs(this, topic, user);
			EventHandler<TopicEventArgs> handler = TopicChanged;

			if (handler != null)
				handler(this, e);
		}

		internal bool UsersInitialized
		{
			get
			{
				lock (syncObject)
				{
					return usersInitialized;
				}
			}
			set
			{
				lock (syncObject)
				{
					usersInitialized = value;
				}
			}
		}

		internal bool HasUser(string nickname)
		{
			lock (syncObject)
			{
				return users.ContainsKey(nickname);
			}
		}

		internal void AddUsers(IEnumerable<IrcUser> newUsers, IEnumerable<char[]> modes)
		{
			if (newUsers == null)
				throw new ArgumentNullException("newUsers");
			else if (modes == null)
				throw new ArgumentNullException("modes");
			else if (newUsers.Count() != modes.Count())
				throw new ArgumentException("Number of user should be equal to the number of modes");

			List<IrcChannelUser> channelUsers = new List<IrcChannelUser>();

			lock (syncObject)
			{
				for (int i = 0; i < newUsers.Count(); i++)
				{
					IrcUser newUser = newUsers.ElementAt(i);
					string nickname = newUser.Nickname;

					if (!users.ContainsKey(nickname))
					{
						users[nickname] = new IrcChannelUser(newUser, modes.ElementAt(i));
						channelUsers.Add(users[nickname]);
					}

					newUser.AddChannel(this);
				}
			}

			ChannelEventArgs e = new ChannelEventArgs(this, channelUsers);
			EventHandler<ChannelEventArgs> handler = UsersAdded;

			if (handler != null)
				handler(this, e);
		}

		internal void RemoveUser(IrcUser user)
		{
			lock (syncObject)
			{
				users.Remove(user.Nickname);
				user.RemoveChannel(this);
			}
		}

		internal void OnUserJoin(IrcUser user)
		{
			IrcChannelUser channelUser;
			string nickname = user.Nickname;

			lock (syncObject)
			{
				if (users.ContainsKey(nickname))
				{
					Log.WriteLine("IrcChannel.OnUserJoin(): User already exists");
					channelUser = users[nickname];
				}
				else
				{
					channelUser = new IrcChannelUser(user);
					users[nickname] = channelUser;

					user.AddChannel(this);
				}
			}

			ChannelEventArgs e = new ChannelEventArgs(this, channelUser);
			EventHandler<ChannelEventArgs> handler = UserJoined;

			if (handler != null)
				handler(this, e);
		}

		internal void OnUserPart(IrcUser user, string message)
		{
			IrcChannelUser channelUser;

			lock (syncObject)
			{
				string nickname = user.Nickname;

				if (!users.ContainsKey(nickname))
					return;

				channelUser = users[nickname];
				users.Remove(nickname);
				user.RemoveChannel(this);
			}

			ChannelEventArgs e = new ChannelEventArgs(this, channelUser, message);
			EventHandler<ChannelEventArgs> handler = UserParted;

			if (handler != null)
				handler(this, e);
		}

		internal void OnKick(IrcUser kicked, IrcUser kickedBy, string message)
		{
			IrcChannelUser kickedUser;
			IrcChannelUser kickedByUser;

			lock (syncObject)
			{
				string kickedNickname = kicked.Nickname;

				if (!users.ContainsKey(kickedNickname))
				{
					Log.WriteLine("Kicked user not found in IrcChannel");
					return;
				}

				string kickedByNickname = kickedBy.Nickname;

				if (!users.ContainsKey(kickedByNickname))
				{
					Log.WriteLine("Kicking user not found in IrcChannel");

					// Remove kicked user despite the error
					users.Remove(kickedNickname);
					kicked.RemoveChannel(this);
					return;
				}

				kickedByUser = users[kickedByNickname];
				kickedUser = users[kickedNickname];

				users.Remove(kickedNickname);
				kicked.RemoveChannel(this);
			}

			KickEventArgs e = new KickEventArgs(this, kickedUser, kickedByUser, message);
			EventHandler<KickEventArgs> handler = UserKicked;

			if (handler != null)
				handler(this, e);
		}

        internal void OnMessageReceived(IrcUser user, string message)
        {
			UserEventArgs e = new UserEventArgs(user, message);
            EventHandler<UserEventArgs> handler = MessageReceived;

            if (handler != null)
                handler(this, e);
        }

		private void connection_StateChanged(object sender, IrcConnectionEventArgs e)
		{
			if (e.EventType == IrcConnectionEventType.Disconnected)
			{
				lock (syncObject)
				{
					users.Clear();
					usersInitialized = false;
					topic = null;
					topicInfo = null;
				}
			}
		}

		private readonly Object syncObject = new Object();
		private string topic = "";
		private IrcTopicInfo topicInfo = null;
		private Dictionary<string, IrcChannelUser> users;
		private bool usersInitialized = false;
		private string rawModes = "";
    }
}
