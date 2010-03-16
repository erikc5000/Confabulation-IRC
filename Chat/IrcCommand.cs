using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Confabulation.Chat.Commands;

namespace Confabulation.Chat
{
	public abstract class IrcCommand
	{
		static IrcCommand()
		{
			Register("away", AwayCommand.Parse);
			Register("nick", NickCommand.Parse);
			Register("join", JoinCommand.Parse);
			Register("kick", KickCommand.Parse);
			Register("mode", ModeCommand.Parse);
			Register("msg", MsgCommand.Parse);
			Register("quit", QuitCommand.Parse);
			Register("notice", NoticeCommand.Parse);
			Register("part", PartCommand.Parse);
			Register("partall", PartAllCommand.Parse);
			Register("raw", RawCommand.Parse);
			Register("topic", TopicCommand.Parse);
		}

		public static IrcCommand Parse(string commandString)
		{
			if (commandString == null)
				throw new ArgumentNullException("commandString");

			commandString = commandString.TrimStart(' ', '\t', '\r', '\n');
			commandString = commandString.TrimEnd('\r', '\n');
			string[] parts = commandString.Split(new char[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);

			if (parts.Length == 0)
				throw new ArgumentException("No command provided");

			string commandName = parts.First().ToLower();
			string commandArgs = null;

			if (parts.Length == 2)
				commandArgs = parts.Last();

			if (commandName.StartsWith("/"))
				commandName = commandName.Remove(0, 1);

			if (!commandMap.ContainsKey(commandName))
			{
				throw new IrcCommandException(IrcCommandExceptionType.InvalidCommand,
											  commandName);
			}

			return commandMap[commandName](commandArgs);
		}

		internal static void Register(string name, Func<string, IrcCommand> parseFunction)
		{
			commandMap.Add(name, parseFunction);
		}

		public abstract void Execute(IrcClient client);

		private static Dictionary<string, Func<string, IrcCommand>> commandMap = new Dictionary<string, Func<string, IrcCommand>>();
	}
}
