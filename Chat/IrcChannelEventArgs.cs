using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Confabulation.Chat
{
	public class IrcChannelEventArgs : EventArgs
	{
		public IrcChannelEventArgs(IrcChannel channel)
		{
			if (channel == null)
				throw new ArgumentNullException("channel");

			Channel = channel;
			User = null;
		}

		public IrcChannelEventArgs(IrcChannel channel, IrcChannelUser user)
		{
			if (channel == null)
				throw new ArgumentNullException("channel");

			if (user == null)
				throw new ArgumentNullException("user");

			Channel = channel;
			ChannelUser = user;
			User = user.GetUser();
		}

		public IrcChannelEventArgs(IrcChannel channel, IrcUser user)
		{
			if (channel == null)
				throw new ArgumentNullException("channel");

			if (user == null)
				throw new ArgumentNullException("user");

			Channel = channel;
			ChannelUser = null;
			User = user;
		}

		public IrcChannel Channel
		{
			get;
			private set;
		}

		public IrcChannelUser ChannelUser
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
