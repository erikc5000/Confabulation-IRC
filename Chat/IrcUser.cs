﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Confabulation.Chat.Events;

namespace Confabulation.Chat
{
	public class IrcUser
	{
		public IrcUser(string nickname, IrcConnection connection)
		{
			if (nickname == null)
				throw new ArgumentNullException("nickname");
			else if (connection == null)
				throw new ArgumentNullException("connection");

			this.nickname = nickname;
			this.Connection = connection;
		}

		public IrcUser(string nickname, string userName, string hostname, IrcConnection connection)
		{
			if (nickname == null)
				throw new ArgumentNullException("nickname");
			else if (userName == null)
				throw new ArgumentNullException("userName");
			else if (hostname == null)
				throw new ArgumentNullException("hostname");
			else if (connection == null)
				throw new ArgumentNullException("connection");

			this.nickname = nickname;
			this.userName = userName;
			this.hostname = hostname;
			this.Connection = connection;
		}

		//public override bool Equals(object obj)
		//{
		//    if (obj == null)
		//        return false;

		//    if (obj is IrcUser)
		//    {
		//        IrcUser other = (IrcUser)obj;
		//        return Irc.NamesEqual(other.Nickname, Nickname, Connection.ServerProperties);
		//    }

		//    return false;
		//}

		public bool Equals(string nickname)
		{
			return Irc.NamesEqual(nickname, Nickname, Connection.ServerProperties);
		}

		public IrcConnection Connection
		{
			get;
			private set;
		}

		public IEnumerable<IrcChannel> Channels
		{
			get
			{
				lock (syncObject)
				{
					return new List<IrcChannel>(channels);
				}
			}
		}

		public string Nickname
		{
			get
			{
				lock (syncObject)
				{
					return nickname;
				}
			}
		}

		public string UserName
		{
			get
			{
				lock (syncObject)
				{
					return userName;
				}
			}
		}

		public string Hostname
		{
			get
			{
				lock (syncObject)
				{
					return hostname;
				}
			}
		}

		public bool IsSelf
		{
			get
			{
				return Connection.User == this;
			}
		}

		public bool IsAway
		{
			get
			{
				lock (syncObject)
				{
					return isAway;
				}
			}
		}

		public string AwayMessage
		{
			get
			{
				lock (syncObject)
				{
					return awayMessage;
				}
			}
		}

		public IEnumerable<string> PreviousNicknames
		{
			get
			{
				lock (syncObject)
				{

					return new List<string>(nicknameHistory);
				}
			}
		}

		public string GetLastNickname()
		{
			lock (syncObject)
			{
				if (nicknameHistory.Count() == 0)
					return null;

				return nicknameHistory.Last();
			}
		}

		public event EventHandler<NicknameEventArgs> NicknameChanged;

		internal void SetAwayState(bool isAway, string message)
		{
			lock (syncObject)
			{
				this.isAway = isAway;
				awayMessage = message;
			}
		}

		internal void ChangeNickname(string newNickname)
		{
			string oldNickname;

			lock (syncObject)
			{
				nicknameHistory.Add(nickname);
				oldNickname = nickname;
				nickname = newNickname;
			}

			NicknameEventArgs e = new NicknameEventArgs(this, oldNickname, newNickname);
			EventHandler<NicknameEventArgs> handler = NicknameChanged;

			if (handler != null)
				handler(this, e);
		}

		internal void AddChannel(IrcChannel channel)
		{
			lock (syncObject)
			{
				if (!channels.Contains(channel))
					channels.Add(channel);
			}
		}

		internal void RemoveChannel(IrcChannel channel)
		{
			lock (syncObject)
			{
				channels.Remove(channel);
			}
		}

		private readonly Object syncObject = new Object();
		private string nickname;
		private string userName = null;
		private string hostname = null;
		private bool isAway = false;
		private string awayMessage = null;
		private List<IrcChannel> channels = new List<IrcChannel>();
		private List<string> nicknameHistory = new List<string>();
	}
}
