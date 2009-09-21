using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Confabulation.Chat;

namespace Confabulation
{
	public class ConnectionItem : ConnectionListItem
	{
		public ConnectionItem(IrcConnection connection) : base()
		{
			if (connection == null)
				throw new ArgumentNullException("connection");

			this.connection = connection;
			chatWindow = new ChatWindow() { Connection = connection };
			connection.ChannelJoined += new EventHandler<IrcChannelEventArgs>(ChannelJoined);
			connection.ChannelParted += new EventHandler<IrcChannelEventArgs>(ChannelParted);
		}

		private void ChannelJoined(object sender, IrcChannelEventArgs e)
		{
			Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
				new AddChannelDelegate(AddChannel), e.Channel);
		}

		private void ChannelParted(object sender, IrcChannelEventArgs e)
		{
			Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
				new RemoveChannelDelegate(RemoveChannel), e.Channel);
		}

		private delegate void AddChannelDelegate(IrcChannel channel);
		private delegate void RemoveChannelDelegate(IrcChannel channel);

		private void AddChannel(IrcChannel channel)
		{
			items.Add(new ChannelItem(channel));
		}

		private void RemoveChannel(IrcChannel channel)
		{
			foreach (object item in items)
			{
				if (item is ChannelItem)
				{
					ChannelItem channelItem = (ChannelItem)item;

					if (channelItem.Channel == channel)
					{
						items.Remove(item);
						break;
					}
				}
			}
		}

		public override IrcConnection Connection
		{
			get
			{
				return connection;
			}
		}

		public override ChatWindow ChatWindow
		{
			get
			{
				return chatWindow;
			}
		}

		public Object Items
		{
			get
			{
				return items;
			}
		}

		private IrcConnection connection;
		private ObservableCollection<Object> items = new ObservableCollection<Object>();
		private ChatWindow chatWindow;
	}
}
