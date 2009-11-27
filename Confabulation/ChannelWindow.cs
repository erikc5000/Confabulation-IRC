using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Confabulation;
using Confabulation.Chat;
using Confabulation.Chat.Commands;

namespace Confabulation
{
	public class ChannelWindow : ChatWindow
	{
		public ChannelWindow(IrcChannel channel)
			: base()
		{
			InitializeComponent();

			this.channel = channel;
			nicknameChangedEventHandler = new EventHandler<IrcUserEventArgs>(user_NicknameChanged);
			channel.MessageReceived += new EventHandler<IrcChannelEventArgs>(channel_MessageReceived);
			channel.UsersAdded += new EventHandler<IrcChannelEventArgs>(channel_UsersAdded);
			channel.UserJoined += new EventHandler<IrcChannelEventArgs>(channel_UserJoined);
			channel.Connection.UserQuit += new EventHandler<IrcUserEventArgs>(Connection_UserQuit);
			channel.Connection.StateChanged += new EventHandler<IrcConnectionEventArgs>(Connection_StateChanged);

			AddUsers();
		}

		public override IrcConnection Connection
		{
			get
			{
				return channel.Connection;
			}
		}

		public IrcChannel Channel
		{
			get
			{
				return channel;
			}
		}

		protected override void TextEntered(string text)
		{
			if (Connection != null)
			{
				try
				{
					if (!text.StartsWith("/"))
					{
						text = text.Trim();
						Connection.Execute(new MsgCommand(Channel, text));
						AddMessage(Connection.User, text);
					}
					else
					{
						Connection.Execute(IrcCommand.Parse(text));
					}
				}
				catch (IrcCommandException)
				{
					AddTextToWindow("*Invalid command*");
				}
				catch (ArgumentException ae)
				{
					AddTextToWindow("*Invalid argument*: " + ae.ParamName);
				}
			}
			else
			{
				AddTextToWindow("*Not connected*");
			}
		}

		private delegate void DisconnectedDelegate();
		private delegate void AddUsersDelegate();
		private delegate void UserJoinedDelegate(IrcChannelUser user);
		private delegate void UserPartedDelegate(IrcChannelUser user, string message);
		private delegate void UserQuitDelegate(IrcUser user, string message);
		private delegate void UserChangedNicknameDelegate(IrcUser user, string oldNickname, string newNickname);

		private void Disconnected()
		{
			AddControlMessage("You went offline");
			usersList.Items.Clear();
		}

		private void channel_UsersAdded(object sender, IrcChannelEventArgs e)
		{
			chatLogDocument.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
						new AddUsersDelegate(AddUsers));
		}

		private void channel_UserJoined(object sender, IrcChannelEventArgs e)
		{
			Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
						new UserJoinedDelegate(UserJoined),
						(Object)e.ChannelUser);
		}

		private void channel_UserParted(object sender, IrcChannelEventArgs e)
		{
			Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
						new UserPartedDelegate(UserParted),
						e.User,
						e.Message);
		}

		private void Connection_UserQuit(object sender, IrcUserEventArgs e)
		{
			Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
						new UserQuitDelegate(UserQuit),
						e.User,
						e.Message);
		}

		private void user_NicknameChanged(object sender, IrcUserEventArgs e)
		{
			Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
						new UserChangedNicknameDelegate(UserChangedNickname),
						e.User,
						e.OldNickname,
						e.NewNickname);
		}

		private void channel_MessageReceived(object sender, IrcChannelEventArgs e)
		{
			Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
						new AddMessageDelegate(AddMessage),
						e.User,
						e.Message);
		}

		private void Connection_StateChanged(object sender, IrcConnectionEventArgs e)
		{
			switch (e.EventType)
			{
				case IrcConnectionEventType.Disconnected:
					// TODO: Clear the users list
					Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
						new DisconnectedDelegate(Disconnected));
					break;

				case IrcConnectionEventType.Registered:
					// TODO: Add support for password protected channels.  Might want to merge
					// this functionality into the core library as an option.
					Connection.Execute(new JoinCommand(Channel.Name));
					break;
		}
		}

		private void AddUsers()
		{
			foreach (IrcChannelUser user in Channel.Users)
			{
				if (!usersList.Items.Contains(user))
				{
					user.NicknameChanged += nicknameChangedEventHandler;
					usersList.Items.Add(user);
				}
			}
		}

		private void UserJoined(IrcChannelUser user)
		{
			string text;

			if (user.IsSelf)
				text = "You entered the channel";
			else
				text = user.Nickname + " entered the channel";

			AddControlMessage(text);

			usersList.Items.Add(user);
			user.NicknameChanged += nicknameChangedEventHandler;
		}

		private void UserParted(IrcChannelUser user, string message)
		{
			string text;

			if (user.IsSelf)
				text = "You left the channel";
			else
				text = user.Nickname + " left the channel";

			if (message != null)
				text += " (" + message + ")";

			AddControlMessage(text);

			user.NicknameChanged -= nicknameChangedEventHandler;
			usersList.Items.Remove(user);
		}

		private void UserQuit(IrcUser user, string message)
		{
			bool found = false;

			foreach (IrcChannelUser channelUser in usersList.Items)
			{
				if (channelUser.Equals(user.Nickname))
				{
					found = true;
					usersList.Items.Remove(channelUser);
					break;
				}
			}

			if (!found)
				return;

			string text;

			if (user.IsSelf)
				text = "You went offline";
			else
				text = user.Nickname + " went offline";

			if (message != null)
				text += " (" + message + ")";

			AddControlMessage(text);
		}

		private void UserChangedNickname(IrcUser user, string oldNickname, string newNickname)
		{
			string text;

			if (user.IsSelf)
				text = "You changed your nickname to " + newNickname;
			else
				text = oldNickname + " changed his/her nickname to " + newNickname;

			AddControlMessage(text);
		}

		private IrcChannel channel = null;
		private EventHandler<IrcUserEventArgs> nicknameChangedEventHandler;
	}
}
