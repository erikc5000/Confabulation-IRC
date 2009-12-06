using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Confabulation.Chat;

namespace Confabulation
{
	public class ConnectionItem : DependencyObject
	{
		public ConnectionItem(IrcConnection connection)
		{
			this.connection = connection;

			Name = connection.Settings.Name;
			State = connection.State;

			connection.StateChanged += new EventHandler<IrcConnectionEventArgs>(connection_StateChanged);
		}

		public static readonly DependencyProperty NameProperty =
			DependencyProperty.Register("Name", typeof(string), typeof(ConnectionItem));

		public static readonly DependencyProperty StateProperty =
			DependencyProperty.Register("State", typeof(IrcConnectionState), typeof(ConnectionItem));

		public string Name
		{
			get { return (string)GetValue(NameProperty); }
			set { SetValue(NameProperty, value); }
		}

		public IrcConnectionState State
		{
			get { return (IrcConnectionState)GetValue(StateProperty); }
			set { SetValue(StateProperty, value); }
		}

		public IrcConnection Connection
		{
			get
			{
				return connection;
			}
		}

		private void connection_StateChanged(object sender, IrcConnectionEventArgs e)
		{
			Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
				(Action)(() => { State = connection.State; }));
		}

		private IrcConnection connection;
	}
}
