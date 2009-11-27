using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Confabulation.Chat
{
	public class ChannelEventArgs : EventArgs
	{
		public ChannelEventArgs(IrcChannel channel)
		{
			if (channel == null)
				throw new ArgumentNullException("channel");

			Channel = channel;
			User = null;
			Message = null;
			AddedUsers = null;
		}

		public ChannelEventArgs(IrcChannel channel, IrcChannelUser user)
		{
			if (channel == null)
				throw new ArgumentNullException("channel");
			else if (user == null)
				throw new ArgumentNullException("user");

			Channel = channel;
			User = user;
			Message = null;
			AddedUsers = null;
		}

		public ChannelEventArgs(IrcChannel channel, IrcChannelUser user, string message)
		{
			if (channel == null)
				throw new ArgumentNullException("channel");
			else if (user == null)
				throw new ArgumentNullException("user");

			Channel = channel;
			User = user;
			Message = message;
			AddedUsers = null;
		}

		public ChannelEventArgs(IrcChannel channel, IEnumerable<IrcChannelUser> addedUsers)
		{
			if (channel == null)
				throw new ArgumentNullException("channel");
			else if (addedUsers == null)
				throw new ArgumentNullException("addedUsers");

			Channel = channel;
			User = null;
			Message = null;
			AddedUsers = addedUsers;
		}

		public IrcChannel Channel
		{
			get;
			private set;
		}

		public IrcChannelUser User
		{
			get;
			private set;
		}

		public string Message
		{
			get;
			private set;
		}

		public IEnumerable<IrcChannelUser> AddedUsers
		{
			get;
			private set;
		}
	}
}
