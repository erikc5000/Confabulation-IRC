using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Confabulation.Chat.MessageHandlers
{
    internal static class PrivmsgMessageHandler
    {
		static PrivmsgMessageHandler()
		{
			IrcMessageHandler.Register("PRIVMSG", Process);
		}

        internal static void Process(IrcConnection connection, IrcMessage message)
        {
            IrcMessagePrefix prefix = message.Prefix;

            if (prefix == null)
            {
                Log.WriteLine("PRIVMSG message is missing prefix");
                return;
            }

            if (message.Parameters.Count() < 2)
            {
                Log.WriteLine("PRIVMSG message is missing parameters");
                return;
            }

            byte[] byteNickname = prefix.ExtractNickname();
            string nickname = Encoding.UTF8.GetString(byteNickname);

            byte[] byteTarget = message.Parameters.ElementAt(0);
            string target = Encoding.UTF8.GetString(byteTarget);

            byte[] byteMessage = message.Parameters.ElementAt(1);
            string messageText = Encoding.UTF8.GetString(byteMessage);

            if (connection.User == null)
            {
                Log.WriteLine("Received PRIVMSG message, but user isn't registered");
                return;
            }

            if (Irc.IsValidChannelName(target))
            {
                IrcChannel channel = connection.FindChannel(target);

                if (channel == null)
                {
                    Log.WriteLine("PrivmsgMessageHandler: Channel doesn't exist!");
                    return;
                }

                IrcUser user = connection.FindUser(nickname);

                if (user == null)
                    user = connection.AddUser(nickname);

                channel.OnMessageReceived(user, messageText);
            }
            else
            {
                //IrcUser user = connection.FindUser(target);

                //if (user == null)
                //    user = connection.AddUser(target);
            }
        }
    }
}
