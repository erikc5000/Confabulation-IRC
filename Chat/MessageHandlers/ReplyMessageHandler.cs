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
						IrcUser user = new IrcUser(parts[0], connection);
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

							IrcUser user = new IrcUser(nickname, userName, hostname, connection);
							connection.SetUser(user);
							return;
						}
					}
				}
			}

			if (message.Parameters.Count() >= 1)
			{
				IrcUser user = new IrcUser(Encoding.UTF8.GetString(message.Parameters.ElementAt(0)), connection);
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
				LogError(message);
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
				LogError(message);
				return;
			}

			string channelName = Encoding.UTF8.GetString(message.Parameters.ElementAt(1));
			IrcChannel channel = connection.FindChannel(channelName);

			if (channel != null)
				channel.SetTopic("");
		}

		static void ProcessTopic(IrcConnection connection, IrcMessage message)
		{
			if (message.Parameters.Count() != 3)
			{
				LogError(message);
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
				LogError(message);
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
				LogError(message);
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

				List<char> modeList = new List<char>();
	
				for (int j = 0; j < names[i].Length && names[i][j] < 0x41; j++)
				{
					char mode = connection.ServerProperties.GetModeFromPrefix(names[i][j]);

					if (mode == '\0')
						Log.WriteLine("RPL_NAMEREPLY: Couldn't find mode from prefix '" + names[i][j] + "'");
					else
						modeList.Add(mode);
				}

				names[i] = names[i].Remove(0, numModes);
				modes[i] = modeList.ToArray();
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
				LogError(message);
				return;
			}

			string channelName = Encoding.UTF8.GetString(message.Parameters.ElementAt(1));
			IrcChannel channel = connection.FindChannel(channelName);

			// No need to process this for channels we aren't in
			if (channel == null)
				return;

			channel.UsersInitialized = true;
		}

		static void ProcessNoNicknameGiven(IrcConnection connection, IrcMessage message)
		{
			if (message.Parameters.Count() != 2)
			{
				LogError(message);
				return;
			}

			//string errorMessage = Encoding.UTF8.GetString(message.Parameters.ElementAt(1));

			if (!connection.UserRegistered)
				connection.TryNextNickname();

		}

		static void ProcessNicknameInUse(IrcConnection connection, IrcMessage message)
		{
			if (message.Parameters.Count() != 3)
			{
				LogError(message);
				return;
			}

			//string nickname = Encoding.UTF8.GetString(message.Parameters.ElementAt(1));

			if (!connection.UserRegistered)
				connection.TryNextNickname();
		}

		static void ProcessErroneousNickname(IrcConnection connection, IrcMessage message)
		{
			if (message.Parameters.Count() != 3)
			{
				LogError(message);
				return;
			}

			//string nickname = Encoding.UTF8.GetString(message.Parameters.ElementAt(1));

			if (!connection.UserRegistered)
				connection.TryNextNickname();
		}

		static void ProcessNickCollision(IrcConnection connection, IrcMessage message)
		{
			if (message.Parameters.Count() != 3)
			{
				LogError(message);
				return;
			}

			//string nickname = Encoding.UTF8.GetString(message.Parameters.ElementAt(1));

			if (!connection.UserRegistered)
				connection.TryNextNickname();
		}

		static void ProcessUnavailableResource(IrcConnection connection, IrcMessage message)
		{
			if (message.Parameters.Count() != 3)
			{
				LogError(message);
				return;
			}

			//string nickOrChannel = Encoding.UTF8.GetString(message.Parameters.ElementAt(1));

			if (!connection.UserRegistered)
				connection.TryNextNickname();

		}

		static void ProcessRestricted(IrcConnection connection, IrcMessage message)
		{
			if (message.Parameters.Count() != 2)
			{
				LogError(message);
				return;
			}

			// This happens in response to the NICK message at initial login I believe.  It's
			// supposed to indicate the +r (restricted) user mode.  Not really clear exactly how
			// it all works.

			//string nickname = Encoding.UTF8.GetString(message.Parameters.ElementAt(0));

			//if (!connection.UserRegistered)
			//	connection.TryNextNickname();

		}

		static void LogError(IrcMessage message)
		{
			Log.WriteLine("ERROR: " + message.GetReplyCode() + "is not formatted correctly");
			Log.WriteLine(message.ToString());
		}

		static ReplyMessageHandler()
		{
			replyMap.Add(IrcNumericReply.RPL_WELCOME, ProcessWelcome);
			replyMap.Add(IrcNumericReply.RPL_ISUPPORT, ProcessISupport);
			replyMap.Add(IrcNumericReply.RPL_NOTOPIC, ProcessNoTopic);
			replyMap.Add(IrcNumericReply.RPL_TOPIC, ProcessTopic);
			replyMap.Add(IrcNumericReply.RPL_TOPICWHOTIME, ProcessTopicWhoTime);
			replyMap.Add(IrcNumericReply.RPL_NAMEREPLY, ProcessNameReply);
			replyMap.Add(IrcNumericReply.RPL_ENDOFNAMES, ProcessEndOfNames);
			replyMap.Add(IrcNumericReply.ERR_NONICKNAMEGIVEN, ProcessNoNicknameGiven);
			replyMap.Add(IrcNumericReply.ERR_NICKNAMEINUSE, ProcessNicknameInUse);
			replyMap.Add(IrcNumericReply.ERR_ERRONEUSNICKNAME, ProcessErroneousNickname);
			replyMap.Add(IrcNumericReply.ERR_NICKCOLLISION, ProcessNickCollision);
			replyMap.Add(IrcNumericReply.ERR_UNAVAILRESOURCE, ProcessUnavailableResource);
			replyMap.Add(IrcNumericReply.ERR_RESTRICTED, ProcessRestricted);
		}

		private static Dictionary<IrcNumericReply, Action<IrcConnection, IrcMessage>> replyMap =
			new Dictionary<IrcNumericReply, Action<IrcConnection, IrcMessage>>();
	}
}
