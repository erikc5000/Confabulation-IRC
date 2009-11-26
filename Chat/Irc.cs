using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Confabulation.Chat
{
	[FlagsAttribute]
	public enum InitialUserMode
	{
		None = 0,
		Wallops = 4,
		Invisible = 8
	}

    public enum MircColor
    {
        White = 0,
        Black = 1,
        DarkBlue = 2,
        DarkGreen = 3,
        Red = 4,
        Brown = 5,
        Purple = 6,
        Olive = 7,
        Yellow = 8,
        Green = 9,
        Teal = 10,
        Cyan = 11,
        Blue = 12,
        Magenta = 13,
        DarkGray = 14,
        LightGray = 15
    }

	public static class Irc
	{
		/*public static byte[] StringToByteArray(string s)
		{
			byte[] byteArray;

			try
			{
				Encoding utf8 = Encoding.GetEncoding("utf-8", new EncoderExceptionFallback(), new DecoderExceptionFallback());
				byteArray = utf8.GetBytes(s);
			}
			catch (EncoderFallbackException)
			{
				try
				{
					byteArray = Encoding.GetEncoding(1252).GetBytes(s);
				}
				catch (Exception)
				{

				}
			}
		}*/

		public static bool NamesEqual(string x, string y, IrcServerProperties properties)
		{
			if (x == null && y == null)
				return true;
			else if (x == null || y == null)
				return false;

			if (x.Length != y.Length)
				return false;

			IrcCaseMapping caseMapping = properties.CaseMapping;
			int maxLowerCase;

			switch (caseMapping)
			{
				case IrcCaseMapping.Ascii:
					maxLowerCase = 90;
					break;

				case IrcCaseMapping.StrictRfc1459:
					maxLowerCase = 93;
					break;

				case IrcCaseMapping.Rfc1459:
				default:
					maxLowerCase = 94;
					break;
			}

			for (int i = 0; i < x.Length; i++)
			{
				char a = x[i];
				char b = y[i];

				if (a >= 65 && a <= maxLowerCase)
					a += (char)32;

				if (b >= 65 && b <= maxLowerCase)
					b += (char)32;

				if (a != b)
					return false;
			}

			return true;
		}

		public static bool IsValidCommand(byte[] command)
		{
			if (command == null)
				throw new ArgumentNullException("command");

			// Commands must have at least one digit
			if (command.Length == 0)
				return false;

			// Decide whether this is a numeric or a text command
			if (Utilities.IsDigit(command.First()))
			{
				if (command.Length != 3)
					return false;

				if (!Utilities.IsDigit(command[1]) || !Utilities.IsDigit(command[2]))
					return false;
			}
			else
			{
				foreach (byte b in command)
				{
					if (!Utilities.IsLetter(b))
						return false;
				}
			}

			return true;
		}

		public static bool IsValidNickname(string nickname)
		{
			if (nickname == null)
				throw new ArgumentNullException("nickname");

			if (nickname.Length == 0)
				return false;

			foreach (char c in nickname)
			{
				if (c == '\0' || c == ' ' || c == '\r' || c == '\n' || c == ':')
					return false;
			}

			return true;
		}

		public static bool IsValidUserName(string userName)
		{
			if (userName == null)
				throw new ArgumentNullException("userName");

			if (userName.Length == 0)
				return false;

			foreach (char c in userName)
			{
				if (c == '\0' || c == ' ' || c == '\r' || c == '\n' || c == '@')
					return false;
			}

			return true;
		}

		public static bool IsValidChannelName(string channelName)
		{
			if (channelName == null)
				throw new ArgumentNullException("channelName");

			if (channelName.Length < 2)
				return false;

			if (channelName[0] != '#' && channelName[0] != '&' && channelName[0] != '+'
				&& channelName[0] != '!')
			{
				return false;
			}

			foreach (char c in channelName)
			{
				if (c == '\0' || c == ' ' || c == '\r' || c == '\n' || c == ',' || c == 0x07)
					return false;
			}

			return true;
		}

		public static bool IsValidChannelKey(string key)
		{
			if (key == null)
				throw new ArgumentNullException("key");

			if (key.Length == 0)
				return false;

			foreach (char c in key)
			{
				if (c > 0x7F || c == '\0' || c == ' ' || c == '\r' || c == '\n'
				    || c == '\t' || c == 0x0B || c == 0x0C)
				{
					return false;
				}
			}

			return true;
		}

		public static bool IsValidMessageTarget(string target)
		{
			if (target == null)
				throw new ArgumentNullException("target");

			string[] msgToArray = target.Split(',');

			foreach (string msgTo in msgToArray)
			{
				if (msgTo.Length == 0)
					return false;

				// TODO: Add more extensive checking
			}

			return true;
		}

		public static bool IsValidTargetMask(string msgTo)
		{
			if (msgTo == null)
				throw new ArgumentNullException("msgTo");

			if (msgTo.Length == 0)
				return false;

			if (msgTo[0] != '$' && msgTo[0] != '#')
				return false;

			// TODO: More extensive checking

			return true;
		}

		public static bool IsValidMessageContent(string message)
		{
			if (message == null)
				throw new ArgumentNullException("message");

			foreach (char c in message)
			{
				if (c == '\0' || c == '\r' || c == '\n')
					return false;
			}

			return true;
		}

		public static bool IsValidMessageContent(byte[] message)
		{
			if (message == null)
				throw new ArgumentNullException("message");

			foreach (byte b in message)
			{
				if (b == '\0' || b == '\r' || b == '\n')
					return false;
			}

			return true;
		}

		public const int DefaultServerPort = 6667;
	}
}
