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
	public partial class ChatBox : UserControl
	{
		public ChatBox() : base()
		{
			InitializeComponent();
		}

		public delegate void AddChatMessageDelegate(IrcUser user, string message);
		public delegate void AddControlMessageDelegate(string message);
		public delegate void AddRawTextDelegate(string text);

		public void AddChatMessage(IrcUser user, string message)
		{
			Paragraph p = new Paragraph();
			//p.KeepWithNext = true;
			p.FontSize = 12;
			p.FontFamily = fontFamily;
			p.Margin = new Thickness(0.0, 1.0, 0.0, 1.0);
			p.TextAlignment = TextAlignment.Left;

			Run nicknameRun = new Run(user.Nickname + ": ");

			if (user.IsSelf)
				nicknameRun.Foreground = Brushes.DarkRed;
			else
				nicknameRun.Foreground = Brushes.DarkOrange;

			p.Inlines.Add(new Bold(nicknameRun));

			List<Inline> inlines = new List<Inline>(ChatMessage.Parse(message));

			foreach (Inline inline in inlines)
				p.Inlines.Add(inline);

			bool shouldScroll = true;

			if (ScrollViewer.VerticalOffset < ScrollViewer.ExtentHeight - ScrollViewer.ViewportHeight)
				shouldScroll = false;

			chatLogDocument.Blocks.Add(p);

			if (shouldScroll)
				ScrollChatLogToBottom();
		}

		public void AddControlMessage(string message)
		{
			Paragraph p = new Paragraph();
			//p.KeepWithNext = true;
			p.FontSize = 12;
			p.FontFamily = fontFamily;
			p.Margin = new Thickness(10.0, 2.0, 10.0, 2.0);
			p.TextAlignment = TextAlignment.Center;

			Run run = new Run(message);
			run.Foreground = Brushes.LightGray;
			p.Inlines.Add(new Bold(run));
			chatLogDocument.Blocks.Add(p);

			ScrollChatLogToBottom();
		}

		public void AddRawText(string text)
		{
			Paragraph p = new Paragraph();
			//p.KeepWithNext = true;
			p.FontSize = 12;
			p.FontFamily = fontFamily;
			p.Margin = new Thickness(0.0, 1.0, 0.0, 1.0);
			p.TextAlignment = TextAlignment.Left;

			Run run = new Run(text);
			p.Inlines.Add(run);

			bool shouldScroll = true;

			if (ScrollViewer != null)
			{
				double verticalOffset = ScrollViewer.VerticalOffset;
				double extentHeight = ScrollViewer.ExtentHeight;

				if (verticalOffset < extentHeight - ScrollViewer.ContentVerticalOffset)
					shouldScroll = true;
			}

			chatLogDocument.Blocks.Add(p);

			if (shouldScroll)
				ScrollChatLogToBottom();
		}

		public event EventHandler<ChatBoxEventArgs> TextEntered;

		private void chatTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter || e.Key == Key.Return)
			{
				TextRange textRange = new TextRange(chatTextBox.Document.ContentStart,
													chatTextBox.Document.ContentEnd);

				ChatBoxEventArgs chatBoxArgs = new ChatBoxEventArgs(textRange.Text);
				EventHandler<ChatBoxEventArgs> handler = TextEntered;

				if (handler != null)
					handler(this, chatBoxArgs);

				chatTextBox.Document.Blocks.Clear();
				chatTextBox.CaretPosition = chatTextBox.Document.ContentStart;

				e.Handled = true;
			}
		}

		private void chatTextBox_ScrollChanged(object sender, ScrollChangedEventArgs e)
		{
			//if (e.VerticalOffset < e.ExtentHeight)
			//    adjustScrollBar = false;
			//else
			//    adjustScrollBar = true;
		}

		public void ScrollChatLogToBottom()
		{
			ScrollViewer.ScrollToBottom();
		}

		private ScrollViewer ScrollViewer
		{
			get
			{
				if (scrollViewer == null)
				{
					DependencyObject obj = chatLog;

					do
					{
						obj = VisualTreeHelper.GetChild(obj as Visual, 0);
					}
					while (!(obj is ScrollViewer));

					scrollViewer = obj as ScrollViewer;
				}

				return scrollViewer;
			}
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			Keyboard.Focus(chatTextBox);
		}

		private ScrollViewer scrollViewer = null;
		private FontFamily fontFamily = new FontFamily("Arial");

		private void chatTextBox_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
		{
			//if (e.NewValue != 1.0)
			//    adjustScrollBar = false;
			//else
			//    adjustScrollBar = true;
		}
	}
}
