using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Text.RegularExpressions;
using Confabulation.Chat;

namespace Confabulation
{
	public static class ChatMessage
	{
		public static string RemoveFormatting(string text)
		{
			return stripFormattingRegex.Replace(text, "");
		}

		public static IEnumerable<Inline> Parse(string text)
		{
			ChatMessageParser parser = new ChatMessageParser();
			parser.Read(text);

			return parser.Inlines;
		}

		private static readonly Regex stripFormattingRegex = new Regex(@"\x02|\x0f|\x16|\x1f|\x03(?:\d{1,2}(?:,\d{1,2})?)?");
	}

	public class ChatMessageParser
	{
		public ChatMessageParser()
		{
		}

		public IEnumerable<Inline> Inlines
		{
			get
			{
				return inlines;
			}
		}

		public void Reset()
		{
			inlines.Clear();
			operations.Clear();
			hyperlinkLocations = null;
		}

		public void Read(string text)
		{
			// Find email addresses
			MatchCollection matches = emailRegex.Matches(text);

			foreach (Match match in matches)
			{
				if (match.Success)
				{
					Uri uri = new Uri("mailto:" + ChatMessage.RemoveFormatting(match.Value));
					AddHyperlinkLocation(new HyperlinkLocation(match.Index, match.Index + match.Length, uri));
				}
			}

			// Find URIs
			matches = uriRegex.Matches(text);

			foreach (Match match in matches)
			{
				if (match.Success)
				{
					Uri uri = new Uri(ChatMessage.RemoveFormatting(match.Value));
					AddHyperlinkLocation(new HyperlinkLocation(match.Index, match.Index + match.Length, uri));
				}
			}

			int runStart = 0;
			int runLength = 0;
			int i = 0;

			while (i < text.Length)
			{
				bool hyperlinkStart = IsHyperlinkStart(i);
				bool hyperlinkEnd = IsHyperlinkEnd(i);

				if (!hyperlinkStart && !hyperlinkEnd && !IsFormattingCode(text[i]))
				{
					i++;
					continue;
				}

				runLength = i - runStart;

				if (runLength > 0)
					AddInline(text.Substring(runStart, runLength));

				switch (text[i])
				{
					case (char)0x02:
						ToggleBold();
						i++;
						runStart = i;
						break;

					case (char)0x03:
						i += ParseColor(text, i);
						runStart = i;
						break;

					case (char)0x0F:
						operations.Clear();
						i++;
						runStart = i;
						break;

					case (char)0x16:
						ToggleItalic();
						i++;
						runStart = i;
						break;

					case (char)0x1F:
						ToggleUnderline();
						i++;
						runStart = i;
						break;

					default:
						if (hyperlinkStart)
							operations[TextOperationType.Hyperlink] = new HyperlinkTextOperation(GetHyperlinkUri(i));
						else if (hyperlinkEnd)
							operations.Remove(TextOperationType.Hyperlink);

						runStart = i;
						i++;
						break;
				}
			}

			runLength = text.Length - runStart;

			if (runLength > 0)
				AddInline(text.Substring(runStart, runLength));
		}

		private enum TextOperationType
		{
			Bold,
			Underline,
			Italic,
			ForegroundColor,
			BackgroundColor,
			Hyperlink
		}

		private abstract class TextOperation
		{
			public abstract Inline Apply(Inline inline);
		}

		private sealed class BoldTextOperation : TextOperation
		{
			public static BoldTextOperation Instance
			{
				get
				{
					return instance;
				}
			}

			public override Inline Apply(Inline inline)
			{
				return new Bold(inline);
			}

			private BoldTextOperation()
			{
			}

			private static readonly BoldTextOperation instance = new BoldTextOperation();
		}

		private sealed class ItalicTextOperation : TextOperation
		{
			public static ItalicTextOperation Instance
			{
				get
				{
					return instance;
				}
			}

			public override Inline Apply(Inline inline)
			{
				return new Italic(inline);
			}

			private ItalicTextOperation()
			{
			}

			private static readonly ItalicTextOperation instance = new ItalicTextOperation();
		}

