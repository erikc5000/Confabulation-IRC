using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Confabulation.Chat
{
	public class IrcConnection
	{
		static IrcConnection()
		{
			//commandMap.Add("JOIN", ProcessJoin);

			//replyMap.Add(IrcNumericReply.RPL_ISUPPORT, ProcessISupport);
		}

		public IrcConnection(IrcConnectionSettings settings)
		{
			//client = new IrcClient(
		}

		~IrcConnection()
		{
			client.MessageReceived -= new EventHandler<IrcMessageEventArgs>(ProcessRawMessage);
		}

		public void Initiate()
		{
			
			client.MessageReceived += new EventHandler<IrcMessageEventArgs>(ProcessRawMessage);
		}

		public void Close()
		{

		}

		public bool UserRegistered
		{
			get
			{
				return userRegistered;
			}
		}

		public event EventHandler<IrcChannelEventArgs> OnChannelJoin;
		public event EventHandler<IrcChannelEventArgs> OnChannelPart;
		public event EventHandler<IrcUserEventArgs> UserQuit;
		//public event EventHandler<InviteEventArgs> OnInvite;
		//public event EventHandler<PrivateMessageEventArgs> PrivateMessageReceived;
		public event EventHandler<ErrorEventArgs> OnError;

		private void ProcessRawMessage(object sender, IrcMessageEventArgs e)
		{
			IrcMessage message = e.Message;

			if (message.IsNumericReply())
			{
				IrcNumericReply replyCode = message.GetReplyCode();
				string replyMessage = Encoding.UTF8.GetString(message.Parameters.ElementAt(0));

				if ((int)replyCode > 400)
					ErrorEvent(new ErrorEventArgs(replyCode, replyMessage));
			}
			else
			{

			}
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
				return;

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
					return;

				IrcChannel channel = connection.channels[channelName];
				//channel.
			}
		}

		private static Dictionary<string, Action<IrcMessage>> commandMap = new Dictionary<string,Action<IrcMessage>>();
		private static Dictionary<IrcNumericReply, Action<IrcMessage>> replyMap = new Dictionary<IrcNumericReply,Action<IrcMessage>>();

		private IrcClient client;
		private bool userRegistered = false;
		private IrcUser self = null;
		private Dictionary<byte[], IrcChannel> channels = new Dictionary<byte[],IrcChannel>();
		private Dictionary<byte[], IrcUser> visibleUsers = new Dictionary<byte[],IrcUser>();
	}
}
