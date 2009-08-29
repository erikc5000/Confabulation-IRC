using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Confabulation.Chat;
using Confabulation.Chat.Commands;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            IrcClient client = new IrcClient("Krypt.CA.US.GameSurge.net", 6667);
			client.ConnectionEvent += new EventHandler<IrcConnectionEventArgs>(OnConnectionEvent);
			client.MessageReceived += new EventHandler<IrcMessageEventArgs>(OnMessageReceived);

            client.Connect();

			while (true)
			{
				string input = Console.ReadLine();

				if (input.ToLower() == "exit")
					break;

				if (client.ConnectionState != IrcConnectionState.Connected)
				{
					Console.WriteLine("Not connected");
					continue;
				}

				try
				{
					//IrcCommand.ParseAndExecute(client, input);
				}
				catch (IrcCommandException)
				{
					Console.WriteLine("Invalid command");
				}
				catch (ArgumentException e)
				{
					Console.WriteLine("Invalid argument: " + e.ParamName);
				}
			}
        }

		static void OnChannelJoin(object sender, IrcChannelEventArgs e)
		{
			throw new NotImplementedException();
		}

		private static void OnConnectionEvent(object sender, IrcConnectionEventArgs args)
		{
			switch (args.EventType)
			{
				case IrcConnectionEventType.Connected:
					System.Console.WriteLine("Connected");
					IrcClient client = (IrcClient)sender;
					//NickCommand.Execute(client, "BlasterTest");
					//UserCommand.Execute(client, "guest", InitialUserMode.Invisible, "The Real Me");
					break;

				case IrcConnectionEventType.ConnectFailed:
					System.Console.WriteLine("Connect failed");
					break;

				case IrcConnectionEventType.Disconnected:
					System.Console.WriteLine("Disconnected");
					break;
			}
		}

		private static void OnMessageReceived(object sender, IrcMessageEventArgs args)
		{
			System.Console.Write(args.Message.ToString());

			if (Encoding.UTF8.GetString(args.Message.GetCommand()) == "PING")
			{
				IrcClient client = (IrcClient)sender;
				//PongCommand.Execute(client, Encoding.UTF8.GetString(args.Message.Parameters.First()));
			}
		}
    }
}
