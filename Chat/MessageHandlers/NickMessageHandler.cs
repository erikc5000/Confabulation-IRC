using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Confabulation.Chat.MessageHandlers
{
	internal static class NickMessageHandler
	{
		static NickMessageHandler()
		{
			IrcMessageHandler.Register("KICK", Process);
		}

		internal static void Process(IrcConnection connection, IrcMessage message)
		{
			IrcMessagePrefix prefix = message.Prefix;

			if (prefix == null)
			{
				Log.WriteLine("NICK message is missing prefix");
				return;
			}

			if (message.Parameters.Count() < 1)
			{
				Log.WriteLine("NICK message is missing parameters");
				return;
			}

			byte[] byteCurrentNickname = prefix.ExtractNickname();
			string currentNickname = Encoding.UTF8.GetString(byteCurrentNickname);

			byte[] byteNewNickname = message.Parameters.ElementAt(0);
			string newNickname = Encoding.UTF8.GetString(byteNewNickname);

			if (connection.User == null)
			{
				Log.WriteLine("Received NICK message, but user isn't registered");
				return;
			}

			bool isSelf = connection.User.Equals(currentNickname);

			if (isSelf)
			{
				connection.User.ChangeNickname(newNickname);
			}
			else
			{
				IrcUser user = connection.FindUser(currentNickname);

				if (user == null)
				{
					Log.WriteLine("Received NICK message from a user that doesn't exist");
					return;
				}

				connection.UpdateUserNickname(user, newNickname);
				user.ChangeNickname(newNickname);
			}
		}
	}
}
