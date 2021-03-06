﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Confabulation.Chat;

namespace Confabulation.Chat.Commands
{
	public class NoticeCommand : IrcCommand
	{
		public static new NoticeCommand Parse(string parameters)
		{
			if (parameters == null || parameters.Length == 0)
				throw new IrcCommandException(IrcCommandExceptionType.TooFewParameters, syntax);

			string[] splitParams = parameters.Split(new char[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);

			if (splitParams.Length < 2)
				throw new IrcCommandException(IrcCommandExceptionType.TooFewParameters, syntax);

			return new NoticeCommand(splitParams[0], splitParams[1]);
		}

		public NoticeCommand(string target, string message)
		{
			if (target == null)
				throw new ArgumentNullException("target");
			else if (message == null)
				throw new ArgumentNullException("message");

			this.target = target;
			this.message = message;
		}

		public NoticeCommand(IrcUser user, string message)
		{
			if (user == null)
				throw new ArgumentNullException("user");
			else if (message == null)
				throw new ArgumentNullException("message");

			this.target = user.Nickname;
			this.message = message;
		}

		public NoticeCommand(IrcChannel channel, string message)
		{
			if (channel == null)
				throw new ArgumentNullException("channel");
			else if (message == null)
				throw new ArgumentNullException("message");

			this.target = channel.Name;
			this.message = message;
		}

		public override void Execute(IrcClient client)
		{
			if (client == null)
				throw new ArgumentNullException("client");

			if (!Irc.IsValidMessageTarget(target))
			{
				throw new IrcCommandException(IrcCommandExceptionType.InvalidParameter,
											  "'" + target + "' is not a valid message target");
			}

			if (!Irc.IsValidMessageContent(message))
			{
				throw new IrcCommandException(IrcCommandExceptionType.InvalidParameter,
											  "Invalid message content");
			}

			client.Send(new IrcMessage(command, Encoding.UTF8.GetBytes(target), Encoding.UTF8.GetBytes(message)));
		}

		private string target;
		private string message;

		private const string syntax = "/notice <target> <message>";
		private static readonly byte[] command = Encoding.UTF8.GetBytes("NOTICE");
	}
}
