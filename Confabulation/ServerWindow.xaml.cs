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
	public partial class ServerWindow : UserControl, IChatWindow
	{
		public ServerWindow(IrcConnection connection)
			: base()
		{
			InitializeComponent();

			this.connection = connection;
			connection.StateChanged += new EventHandler<IrcConnectionEventArgs>(connection_StateChanged);
			connection.RawMessageReceived += new EventHandler<IrcMessageEventArgs>(connection_RawMessageReceived);
		}

		public IrcConnection Connection
		{
			get
			{
				return connection;
			}
		}

		private void TextEntered(object sender, ChatBoxEventArgs e)
		{
			string text = e.Text;

			if (Connection != null)
			{
				try
				{
					Connection.Execute(IrcCommand.Parse(text));
				}
				catch (IrcCommandException)
				{
					chatBox.AddRawText("*Invalid command*");
				}
				catch (ArgumentException ae)
				{
					chatBox.AddRawText("*Invalid argument*: " + ae.ParamName);
				}
			}
			else
			{
				chatBox.AddRawText("*Not connected*");
			}
		}

		private void connection_StateChanged(object sender, IrcConnectionEventArgs e)
		{
			switch (e.EventType)
			{
				case IrcConnectionEventType.Connected:
					Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
						new ChatBox.AddRawTextDelegate(chatBox.AddRawText), (Object)"*Connected*");
					break;

				case IrcConnectionEventType.ConnectFailed:
					Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
						new ChatBox.AddRawTextDelegate(chatBox.AddRawText), (Object)"*Connection Failed*");
					break;

				case IrcConnectionEventType.Disconnected:
					Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
						new ChatBox.AddRawTextDelegate(chatBox.AddRawText), (Object)"*Disconnected*");
					break;
			}
		}

		private void connection_RawMessageReceived(object sender, IrcMessageEventArgs e)
		{
			Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
				new ChatBox.AddRawTextDelegate(chatBox.AddRawText), e.Message.ToString());
		}

		private IrcConnection connection = null;
	}
}
