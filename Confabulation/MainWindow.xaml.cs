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
using System.Windows.Interop;
using System.Runtime.InteropServices;
using Confabulation.Chat;
using Confabulation.Chat.Commands;

namespace Confabulation
{
	// RECT structure required by WINDOWPLACEMENT structure
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct RECT
	{
		public int Left;
		public int Top;
		public int Right;
		public int Bottom;

		public RECT(int left, int top, int right, int bottom)
		{
			this.Left = left;
			this.Top = top;
			this.Right = right;
			this.Bottom = bottom;
		}
	}

	// POINT structure required by WINDOWPLACEMENT structure
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct POINT
	{
		public int X;
		public int Y;

		public POINT(int x, int y)
		{
			this.X = x;
			this.Y = y;
		}
	}

	// WINDOWPLACEMENT stores the position, size, and state of a window
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct WINDOWPLACEMENT
	{
		public int length;
		public int flags;
		public int showCmd;
		public POINT minPosition;
		public POINT maxPosition;
		public RECT normalPosition;
	}

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
		#region Win32 API declarations to set and get window placement
		[DllImport("user32.dll")]
		static extern bool SetWindowPlacement(IntPtr hWnd, [In] ref WINDOWPLACEMENT lpwndpl);

		[DllImport("user32.dll")]
		static extern bool GetWindowPlacement(IntPtr hWnd, out WINDOWPLACEMENT lpwndpl);

		const int SW_SHOWNORMAL = 1;
		const int SW_SHOWMINIMIZED = 2;
		#endregion

        public MainWindow()
        {
            InitializeComponent();

			App app = (App)App.Current;
			app.ConnectionAdded += new EventHandler<ConnectionEventArgs>(app_ConnectionAdded);
			app.ConnectionRemoved += new EventHandler<ConnectionEventArgs>(app_ConnectionRemoved);
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
			//ConnectionList.Items.Add(e.Connection);
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

		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);

			try
			{
				// Load window placement details for previous application session from application settings
				// Note - if window was closed on a monitor that is now disconnected from the computer,
				//        SetWindowPlacement will place the window onto a visible monitor.
				WINDOWPLACEMENT wp = (WINDOWPLACEMENT)Properties.Settings.Default.WindowPlacement;

				if (!(wp.normalPosition.Top == 0
					&& wp.normalPosition.Bottom == 0
					&& wp.normalPosition.Right == 0
					&& wp.normalPosition.Left == 0))
				{
					wp.length = Marshal.SizeOf(typeof(WINDOWPLACEMENT));
					wp.flags = 0;
					wp.showCmd = (wp.showCmd == SW_SHOWMINIMIZED ? SW_SHOWNORMAL : wp.showCmd);
					IntPtr hwnd = new WindowInteropHelper(this).Handle;
					SetWindowPlacement(hwnd, ref wp);
				}
			}
			catch
			{
			}
		}

		// WARNING - Not fired when Application.SessionEnding is fired
		protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
		{
			base.OnClosing(e);

			// Persist window placement details to application settings
			WINDOWPLACEMENT wp = new WINDOWPLACEMENT();
			IntPtr hwnd = new WindowInteropHelper(this).Handle;
			GetWindowPlacement(hwnd, out wp);
			Properties.Settings.Default.WindowPlacement = wp;
			Properties.Settings.Default.Save();
		}

		private IrcConnection activeConnection = null;
    }
}
