using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Confabulation.Chat
{
	public class IrcKickEventArgs : EventArgs
	{
		public IrcKickEventArgs(IrcChannel channel,
		                        IrcChannelUser kicked,
		                        IrcChannelUser kickedBy)
		{
			if (channel == null)
				throw new ArgumentNullException("channel");

			if (kicked == null)
				throw new ArgumentNullException("kicked");

			if (kickedBy == null)
				throw new ArgumentNullException("kickedBy");

			Channel = channel;
			KickedBy = kickedBy;
			Kicked = kicked;
			Message = null;
		}

		public IrcKickEventArgs(IrcChannel channel,
								IrcChannelUser kicked,
								IrcChannelUser kickedBy,
		                        string message)
		{
			if (channel == null)
				throw new ArgumentNullException("channel");

			if (kicked == null)
				throw new ArgumentNullException("kicked");

			if (kickedBy == null)
				throw new ArgumentNullException("kickedBy");

			if (message == null)
				throw new ArgumentNullException("message");

			Channel = channel;
			KickedBy = kickedBy;
			Kicked = kicked;
			Message = message;
		}

		public IrcChannel Channel
		{
			get;
			private set;
		}

		public IrcChannelUser KickedBy
		{
			get;
			private set;
		}

		public IrcChannelUser Kicked
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
