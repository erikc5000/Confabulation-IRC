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

namespace Confabulation
{
	/// <summary>
	/// Interaction logic for ChatWindow.xaml
	/// </summary>
	public partial class ChatWindow : UserControl
	{
		public ChatWindow()
		{
			InitializeComponent();
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
			p.TextAlignment = TextAlignment.Left;

			bool bold = false;

			foreach (string s in boldParts)
			{
				if (!bold)
					p.Inlines.Add(new Run(s));
				else
					p.Inlines.Add(new Bold(new Run(s)));

				bold = !bold;
			}

			ChatLogDocument.Blocks.Add(p);

			DependencyObject obj = ChatLog;

			do
			{
				obj = VisualTreeHelper.GetChild(obj as Visual, 0);
			}
			while (!(obj is ScrollViewer));

			ScrollViewer scrollViewer = obj as ScrollViewer;
			scrollViewer.ScrollToBottom();
		}

		private void ChatTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter || e.Key == Key.Return)
			{
				try
				{
					TextRange textRange = new TextRange(ChatTextBox.Document.ContentStart,
														ChatTextBox.Document.ContentEnd);

					if (Connection != null)
						Connection.Execute(IrcCommand.Parse(textRange.Text));
				}
				catch (IrcCommandException)
				{
					AddTextToWindow("Invalid command");
				}
				catch (ArgumentException ae)
				{
					AddTextToWindow("Invalid argument: " + ae.ParamName);
				}

				ChatTextBox.Document.Blocks.Clear();
				ChatTextBox.CaretPosition = ChatTextBox.Document.ContentStart;

				e.Handled = true;
			}
		}

		public IrcConnection Connection
		{
			get
			{
				return connection;
			}
			set
			{
				connection = value;
				connection.StateChanged += new EventHandler<IrcConnectionEventArgs>(connection_StateChanged);
				connection.RawMessageReceived += new EventHandler<IrcMessageEventArgs>(connection_RawMessageReceived);
			}
		}

		private void connection_StateChanged(object sender, IrcConnectionEventArgs e)
		{
			switch (e.EventType)
			{
				case IrcConnectionEventType.Connected:
					ChatLogDocument.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
						new AddToWindowDelegate(AddTextToWindow), (Object)"*Connected*");
					break;

				case IrcConnectionEventType.ConnectFailed:
					ChatLogDocument.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
						new AddToWindowDelegate(AddTextToWindow), (Object)"*Connection Failed*");
					break;

				case IrcConnectionEventType.Disconnected:
					ChatLogDocument.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
						new AddToWindowDelegate(AddTextToWindow), (Object)"*Disconnected*");
					break;
			}
		}

		private void connection_RawMessageReceived(object sender, IrcMessageEventArgs e)
		{
			ChatLogDocument.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
				new AddToWindowDelegate(AddTextToWindow), (Object)e.Message.ToString());
		}

		private IrcConnection connection;
	}
}
