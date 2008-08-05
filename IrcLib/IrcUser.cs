using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Confabulation.Chat
{
    public class IrcUser
    {
        public IrcUser(byte[] nickname)
        {
			if (nickname == null)
				throw new ArgumentNullException("nickname");

            this.nickname = nickname;
        }

        public byte[] Nickname
        {
            get
            {
                return nickname;
            }
        }

		public bool IsAway
		{
			get
			{
				return isAway;
			}
		}

		public byte[] AwayMessage
		{
			get
			{
				return awayMessage;
			}
		}

		internal void SetNickname(byte[] nickname)
		{
			this.nickname = nickname;
		}

		internal void SetAwayState(bool isAway, byte[] message)
		{
			this.isAway = isAway;
			awayMessage = message;
		}

        private byte[] nickname;
		private bool isAway = false;
		private byte[] awayMessage = null;
    }
}
