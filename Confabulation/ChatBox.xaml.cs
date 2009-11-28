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

		public delegate void AddMessageDelegate(IrcUser user, string message);
		public delegate void AddControlMessageDelegate(string message);
		public delegate void AddToWindowDelegate(string text);

		static public List<Inline> ParseMessage(string message)
		{
			List<Inline> inlines = new List<Inline>();

			bool bold = false;
			bool underline = false;
			bool italic = false;
			bool defaultBGColor = true;
			bool defaultFGColor = true;
			MircColor bgColor = MircColor.White;
			MircColor fgColor = MircColor.Black;

			int runStart = 0;
			int runLength = 0;

			for (int i = 0; i < message.Length; i++)
			{
				if (!IsFormattingCode(message[i]))
					continue;

				runLength = i - runStart;

				if (runLength > 0)
				{
					string runText = message.Substring(runStart, runLength);
					Run run = new Run(runText);

					if (!defaultFGColor)
						run.Foreground = GetBrushFromMircColor(fgColor);

					if (!defaultBGColor)
						run.Background = GetBrushFromMircColor(bgColor);

					Inline insertInline = run;

					if (bold)
						insertInline = new Bold(insertInline);

					if (underline)
						insertInline = new Underline(insertInline);

					if (italic)
						insertInline = new Italic(insertInline);

					inlines.Add(insertInline);
				}

				switch (message[i])
				{
					case (char)0x02:
						bold = !bold;
						runStart = i + 1;
						break;

					case (char)0x03:
						int j = i + 1;

						if (j >= message.Length)
							return inlines;

						char c1 = message[j];

						if (!Utilities.IsDigit(c1))
						{
							defaultBGColor = true;
							defaultFGColor = true;
							runStart = j;
							i = runStart - 1;
							break;
						}

						j++;

						if (j >= message.Length)
							return inlines;

						char c2 = message[j];

						bool hasBGColor = false;
						string code = "";
						code += c1;

						if (Utilities.IsDigit(c2))
						{
							code += c2;

							j++;

							if (j < message.Length)
							{
								if (message[j] == ',')
									hasBGColor = true;
							}
						}
						else if (c2 == ',')
						{
							hasBGColor = true;
						}

						fgColor = (MircColor)Int32.Parse(code);
						defaultFGColor = false;

						if (!hasBGColor)
						{
							runStart = j;
							i = runStart - 1;
							break;
						}

						j++;

						if (j >= message.Length)
						{
							runStart = j - 1;
							i = runStart - 1;
							break;
						}

						c1 = message[j];

						if (!Utilities.IsDigit(c1))
						{
							runStart = j - 1;
							i = runStart - 1;
							break;
						}

						j++;

						if (j >= message.Length)
							return inlines;

						c2 = message[j];

						string code2 = "";
						code2 += c1;

						if (Utilities.IsDigit(c2))
						{
							code2 += c2;
							runStart = j + 1;
							i = j;
						}
						else
						{
							runStart = j;
							i = j - 1;
						}

						bgColor = (MircColor)Int32.Parse(code2);
						defaultBGColor = false;

						break;

					case (char)0x0F:
						bold = false;
						underline = false;
						italic = false;
						defaultBGColor = true;
						defaultFGColor = true;
						runStart = i + 1;
						break;

					case (char)0x16:
						italic = !italic;
						runStart = i + 1;
						break;

					case (char)0x1F:
						underline = !underline;
						runStart = i + 1;
						break;
				}
			}

			runLength = message.Length - runStart;

			if (runLength > 0)
			{
				string runText = message.Substring(runStart, runLength);
				Run run = new Run(runText);

				if (!defaultFGColor)
					run.Foreground = GetBrushFromMircColor(fgColor);

				if (!defaultBGColor)
					run.Background = GetBrushFromMircColor(bgColor);

				Inline insertInline = run;

				if (bold)
					insertInline = new Bold(insertInline);

				if (underline)
					insertInline = new Underline(insertInline);

				if (italic)
					insertInline = new Italic(insertInline);

				inlines.Add(insertInline);
			}

			return inlines;
		}

		public void AddMessage(IrcUser user, string message)
		{
			Paragraph p = new Paragraph();
			//p.KeepWithNext = true;
			p.FontSize = 12;
			p.FontFamily = new FontFamily("Arial");
			p.Margin = new Thickness(0.0, 1.0, 0.0, 1.0);
			p.TextAlignment = TextAlignment.Left;

			Run nicknameRun = new Run(user.Nickname + ": ");

			if (user.IsSelf)
				nicknameRun.Foreground = Brushes.Red;
			else
				nicknameRun.Foreground = Brushes.Blue;

			p.Inlines.Add(new Bold(nicknameRun));

			List<Inline> messageInlines = ParseMessage(message);

			foreach (Inline inline in messageInlines)
				p.Inlines.Add(inline);

			chatLogDocument.Blocks.Add(p);
			ScrollChatLogToBottom();
		}

		public void AddControlMessage(string message)
		{
			Paragraph p = new Paragraph();
			//p.KeepWithNext = true;
			p.FontSize = 12;
			p.FontFamily = new FontFamily("Arial");
			p.Margin = new Thickness(10.0, 2.0, 10.0, 2.0);
			p.TextAlignment = TextAlignment.Center;

			Run run = new Run(message);
			run.Foreground = Brushes.LightGray;
			p.Inlines.Add(new Bold(run));
			chatLogDocument.Blocks.Add(p);

			ScrollChatLogToBottom();
		}

		public void AddTextToWindow(string text)
		{
			// 0x02 = Bold
			// 0x0F = Plain
			// 0x16 = Italic
			// 0x1F = Underline
			// 0x03 [FF,BB] = color code
			string[] boldParts = text.Split((char)0x02);
			Paragraph p = new Paragraph();
			p.KeepWithNext = true;
			p.FontSize = 12;
			p.FontFamily = new FontFamily("Courier New");
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

			chatLogDocument.Blocks.Add(p);
			ScrollChatLogToBottom();
		}

		public event EventHandler<ChatBoxEventArgs> TextEntered;

		private void ChatTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
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

		public void ScrollChatLogToBottom()
		{
			DependencyObject obj = chatLog;

			do
			{
				obj = VisualTreeHelper.GetChild(obj as Visual, 0);
			}
			while (!(obj is ScrollViewer));

			ScrollViewer scrollViewer = obj as ScrollViewer;
			scrollViewer.ScrollToBottom();
		}

		static private bool IsFormattingCode(char c)
		{
			if (c == (char)0x02
				|| c == (char)0x03
				|| c == (char)0x0F
				|| c == (char)0x16
				|| c == (char)0x1F)
			{
				return true;
			}

			return false;
		}

		static private Brush GetBrushFromMircColor(MircColor color)
		{
			switch (color)
			{
				case MircColor.White:
					return Brushes.White;

				case MircColor.Black:
					return Brushes.Black;

				case MircColor.DarkBlue:
					return Brushes.DarkBlue;

				case MircColor.DarkGreen:
					return Brushes.DarkGreen;

				case MircColor.Red:
					return Brushes.Red;

				case MircColor.Brown:
					return Brushes.Brown;

				case MircColor.Purple:
					return Brushes.Purple;

				case MircColor.Olive:
					return Brushes.Olive;

				case MircColor.Yellow:
					return Brushes.Yellow;

				case MircColor.Green:
					return Brushes.Green;

				case MircColor.Teal:
					return Brushes.Teal;

				case MircColor.Cyan:
					return Brushes.Cyan;

				case MircColor.Blue:
					return Brushes.Blue;

				case MircColor.Magenta:
					return Brushes.Magenta;

				case MircColor.DarkGray:
					return Brushes.DarkGray;

				case MircColor.LightGray:
					return Brushes.LightGray;
			}

			return Brushes.Black;
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			Keyboard.Focus(chatTextBox);
		}
	}
}