		private sealed class UnderlineTextOperation : TextOperation
		{
			public static UnderlineTextOperation Instance
			{
				get
				{
					return instance;
				}
			}

			public override Inline Apply(Inline inline)
			{
				return new Underline(inline);
			}

			private UnderlineTextOperation()
			{
			}

			private static readonly UnderlineTextOperation instance = new UnderlineTextOperation();
		}

		private class HyperlinkTextOperation : TextOperation
		{
			public HyperlinkTextOperation(Uri uri)
			{
				this.uri = uri;
			}

			public override Inline Apply(Inline inline)
			{
				Hyperlink h = new Hyperlink(inline) { NavigateUri = uri, TextDecorations = null };
				h.Click += new RoutedEventHandler(Hyperlink_Click);
				return h;
			}

			private Uri uri;
		}

		private class ForegroundColorTextOperation : TextOperation
		{
			public ForegroundColorTextOperation(MircColor color)
			{
				this.color = color;
			}

			public override Inline Apply(Inline inline)
			{
				inline.Foreground = GetBrushFromMircColor(color);
				return inline;
			}

			private MircColor color;
		}

		private class BackgroundColorTextOperation : TextOperation
		{
			public BackgroundColorTextOperation(MircColor color)
			{
				this.color = color;
			}

			public override Inline Apply(Inline inline)
			{
				inline.Background = GetBrushFromMircColor(color);
				return inline;
			}

			private MircColor color;
		}

		private class HyperlinkLocation
		{
			public HyperlinkLocation(int startIndex, int endIndex, Uri uri)
			{
				StartIndex = startIndex;
				EndIndex = endIndex;
				Uri = uri;
			}

			public int StartIndex
			{
				get;
				private set;
			}

			public int EndIndex
			{
				get;
				private set;
			}

			public Uri Uri
			{
				get;
				private set;
			}
		}

		private void AddInline(string runText)
		{
			Inline insertInline = new Run(runText);

			foreach (TextOperation operation in operations.Values)
				insertInline = operation.Apply(insertInline);

			inlines.Add(insertInline);
		}

		/// <summary>
		/// Returns the length of the color code parsed
		/// </summary>
		/// <param name="text">The text to parse</param>
		/// <param name="startIndex">The index of the color code within the text</param>
		/// <returns></returns>
		private int ParseColor(string text, int startIndex)
		{
			int i = startIndex + 1;

			if (i >= text.Length)
			{
				// No text to follow this, so it's pointless to continue
				return 1;
			}

			char c1 = text[i];

			if (!Utilities.IsDigit(c1))
			{
				// The color is reset
				RemoveBackgroundColor();
				RemoveForegroundColor();

				return 1;
			}

			i++;

			if (i >= text.Length)
			{
				// No text to follow this, so it's pointless to continue
				return i - startIndex;
			}

			char c2 = text[i];

			bool hasBGColor = false;
			string fgCode = "";
			fgCode += c1;

			if (Utilities.IsDigit(c2))
			{
				// The foreground color has two digits
				fgCode += c2;
				i++;

				if (i < text.Length)
				{
					if (text[i] == ',')
						hasBGColor = true;
				}
			}
			else if (c2 == ',')
			{
				// The foreground color has a single digit
				hasBGColor = true;
			}

			SetForegroundColor(Byte.Parse(fgCode));

			if (!hasBGColor)
			{
				// We only have a foreground color, so we're done
				return i - startIndex;
			}

			i++;

			if (i >= text.Length)
			{
				// No text to follow this, so it's pointless to continue
				return i - startIndex;
			}

			c1 = text[i];

			if (!Utilities.IsDigit(c1))
			{
				// We found a comma, but there's no color after it, so it's
				// actually the first character of the next run
				return i - startIndex - 1;
			}

			i++;

			if (i >= text.Length)
			{
				// No text to follow this, so it's pointless to continue
				return i - startIndex;
			}

			c2 = text[i];

			string bgCode = "";
			bgCode += c1;

			if (Utilities.IsDigit(c2))
			{
				// The background color has two digits
				bgCode += c2;
				i++;
			}
			else
			{
				// The background color has a single digit
			}

			SetBackgroundColor(Byte.Parse(bgCode));

			return i - startIndex;
		}

