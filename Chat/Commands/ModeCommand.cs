using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Confabulation.Chat.Commands
{
	public class ModeCommand : IrcCommand
	{
		// Modes are parsed and handled in a very primitive way right now that provides limited checking.
		// This will need to be revisited, but it works for the moment.

		public static new ModeCommand Parse(string parameters)
		{
			if (parameters == null || parameters.Length == 0)
				throw new IrcCommandException(IrcCommandExceptionType.TooFewParameters, syntax);

			string[] splitParams = parameters.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

			if (splitParams.Length < 2)
				throw new IrcCommandException(IrcCommandExceptionType.TooFewParameters, syntax);

			string target = splitParams[0];
			string[] modes = new string[splitParams.Count() - 1];
			
			for (int i = 1; i < splitParams.Count(); i++)
				modes[i] = splitParams[i];

			return new ModeCommand(target, modes);
		}

		static ModeCommand()
		{
			IrcCommand.Register("mode", ModeCommand.Parse);
		}

		public ModeCommand(string target, params string[] modes)
		{
			if (target == null)
				throw new ArgumentNullException("target");
			else if (modes == null)
				throw new ArgumentNullException("modes");

			this.target = target;
			this.modes = modes;
		}


		public override void Execute(IrcClient client)
		{
			if (client == null)
				throw new ArgumentNullException("client");

			if (!Irc.IsValidChannelName(target) && !Irc.IsValidNickname(target))
			{
				throw new IrcCommandException(IrcCommandExceptionType.InvalidParameter,
											  "Invalid target");
			}

			byte[][] byteParameters = new byte[modes.Count() + 1][];
			byteParameters[0] = Encoding.UTF8.GetBytes(target);
			int i = 1;

			foreach (string s in modes)
			{
				if (!Irc.IsValidMessageContent(s))
					throw new IrcCommandException(IrcCommandExceptionType.InvalidParameter, "Modes are invalid");

				byteParameters[i] = Encoding.UTF8.GetBytes(s);
				i++;
			}

			client.Send(new IrcMessage(command, byteParameters));
		}

		private string target = null;
		private string[] modes = null;

		private static readonly byte[] command = Encoding.UTF8.GetBytes("MODE");
		private const string syntax = "/mode nickname|channel [[+|-]modes modeparams]] ...";
	}
}
