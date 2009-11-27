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
using System.Collections.ObjectModel;
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
			channel.UserParted += new EventHandler<IrcChannelEventArgs>(channel_UserParted);
			channel.UserKicked += new EventHandler<IrcKickEventArgs>(channel_UserKicked);
			channel.Connection.UserQuit += new EventHandler<IrcUserEventArgs>(Connection_UserQuit);
			channel.Connection.StateChanged += new EventHandler<IrcConnectionEventArgs>(Connection_StateChanged);

			usersList.DataContext = userItems;
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
		private delegate void UserKickedDelegate(IrcChannelUser kicked, IrcChannelUser kickedBy, string message);
		private delegate void UserQuitDelegate(IrcUser user, string message);
		private delegate void UserChangedNicknameDelegate(IrcUser user, string oldNickname, string newNickname);

		private void Disconnected()
		{
			AddControlMessage("You went offline");
			RemoveAllUserItems();
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
						e.ChannelUser);
		}

		private void channel_UserParted(object sender, IrcChannelEventArgs e)
		{
			Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
						new UserPartedDelegate(UserParted),
						e.ChannelUser,
						e.Message);
		}

		private void channel_UserKicked(object sender, IrcKickEventArgs e)
		{
			Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
						new UserKickedDelegate(UserKicked),
						e.Kicked,
						e.KickedBy,
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
				string nickname = user.Nickname;

				if (!userItemMap.ContainsKey(nickname))
				{
					user.NicknameChanged += nicknameChangedEventHandler;
					AddUserItem(user);
				}
			}
		}

		private void UserJoined(IrcChannelUser user)
		{
			string nickname = user.Nickname;
			string text;

			if (user.IsSelf)
				text = "You entered the channel";
			else
				text = nickname + " entered the channel";

			AddControlMessage(text);

			if (!userItemMap.ContainsKey(nickname))
			{
				user.NicknameChanged += nicknameChangedEventHandler;
				AddUserItem(user);
			}
			else
			{
				Log.WriteLine("ChannelWindow: Joining user already exists");
			}
		}

		private void UserParted(IrcChannelUser user, string message)
		{
			string nickname = user.Nickname;
			string text;

			if (user.IsSelf)
				text = "You left the channel";
			else
				text = nickname + " left the channel";

			if (message != null)
				text += " (" + message + ")";

			AddControlMessage(text);

			if (userItemMap.ContainsKey(nickname))
			{
				user.NicknameChanged -= nicknameChangedEventHandler;
				RemoveUserItem(nickname);
			}
			else
			{
				Log.WriteLine("ChannelWindow: Parting user doesn't exist");
			}
		}

		private void UserKicked(IrcChannelUser kicked, IrcChannelUser kickedBy, string message)
		{
			string kickedNickname = kicked.Nickname;
			string kickedByNickname = kickedBy.Nickname;
			string text;

			if (kicked.IsSelf)
				text = "You were kicked by " + kickedByNickname;
			else if (kickedBy.IsSelf)
				text = "You kicked " + kickedByNickname;
			else
				text = kicked + " was kicked by " + kickedBy;

			if (message != null)
				text += " (" + message + ")";

			AddControlMessage(text);

			if (userItemMap.ContainsKey(kickedNickname))
			{
				kicked.NicknameChanged -= nicknameChangedEventHandler;
				RemoveUserItem(kickedNickname);
			}
			else
			{
				Log.WriteLine("ChannelWindow: Kicked user doesn't exist");
			}
		}

		private void UserQuit(IrcUser user, string message)
		{
			string nickname = user.Nickname;

			if (!userItemMap.ContainsKey(nickname))
				return;

			RemoveUserItem(nickname);

			string text;

			if (user.IsSelf)
				text = "You went offline";
			else
				text = nickname + " went offline";

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

			if (!userItemMap.ContainsKey(oldNickname))
			{
				Log.WriteLine("ChannelWindow.UserNicknameChanged: Old nickname item doesn't exist");
				return;
			}

			ChannelUserItem item = userItemMap[oldNickname];
			userItemMap.Remove(oldNickname);
			item.Nickname = newNickname;

			if (userItemMap.ContainsKey(newNickname))
				Log.WriteLine("ChannelWindow.UserNicknameChanged: New nickname overwrites existing item");

			userItemMap[newNickname] = item;
		}

		private void AddUserItem(IrcChannelUser channelUser)
		{
			string nickname = channelUser.Nickname;
			ChannelUserItem item = new ChannelUserItem(channelUser);
			userItems.Add(item);
			userItemMap[nickname] = item;
		}

		private void RemoveUserItem(string nickname)
		{
			ChannelUserItem item = userItemMap[nickname];
			userItems.Remove(item);
			userItemMap.Remove(nickname);
		}

		private void RemoveAllUserItems()
		{
			userItems.Clear();
			userItemMap.Clear();
		}

		private IrcChannel channel = null;
		private Dictionary<string, ChannelUserItem> userItemMap = new Dictionary<string, ChannelUserItem>();
		private ObservableCollection<ChannelUserItem> userItems = new ObservableCollection<ChannelUserItem>();
		private EventHandler<IrcUserEventArgs> nicknameChangedEventHandler;
	}
}
