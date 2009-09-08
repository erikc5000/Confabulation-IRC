using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Confabulation.Chat.Commands;
using Confabulation.Chat.MessageHandlers;

namespace Confabulation.Chat
{
	public class IrcConnection
	{
		public IrcConnection(IrcConnectionSettings settings)
		{
			if (settings == null)
				throw new ArgumentNullException("settings");

			this.settings = settings;
			this.nameComparer = new IrcNameComparer(serverProperties);
			this.channels = new Dictionary<string, IrcChannel>(nameComparer);
			this.visibleUsers = new Dictionary<string, IrcUser>(nameComparer);

			IrcServer server = settings.Server;
			this.client = new IrcClient(server.Hostname, server.GetFirstPort());
			this.client.ConnectionEvent += new EventHandler<IrcConnectionEventArgs>(ProcessConnectionEvent);
			this.client.MessageReceived += new EventHandler<IrcMessageEventArgs>(ProcessRawMessage);
		}

		~IrcConnection()
		{
			Close();
		}

		public void Initiate()
		{
			client.Connect();
		}

		public void Close()
		{
			client.Disconnect();
		}

		public void Execute(IrcCommand command)
		{
			command.Execute(client);
		}

		public bool UserRegistered
		{
			get
			{
				lock (userRegisteredLock)
				{
					return userRegistered;
				}
			}
		}

		public IrcConnectionState State
		{
			get
			{
				return client.ConnectionState;
			}
		}

		public IrcUser User
		{
			get
			{
				lock (userRegisteredLock)
				{
					return self;
				}
			}
		}

		public List<IrcChannel> Channels
		{
			get
			{
				lock (channelsLock)
				{
					return new List<IrcChannel>(channels.Values);
				}
			}
		}

		public IrcServerProperties ServerProperties
		{
			get
			{
				return serverProperties;
			}
		}

		public event EventHandler<IrcMessageEventArgs> RawMessageReceived;
		public event EventHandler<IrcConnectionEventArgs> StateChanged;
		public event EventHandler<IrcChannelEventArgs> ChannelJoined;
		public event EventHandler<IrcChannelEventArgs> ChannelParted;
		public event EventHandler<IrcUserEventArgs> UserQuit;
		//public event EventHandler<InviteEventArgs> OnInvite;
		//public event EventHandler<PrivateMessageEventArgs> PrivateMessageReceived;
		//public event EventHandler<PrivateMessageEventArgs> PrivateNoticeReceived;
		public event EventHandler<ErrorEventArgs> OnError;

		private void ProcessRawMessage(object sender, IrcMessageEventArgs e)
		{
			IrcMessageHandler.Process(this, e.Message);

			EventHandler<IrcMessageEventArgs> handler = RawMessageReceived;

			if (handler != null)
				handler(this, e);
		}

		private void ProcessConnectionEvent(object sender, IrcConnectionEventArgs e)
		{
			switch (e.EventType)
			{
				case IrcConnectionEventType.Connected:
					Execute(new NickCommand(settings.User.Nicknames.First()));
					Execute(new UserCommand(settings.User.UserName, settings.InitialUserModes, settings.User.RealName));
					break;

				case IrcConnectionEventType.Disconnected:
					lock (userRegisteredLock)
					{
						userRegistered = false;
						self = null;
					}
					break;
			}

			EventHandler<IrcConnectionEventArgs> handler = StateChanged;

			if (handler != null)
				handler(this, e);
		}

		internal void ErrorEvent(ErrorEventArgs e)
		{
			EventHandler<ErrorEventArgs> handler = OnError;

			if (handler != null)
				handler(this, e);
		}

		internal void ChannelJoinEvent(IrcChannelEventArgs e)
		{
			EventHandler<IrcChannelEventArgs> handler = ChannelJoined;

			if (handler != null)
				handler(this, e);
		}

		internal void ChannelPartEvent(IrcChannelEventArgs e)
		{
			EventHandler<IrcChannelEventArgs> handler = ChannelParted;

			if (handler != null)
				handler(this, e);
		}

		internal void SetUser(IrcUser user)
		{
			lock (userRegisteredLock)
			{
				self = user;
				userRegistered = true;
			}

			IrcConnectionEventArgs e = new IrcConnectionEventArgs(IrcConnectionEventType.Registered);
			EventHandler<IrcConnectionEventArgs> handler = StateChanged;

			if (handler != null)
				handler(this, e);
		}

		internal IrcChannel FindChannel(string channelName)
		{
			lock (channelsLock)
			{
				if (!channels.ContainsKey(channelName))
					return null;

				return channels[channelName];
			}
		}

		internal IrcChannel AddChannel(string channelName)
		{
			lock (channelsLock)
			{
				if (!channels.ContainsKey(channelName))
					channels[channelName] = new IrcChannel(channelName, this);

				return channels[channelName];
			}
		}

		internal void RemoveChannel(IrcChannel channel)
		{
			channels.Remove(channel.Name);

			foreach (IrcChannelUser user in channel.Users)
			{
				bool shouldKeep = false;

				foreach (IrcChannel testChannel in channels.Values)
				{
					if (testChannel.HasUser(user.Nickname))
					{
						shouldKeep = true;
						break;
					}
				}

				if (!shouldKeep)
					visibleUsers.Remove(user.Nickname);
			}
		}

		internal IrcUser FindUser(string nickname)
		{
			lock (visibleUsersLock)
			{
				if (!visibleUsers.ContainsKey(nickname))
					return null;

				return visibleUsers[nickname];
			}
		}

		internal IrcUser AddUser(string nickname)
		{
			lock (visibleUsersLock)
			{
				if (!visibleUsers.ContainsKey(nickname))
					visibleUsers[nickname] = new IrcUser(nickname);

				return visibleUsers[nickname];
			}
		}

		internal void RemoveUser(IrcUser user)
		{
			lock (visibleUsersLock)
			{
				if (!visibleUsers.ContainsKey(user.Nickname))
					return;

				visibleUsers.Remove(user.Nickname);
			}
		}

		private IrcClient client;
		private IrcConnectionSettings settings;
		private IrcServerProperties serverProperties = new IrcServerProperties();
		private readonly Object userRegisteredLock = new Object();
		private bool userRegistered = false;
		private IrcUser self = null;
		private readonly Object channelsLock = new Object();
		private Dictionary<string, IrcChannel> channels;
		private readonly Object visibleUsersLock = new Object();
		private Dictionary<string, IrcUser> visibleUsers;
		private IrcNameComparer nameComparer;
	}
}
