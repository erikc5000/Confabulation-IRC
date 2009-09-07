using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Confabulation.Chat
{
    public class IrcUser
    {
        public IrcUser(string nickname)
        {
			if (nickname == null)
				throw new ArgumentNullException("nickname");

            this.Nickname = nickname;
        }

		public IrcUser(string nickname, string userName, string hostname)
		{
			if (nickname == null)
				throw new ArgumentNullException("nickname");
			else if (userName == null)
				throw new ArgumentNullException("userName");
			else if (hostname == null)
				throw new ArgumentNullException("hostname");

			this.Nickname = nickname;
			this.UserName = userName;
			this.Hostname = hostname;
		}

		public string Nickname
		{
			get;
			private set;
		}

		public string UserName
		{
			get;
			private set;
		}

		public string Hostname
		{
			get;
			private set;
		}

		public bool IsAway
		{
			get
			{
				return isAway;
			}
		}

		public string AwayMessage
		{
			get
			{
				return awayMessage;
			}
		}

		public event EventHandler<IrcChannelEventArgs> NicknameChanged;

		internal void SetAwayState(bool isAway, string message)
		{
			this.isAway = isAway;
			awayMessage = message;
		}

		private bool isAway = false;
		private string awayMessage = null;
    }
}
