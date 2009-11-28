using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
		}


		private delegate void AddChannelDelegate(IrcChannel channel);
		private delegate void RemoveChannelDelegate(IrcChannel channel);
		private delegate void SetConnectButtonStateDelegate();

		private void app_ConnectionAdded(object sender, ConnectionEventArgs e)
		{
			TabItem item = new TabItem();
			item.Header = e.Connection.Settings.Name;
			item.Content = new ServerWindow(e.Connection);

			tabControl.Items.Add(item);
			tabControl.SelectedItem = item;

			e.Connection.ChannelJoined += new EventHandler<ChannelEventArgs>(ChannelJoined);
			e.Connection.ChannelParted += new EventHandler<ChannelEventArgs>(ChannelParted);
		}

		private void app_ConnectionRemoved(object sender, ConnectionEventArgs e)
		{
			foreach (TabItem item in tabControl.Items)
			{
				if (item.Content is IChatWindow)
				{
					IChatWindow window = (IChatWindow)item.Content;

					if (window.Connection == e.Connection)
						tabControl.Items.Remove(item);
				}
			}

			if (activeConnection == e.Connection)
			{
				activeConnection.StateChanged -= new EventHandler<IrcConnectionEventArgs>(activeConnection_StateChanged);
				activeConnection = null;
			}
		}

		private void activeConnection_StateChanged(object sender, IrcConnectionEventArgs e)
		{
			Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
								   new SetConnectButtonStateDelegate(SetConnectButtonState));
		}

		private void ChannelJoined(object sender, ChannelEventArgs e)
		{
			Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
								   new AddChannelDelegate(AddChannel),
								   e.Channel);
		}

		private void ChannelParted(object sender, ChannelEventArgs e)
		{
			Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
								   new AddChannelDelegate(RemoveChannel),
								   e.Channel);
		}

		private void AddChannel(IrcChannel channel)
		{
			TabItem item = new TabItem();
			item.Header = channel.Name;
			item.Content = new ChannelWindow(channel);

			tabControl.Items.Add(item);
			tabControl.SelectedItem = item;
		}

		private void RemoveChannel(IrcChannel channel)
		{
			foreach (TabItem item in tabControl.Items)
			{
				if (item.Content is ChannelWindow)
				{
					ChannelWindow window = (ChannelWindow)item.Content;

					if (window.Channel == channel)
					{
						tabControl.Items.Remove(item);
						break;
					}
				}
			}
		}

		private void SetConnectButtonState()
		{
			if (activeConnection == null || activeConnection.State == IrcConnectionState.Disconnected)
				connectButtonTextBlock.Text = Properties.Resources.ConnectText;
			else
				connectButtonTextBlock.Text = Properties.Resources.DisconnectText;
		}

		private void ConnectButton_Click(object sender, RoutedEventArgs e)
		{
			if (activeConnection == null)
				return;

			if (activeConnection.State == IrcConnectionState.Disconnected)
				activeConnection.Initiate();
			else if (activeConnection.State == IrcConnectionState.Connected)
				activeConnection.Execute(new QuitCommand());
			else
				activeConnection.Close();
		}

		private void NewConnection_Click(object sender, RoutedEventArgs e)
		{
			NewConnectionWindow ncWin = new NewConnectionWindow();
			ncWin.Owner = this;
			ncWin.ShowDialog();
		}

		private void ManageConnections_Click(object sender, RoutedEventArgs e)
		{
			ConnectionsWindow cWin = new ConnectionsWindow();
			cWin.Owner = this;
			cWin.Show();
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

		private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (!(sender is TabControl))
				return;

			TabItem item = (TabItem)tabControl.SelectedItem;

			if (item == null)
				return;

			IChatWindow window = (IChatWindow)item.Content;

			if (activeConnection != window.Connection)
			{
				if (activeConnection != null)
					activeConnection.StateChanged -= new EventHandler<IrcConnectionEventArgs>(activeConnection_StateChanged);

				activeConnection = window.Connection;
				activeConnection.StateChanged += new EventHandler<IrcConnectionEventArgs>(activeConnection_StateChanged);
			}
		}

		private IrcConnection activeConnection = null;
	}
}
