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
using Confabulation.Chat.Events;

namespace Confabulation
{
	public partial class ChannelWindow : UserControl, IChatWindow
	{
		public ChannelWindow(IrcChannel channel)
			: base()
		{
			InitializeComponent();

			this.channel = channel;
			nicknameChangedEventHandler = new EventHandler<NicknameEventArgs>(user_NicknameChanged);
			channel.MessageReceived += new EventHandler<UserEventArgs>(channel_MessageReceived);
			channel.UsersAdded += new EventHandler<ChannelEventArgs>(channel_UsersAdded);
			channel.UserJoined += new EventHandler<ChannelEventArgs>(channel_UserJoined);
			channel.UserParted += new EventHandler<ChannelEventArgs>(channel_UserParted);
			channel.UserKicked += new EventHandler<KickEventArgs>(channel_UserKicked);
			channel.TopicSet += new EventHandler<TopicEventArgs>(channel_TopicSet);
			channel.TopicInfoSet += new EventHandler<TopicEventArgs>(channel_TopicInfoSet);
			channel.TopicChanged += new EventHandler<TopicEventArgs>(channel_TopicChanged);
			channel.Connection.UserQuit += new EventHandler<UserEventArgs>(Connection_UserQuit);
			channel.Connection.StateChanged += new EventHandler<IrcConnectionEventArgs>(Connection_StateChanged);

			usersList.DataContext = userItems;

			SetTopic(channel.Topic, channel.TopicInfo);
			AddUsers();
		}

		public IrcConnection Connection
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

		private void TextEntered(object sender, ChatBoxEventArgs e)
		{
			string text = e.Text;

			if (Connection != null)
			{
				try
				{
					if (!text.StartsWith("/"))
					{
						text = text.Trim();
						Connection.Execute(new MsgCommand(Channel, text));
						chatBox.AddMessage(Connection.User, text);
					}
					else
					{
						Connection.Execute(IrcCommand.Parse(text));
					}
				}
				catch (IrcCommandException)
				{
					chatBox.AddTextToWindow("*Invalid command*");
				}
				catch (ArgumentException ae)
				{
					chatBox.AddTextToWindow("*Invalid argument*: " + ae.ParamName);
				}
			}
			else
			{
				chatBox.AddTextToWindow("*Not connected*");
			}
		}

		private delegate void DisconnectedDelegate();
		private delegate void AddUsersDelegate(IEnumerable<IrcChannelUser> users);
		private delegate void UserJoinedDelegate(IrcChannelUser user);
		private delegate void UserPartedDelegate(IrcChannelUser user, string message);
		private delegate void UserKickedDelegate(IrcChannelUser kicked, IrcChannelUser kickedBy, string message);
		private delegate void SetTopicDelegate(string topic, IrcTopicInfo topicInfo);
		private delegate void TopicChangedDelegate(string topic, IrcUser setBy);
		private delegate void UserQuitDelegate(IrcUser user, string message);
		private delegate void UserChangedNicknameDelegate(IrcUser user, string oldNickname, string newNickname);

		private void Disconnected()
		{
			chatBox.AddControlMessage(Properties.Resources.SelfDisconnected);
			RemoveAllUserItems();
		}

		private void channel_UsersAdded(object sender, ChannelEventArgs e)
		{
			Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
						new AddUsersDelegate(AddUsers),
						e.AddedUsers);
		}

