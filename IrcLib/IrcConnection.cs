using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Confabulation.Chat.Commands;

namespace Confabulation.Chat
{
	public class IrcConnection
	{
		static IrcConnection()
		{
			commandMap.Add("JOIN", ProcessJoin);
			commandMap.Add("PART", ProcessPart);

			//replyMap.Add(IrcNumericReply.RPL_ISUPPORT, ProcessISupport);
		}

		public IrcConnection(IrcConnectionSettings settings)
		{
			if (settings == null)
				throw new ArgumentNullException("settings");

			this.settings = settings;

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
			private set
			{
				lock (userRegisteredLock)
				{
					userRegistered = value;
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

		public IrcUser Self
		{
			get
			{
				return self;
			}
		}

		public event EventHandler<IrcConnectionEventArgs> StateChanged;
		public event EventHandler<IrcChannelEventArgs> ChannelJoined;
		public event EventHandler<IrcChannelEventArgs> ChannelParted;
		public event EventHandler<IrcUserEventArgs> UserQuit;
		//public event EventHandler<InviteEventArgs> OnInvite;
		//public event EventHandler<PrivateMessageEventArgs> PrivateMessageReceived;
		public event EventHandler<ErrorEventArgs> OnError;

		private void ProcessRawMessage(object sender, IrcMessageEventArgs e)
		{
			IrcMessage message = e.Message;

			if (message.IsNumericReply())
			{
				//IrcNumericReply replyCode = message.GetReplyCode();
				//string replyMessage = Encoding.UTF8.GetString(message.Parameters.ElementAt(0));

				//if ((int)replyCode > 400)
				//    ErrorEvent(new ErrorEventArgs(replyCode, replyMessage));
			}
			else
			{
				string command = Encoding.UTF8.GetString(message.GetCommand());
				commandMap[command](this, message);
			}
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
					UserRegistered = false;
					break;
			}

			EventHandler<IrcConnectionEventArgs> handler = StateChanged;

			if (handler != null)
				handler(this, e);
		}

		private void ErrorEvent(ErrorEventArgs args)
		{
			EventHandler<ErrorEventArgs> handler = OnError;

			if (handler != null)
				handler(this, args);
		}

		private static void ProcessJoin(IrcConnection connection, IrcMessage message)
		{
			IrcMessagePrefix prefix = message.Prefix;

			if (prefix == null)
			{
				Log.WriteLine("JOIN message is missing prefix");
				return;
			}

			if (message.Parameters.Count() < 1)
			{
				Log.WriteLine("JOIN message is missing parameters");
				return;
			}

			byte[] byteNickname = prefix.ExtractNickname();
			string nickname = Encoding.UTF8.GetString(byteNickname);

			byte[] byteChannelName = message.Parameters.ElementAt(0);
			string channelName = Encoding.UTF8.GetString(byteChannelName);

			if (connection.self == null)
			{
				Log.WriteLine("Received JOIN message, but user isn't registered");
				return;
			}

			if (nickname.Equals(connection.Self.Nickname))
			{
				lock (connection.channelsLock)
				{
					if (connection.channels.ContainsKey(channelName))
					{
						Log.WriteLine("Received JOIN message for a channel the user is already in");
						return;
					}
				}
			}
			else
			{
				lock (connection.channelsLock)
				{
					if (!connection.channels.ContainsKey(channelName))
					{
						Log.WriteLine("Received JOIN message for a user not in any existing channels");
						return;
					}
				}
			}

			IrcChannel channel = connection.GetChannel(channelName);
			IrcUser user = connection.GetUser(nickname);

			channel.Users.Add(user);

			IrcChannelEventArgs e = new IrcChannelEventArgs(channel, user);
			EventHandler<IrcChannelEventArgs> handler = connection.ChannelJoined;

			if (handler != null)
				handler(connection, e);
		}

		private static void ProcessPart(IrcConnection connection, IrcMessage message)
		{
			IrcMessagePrefix prefix = message.Prefix;

			if (prefix == null)
			{
				Log.WriteLine("PART message is missing prefix");
				return;
			}

			if (message.Parameters.Count() < 1)
			{
				Log.WriteLine("PART message is missing parameters");
				return;
			}

			byte[] byteNickname = prefix.ExtractNickname();
			string nickname = Encoding.UTF8.GetString(byteNickname);

			byte[] byteChannelName = message.Parameters.ElementAt(0);
			string channelName = Encoding.UTF8.GetString(byteChannelName);

			string partMessage = null;

			if (message.Parameters.Count() > 1)
			{
				byte[] bytePartMessage = message.Parameters.ElementAt(1);
				partMessage = Encoding.UTF8.GetString(bytePartMessage);
			}

			if (connection.self == null)
			{
				Log.WriteLine("Received PART message, but user isn't registered");
				return;
			}

			if (nickname.Equals(connection.Self.Nickname))
			{
				lock (connection.channelsLock)
				{
					if (!connection.channels.ContainsKey(channelName))
					{
						Log.WriteLine("Received PART message for a channel the user is not in");
						return;
					}
				}
			}
			else
			{
				lock (connection.channelsLock)
				{
					if (connection.channels.ContainsKey(channelName))
					{
						Log.WriteLine("Received PART message for a user not in any existing channels");
						return;
					}
				}
			}

			IrcChannel channel = connection.GetChannel(channelName);
			IrcUser user = connection.GetUser(nickname);

			IrcChannelEventArgs e = new IrcChannelEventArgs(channel, user);
			e.Message = partMessage;
			EventHandler<IrcChannelEventArgs> handler = connection.ChannelParted;

			if (handler != null)
				handler(connection, e);

			if (nickname.Equals(connection.Self.Nickname))
				connection.RemoveChannel(channel);
		}

		private IrcChannel GetChannel(string channelName)
		{
			lock (channelsLock)
			{
				if (channels.ContainsKey(channelName))
					channels[channelName] = new IrcChannel(channelName);

				return channels[channelName];
			}
		}

		private void RemoveChannel(IrcChannel channel)
		{
			foreach (IrcUser user in channel.Users)
			{
				bool shouldKeep = false;

				foreach (IrcChannel testChannel in channels.Values)
				{
					if (testChannel.Users.Contains(user))
					{
						shouldKeep = true;
						break;
					}
				}

				if (!shouldKeep)
					visibleUsers.Remove(user.Nickname);
			}

			channels.Remove(channel.Name);
		}

		private IrcUser GetUser(string userName)
		{
			lock (visibleUsersLock)
			{
				if (visibleUsers.ContainsKey(userName))
					visibleUsers[userName] = new IrcUser(userName);

				return visibleUsers[userName];
			}
		}

		

		private static Dictionary<string, Action<IrcConnection,IrcMessage>> commandMap =
			new Dictionary<string,Action<IrcConnection,IrcMessage>>();

		private static Dictionary<IrcNumericReply, Action<IrcMessage>> replyMap =
			new Dictionary<IrcNumericReply,Action<IrcMessage>>();

		private IrcClient client;
		private IrcConnectionSettings settings;
		private Object userRegisteredLock = new Object();
		private bool userRegistered = false;
		private IrcUser self = null;
		private Object channelsLock = new Object();
		private Dictionary<string, IrcChannel> channels = new Dictionary<string,IrcChannel>();
		private Object visibleUsersLock = new Object();
		private Dictionary<string, IrcUser> visibleUsers = new Dictionary<string, IrcUser>();
	}
}
