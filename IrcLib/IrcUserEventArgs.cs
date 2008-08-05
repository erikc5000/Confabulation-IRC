using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Confabulation.Chat
{
	public class IrcUserEventArgs : EventArgs
	{
		public IrcUserEventArgs(IrcUser user, string message)
		{
			if (user == null)
				throw new ArgumentNullException("user");

			User = user;
			Message = message;
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
	}
}
