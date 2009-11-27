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
			lock (syncObject)
			{
				this.topic = topic;
			}

			IrcChannelEventArgs e = new IrcChannelEventArgs(this);
			e.Message = topic;

			EventHandler<IrcChannelEventArgs> handler = TopicSet;

			if (handler != null)
				handler(this, e);
		}

		internal void SetTopicInfo(string setBy, DateTime time)
		{
			lock (syncObject)
			{
				this.topicInfo = new IrcTopicInfo(setBy, time);
			}

			IrcChannelEventArgs e = new IrcChannelEventArgs(this);
			EventHandler<IrcChannelEventArgs> handler = TopicInfoSet;

			if (handler != null)
				handler(this, e);
		}

		internal void ChangeTopic(string topic, IrcUser user)
		{
			lock (syncObject)
			{
				this.topic = topic;
				this.topicInfo = new IrcTopicInfo(user.Nickname, DateTime.Now);
			}

			IrcChannelEventArgs e = new IrcChannelEventArgs(this, user);
			e.Message = topic;
			EventHandler<IrcChannelEventArgs> handler = TopicChanged;

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

		internal void AddUser(IrcUser user, params char[] modes)
		{
			lock (syncObject)
			{
				if (!users.ContainsKey(user.Nickname))
					users[user.Nickname] = new IrcChannelUser(user, modes);

				user.AddChannel(this);
			}

			IrcChannelEventArgs e = new IrcChannelEventArgs(this);
			EventHandler<IrcChannelEventArgs> handler = UsersAdded;

			if (handler != null)
				handler(this, e);
		}

		internal void AddUsers(IrcUser[] newUsers, char[][] modes)
		{
			if (newUsers == null)
				throw new ArgumentNullException("newUsers");
			else if (modes == null)
				throw new ArgumentNullException("modes");
			else if (newUsers.Count() != modes.Count())
				throw new ArgumentException("Number of user should be equal to the number of modes");

			lock (syncObject)
			{
				for (int i = 0; i < newUsers.Count(); i++)
				{
					if (!users.ContainsKey(newUsers[i].Nickname))
						users[newUsers[i].Nickname] = new IrcChannelUser(newUsers[i], modes[i]);

					newUsers[i].AddChannel(this);
				}
			}

			IrcChannelEventArgs e = new IrcChannelEventArgs(this);
			EventHandler<IrcChannelEventArgs> handler = UsersAdded;

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
			IrcChannelUser channelUser = null;
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

			IrcChannelEventArgs e = new IrcChannelEventArgs(this, channelUser);
			EventHandler<IrcChannelEventArgs> handler = UserJoined;

			if (handler != null)
				handler(this, e);
		}

		internal void OnUserPart(IrcUser user, string message)
		{
			IrcChannelUser channelUser = null;

			lock (syncObject)
			{
				string nickname = user.Nickname;

				if (!users.ContainsKey(nickname))
					return;

				channelUser = users[nickname];
				users.Remove(nickname);
				user.RemoveChannel(this);
			}

			IrcChannelEventArgs e = new IrcChannelEventArgs(this, channelUser);
			e.Message = message;
			EventHandler<IrcChannelEventArgs> handler = UserParted;

			if (handler != null)
				handler(this, e);
		}

        internal void OnMessageReceived(IrcUser user, string message)
        {
			IrcChannelUser channelUser = null;

			lock (syncObject)
			{
				string nickname = user.Nickname;

				if (users.ContainsKey(nickname))
					channelUser = users[nickname];
			}

			IrcChannelEventArgs e;
			
			if (channelUser != null)
				e = new IrcChannelEventArgs(this, channelUser); 
			else
				e = new IrcChannelEventArgs(this, user);

            e.Message = message;
            EventHandler<IrcChannelEventArgs> handler = MessageReceived;

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
		private string topic = null;
		private IrcTopicInfo topicInfo = null;
		private Dictionary<string, IrcChannelUser> users;
		private bool usersInitialized = false;
		private string rawModes = "";
    }
}
