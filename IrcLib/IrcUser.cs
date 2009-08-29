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

		public string Nickname
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

		internal void SetAwayState(bool isAway, string message)
		{
			this.isAway = isAway;
			awayMessage = message;
		}

		private bool isAway = false;
		private string awayMessage = null;
    }
}
