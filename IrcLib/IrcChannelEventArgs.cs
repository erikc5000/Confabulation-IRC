using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Confabulation.Chat
{
	public class IrcChannelEventArgs : EventArgs
	{
		public IrcChannelEventArgs(IrcChannel channel, IrcUser user)
		{
			if (channel == null)
				throw new ArgumentNullException("channel");

			if (user == null)
				throw new ArgumentNullException("user");

			Channel = channel;
			User = user;
		}

		public IrcChannel Channel
		{
			get;
			private set;
		}

		public IrcUser User
		{
			get;
			private set;
		}

		public string Message
		{
			get;
			internal set;
		}
	}
}
