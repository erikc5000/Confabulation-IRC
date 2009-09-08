using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Confabulation.Chat
{
	public enum IrcNumericReply
	{
		// Command responses
		RPL_WELCOME = 001,
		RPL_YOURHOST = 002,
		RPL_CREATED = 003,
		RPL_MYINFO = 004,
		RPL_ISUPPORT = 005,
		RPL_TRACELINK = 200,
		RPL_TRACECONNECTING = 201,
		RPL_TRACEHANDSHAKE = 202,
		RPL_TRACEUNKNOWN = 203,
		RPL_TRACEOPERATOR = 204,
		RPL_TRACEUSER = 205,
		RPL_TRACESERVER = 206,
		RPL_TRACESERVICE = 207,
		RPL_TRACENEWTYPE = 208,
		RPL_TRACECLASS = 209,
		RPL_TRACERECONNECT = 210,
		RPL_STATSLINKINFO = 211,
		RPL_STATSCOMMANDS = 212,
		RPL_ENDOFSTATS = 219,
		RPL_UMODEIS = 221,
		RPL_SERVLIST = 234,
		RPL_SERVLISTEND = 235,
		RPL_STATSUPTIME = 242,
		RPL_STATSOLINE = 243,
		RPL_LUSERCLIENT = 251,
		RPL_LUSEROP = 252,
		RPL_LUSERUNKNOWN = 253,
		RPL_LUSERCHANNELS = 254,
		RPL_LUSERME = 255,
		RPL_ADMINME = 256,
		RPL_ADMINLOC1 = 257,
		RPL_ADMINLOC2 = 258,
		RPL_ADMINEMAIL = 259,
		RPL_TRACELOG = 261,
		RPL_TRACEEND = 262,
		RPL_TRYAGAIN = 263,
		RPL_AWAY = 301,
		RPL_USERHOST = 302,
		RPL_ISON = 303,
		RPL_UNAWAY = 305,
		RPL_NOWAWAY = 306,
		RPL_WHOISUSER = 311,
		RPL_WHOISSERVER = 312,
		RPL_WHOISOPERATOR = 313,
		RPL_WHOWASUSER = 314,
		RPL_ENDOFWHO = 315,
		RPL_WHOISIDLE = 317,
		RPL_ENDOFWHOIS = 318,
		RPL_WHOISCHANNELS = 319,
		RPL_LISTSTART = 321,
		RPL_LIST = 322,
		RPL_LISTEND = 323,
		RPL_CHANNELMODEIS = 324,
		RPL_UNIQOPIS = 325,
		RPL_NOTOPIC = 331,
		RPL_TOPIC = 332,
		RPL_TOPICWHOTIME = 333,
		RPL_INVITING = 341,
		RPL_SUMMONING = 342,
		RPL_INVITELIST = 346,
		RPL_ENDOFINVITELIST = 347,
		RPL_EXCEPTLIST = 348,
		RPL_ENDOFEXCEPTLIST = 349,
		RPL_VERSION = 351,
		RPL_WHOREPLY = 352,
		RPL_NAMEREPLY = 353,
		RPL_LINKS = 364,
		RPL_ENDOFLINKS = 365,
		RPL_ENDOFNAMES = 366,
		RPL_BANLIST = 367,
		RPL_ENDOFBANLIST = 368,
		RPL_ENDOFWHOWAS = 369,
		RPL_INFO = 371,
		RPL_ENDOFINFO = 374,
		RPL_MOTDSTART = 375,
		RPL_MOTD = 372,
		RPL_ENDOFMOTD = 376,
		RPL_YOUREOPER = 381,
		RPL_REHASHING = 382,
		RPL_YOURESERVICE = 383,
		RPL_TIME = 391,
		RPL_USERSSTART = 392,
		RPL_USERS = 393,
		RPL_ENDOFUSERS = 394,
		RPL_NOUSERS = 395,

		// Error replies
		ERR_NOSUCHNICK = 401,
		ERR_NOSUCHSERVER = 402,
		ERR_NOSUCHCHANNEL = 403,
		ERR_CANNOTSENDTOCHAN = 404,
		ERR_TOOMANYCHANNELS = 405,
		ERR_WASNOSUCHNICK = 406,
		ERR_TOOMANYTARGETS = 407,
		ERR_NOSUCHSERVICE = 408,
		ERR_NOORIGIN = 409,
		ERR_NORECIPIENT = 411,
		ERR_NOTEXTTOSEND = 412,
		ERR_NOTOPLEVEL = 413,
		ERR_WILDTOPLEVEL = 414,
		ERR_BADMASK = 415,
		ERR_UNKNOWNCOMMAND = 421,
		ERR_NOMOTD = 422,
		ERR_NOADMININFO = 423,
		ERR_FILEERROR = 424,
		ERR_NONICKNAMEGIVEN = 431,
		ERR_ERRONEUSNICKNAME = 432,
		ERR_NICKNAMEINUSE = 433,
		ERR_NICKCOLLISION = 436,
		ERR_UNAVAILRESOURCE = 437,
		ERR_USERNOTINCHANNEL = 441,
		ERR_NOTONCHANNEL = 442,
		ERR_USERONCHANNEL = 443,
		ERR_NOLOGIN = 444,
		ERR_SUMMONDISABLED = 445,
		ERR_USERSDISABLED = 446,
		ERR_NOTREGISTERED = 451,
		ERR_NEEDMOREPARAMS = 461,
		ERR_ALREADYREGISTRED = 462,
		ERR_NOPERMFORHOST = 463,
		ERR_PASSWDMISMATCH = 464,
		ERR_YOUREBANNEDCREEP = 465,
		ERR_YOUWILLBEBANNED = 466,
		ERR_KEYSET = 467,
		ERR_CHANNELISFULL = 471,
		ERR_UNKNOWNMODE = 472,
		ERR_INVITEONLYCHAN = 473,
		ERR_BANNEDFROMCHAN = 474,
		ERR_BADCHANNELKEY = 475,
		ERR_BADCHANMASK = 476,
		ERR_NOCHANMODES = 477,
		ERR_BANLISTFULL = 478,
		ERR_NOPRIVILEGES = 481,
		ERR_CHANOPRIVSNEEDED = 482,
		ERR_CANTKILLSERVER = 483,
		ERR_RESTRICTED = 484,
		ERR_UNIQOPPRIVSNEEDED = 485,
		ERR_NOOPERHOST = 491,
		ERR_UMODEUNKNOWNFLAG = 501,
		ERR_USERSDONTMATCH = 502
	}

	public class IrcMessage
	{
		public IrcMessage(byte[] message, int offset, int length)
		{
			// Make sure the parameters are good
			if (message == null)
				throw new ArgumentNullException("message");

			if (offset < 0 || offset >= message.Length)
				throw new ArgumentOutOfRangeException("offset");

			if (length < 0 || length > message.Length - offset)
				throw new ArgumentOutOfRangeException("length");

			// Calculate the message length before anything else
			messageLength = length;

			for (int i = offset + messageLength - 1; i >= offset; i--)
			{
				if (message[i] != '\n' && message[i] != '\r')
					break;

				messageLength--;
			}

			if (messageLength == 0)
				throw new ArgumentException("Message is empty");

			int spaceIndex;
			int currentIndex = offset;

			// Check for prefix
			if (message[currentIndex] == (byte)':')
			{
				int prefixLength;
				spaceIndex = Array.IndexOf(message, (byte)' ', currentIndex + 1);

				if (spaceIndex >= messageLength + offset || spaceIndex < 0)
					prefixLength = messageLength - 1;
				else
					prefixLength = spaceIndex - 1 - offset;

				byte[] prefixString = new byte[prefixLength];
				Array.Copy(message, currentIndex + 1, prefixString, 0, prefixLength);
				prefix = new IrcMessagePrefix(prefixString);

				currentIndex = spaceIndex + 1;
			}
			
			// Get the command
			int commandLength;
			spaceIndex = Array.IndexOf(message, (byte)' ', currentIndex);

			if (spaceIndex >= messageLength + offset || spaceIndex < 0)
				commandLength = messageLength - currentIndex + offset;
			else
				commandLength = spaceIndex - currentIndex;

			command = new byte[commandLength];
			Array.Copy(message, currentIndex, command, 0, commandLength);

			if (!Irc.IsValidCommand(command))
				throw new ArgumentException("Invalid command");

			// If no space was found earlier, there can't be any parameters
			if (spaceIndex < 0)
				return;

			// Parse parameters
			currentIndex = spaceIndex + 1;
			int numParams = 0;

			while (currentIndex < messageLength + offset)
			{
				int paramLength;

				// ':' signifies that this is the last parameter and any
				// further spaces are considered part of it.  If this is the
				// 15th parameter, it is automatically considered the last.
				if (message[currentIndex] == (byte)':'
				    || numParams + 1 == maxParameters)
				{
					if (message[currentIndex] != (byte)':')
						messageLength++;

					currentIndex++;
					paramLength = messageLength + offset - currentIndex;
				}
				else
				{
					spaceIndex = Array.IndexOf(message, (byte)' ', currentIndex);

					if (spaceIndex >= messageLength + offset || spaceIndex < 0)
						paramLength = messageLength + offset - currentIndex;
					else
						paramLength = spaceIndex - currentIndex;
				}

				// We ignore zero length parameters, but maybe they should be
				// error conditions
				if (paramLength <= 0)
					throw new ArgumentException("Zero length arguments are not allowed");

				byte[] param = new byte[paramLength];
				Array.Copy(message, currentIndex, param, 0, paramLength);

				if (!Irc.IsValidMessageContent(param))
					throw new ArgumentException("Invalid parameter");

				parameters.Add(param);
				numParams++;

				currentIndex += paramLength + 1;
			}

			messageLength += crlf.Length;

			if (messageLength > maxMessageSize)
				throw new ArgumentException("Message exceeds the maximum size");
		}

		public IrcMessage(byte[] command)
		{
			if (command == null)
				throw new ArgumentNullException("command");

			if (!Irc.IsValidCommand(command))
				throw new ArgumentException("Invalid command");

			messageLength = command.Length + crlf.Length;

			if (messageLength > maxMessageSize)
				throw new ArgumentException("Message exceeds the maximum size");

			this.command = command;
		}

		public IrcMessage(byte[] command, params byte[][] parameters)
		{
			if (command == null)
				throw new ArgumentNullException("command");

			if (parameters == null)
				throw new ArgumentNullException("parameters");

			if (!Irc.IsValidCommand(command))
				throw new ArgumentException("Invalid command");

			messageLength = command.Length + crlf.Length;

			foreach (byte[] param in parameters)
			{
				if (!Irc.IsValidMessageContent(param))
					throw new ArgumentException("Invalid parameter");

				messageLength += param.Length + 1;
			}

			if (parameters.Count() > 0 && Utilities.ContainsSpace(parameters.Last()))
				messageLength += 1;

			if (messageLength > maxMessageSize)
				throw new ArgumentException("Message exceeds the maximum size");

			this.command = command;
			this.parameters.AddRange(parameters);
		}

		public byte[] ToByteArray()
		{
			// Create the byte array and copy components into it
			byte[] message = new byte[messageLength];

			int currentIndex = 0;

			if (prefix != null)
			{
				message[currentIndex] = (byte)':';
				currentIndex++;

				prefix.Text.CopyTo(message, currentIndex);
				currentIndex += prefix.Text.Length;

				message[currentIndex] = (byte)' ';
				currentIndex++;
			}

			command.CopyTo(message, currentIndex);
			currentIndex += command.Length;

			foreach (byte[] param in parameters)
			{
				message[currentIndex] = (byte)' ';
				currentIndex++;

				if (param == parameters.Last() && Utilities.ContainsSpace(param))
				{
					message[currentIndex] = (byte)':';
					currentIndex++;
				}

				param.CopyTo(message, currentIndex);
				currentIndex += param.Length;
			}

			crlf.CopyTo(message, currentIndex);

			return message;
		}

		public bool IsNumericReply()
		{
			return Utilities.IsDigit(command[0]);
		}

		public bool IsCommand()
		{
			return !IsNumericReply();
		}

		public IrcNumericReply GetReplyCode()
		{
			int digit1 = command[0] - 48;
			int digit2 = command[1] - 48;
			int digit3 = command[2] - 48;
			int code = digit1 * 100 + digit2 * 10 + digit3;

			return (IrcNumericReply)code;
		}

		public byte[] GetCommand()
		{
			return command;
		}

		public override string ToString()
		{
			return Encoding.UTF8.GetString(ToByteArray());
		}

		public IrcMessagePrefix Prefix
		{
			get
			{
				return prefix;
			}
		}

		public IEnumerable<byte[]> Parameters
		{
			get
			{
				return parameters;
			}
		}

		private const int maxMessageSize = 512;
		private const int maxParameters = 15;
		private static readonly byte[] crlf = new byte[] { 0x0D, 0x0A };

		private int messageLength;
		private IrcMessagePrefix prefix = null;
		private byte[] command = null;
		private List<byte[]> parameters = new List<byte[]>();
	}
}
