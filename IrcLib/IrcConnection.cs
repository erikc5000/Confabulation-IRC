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
			client.ConnectionEvent += new EventHandler<IrcConnectionEventArgs>(ProcessConnectionEvent);
			client.MessageReceived += new EventHandler<IrcMessageEventArgs>(ProcessRawMessage);
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

			}
		}

		private void ProcessConnectionEvent(object sender, IrcConnectionEventArgs e)
		{
			switch (e.EventType)
			{
				case IrcConnectionEventType.Connected:
					NickCommand.Execute(client, settings.User.Nicknames.First());
					UserCommand.Execute(client, settings.User.UserName, settings.InitialUserModes, settings.User.RealName);
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
				return;

			if (message.Parameters.Count() < 1)
			{
				Log.WriteLine("JOIN message is missing parameters");
				return;
			}

			byte[] nickname = prefix.ExtractNickname();
			byte[] channelName = message.Parameters.ElementAt(0);

			if (nickname.Equals(connection.self.Nickname))
			{
				IrcChannel newChannel = new IrcChannel(channelName);
				connection.channels[channelName] = newChannel;
			}
			else
			{
				if (!connection.channels.ContainsKey(channelName))
				{
					Log.WriteLine("Received JOIN message for a user not in any existing channels");
				}

				IrcChannel channel = connection.channels[channelName];
				//channel.
			}
		}

		private static void ProcessPart(IrcConnection connection, IrcMessage message)
		{
			IrcMessagePrefix prefix = message.Prefix;

			if (prefix == null)
				return;

			if (message.Parameters.Count() < 1)
			{
				Log.WriteLine("PART message is missing parameters");
				return;
			}

			byte[] nickname = prefix.ExtractNickname();
			byte[] channelName = message.Parameters.ElementAt(0);

			if (nickname.Equals(connection.self.Nickname))
			{
				IrcChannel newChannel = new IrcChannel(channelName);
				connection.channels[channelName] = newChannel;
			}
			else
			{
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
		private Dictionary<byte[], IrcChannel> channels = new Dictionary<byte[],IrcChannel>();
		private Dictionary<byte[], IrcUser> visibleUsers = new Dictionary<byte[], IrcUser>();
	}
}
