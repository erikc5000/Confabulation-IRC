using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Confabulation.Chat.MessageHandlers
{
	internal static class ReplyMessageHandler
	{
		internal static void Process(IrcConnection connection, IrcMessage message)
		{
			IrcNumericReply replyCode = message.GetReplyCode();

			if (replyMap.ContainsKey(replyCode))
			{
				replyMap[replyCode](connection, message);
				return;
			}

		//    RPL_WELCOME = 001,
		//RPL_YOURHOST = 002,
		//RPL_CREATED = 003,
		//RPL_MYINFO = 004,
		//RPL_ISUPPORT = 005,
		//RPL_TRACELINK = 200,
		//RPL_TRACECONNECTING = 201,
		//RPL_TRACEHANDSHAKE = 202,
		//RPL_TRACEUNKNOWN = 203,
		//RPL_TRACEOPERATOR = 204,
		//RPL_TRACEUSER = 205,
		//RPL_TRACESERVER = 206,
		//RPL_TRACESERVICE = 207,
		//RPL_TRACENEWTYPE = 208,
		//RPL_TRACECLASS = 209,
		//RPL_TRACERECONNECT = 210,
		//RPL_STATSLINKINFO = 211,
		//RPL_STATSCOMMANDS = 212,
		//RPL_ENDOFSTATS = 219,
		//RPL_UMODEIS = 221,
		//RPL_SERVLIST = 234,
		//RPL_SERVLISTEND = 235,
		//RPL_STATSUPTIME = 242,
		//RPL_STATSOLINE = 243,
		}

		static void ProcessWelcome(IrcConnection connection, IrcMessage message)
		{
			if (message.Parameters.Count() == 2)
			{
				string messageString = Encoding.UTF8.GetString(message.Parameters.ElementAt(1));
				int lastSpaceIndex = messageString.LastIndexOf(' ');

				if (lastSpaceIndex >= 0)
				{
					string userInfo = messageString.Substring(lastSpaceIndex + 1);
					string[] parts = userInfo.Split(new char[] { '!' }, 2);

					if (parts.Count() == 1)
					{
						IrcUser user = new IrcUser(parts[0]);
						connection.SetUser(user);
						return;
					}
					else if (parts.Count() == 2)
					{
						string nickname = parts[0];
						parts = parts[1].Split(new char[] { '@' }, 2);

						if (parts.Count() == 2)
						{
							string userName = parts[0];
							string hostname = parts[1];

							IrcUser user = new IrcUser(nickname, userName, hostname);
							connection.SetUser(user);
							return;
						}
					}
				}
			}

			if (message.Parameters.Count() >= 1)
			{
				IrcUser user = new IrcUser(Encoding.UTF8.GetString(message.Parameters.ElementAt(0)));
				connection.SetUser(user);
				return;
			}

			Log.WriteLine("RPL_WELCOME doesn't appear to have properly formatted user info");
			connection.Close();
		}

		static void ProcessISupport(IrcConnection connection, IrcMessage message)
		{
			int count = message.Parameters.Count();

			if (count < 3)
			{
				Log.WriteLine("RPL_ISUPPORT doesn't appear to be formatted correctly");
				return;
			}

			foreach (byte[] byteParam in message.Parameters)
			{
				if (byteParam == message.Parameters.First() ||
					byteParam == message.Parameters.Last())
				{
					continue;
				}

				string param = Encoding.UTF8.GetString(byteParam);

				if (param.Contains('='))
				{
					string[] parts = param.Split('=');

					if (parts.Count() != 2)
					{
						Log.WriteLine("RPL_ISUPPORT parameter isn't formatted correctly");
						continue;
					}

					connection.ServerProperties.Add(parts[0], parts[1]);
				}
				else
				{
					connection.ServerProperties.Add(param, null);
				}
			}
		}

		static void ProcessNoTopic(IrcConnection connection, IrcMessage message)
		{
			if (message.Parameters.Count() != 3)
			{
				Log.WriteLine("RPL_NOTOPIC doesn't appear to be formatted correctly");
				return;
			}

			string channelName = Encoding.UTF8.GetString(message.Parameters.ElementAt(1));
			IrcChannel channel = connection.FindChannel(channelName);

			if (channel != null)
				channel.SetTopic(null);
		}

		static void ProcessTopic(IrcConnection connection, IrcMessage message)
		{
			if (message.Parameters.Count() != 3)
			{
				Log.WriteLine("RPL_TOPIC doesn't appear to be formatted correctly");
				return;
			}

			string channelName = Encoding.UTF8.GetString(message.Parameters.ElementAt(1));
			string topic = Encoding.UTF8.GetString(message.Parameters.ElementAt(2));
			IrcChannel channel = connection.FindChannel(channelName);

			if (channel != null)
				channel.SetTopic(topic);
		}

		static void ProcessTopicWhoTime(IrcConnection connection, IrcMessage message)
		{
			if (message.Parameters.Count() != 4)
			{
				Log.WriteLine("RPL_TOPICWHOTIME doesn't appear to be formatted correctly");
				return;
			}

			string channelName = Encoding.UTF8.GetString(message.Parameters.ElementAt(1));
			IrcChannel channel = connection.FindChannel(channelName);

			if (channel == null)
			{
				Log.WriteLine("RPL_TOPICWHOTIME: Channel == null");
				return;
			}

			string nickname = Encoding.UTF8.GetString(message.Parameters.ElementAt(2));

			double unixTimestamp = Double.Parse(Encoding.UTF8.GetString(message.Parameters.ElementAt(3)));
			DateTime time = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
			time.AddSeconds(unixTimestamp);

			channel.SetTopicInfo(nickname, time);
		}

		static void ProcessNameReply(IrcConnection connection, IrcMessage message)
		{
			if (message.Parameters.Count() != 4)
			{
				Log.WriteLine("RPL_NAMEREPLY doesn't appear to be formatted correctly");
				return;
			}

			string channelName = Encoding.UTF8.GetString(message.Parameters.ElementAt(2));
			IrcChannel channel = connection.FindChannel(channelName);

			// Don't process for channels we aren't in
			if (channel == null)
				return;

			// Don't do any further processing if this message wasn't sent as
			// part of the channel initialization process
			if (channel.UsersInitialized)
				return;

			string nameString = Encoding.UTF8.GetString(message.Parameters.ElementAt(3));

			string[] names = nameString.Split(' ');
			char[][] modes = new char[names.Length][];

			for (int i = 0; i < names.Length; i++)
			{
				int numModes = 0;

				for (int j = 0; j < names[i].Length && names[i][j] < 0x41; j++)
					numModes++;

				modes[i] = new char[numModes];

				for (int j = 0; j < names[i].Length && names[i][j] < 0x41; j++)
				{
					char mode = connection.ServerProperties.GetModeFromPrefix(names[i][j]);

					if (mode == '\0')
						Log.WriteLine("RPL_NAMEREPLY: Couldn't find mode from prefix '" + names[i][j] + "'");

					modes[i][j] = mode;
				}

				names[i] = names[i].Remove(0, numModes);
			}

			IrcUser[] users = new IrcUser[names.Length];

			for (int i = 0; i < names.Length; i++)
			{
				users[i] = connection.FindUser(names[i]);

				if (users[i] == null)
					users[i] = connection.AddUser(names[i]);
			}

			channel.AddUsers(users, modes);
		}

		static void ProcessEndOfNames(IrcConnection connection, IrcMessage message)
		{
			if (message.Parameters.Count() != 3)
			{
				Log.WriteLine("RPL_ENDOFNAMES doesn't appear to be formatted correctly");
				return;
			}

			string channelName = Encoding.UTF8.GetString(message.Parameters.ElementAt(1));
			IrcChannel channel = connection.FindChannel(channelName);

			// No need to process this for channels we aren't in
			if (channel == null)
				return;

			channel.UsersInitialized = true;
		}

		static ReplyMessageHandler()
		{
			replyMap.Add(IrcNumericReply.RPL_WELCOME, ProcessWelcome);
			replyMap.Add(IrcNumericReply.RPL_ISUPPORT, ProcessISupport);
			replyMap.Add(IrcNumericReply.RPL_NOTOPIC, ProcessNoTopic);
			replyMap.Add(IrcNumericReply.RPL_TOPIC, ProcessTopic);
			replyMap.Add(IrcNumericReply.RPL_NAMEREPLY, ProcessNameReply);
			replyMap.Add(IrcNumericReply.RPL_ENDOFNAMES, ProcessEndOfNames);
			replyMap.Add(IrcNumericReply.RPL_TOPICWHOTIME, ProcessTopicWhoTime);
		}

		private static Dictionary<IrcNumericReply, Action<IrcConnection, IrcMessage>> replyMap =
			new Dictionary<IrcNumericReply, Action<IrcConnection, IrcMessage>>();
	}
}