		private void channel_UserJoined(object sender, ChannelEventArgs e)
		{
			Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
						new UserJoinedDelegate(UserJoined),
						e.User);
		}

		private void channel_UserParted(object sender, ChannelEventArgs e)
		{
			Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
						new UserPartedDelegate(UserParted),
						e.User,
						e.Message);
		}

		private void channel_UserKicked(object sender, KickEventArgs e)
		{
			Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
						new UserKickedDelegate(UserKicked),
						e.Kicked,
						e.KickedBy,
						e.Message);
		}

		private void Connection_UserQuit(object sender, UserEventArgs e)
		{
			Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
						new UserQuitDelegate(UserQuit),
						e.User,
						e.Message);
		}

		private void user_NicknameChanged(object sender, NicknameEventArgs e)
		{
			Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
						new UserChangedNicknameDelegate(UserChangedNickname),
						e.User,
						e.OldNickname,
						e.NewNickname);
		}

		private void channel_TopicSet(object sender, TopicEventArgs e)
		{
			Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
						new SetTopicDelegate(SetTopic),
						e.Topic,
						null);
		}

		private void channel_TopicInfoSet(object sender, TopicEventArgs e)
		{
			Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
						new SetTopicDelegate(SetTopic),
						e.Topic,
						e.TopicInfo);
		}

		private void channel_TopicChanged(object sender, TopicEventArgs e)
		{
			Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
						new TopicChangedDelegate(TopicChanged),
						e.Topic,
						e.User);
		}

		private void channel_MessageReceived(object sender, UserEventArgs e)
		{
			Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
						new ChatBox.AddMessageDelegate(chatBox.AddMessage),
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
			AddUsers(Channel.Users);
		}

		private void AddUsers(IEnumerable<IrcChannelUser> users)
		{
			foreach (IrcChannelUser user in users)
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
				text = Properties.Resources.SelfJoinedChannel;
			else
				text = String.Format(Properties.Resources.UserJoinedChannel, nickname);

			chatBox.AddControlMessage(text);

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
				text = Properties.Resources.SelfPartedChannel;
			else
				text = String.Format(Properties.Resources.UserPartedChannel, nickname);

			if (message != null)
				text += " (" + message + ")";

			chatBox.AddControlMessage(text);

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
				text = String.Format(Properties.Resources.SelfKickedByUser, kickedByNickname);
			else if (kickedBy.IsSelf)
				text = String.Format(Properties.Resources.SelfKickedUser, kickedByNickname);
			else
				text = String.Format(Properties.Resources.UserKicked, kickedNickname, kickedByNickname);

			if (message != null)
				text += " (" + message + ")";

			chatBox.AddControlMessage(text);

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
				text = Properties.Resources.SelfDisconnected;
			else
				text = String.Format(Properties.Resources.UserQuit, nickname);

			if (message != null)
				text += " (" + message + ")";

			chatBox.AddControlMessage(text);
		}

		private void UserChangedNickname(IrcUser user, string oldNickname, string newNickname)
		{
			string text;

			if (user.IsSelf)
				text = String.Format(Properties.Resources.SelfChangedNickname, newNickname);
			else
				text = String.Format(Properties.Resources.UserChangedNickname, oldNickname, newNickname);

			chatBox.AddControlMessage(text);

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

		private void TopicChanged(string topic, IrcUser setBy)
		{
			string nickname = setBy.Nickname;
			string text;

			if (topic == "")
			{
				if (setBy.IsSelf)
					text = Properties.Resources.SelfClearedTopic;
				else
					text = String.Format(Properties.Resources.UserClearedTopic, nickname);
			}
			else
			{
				if (setBy.IsSelf)
					text = String.Format(Properties.Resources.SelfChangedTopic, topic);
				else
					text = String.Format(Properties.Resources.UserChangedTopic, nickname, topic);
			}

			chatBox.AddControlMessage(text);
			SetTopic(topic, new IrcTopicInfo(nickname, DateTime.Now));
		}

		private void SetTopic(string topic, IrcTopicInfo topicInfo)
		{
			topicTextBlock.Inlines.Clear();

			List<Inline> inlines = ChatBox.ParseMessage(topic);

			foreach (Inline inline in inlines)
				topicTextBlock.Inlines.Add(inline);

			if (topicInfo != null)
			{
				string text = "\n " + topicInfo.SetBy + " " + topicInfo.Time.ToString();

				Run run = new Run(text);
				run.Foreground = Brushes.Gray;
				run.FontSize = 8;
				topicTextBlock.Inlines.Add(run);
			}
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
		private EventHandler<NicknameEventArgs> nicknameChangedEventHandler;
	}
}
