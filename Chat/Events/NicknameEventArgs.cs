using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Confabulation.Chat.Events
{
	public class NicknameEventArgs : EventArgs
	{
		public NicknameEventArgs(IrcUser user, string oldNickname, string newNickname)
		{
			if (user == null)
				throw new ArgumentNullException("user");
			if (oldNickname == null)
				throw new ArgumentNullException("oldNickname");
			if (newNickname == null)
				throw new ArgumentNullException("newNickname");

			User = user;
			OldNickname = oldNickname;
			NewNickname = newNickname;
		}

		public IrcUser User
		{
			get;
			private set;
		}

		public string OldNickname
		{
			get;
			private set;
		}

		public string NewNickname
		{
			get;
			private set;
		}
	}
}
