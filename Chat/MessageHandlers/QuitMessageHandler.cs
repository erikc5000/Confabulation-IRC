using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Confabulation.Chat.MessageHandlers
{
    internal static class QuitMessageHandler
    {
        internal static void Process(IrcConnection connection, IrcMessage message)
        {
            IrcMessagePrefix prefix = message.Prefix;

            if (prefix == null)
            {
                Log.WriteLine("QUIT message is missing prefix");
                return;
            }

            byte[] byteNickname = prefix.ExtractNickname();
            string nickname = Encoding.UTF8.GetString(byteNickname);

            string quitMessage = null;
            if (message.Parameters.Count() > 0)
            {
                byte[] byteQuitMessage = message.Parameters.ElementAt(0);
                quitMessage = Encoding.UTF8.GetString(byteQuitMessage);
            }

            if (connection.User == null)
            {
                Log.WriteLine("Received QUIT message, but user isn't registered");
                return;
            }

            bool isSelf = connection.User.Equals(nickname);

            if (isSelf)
            {
                Log.WriteLine("Got QUIT message for self");
            }
            else
            {
                IrcUser user = connection.FindUser(nickname);

                if (user == null)
                {
                    Log.WriteLine("Received QUIT message from a user that doesn't exist");
                    return;
                }

                connection.OnUserQuit(user, quitMessage);
            }
        }
    }
}
