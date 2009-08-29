using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Confabulation.Chat.Commands;

namespace Confabulation.Chat
{
    public abstract class IrcCommand
    {
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

		static IrcCommand()
		{
			commandMap.Add("nick", NickCommand.Parse);
			commandMap.Add("join", JoinCommand.Parse);
			commandMap.Add("quit", QuitCommand.Parse);
			commandMap.Add("msg", MsgCommand.Parse);
			commandMap.Add("notice", NoticeCommand.Parse);
			commandMap.Add("part", PartCommand.Parse);
			commandMap.Add("partall", PartAllCommand.Parse);
			commandMap.Add("topic", TopicCommand.Parse);
			commandMap.Add("away", AwayCommand.Parse);
			commandMap.Add("raw", RawCommand.Parse);
			commandMap.Add("kick", KickCommand.Parse);
		}

		public abstract void Execute(IrcClient client);

		private static Dictionary<string, Func<string, IrcCommand>> commandMap = new Dictionary<string, Func<string, IrcCommand>>();
	}
}
