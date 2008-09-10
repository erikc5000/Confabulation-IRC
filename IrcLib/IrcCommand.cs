using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Confabulation.Chat.Commands;

namespace Confabulation.Chat
{
    public static class IrcCommand
    {
		public static void ParseAndExecute(IrcClient client, string commandString)
		{
			if (client == null)
				throw new ArgumentNullException("client");
			else if (commandString == null)
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

			commandMap[commandName](client, commandArgs);
		}

		static IrcCommand()
		{
			commandMap.Add("nick", NickCommand.ParseAndExecute);
			commandMap.Add("join", JoinCommand.ParseAndExecute);
			commandMap.Add("quit", QuitCommand.ParseAndExecute);
			commandMap.Add("msg", MsgCommand.ParseAndExecute);
			commandMap.Add("notice", NoticeCommand.ParseAndExecute);
			commandMap.Add("part", PartCommand.ParseAndExecute);
			commandMap.Add("partall", PartAllCommand.ParseAndExecute);
			commandMap.Add("topic", TopicCommand.ParseAndExecute);
			commandMap.Add("away", AwayCommand.ParseAndExecute);
			commandMap.Add("raw", RawCommand.ParseAndExecute);
			commandMap.Add("kick", KickCommand.ParseAndExecute);
		}

		private static Dictionary<string, Action<IrcClient, string>> commandMap = new Dictionary<string, Action<IrcClient, string>>();
	}
}
