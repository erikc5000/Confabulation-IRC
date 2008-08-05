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

			client.ConnectionEvent += new EventHandler<IrcConnectionEventArgs>(client_ConnectionEvent);
			client.MessageReceived += new EventHandler<IrcMessageEventArgs>(client_RawMessageReceived);
        }

		private void ConnectButton_Click(object sender, RoutedEventArgs e)
		{
			if (client.ConnectionState == IrcConnectionState.Disconnected)
				client.Connect();
			else
				client.Disconnect();
		}

		private void client_ConnectionEvent(object sender, IrcConnectionEventArgs e)
		{
			switch (e.EventType)
			{
				case IrcConnectionEventType.Connected:
					ChatWindowDocument.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
						new AddToWindowDelegate(AddTextToWindow), (Object)"Connected");
					IrcClient client = (IrcClient)sender;
					NickCommand.Execute(client, "BlasterTest");
					UserCommand.Execute(client, "guest", InitialUserMode.Invisible, "The Real Me");
					break;

				case IrcConnectionEventType.ConnectFailed:
					ChatWindowDocument.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
						new AddToWindowDelegate(AddTextToWindow), (Object)"Connection Failed");
					break;

				case IrcConnectionEventType.Disconnected:
					ChatWindowDocument.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
						new AddToWindowDelegate(AddTextToWindow), (Object)"Disconnected");
					break;
			}
		}

		private void client_RawMessageReceived(object sender, IrcMessageEventArgs e)
		{
			ChatWindowDocument.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
						new AddToWindowDelegate(AddTextToWindow), (Object)e.Message.ToString());

			if (Encoding.ASCII.GetString(e.Message.GetCommand()) == "PING")
			{
				IrcClient client = (IrcClient)sender;
				PongCommand.Execute(client, Encoding.UTF8.GetString(e.Message.Parameters.First()));
			}
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

			ChatWindowDocument.Blocks.Add(p);

			DependencyObject obj = ChatWindow;

			do
			{
				obj = VisualTreeHelper.GetChild(obj as Visual, 0);
			}
			while (!(obj is ScrollViewer));

			ScrollViewer scrollViewer = obj as ScrollViewer;
			scrollViewer.ScrollToBottom();
		}

		IrcClient client = new IrcClient("Krypt.CA.US.GameSurge.net", 6667);

		private void ChatTextBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter || e.Key == Key.Return)
			{
				if (client.ConnectionState != IrcConnectionState.Connected)
				{
					AddTextToWindow("Not connected");
				}
				else
				{
					try
					{
						TextRange textRange = new TextRange(ChatTextBox.Document.ContentStart,
															ChatTextBox.Document.ContentEnd);

						IrcCommand.ParseAndExecute(client, textRange.Text);
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

				ChatTextBox.Document.Blocks.Clear();
				ChatTextBox.CaretPosition = ChatTextBox.Document.ContentStart;

				e.Handled = true;
			}
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			NewConnectionWindow ncWin = new NewConnectionWindow();
			ncWin.Owner = this;
			ncWin.ShowDialog();
		}
    }
}
