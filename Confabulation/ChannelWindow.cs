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

		public override void UserQuit(IrcUser user, string message)
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

			string text = user.Nickname + " went offline";

			if (message != null)
				text += " (" + message + ")";

			AddControlMessage(text);
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

		private delegate void AddUsersDelegate();
		private delegate void UserJoinedDelegate(IrcChannelUser user);
		private delegate void UserPartedDelegate(IrcChannelUser user, string message);
		private delegate void UserChangedNicknameDelegate(IrcUser user, string oldNickname, string newNickname);

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
			AddControlMessage(user.Nickname + " entered the channel");

			usersList.Items.Add(user);
			user.NicknameChanged += nicknameChangedEventHandler;
		}

		private void UserParted(IrcChannelUser user, string message)
		{
			string text = user.Nickname + " left the channel";

			if (message != null)
				text += " (" + message + ")";

			AddControlMessage(text);

			user.NicknameChanged -= nicknameChangedEventHandler;
			usersList.Items.Remove(user);
		}

		private void UserChangedNickname(IrcUser user, string oldNickname, string newNickname)
		{
			string text = oldNickname + " changed name to " + newNickname;

			AddControlMessage(text);
		}

		private IrcChannel channel = null;
		private EventHandler<IrcUserEventArgs> nicknameChangedEventHandler;
	}
}
