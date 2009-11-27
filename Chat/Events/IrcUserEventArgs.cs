using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Confabulation.Chat
{
	public class IrcUserEventArgs : EventArgs
	{
		public IrcUserEventArgs(IrcUser user)
		{
			if (user == null)
				throw new ArgumentNullException("user");

			User = user;
			Message = null;
			OldNickname = null;
			NewNickname = null;
		}

		public IrcUserEventArgs(IrcUser user, string message)
		{
			if (user == null)
				throw new ArgumentNullException("user");

			User = user;
			Message = message;
			OldNickname = null;
			NewNickname = null;
		}

		public IrcUserEventArgs(IrcUser user, string oldNickname, string newNickname)
		{
			if (user == null)
				throw new ArgumentNullException("user");

			User = user;
			Message = null;
			OldNickname = oldNickname;
			NewNickname = newNickname;
		}

		public IrcUser User
		{
			get;
			private set;
		}

		public string Message
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