		private void SetBackgroundColor(Byte byteColor)
		{
			if (byteColor >= numMircColors)
			{
				RemoveBackgroundColor();
				return;
			}

			MircColor color = (MircColor)byteColor;
			operations[TextOperationType.BackgroundColor] = new BackgroundColorTextOperation(color);
		}

		private void RemoveBackgroundColor()
		{
			operations.Remove(TextOperationType.BackgroundColor);
		}

		private void SetForegroundColor(Byte byteColor)
		{
			if (byteColor >= numMircColors)
			{
				RemoveForegroundColor();
				return;
			}

			MircColor color = (MircColor)byteColor;
			operations[TextOperationType.ForegroundColor] = new ForegroundColorTextOperation(color);
		}

		private void RemoveForegroundColor()
		{
			operations.Remove(TextOperationType.ForegroundColor);
		}

		private void ToggleBold()
		{
			if (operations.ContainsKey(TextOperationType.Bold))
				operations.Remove(TextOperationType.Bold);
			else
				operations.Add(TextOperationType.Bold, BoldTextOperation.Instance);
		}

		private void ToggleItalic()
		{
			if (operations.ContainsKey(TextOperationType.Italic))
				operations.Remove(TextOperationType.Italic);
			else
				operations.Add(TextOperationType.Italic, ItalicTextOperation.Instance);
		}

		private void ToggleUnderline()
		{
			if (operations.ContainsKey(TextOperationType.Underline))
				operations.Remove(TextOperationType.Underline);
			else
				operations.Add(TextOperationType.Underline, UnderlineTextOperation.Instance);
		}

		private bool IsHyperlinkStart(int index)
		{
			if (hyperlinkLocations != null)
			{
				foreach (HyperlinkLocation hl in hyperlinkLocations)
				{
					if (hl.StartIndex == index)
						return true;
				}
			}

			return false;
		}

		private bool IsHyperlinkEnd(int index)
		{
			if (hyperlinkLocations != null)
			{
				foreach (HyperlinkLocation hl in hyperlinkLocations)
				{
					if (hl.EndIndex == index)
						return true;
				}
			}

			return false;
		}

		private Uri GetHyperlinkUri(int index)
		{
			if (hyperlinkLocations != null)
			{
				foreach (HyperlinkLocation hl in hyperlinkLocations)
				{
					if (index >= hl.StartIndex && index <= hl.EndIndex)
						return hl.Uri;
				}
			}

			return null;
		}

		private void AddHyperlinkLocation(HyperlinkLocation hyperlinkLocation)
		{
			if (hyperlinkLocations == null)
				hyperlinkLocations = new List<HyperlinkLocation>();

			hyperlinkLocations.Add(hyperlinkLocation);
		}

		private static bool IsFormattingCode(char c)
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

		private static Brush GetBrushFromMircColor(MircColor color)
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
					return Brushes.Green;

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
					return Brushes.LightGreen;

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

		private static void Hyperlink_Click(object sender, RoutedEventArgs e)
		{
			Hyperlink hyperlink = (Hyperlink)sender;
			System.Diagnostics.Process.Start(hyperlink.NavigateUri.AbsoluteUri);
		}

		private const uint numMircColors = 16;

		private static readonly Regex uriRegex = new Regex(@"(((file|https?|ftp)://)|mailto:)[^\n\r\t ]+", RegexOptions.IgnoreCase | RegexOptions.Compiled);
		private static readonly Regex emailRegex = new Regex(@"[^\n\r\t ]+@[^\n\r\t ]+\.[^\n\r\t ]+");

		private List<Inline> inlines = new List<Inline>();
		private Dictionary<TextOperationType, TextOperation> operations = new Dictionary<TextOperationType, TextOperation>();
		private List<HyperlinkLocation> hyperlinkLocations = null;
	}
}
