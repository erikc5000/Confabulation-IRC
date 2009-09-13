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
using Confabulation.Chat;
using Confabulation.Chat.Commands;

namespace Confabulation
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

			App app = (App)App.Current;
			//app.ConnectionAdded += new EventHandler<ConnectionEventArgs>(app_ConnectionAdded);
			//app.ConnectionRemoved += new EventHandler<ConnectionEventArgs>(app_ConnectionRemoved);
			//client.ConnectionEvent += new EventHandler<IrcConnectionEventArgs>(client_ConnectionEvent);
			//client.MessageReceived += new EventHandler<IrcMessageEventArgs>(client_RawMessageReceived);
			//TreeViewItem item = new TreeViewItem();
			//item.Header = "MyConnection";
			//ConnectionList.Items.Add(item);
			//ConnectionList.DataContext = app.Connections;
        }

		private void app_ConnectionAdded(object sender, ConnectionEventArgs e)
		{
			//TreeViewItem item = new TreeViewItem();
			//item.Header = e.Connection.Settings.Name;

			//TreeViewItem channelItem = new TreeViewItem();
			//channelItem.Header = "#mychannel";
			//item.Items.Add(channelItem);
			ConnectionList.Items.Add(e.Connection);
			//Connections connections = (Connections)FindResource("Connections");

			//if (connections != null)
			//	connections.Add(e.Connection);

			if (activeConnection == null)
			{
				activeConnection = e.Connection;
				activeConnection.StateChanged += new EventHandler<IrcConnectionEventArgs>(client_ConnectionEvent);
				activeConnection.RawMessageReceived += new EventHandler<IrcMessageEventArgs>(client_RawMessageReceived);
			}
		}

		private void app_ConnectionRemoved(object sender, ConnectionEventArgs e)
		{
			if (activeConnection == e.Connection)
			{
				activeConnection.StateChanged -= new EventHandler<IrcConnectionEventArgs>(client_ConnectionEvent);
				activeConnection.RawMessageReceived -= new EventHandler<IrcMessageEventArgs>(client_RawMessageReceived);
				activeConnection = null;
			}
		}

		private void ConnectButton_Click(object sender, RoutedEventArgs e)
		{
			if (activeConnection == null)
				return;

			if (activeConnection.State == IrcConnectionState.Disconnected)
				activeConnection.Initiate();
			else
				activeConnection.Close();
		}

		private void client_ConnectionEvent(object sender, IrcConnectionEventArgs e)
		{
			switch (e.EventType)
			{
				case IrcConnectionEventType.Connected:
					//ChatWindowDocument.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
					//	new AddToWindowDelegate(AddTextToWindow), (Object)"Connected");
					break;

				case IrcConnectionEventType.ConnectFailed:
					//ChatWindowDocument.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
					//	new AddToWindowDelegate(AddTextToWindow), (Object)"Connection Failed");
					break;

				case IrcConnectionEventType.Disconnected:
					//ChatWindowDocument.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
					//	new AddToWindowDelegate(AddTextToWindow), (Object)"Disconnected");
					break;
			}
		}

		private void client_RawMessageReceived(object sender, IrcMessageEventArgs e)
		{
			//ChatWindowDocument.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
			//			new AddToWindowDelegate(AddTextToWindow), (Object)e.Message.ToString());
		}

		private delegate void AddToWindowDelegate(string text);

		private void AddTextToWindow(string text)
		{
			// 2 = Bold
			// 15 = Plain
			// 22 = Italic
			// 31 = Underline
			// 3 [followed by 2 digits] = color code
			string[] boldParts = text.Split((char)0x02);
			Paragraph p = new Paragraph();
			p.KeepWithNext = true;
			p.FontSize = 12;
			p.Margin = new Thickness(0.0);

			bool bold = false;

			foreach (string s in boldParts)
			{
				if (!bold)
					p.Inlines.Add(new Run(s));
				else
					p.Inlines.Add(new Bold(new Run(s)));

				bold = !bold;
			}

			//ChatWindowDocument.Blocks.Add(p);

			//DependencyObject obj = ChatWindow;

			//do
			//{
			//    obj = VisualTreeHelper.GetChild(obj as Visual, 0);
			//}
			//while (!(obj is ScrollViewer));

			//ScrollViewer scrollViewer = obj as ScrollViewer;
			//scrollViewer.ScrollToBottom();
		}

		private void ChatTextBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter || e.Key == Key.Return)
			{
				if (activeConnection == null || activeConnection.State != IrcConnectionState.Connected)
				{
					AddTextToWindow("Not connected");
				}
				else
				{
					try
					{
						//TextRange textRange = new TextRange(ChatTextBox.Document.ContentStart,
						//									ChatTextBox.Document.ContentEnd);

						//App app = (App)App.Current;
						//app.Connections.First().Execute(IrcCommand.Parse(textRange.Text));
					}
					catch (IrcCommandException)
					{
						AddTextToWindow("Invalid command");
					}
					catch (ArgumentException ae)
					{
						AddTextToWindow("Invalid argument: " + ae.ParamName);
					}
				}

				//ChatTextBox.Document.Blocks.Clear();
				//ChatTextBox.CaretPosition = ChatTextBox.Document.ContentStart;

				e.Handled = true;
			}
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			NewConnectionWindow ncWin = new NewConnectionWindow();
			ncWin.Owner = this;
			ncWin.ShowDialog();
		}

		private IrcConnection activeConnection = null;
    }
}
